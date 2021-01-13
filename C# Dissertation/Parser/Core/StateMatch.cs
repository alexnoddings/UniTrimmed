using System;

namespace AlexNoddings.Protocols.Parser.Core
{
    internal struct StateMatch
    {
        internal TokenType Type { get; }
        internal string Value { get; }
        internal State State { get; }

        public StateMatch(TokenType type, string value, State state)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            State = state ?? throw new ArgumentNullException(nameof(state));
        }

        public StateMatch(TokenTypeMatch tokenTypeMatch, State state) 
            : this(tokenTypeMatch.Type, tokenTypeMatch.Value, state) { }
    }
}
