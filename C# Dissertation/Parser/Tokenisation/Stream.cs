using System;
using System.Collections.Generic;

namespace AlexNoddings.Protocols.Parser.Tokenisation
{
    internal class Stream<T> : IDisposable
    {
        private readonly IEnumerator<T> _enumerator;

        internal Stream(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            _enumerator.MoveNext();
        }

        internal T Peek => _enumerator.Current;

        internal T Next()
        {
            T current = _enumerator.Current;
            _enumerator.MoveNext();
            return current;
        }

        internal void MoveNext() => _enumerator.MoveNext();

        public void Dispose() => _enumerator.Dispose();
    }
}
