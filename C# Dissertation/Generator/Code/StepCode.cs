using System;
using AlexNoddings.Protocols.Core;

namespace AlexNoddings.Protocols.Generator.Code
{
    internal class StepCode
    {
        private IStep Step { get; }

        internal StepCode(IStep agent) => Step = agent ?? throw new ArgumentNullException(nameof(agent));

        internal string DisplayStr => $"{Step.FromAgent.Name} -> {Step.ToAgent.Name}: {Step.Knowledge}";
    }
}
