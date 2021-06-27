using System;
using System.Linq;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class IfInstruction : AbstractInterpreterInstruction<IfExpression>
    {
        public bool LastEvaluation { get; set; } = false;
        public int EndIfIndex { get; set; } = -1;
        public IfInstruction(Interpreter interpreter, IfExpression expression, int functionIndex) 
            : base(interpreter, expression, functionIndex)
        {
            
        }
        private TagVariable GetVariable(VariableExpression variableExpression, IStateMachine<string, IEchseContext> machine)
        {
            //get the variable out of the function scope 
            var variable = machine
                .SharedContext
                .Variables
                .FirstOrDefault(t => t.Name == variableExpression.Name &&
                                     t.Scope == Scope?.Expression.Name);

            var lastFn = Scope;
            var identifierName = Expression.Left.Name;
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
            if (variable.DataTypeSymbol != LexiconSymbol.TagDataType &&
                variable.DataTypeSymbol != LexiconSymbol.RefDataType)
            {
                throw new InvalidOperationException($"Syntax error. Data type of variable is not a tag or ref.");
            }
            // Console.WriteLine($"Variable:{variable.Name} Current Value:{variable.Value}");
            // Console.WriteLine(System.Environment.NewLine);
            return variable;
        }

        public override void Handle(IStateMachine<string, IEchseContext> machine)
        {
            var leftVar = GetVariable(Expression.Left, machine);
            var rightVar = Expression.Right is IdentifierExpression ? GetVariable(Expression.Right, machine) : null;

            LastEvaluation =  leftVar != null &&
                            Expression.Comparison.ComparisonSymbol == LexiconSymbol.Equal &&
                            ((leftVar.DataTypeSymbol == LexiconSymbol.TagDataType &&
                            Expression.Right is TagExpression && leftVar.Value == Expression.Right.Name) ||
                            (leftVar.DataTypeSymbol == LexiconSymbol.RefDataType &&
                            Expression.Right is RefExpression && leftVar.Value == Expression.Right.Name) ||
                            (leftVar != null && rightVar != null && rightVar.DataTypeSymbol == leftVar.DataTypeSymbol && leftVar.Value == rightVar.Value));

            EndIfIndex = FunctionIndex;
            var endIfInstructionFound = false;
            var currentInstructionIndex = EndIfIndex;
            while (!(endIfInstructionFound = Owner.Instructions[EndIfIndex += 1] is EndIfInstruction) &&
                EndIfIndex <= Owner.Instructions.Count)
            {
                if (!LastEvaluation)
                    continue;
                var currentInstruction = Owner.Instructions[EndIfIndex];
                if (currentInstruction is EndIfInstruction)
                {
                    // Console.WriteLine("EndIf instruction found");
                    break;
                }

                // Console.WriteLine(currentInstruction);
                if (currentInstruction is FunctionInstruction)
                {
                    // Console.WriteLine("function instruction found. aborting function instruction");
                    break;
                }
                currentInstruction.Handle(machine);

                if (currentInstruction is IfInstruction)
                {
                    /* If there is a nested if, the if instruction hasn't got the EndIfIndex for the current If yet. 
                     * Example:
                     * 
                        If a is .foo                    # true  -> this is current function
                            If b is .bar                # false -> if this instruction is false, it jumps back to this scope here
                                Set Tag b = .blablabla  # this line should be skipped, from HERE (by here I mean the THIS IfInstructon)
                            EndIf                       # this is line should not be touched at all
                            Set Tag c = .goo            # this line should NOT BE ignored
                        EndIf                           # this is the EndIfIndex we want
                     */
                    EndIfIndex = (currentInstruction as IfInstruction).EndIfIndex; // Does it need +1 ???
                    break;
                }
                if (currentInstruction is ReturnInstruction)
                {
                    // Console.WriteLine("wait instruction found. aborting function instruction");
                    break;
                }
                if (currentInstruction is WaitInstruction)
                {
                    // Console.WriteLine("wait instruction found. aborting function instruction");
                    break;
                }
            }

            base.Handle(machine);
        }
    }
}
