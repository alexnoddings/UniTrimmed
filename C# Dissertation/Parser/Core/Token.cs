using System;

namespace AlexNoddings.Protocols.Parser.Core
{
    internal struct Token
    {
        internal TokenType Type { get; }
        internal State State { get; }
        internal string Value { get; }
        internal Position Position { get; }

        public Token(TokenType type, State state, string value, Position position)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            State = state ?? throw new ArgumentNullException(nameof(state));
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Position = position;
        }

        public Token(StateMatch stateMatch, Position position) 
            : this(stateMatch.Type, stateMatch.State, stateMatch.Value, position) { }

        public override string ToString() => $"{Value} ({Type.Name}@{State.Name})";
    }
}
