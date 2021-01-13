using System;
using System.Collections.Generic;
using AlexNoddings.Protocols.Parser.Core;

namespace AlexNoddings.Protocols.Parser.Tokenisation
{
    internal struct StateStack
    {
        private readonly Stack<State> _stack;

        internal State Current => _stack.Peek();
        internal bool IsAtBase => _stack.Count == 1;

        internal StateStack(State baseState) => _stack =
            new Stack<State>(new[] {baseState ?? throw new ArgumentNullException(nameof(baseState))});

        internal void Push(State newState) =>
            _stack.Push(newState ?? throw new ArgumentNullException(nameof(newState)));

        internal State Pop()
        {
            if (IsAtBase)
                throw new InvalidOperationException("Cannot pop base state");
            return _stack.Pop();
        }
    }
}
