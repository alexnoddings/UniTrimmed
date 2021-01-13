using System;
using System.Collections.Generic;
using System.Linq;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Parser.Parsing
{
    internal struct ParsedAgent : IAgent
    {
        public string Name { get; }

        public IReadOnlyCollection<IKnowledge> Knowledge { get; }

        internal bool IsAttacker { get; }

        internal ParsedAgent(string name, IEnumerable<IKnowledge> knowledge, bool isAttacker)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Knowledge = knowledge?.ToList() ?? throw new ArgumentNullException(nameof(knowledge));
            IsAttacker = isAttacker;
        }

        internal ParsedAgent(string name, IKnowledge knowledge, bool isAttacker)
            : this(name, new[] {knowledge}, isAttacker) { }
    }
}