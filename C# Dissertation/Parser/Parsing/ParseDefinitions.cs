using AlexNoddings.Protocols.Parser.Core;

namespace AlexNoddings.Protocols.Parser.Parsing
{
    internal static class ParseDefinitions
    {
        internal static readonly State BaseState  = new State(nameof(BaseState));
        internal static readonly State KnowledgeState = new State(nameof(KnowledgeState));
        internal static readonly State AgentsBlockState = new State(nameof(AgentsBlockState));
        internal static readonly State StepsBlockState = new State(nameof(StepsBlockState));

        internal static readonly TokenType WhiteSpaceToken = TokenType.Unimportant(nameof(WhiteSpaceToken), "\\s+");
        internal static readonly TokenType CommentToken = TokenType.Unimportant(nameof(CommentToken), "\\/\\*.*?\\*\\/");
        internal static readonly TokenType AgentNameToken = TokenType.Simple(nameof(AgentNameToken), "[a-zA-Z][a-zA-Z0-9]*");
        internal static readonly TokenType KnowledgeOpenToken = TokenType.Push(nameof(KnowledgeOpenToken), "\\{", KnowledgeState);

        internal static readonly TokenType BaseStateOpenAgentsBlock = TokenType.Push(nameof(BaseStateOpenAgentsBlock), "agents\\s*\\{", AgentsBlockState);
        internal static readonly TokenType BaseStateOpenStepsBlock = TokenType.Push(nameof(BaseStateOpenStepsBlock), "steps\\s*\\{", StepsBlockState);

        internal static readonly TokenType KnowledgeStateNameToken = TokenType.Simple(nameof(KnowledgeStateNameToken), "([a-zA-Z][a-zA-Z0-9]*_[a-zA-Z0-9]+|#?[a-zA-Z][a-zA-Z0-9]*)");
        internal static readonly TokenType KnowledgeStateSeparatorToken = TokenType.Simple(nameof(KnowledgeStateSeparatorToken), ",");
        internal static readonly TokenType KnowledgeStatePopToken = TokenType.Pop(nameof(KnowledgeStatePopToken), "\\}");

        internal static readonly TokenType AgentsBlockStateAttackerToken = TokenType.Simple(nameof(AgentsBlockStateAttackerToken), "!");
        internal static readonly TokenType AgentsBlockStatePopToken = TokenType.Pop(nameof(AgentsBlockStatePopToken), "\\}");

        internal static readonly TokenType StepsBlockStatePopToken = TokenType.Pop(nameof(StepsBlockStatePopToken), "\\}");

        static ParseDefinitions()
        {
            BaseState.AddTokenTypes(WhiteSpaceToken, CommentToken, BaseStateOpenAgentsBlock, BaseStateOpenStepsBlock);
            KnowledgeState.AddTokenTypes(WhiteSpaceToken, KnowledgeOpenToken, KnowledgeStateNameToken, KnowledgeStateSeparatorToken, KnowledgeStatePopToken);
            AgentsBlockState.AddTokenTypes(WhiteSpaceToken, CommentToken, AgentNameToken, AgentsBlockStateAttackerToken, KnowledgeOpenToken, AgentsBlockStatePopToken);
            StepsBlockState.AddTokenTypes(WhiteSpaceToken, CommentToken, AgentNameToken, KnowledgeOpenToken, StepsBlockStatePopToken);
        }
    }
}
