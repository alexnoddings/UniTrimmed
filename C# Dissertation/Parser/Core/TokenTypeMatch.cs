using System;
using System.Collections.Generic;
using System.Text;

namespace AlexNoddings.Protocols.Parser.Core
{
    internal struct TokenTypeMatch
    {
        internal TokenType Type { get; }
        internal string Value { get; }

        public TokenTypeMatch(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}
