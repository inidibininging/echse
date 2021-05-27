using System;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public class DestroyInstruction : AbstractInterpreterInstruction<DestroyExpression>
    {
        public DestroyInstruction(Interpreter interpreter, DestroyExpression createExpression, int functionIndex) 
            : base(interpreter, createExpression, functionIndex)
        {
        }

        public override void Handle(IStateMachine<string, IEchseContext> machine)
        {
            if (Expression == null)
                throw new ArgumentNullException(nameof(DestroyExpression));

            var factionToDestroy = Expression.Identifier.Name;
            machine.SharedContext.RemoveTag(factionToDestroy);
        }
    }
}
