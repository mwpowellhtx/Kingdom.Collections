using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    internal class RandomIntValuesAttribute : ValuesAttribute
    {
        private static readonly Lazy<Random> LazyRnd = new Lazy<Random>(
            () => new Random((int) DateTime.UtcNow.Ticks%int.MaxValue));

        private static Random Rnd
        {
            get { return LazyRnd.Value; }
        }

        private static IEnumerable<object> GetRandomValues()
        {
            return Enumerable.Range(0, 4).Select(_ => (uint) Rnd.Next()).Cast<object>();
        }

        private static readonly Lazy<object[]> LazyValues
            = new Lazy<object[]>(() => GetRandomValues().ToArray());

        internal RandomIntValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
