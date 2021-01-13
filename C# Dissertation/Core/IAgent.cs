using System.Collections.Generic;

namespace AlexNoddings.Protocols.Core
{
    public interface IAgent
    {
        public string Name { get; }
        public IReadOnlyCollection<IKnowledge> Knowledge { get; }
    }
}
