using System;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Parser.Parsing
{
    internal struct GameStep : IStep
    {
        public IAgent FromAgent { get; }
        public IAgent ToAgent { get; }
        public IKnowledge Knowledge { get; }

        public GameStep(IAgent fromAgent, IAgent agent, IKnowledge knowledge)
        {
            FromAgent = fromAgent ?? throw new ArgumentNullException(nameof(fromAgent));
            ToAgent = agent ?? throw new ArgumentNullException(nameof(agent));
            Knowledge = knowledge ?? throw new ArgumentNullException(nameof(knowledge));
        }
    }
}
