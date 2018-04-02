using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Kingdom.Collections
{
    using CombinatorialValuesAttribute = ValuesAttribute; // xunit bridge

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="!:https://msdn.microsoft.com/en-us/library/a1sway8w.aspx">&lt;&lt; Operator (C# Reference)</see>
    /// <inheritdoc />
    public class ShiftCountValuesAttribute : CombinatorialValuesAttribute
    {
        private static IEnumerable<object> GetValues()
        {
            const int size = sizeof(uint)*8;
            // Save the zero case for a separate test where we are explicitly expecting an exception.
            const int half = size/2;
            // Signal for "default shift", that is, default use case is count one (1).
            yield return (int?) null;
            yield return (int?) 0;
            yield return (int?) half;
            /* This is more of a limitation of the language << operator,
             * which is constrained to 0 to 31 bits shifted for integer types. */
            yield return (int?) size - 1;
        }

        private static Lazy<object[]> LazyValues { get; }
            = new Lazy<object[]>(() => GetValues().ToArray());

        internal ShiftCountValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
