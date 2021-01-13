using System.Collections.Generic;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Parser.Parsing
{
    public interface IParsedGame
    {
        public IReadOnlyCollection<IAgent> Agents { get; }
        public IAgent Attacker { get; }
        public IReadOnlyCollection<IStep> Steps { get; }
    }
}
