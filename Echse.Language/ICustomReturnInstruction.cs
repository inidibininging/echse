using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Echse.Language
{
    public interface ICustomReturnInstruction : IState<string, IEchseContext>
    {
        public string ReturnTagValue { get; }
    }
}