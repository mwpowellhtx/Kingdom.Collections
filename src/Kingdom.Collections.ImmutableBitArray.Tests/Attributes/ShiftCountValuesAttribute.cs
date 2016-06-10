using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    /// <summary>
    /// 
    /// </summary>
    /// <see cref="https://msdn.microsoft.com/en-us/library/a1sway8w.aspx">&lt;&lt; Operator (C# Reference)</see>
    internal class ShiftCountValuesAttribute : ValuesAttribute
    {
        private static IEnumerable<object> GetValues()
        {
            const int size = sizeof(uint)*8;
            // Save the zero case for a separate test where we are explicitly expecting an exception.
            const int half = size/2;
            // The default default use case is 1.
            yield return (int?) null;
            yield return (int?) half;
            /* This is more of a limitation of the language << operator,
             * which is constrained to 0 to 31 bits shifted for integer types. */
            yield return (int?) size - 1;
        }

        private static readonly Lazy<object[]> LazyValues
            = new Lazy<object[]>(() => GetValues().ToArray());

        internal ShiftCountValuesAttribute()
            : base(LazyValues.Value)
        {
        }
    }
}
