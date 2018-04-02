using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Kingdom.Collections
{
    using CombinatorialValuesAttribute = ValuesAttribute; // xunit bridge
    using static ImmutableBitArray.Elasticity;

    public class ElasticityValuesAttribute : CombinatorialValuesAttribute
    {
        private static IEnumerable<object> GetValues()
        {
            yield return None;
            yield return Expansion;
            yield return Contraction;
            yield return Both;
        }

        private static Lazy<object[]> LazyValues { get; }
            = new Lazy<object[]>(() => GetValues().ToArray());

        internal ElasticityValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
