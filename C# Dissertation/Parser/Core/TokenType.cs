using System;
using System.Text.RegularExpressions;

namespace AlexNoddings.Protocols.Parser.Core
{
    internal class TokenType
    {
        internal string Name { get; }
        internal State? PushState { get; }
        internal bool ShouldPopState { get; }
        internal bool IsImportant { get; }
        private Regex Regex { get; }

        private TokenType(string name, bool isImportant, Regex regex, State? pushState, bool shouldPopState)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            IsImportant = isImportant;
            Regex = regex ?? throw new ArgumentNullException(nameof(name));
            PushState = pushState;
            ShouldPopState = shouldPopState;
        }

        private TokenType(string name, bool isImportant, string regex, State? pushState, bool shouldPopState)
            : this(name, isImportant, new Regex(regex, RegexOptions.Compiled), pushState, shouldPopState) { }

        internal TokenTypeMatch? Match(string text)
        {
            Match match = Regex.Match(text, 0);
            if (!match.Success || match.Index > 0)
                return null;
            return new TokenTypeMatch(this, match.Value);
        }

        public override string ToString() => $"{Name} ({Regex})";

        internal static TokenType Simple(string name, string regex) => 
            new TokenType(name, true, regex, null, false);

        internal static TokenType Push(string name, string regex, State pushState) => 
            new TokenType(name, true, regex, pushState ?? throw new ArgumentNullException(nameof(pushState)), false);

        internal static TokenType Pop(string name, string regex) => 
            new TokenType(name, true, regex, null, true);

        internal static TokenType Unimportant(string name, string regex) =>
            new TokenType(name, false, regex, null, false);
    }
}
