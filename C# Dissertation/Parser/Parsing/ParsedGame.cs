using System;
using System.Collections.Generic;
using System.Linq;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Parser.Parsing
{
    internal struct ParsedGame : IParsedGame
    {
        public IReadOnlyCollection<IAgent> Agents { get; }
        public IAgent Attacker { get; }
        public IReadOnlyCollection<IStep> Steps { get; }

        public ParsedGame(IEnumerable<IAgent> agents, IAgent attacker, IEnumerable<IStep> steps)
        {
            Agents = agents?.ToList() ?? throw new ArgumentNullException(nameof(agents));
            Attacker = attacker ?? throw new ArgumentNullException(nameof(attacker));
            Steps = steps?.ToList() ?? throw new ArgumentNullException(nameof(steps));
        }
    }
}
