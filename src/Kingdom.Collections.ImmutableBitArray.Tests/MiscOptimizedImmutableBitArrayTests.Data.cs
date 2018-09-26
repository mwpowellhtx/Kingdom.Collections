using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using static DateTime;

    // ReSharper disable once UnusedMember.Global
    public partial class MiscOptimizedImmutableBitArrayTests
    {
        private static Random Rnd { get; } = new Random((int) (UtcNow.Ticks % int.MaxValue));

        private static IEnumerable<object[]> _lengthCountData;

        public static IEnumerable<object[]> LengthCountData
        {
            get
            {
                IEnumerable<object[]> GetAll()
                {
                    IEnumerable<object> GetOne(IEnumerable<byte> bytes, int lengthDelta)
                    {
                        yield return GetRange(bytes.ToArray());
                        yield return lengthDelta;
                    }

                    IEnumerable<byte> GetBytes(int count)
                    {
                        while (count-- > 0)
                        {
                            yield return (byte) (Rnd.Next() % byte.MaxValue);
                        }
                    }

                    const byte maxByte = byte.MaxValue;

                    for (var byteCount = 0; byteCount < sizeof(uint); byteCount++)
                    {
                        /* TODO: TBD: furnishing Prime Numbers here is completely arbitrary; I
                         * just want something that is guaranteed not to coincide with a bit/byte
                         * boundary for test purposes. */

                        /* Poor man's range of Prime Numbers for test purposes. Could be any
                         * range, just as long as it exercises exceeding the width of the Bits in
                         * a Byte, and then some, for test purposes. */

                        // Additionally, Zero (no-change) is especially just as valid a thing to verify.
                        foreach (var lengthDelta in GetRange(0, 1, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37))
                        {
                            // Starting with some Random Test Cases.
                            yield return GetOne(GetBytes(byteCount), lengthDelta).ToArray();
                            yield return GetOne(GetBytes(byteCount), -lengthDelta).ToArray();

                            // But let us also better verify that boundary bytes are masked correctly.
                            yield return GetOne(GetRange(new byte[byteCount].Select(_ => maxByte).ToArray()), lengthDelta).ToArray();
                            yield return GetOne(GetRange(new byte[byteCount].Select(_ => maxByte).ToArray()), -lengthDelta).ToArray();
                        }
                    }
                }

                return _lengthCountData ?? (_lengthCountData = GetAll().ToArray());
            }
        }
    }
}
