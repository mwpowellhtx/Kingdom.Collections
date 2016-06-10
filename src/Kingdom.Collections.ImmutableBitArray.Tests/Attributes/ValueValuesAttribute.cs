using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    public class ValueValuesAttribute : ValuesAttribute
    {
        private static object[] Values { get; set; }

        private static IEnumerable<object> GetValueValues()
        {
            yield return false;
            yield return true;
        }

        static ValueValuesAttribute()
        {
            Values = GetValueValues().ToArray();
        }

        public ValueValuesAttribute()
            : base(Values)
        {
        }
    }
}
