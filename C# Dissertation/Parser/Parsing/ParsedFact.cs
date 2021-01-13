using System;
using System.Collections.Generic;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Parser.Parsing
{
    internal struct ParsedFact : IKnowledge
    {
        internal string Name { get; }

        public IKnowledge? Key { get; }

        public IReadOnlyCollection<IKnowledge> Parts { get; }

        public ParsedFact(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Key = null;
            Parts = new List<IKnowledge>(0);
        }

        public override string ToString() => Name;
    }
}
