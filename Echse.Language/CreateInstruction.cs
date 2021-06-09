using System;
using System.Collections.Generic;
using System.Linq;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class CreateInstruction<T> : AbstractInterpreterInstruction<CreateExpression>
    {
        private readonly Func<string, bool> _predicate;
        private readonly Action<string, string, IEnumerable<string>> _functionWithArgumentsToCall;

        public CreateInstruction(Interpreter interpreter, 
                                CreateExpression createExpression, 
                                int functionIndex, 
                                Func<string, bool> predicate,
                                Action<string, string, IEnumerable<string>> functionWithArgumentsToCall)
            : base(interpreter, createExpression, functionIndex)
        {
            _predicate = predicate;
            _functionWithArgumentsToCall = functionWithArgumentsToCall;
        }
        

        public override void Handle(IStateMachine<string, IEchseContext> machine)
        {
            var arguments = Expression.Identifiers.Select(id => 
            {
                if (id is IdentifierExpression)
                    return GetVariable(machine, id).Value;
                else
                    return id.Name;
            });

            if (!_predicate(Expression.Creator.Name)) return;

            _functionWithArgumentsToCall(Scope.Expression.Name ,Expression.Creator.Name, arguments);
        }

        private TagVariable GetVariable(IStateMachine<string, IEchseContext> machine, VariableExpression identifier)
        {
            //get the variable out of the function scope 
            var variable = machine
                .SharedContext
                .Variables
                .FirstOrDefault(t => t.Name == identifier.Name &&
                                     t.Scope == Scope?.Expression.Name);

            var lastFn = Scope;
            var identifierName = identifier.Name;
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
                    .Arguments[argumentIndex]
                    .Name;
                lastFn = lastFn.LastCaller.Scope;

            }
            if(variable.DataTypeSymbol != LexiconSymbol.TagDataType)
                throw new InvalidOperationException("Syntax error. Cannot execute a create instruction. Data type of variable is not a tag.");
            return variable;
        }
    }
}
