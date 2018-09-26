using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    internal static class EnumerableExtensionMethods
    {
        public static IEnumerable<T> ReverseTake<T>(this IEnumerable<T> values, int count = 0)
        {
            var i = 0;

            while (i++ < count)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                yield return values.ElementAt(
                    // ReSharper disable once PossibleMultipleEnumeration
                    values.Count() - i - 1);
            }
        }
    }
}
