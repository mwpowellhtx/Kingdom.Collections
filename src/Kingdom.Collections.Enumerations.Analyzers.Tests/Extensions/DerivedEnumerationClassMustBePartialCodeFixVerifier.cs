using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    internal static class ExtensionMethods
    {
        public static T[] ToArrayArray<T>(this T element, params T[] elements)
            => new[] {element}.Concat(elements).ToArray();

        public static IEnumerable<T> EnsureAtLeastEmpty<T>(this IEnumerable<T> values)
            => values ?? new T[0];
    }
}
