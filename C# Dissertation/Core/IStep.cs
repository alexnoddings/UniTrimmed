namespace AlexNoddings.Protocols.Core
{
    public interface IStep
    {
        public IAgent FromAgent { get; }
        public IAgent ToAgent { get; }
        public IKnowledge Knowledge { get; }
    }
}
