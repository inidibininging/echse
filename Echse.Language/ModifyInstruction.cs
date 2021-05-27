using System;
using System.Collections.Generic;
using System.Linq;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class ModifyInstruction<TEntity> : AbstractInterpreterInstruction<ModifyAttributeExpression>
    {
        private readonly Func<string, IEnumerable<TEntity>> _tagPredicate;
        private readonly Func<EntityExpression, IEnumerable<TEntity>> _entityPredicate;
        private readonly Action<TEntity, ModifyAttributeExpression> _entityHandler;

        public ModifyInstruction(Interpreter interpreter, 
            ModifyAttributeExpression modifyExpression, 
            int functionIndex,
            Func<string, IEnumerable<TEntity>> tagPredicate,
            Func<EntityExpression, IEnumerable<TEntity>> entityPredicate,
            Action<TEntity, ModifyAttributeExpression> entityHandler
            ) 
            : base(interpreter, modifyExpression, functionIndex)
        {
            _tagPredicate = tagPredicate;
            _entityPredicate = entityPredicate;
            _entityHandler = entityHandler;
            // Console.WriteLine("adding mod instruction");
        }

        
        public override void Handle(IStateMachine<string, IEchseContext> machine)
        {
            if (Expression.Identifier is IdentifierExpression)
                HandleByIdentifier(machine);
            if (Expression.Identifier is EntityExpression)
                HandleByEntity(machine);
            if (Expression.Identifier is TagExpression)
                HandleByTag(machine, Expression.Identifier.Name);
        }


        
        private void HandleByIdentifier(IStateMachine<string, IEchseContext> machine)
        {
            //get the variable out of the function scope 
            var variable = machine
                .SharedContext
                .Variables
                .FirstOrDefault(t => t.Name == Expression.Identifier.Name &&
                                     t.Scope == Scope?.Expression.Name);

            var lastFn = Scope;
            var identifierName = Expression.Identifier.Name;
            while (variable == null)
            {
                var argumentIndex = lastFn.GetFunctionArgumentIndex(identifierName);
                variable = lastFn
                            .LastCaller
                            .GetVariableOfFunction(machine, argumentIndex);
                if (variable != null) 
                    continue;
                identifierName = lastFn
                    .LastCaller
                    .Expression
                    .Arguments
                    .Arguments
                    .ElementAt(argumentIndex)
                    .Name;
                lastFn = lastFn.LastCaller.Scope;
                
            }
            if (variable.DataTypeSymbol != LexiconSymbol.TagDataType)
                throw new InvalidOperationException($"Syntax error. Cannot execute a modify instruction. Data type of variable is not a tag.");

            HandleByTag(machine, variable.Value);
        }
        
        private void HandleByEntity(IStateMachine<string, IEchseContext> machine)
        {
            foreach(var entity in _entityPredicate(Expression.Identifier as EntityExpression))
                _entityHandler(entity, Expression);
        }
        
        private void HandleByTag(IStateMachine<string, IEchseContext> machine, string tag)
        {
            foreach (var entity in _tagPredicate(tag))
                _entityHandler(entity, Expression);
        }
    }
}
