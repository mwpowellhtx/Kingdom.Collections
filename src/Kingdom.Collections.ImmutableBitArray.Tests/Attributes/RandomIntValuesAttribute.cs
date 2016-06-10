using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Kingdom.Collections
{
    internal class RandomIntValuesAttribute : ValuesAttribute
    {
        private static readonly Lazy<Random> LazyRnd = new Lazy<Random>(
            () => new Random((int) DateTime.UtcNow.Ticks%int.MaxValue));

        private static Random Rnd
        {
            get { return LazyRnd.Value; }
        }

        private static IEnumerable<object> GetRandomIntValues()
        {
            return Enumerable.Range(0, 4).Select(_ => (uint) Rnd.Next()).Cast<object>();
        }

        internal RandomIntValuesAttribute()
            : base(GetRandomIntValues().ToArray())
        {
        }
    }
}
