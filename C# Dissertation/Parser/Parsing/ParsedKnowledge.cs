using System;
using System.Collections.Generic;
using System.Linq;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Parser.Parsing
{
    internal struct ParsedKnowledge : IKnowledge
    {
        public IKnowledge? Key { get; }

        public IReadOnlyCollection<IKnowledge> Parts { get; }

        public ParsedKnowledge(IKnowledge part, IKnowledge? key = null)
        {
            Parts = new[] {part ?? throw new ArgumentNullException(nameof(part))};
            Key = key;
        }

        public ParsedKnowledge(IEnumerable<IKnowledge> parts, IKnowledge? key = null)
        {
            Parts = parts?.ToList() ?? throw new ArgumentNullException(nameof(parts));
            Key = key;
        }

        public override string ToString() => $"{{{string.Join(", ", Parts)}}}{Key?.ToString() ?? string.Empty}";
    }
}
