using System;
using System.Linq;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class CreateInstruction<T> : AbstractInterpreterInstruction<CreateExpression>
    {
        private readonly Func<string, bool> _predicate;
        private readonly Action<string, string> _functionWithArgumentsToCall;

        public CreateInstruction(Interpreter interpreter, 
                                CreateExpression createExpression, 
                                int functionIndex, 
                                Func<string, bool> predicate,
                                Action<string, string> functionWithArgumentsToCall)
            : base(interpreter, createExpression, functionIndex)
        {
            _predicate = predicate;
            _functionWithArgumentsToCall = functionWithArgumentsToCall;
        }
        

        public override void Handle(IStateMachine<string, IEchseContext> machine)
        {

            var argumentAsVariable = Expression.Identifier.Name;
            if (Expression.Identifier is IdentifierExpression)
            {                
                argumentAsVariable = GetVariable(machine).Value;
            }

            if (!_predicate(Expression.Creator.Name)) return;
            _functionWithArgumentsToCall(Expression.Creator.Name, argumentAsVariable);
        }

        private TagVariable GetVariable(IStateMachine<string, IEchseContext> machine)
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
            if(variable.DataTypeSymbol != LexiconSymbol.TagDataType)
                throw new InvalidOperationException($"Syntax error. Cannot execute a modify instruction. Data type of variable is not a tag.");
            return variable;
        }
    }
}
