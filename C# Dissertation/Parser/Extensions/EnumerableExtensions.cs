using System.Collections.Generic;
using AlexNoddings.Protocols.Parser.Tokenisation;

namespace AlexNoddings.Protocols.Parser.Extensions
{
    internal static class EnumerableExtensions
    {
        internal static Stream<T> AsStream<T>(this IEnumerable<T> enumerable) => new Stream<T>(enumerable.GetEnumerator());
    }
}
