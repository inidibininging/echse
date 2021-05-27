using System.Linq;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class FunctionInstruction : AbstractInterpreterInstruction<FunctionExpression>
    {
        public ExecuteInstruction LastCaller { get; set; }
        public ReturnExpression LastReturnValue {get; set;}
        public FunctionInstruction(Interpreter interpreter, FunctionExpression function, int functionIndex) 
            : base(interpreter, function, functionIndex)
        {
            // Console.WriteLine("adding function instruction " + function.Name);
            interpreter.Context.NewService.New(Expression.Name,this);
        }

        public int GetFunctionArgumentIndex(string identifierName)
        {
            //look up for the function index in the function                
            return
                Expression
                    .Arguments
                    .Arguments.IndexOf(
                        Expression
                        .Arguments
                        .Arguments
                        .FirstOrDefault(arg => arg.Name == identifierName));
        }
        
        public override void Handle(IStateMachine<string, IEchseContext> machine) {
            var currentInstructionIndex = FunctionIndex+1;
            while(currentInstructionIndex < Owner.Instructions.Count) {
                var currentInstruction = Owner.Instructions[currentInstructionIndex];
                // Console.WriteLine(currentInstruction);
                currentInstructionIndex++;

                if (currentInstruction is FunctionInstruction){
                    // Console.WriteLine("function instruction found. aborting function instruction");
                    break;
                }

                currentInstruction.Handle(machine);
                if (currentInstruction is IfInstruction)
                {
                    currentInstructionIndex = (currentInstruction as IfInstruction).EndIfIndex;
                }

                if (currentInstruction is WaitInstruction)
                {
                    // Console.WriteLine("wait instruction found. aborting function instruction");
                    break;
                }
                if (currentInstruction is ReturnInstruction)
                {
                    break;
                }
            }
            // Console.WriteLine($"function {Expression.Name} executed");
        }

        public override string ToString()
        {
            return Expression.Name;
        }
    }
}
