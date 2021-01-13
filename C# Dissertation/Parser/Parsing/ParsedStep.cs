using System;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Parser.Parsing
{
    internal struct ParsedStep
    {
        internal string FromAgentName { get; }
        internal string ToAgentName { get; }
        internal IKnowledge Knowledge { get; }

        internal ParsedStep(string fromAgentName, string toAgentName, IKnowledge knowledge)
        {
            FromAgentName = fromAgentName ?? throw new ArgumentNullException(nameof(fromAgentName));
            ToAgentName = toAgentName ?? throw new ArgumentNullException(nameof(toAgentName));
            Knowledge = knowledge ?? throw new ArgumentNullException(nameof(knowledge));
        }
    }
}
