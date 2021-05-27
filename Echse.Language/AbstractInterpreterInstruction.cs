using System;
using System.Linq;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public abstract class AbstractInterpreterInstruction<TExpr> 
        : IAbstractInterpreterInstruction<TExpr> where TExpr : class, IAbstractLanguageExpression
    {
        protected Interpreter Owner { get;}
        protected int ConcurrentExecutions {get; set;}
        public int FunctionIndex { get; protected set; }

        public FunctionInstruction Scope { get; protected set; }
        // public int ScopeIndex { get; protected set; }

        public TExpr Expression { get; private set; }
        protected AbstractInterpreterInstruction(Interpreter interpreter, TExpr expression, int functionIndex) {
            Owner = interpreter;
            FunctionIndex = functionIndex > -1
                ? functionIndex
                : throw new ArgumentOutOfRangeException(nameof(functionIndex));
            interpreter.Instructions.Add(this);
            Expression = expression ?? throw new ArgumentException("Expression is null for the current instruction");
            ApplyScope();
        }
        
        protected void ApplyScope()
        {
            FunctionInstruction fn = Owner.Instructions.ElementAt(FunctionIndex) as FunctionInstruction;
            var currentInstructionIndex = FunctionIndex - 1 ; //this should be set -1 because the function is not registered yet
            if (fn == null && currentInstructionIndex > -1)
            {
                while ((fn = Owner.Instructions.ElementAt(currentInstructionIndex) as FunctionInstruction) == null)
                {
                    if(currentInstructionIndex < 0)
                        throw new ArgumentNullException(nameof(currentInstructionIndex));
                    currentInstructionIndex--;
                }    
            }
            Scope = fn;
            // ScopeIndex = currentInstructionIndex;
        }


        
        
        public virtual void Handle(IStateMachine<string, IEchseContext> machine)
        {
            ConcurrentExecutions++;
        }
    }
}
