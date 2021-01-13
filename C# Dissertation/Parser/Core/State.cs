using System;
using System.Collections.Generic;
using System.Linq;

namespace AlexNoddings.Protocols.Parser.Core
{
    internal class State
    {
        internal string Name { get; }
        internal IReadOnlyCollection<TokenType> TokenTypes => _tokens.AsReadOnly();

        private readonly List<TokenType> _tokens = new List<TokenType>();

        internal State(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));

        internal void AddTokenTypes(params TokenType[] tokenTypes) => _tokens.AddRange(tokenTypes);

        internal StateMatch? Match(string text)
        {
            TokenTypeMatch? match = _tokens.Select(m => m.Match(text)).FirstOrDefault(m => m != null);
            if (match != null)
                return new StateMatch(match.Value, this);
            return null;
        }
    }
}
