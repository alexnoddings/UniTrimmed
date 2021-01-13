using System;
using System.Collections.Generic;
using System.Linq;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Runtime.Internal
{
    internal struct SimpleKnowledge : IKnowledge
    {
        public IKnowledge? Key { get; }
        public IReadOnlyCollection<IKnowledge> Parts { get; }

        public SimpleKnowledge(IKnowledge? key, IEnumerable<IKnowledge> parts)
        {
            Key = key;
            Parts = parts?.ToList() ?? throw new ArgumentNullException(nameof(parts));
        }
    }
}
