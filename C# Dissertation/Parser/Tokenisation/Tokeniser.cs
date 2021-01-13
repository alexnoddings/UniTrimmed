using System;
using System.Collections.Generic;
using System.Linq;
using AlexNoddings.Protocols.Parser.Core;
using AlexNoddings.Protocols.Parser.Parsing;

namespace AlexNoddings.Protocols.Parser.Tokenisation
{
    internal static class Tokeniser
    {
        internal static IEnumerable<Token> Tokenise(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            string textLeft = text;
            var offset = 0;
            var stateStack = new StateStack(ParseDefinitions.BaseState);

            while (true)
            {
                State state = stateStack.Current;

                if (textLeft.Length == 0)
                {
                    if (!stateStack.IsAtBase)
                        throw new InvalidOperationException($"Tokensier ended above stack base in state {state.Name}");
                    break;
                }

                StateMatch match = state.Match(textLeft) 
                                   ?? throw new InvalidOperationException($"State {state.Name} could not parse text \"{textLeft.Substring(0, 16)}...\"");

                int matchSize = match.Value.Length;
                if (matchSize == 0)
                    throw new InvalidOperationException($"Invalid token type {match.Type.Name} in state {state.Name} (matches empty string, causing an infinite loop)");

                int foundOffset = offset;
                offset += matchSize;
                textLeft = text.Substring(offset);

                if (match.Type.PushState != null)
                {
                    stateStack.Push(match.Type.PushState);
                }
                else if (match.Type.ShouldPopState)
                {
                    stateStack.Pop();
                }

                yield return new Token(match, CalculatePosition(text, foundOffset));
            }
        }

        private static Position CalculatePosition(string text, int offset)
        {
            int line = text.Take(offset).Count(c => c == '\n');
            int lastNewLine = text.Substring(0, offset).LastIndexOf('\n');
            int column = offset - lastNewLine;
            return new Position(offset, line, column);
        }
    }
}
