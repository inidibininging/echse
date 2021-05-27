using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class ReturnInstruction : AbstractInterpreterInstruction<ReturnExpression>
    {
        public ReturnInstruction(Interpreter interpreter, ReturnExpression expression, int functionIndex) : base(interpreter, expression, functionIndex)
        {
        }

        public override void Handle(IStateMachine<string, IEchseContext> machine)
        {
            Scope.LastReturnValue = Expression;
            base.Handle(machine);
        }
    }
}
