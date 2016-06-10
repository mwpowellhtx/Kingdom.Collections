//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Kingdom.Collections
//{
//    internal static class ValuesValuesAttributeUtils<TFixture, TValue>
//        where TFixture : ValuesFixtureBase<TValue>, new()
//    {
//        private static int GetMaximumPossibleShift(long targetValue)
//        {
//            var shift = 0;
//            while (1L << ++shift < targetValue)
//            {
//            }
//            return shift - 1;
//        }

//        private static IEnumerable<int> GetShifts(int start, int count = 1, int step = 1)
//        {
//            var results = new List<int>();

//            while (count-- > 0)
//            {
//                results.Add(start);
//                start += step;
//            }

//            return results;
//        }

//        internal static IEnumerable<object> GetValuesFixtures(Func<int> getSize,
//            Func<TValue> getDefault, Func<long> getMax, Func<long> getMiddle,
//            Func<int, TValue> getShifted, Func<long, TValue> getValue)
//        {
//            // In terms of number of bits.
//            var size = getSize()*8;
//            var max = getMax();
//            var middle = getMiddle();
//            var defaultValue = getDefault();

//            // Make a concerted effort to focus the number of test cases.
//            var lengthShifts = GetShifts(1)
//                .Concat(GetShifts(GetMaximumPossibleShift(middle)))
//                .Concat(GetShifts(GetMaximumPossibleShift(max)))
//                .ToArray()
//                ;

//            foreach (var lengthShift in lengthShifts)
//            {
//                var length = 1 << lengthShift;

//                var values = Enumerable.Range(0, length).Select(x => defaultValue).ToArray();

//                var shift = 0;

//                // Focus in one one of the elements for test purposes.
//                while (shift < size)
//                {
//                    // Walk the shifted boolean value throughout the range of values.
//                    values[length/size] = getValue(1L << shift);

//                    yield return new TFixture {Values = values.ToArray()};

//                    shift += 2;
//                }
//            }
//        }
//    }
//}
