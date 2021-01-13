using System.Collections.Generic;

namespace AlexNoddings.Protocols.Core
{
    public interface IKnowledge
    {
        public IKnowledge? Key { get; }
        public IReadOnlyCollection<IKnowledge> Parts { get; }
    }
}
