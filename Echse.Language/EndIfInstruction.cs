using System;

namespace Echse.Language
{
    public class EndIfInstruction : AbstractInterpreterInstruction<EndIfExpression>
    {
        public int IfScopeIndex { get; set; }
        public EndIfInstruction(Interpreter interpreter, EndIfExpression expression, int functionIndex) 
            : base(interpreter, expression, functionIndex)
        {
            GetIfScope();
        }

        private void GetIfScope()
        {
            var currentSeekIndex = FunctionIndex;

            while (currentSeekIndex != -1 && Owner.Instructions[currentSeekIndex -= 1].GetType() != typeof(IfInstruction))
                continue;
            if (currentSeekIndex < 0)
                throw new InvalidOperationException($"Syntax Error: If scope not found for EndIf in index {FunctionIndex}");
            IfScopeIndex = currentSeekIndex;
        }
    }
}
