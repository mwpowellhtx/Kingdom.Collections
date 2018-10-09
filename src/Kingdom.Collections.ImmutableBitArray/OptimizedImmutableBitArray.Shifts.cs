using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using static Math;
    using static Elasticity;

    public partial class OptimizedImmutableBitArray
    {
        private static void VerifyShiftArguments(int startIndex, int count, int length, Elasticity? elasticity)
        {
            bool TryArgumentOutOfRange<T>(string argName, T argValue, Func<T, bool> outOfRange)
            {
                if (!outOfRange(argValue))
                {
                    return false;
                }

                if (elasticity.Contains(Silent))
                {
                    return true;
                }

                throw new ArgumentOutOfRangeException(argName, argValue, $"'{argName}' ('{argValue}') invalid.");
            }

            if (TryArgumentOutOfRange(nameof(startIndex), startIndex, x => x < 0 || x >= length)
                || TryArgumentOutOfRange(nameof(count), count, x => x < 0))
            {
                // Zero Count also returns early. There is nothing to do in that case.
            }
        }

        private delegate IEnumerable<bool> ShiftCallback(IEnumerable<bool> preserveBits, IEnumerable<bool> insertBits,
            IEnumerable<bool> shiftBits);

        private static OptimizedImmutableBitArray Shift(IEnumerable<bool> array, int startIndex, int count
            , ShiftCallback callback)
        {
            array = array.ToArray();

            return new OptimizedImmutableBitArray(callback(array.Take(startIndex)
                , GetRange(count, () => false), array.Skip(startIndex)));
        }

        /// <summary>
        /// Shifts the current Bit Array <paramref name="count"/> Bits to the Left. Leaves the
        /// Bits right of the <paramref name="startIndex"/> alone, while Shifting the left of the
        /// same to the Left. Applies the <paramref name="elasticity"/> specification allowing for
        /// <see cref="Elasticity.Expansion"/>. When unspecified constrains the Shifted bits to the current
        /// Bit Array <see cref="Length"/>.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="elasticity"></param>
        /// <returns></returns>
        public OptimizedImmutableBitArray ShiftLeft(int startIndex, int count = 1, Elasticity? elasticity = null)
        {
            if (count == 0)
            {
                return new OptimizedImmutableBitArray(ToBytes());
            }

            // ReSharper disable once InconsistentNaming
            var this_Length = Length;

            VerifyShiftArguments(startIndex, count, this_Length, elasticity);

            return Shift(this, startIndex, count
                , (pbits, ibits, sbits) => elasticity.Contains(Expansion)
                    ? pbits.Concat(ibits).Concat(sbits)
                    : pbits.Concat(ibits.Concat(sbits).Take(this_Length - startIndex))
            );
        }

        /// <summary>
        /// Shifts the current Bit Array <paramref name="count"/> Bits to the Right. Leaves the
        /// Bits right of the <paramref name="startIndex"/> alone, while Shifting the left of the
        /// same to the Right. Applies the <paramref name="elasticity"/> specification allowing for
        /// <see cref="Contraction"/>. When unspecified constrains the Shifted bits to the current
        /// Bit Array <see cref="Length"/>.
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="elasticity"></param>
        /// <returns></returns>
        public OptimizedImmutableBitArray ShiftRight(int startIndex, int count = 1, Elasticity? elasticity = null)
        {
            // Return very early when there is nothing else to do.
            if (count == 0)
            {
                return new OptimizedImmutableBitArray(ToBytes());
            }

            // ReSharper disable once InconsistentNaming
            var this_Length = Length;

            VerifyShiftArguments(startIndex, count, this_Length, elasticity);

            return Shift(this, startIndex, count
                , (pbits, ibits, sbits) =>
                {
                    var xbits = sbits.Concat(ibits);
                    // ReSharper disable once InconsistentNaming, PossibleMultipleEnumeration
                    var xbits_Count = xbits.Count();
                    return elasticity.Contains(Contraction)
                        // ReSharper disable once PossibleMultipleEnumeration
                        ? pbits.Concat(xbits.Skip(count).Take(Max(0, xbits_Count - count - count)))
                        // ReSharper disable once PossibleMultipleEnumeration
                        : pbits.Concat(xbits.Skip(count));
                }
            );
        }

        /// <summary>
        /// 0
        /// </summary>
        private const int DefaultStartIndex = 0;

        // TODO: TBD: want to separate these a little bit along the lines of Elasticity...
        /// <inheritdoc />
        /// <see cref="DefaultStartIndex"/>
        /// <see cref="ShiftLeft(int,int,Elasticity?)"/>
        public OptimizedImmutableBitArray ShiftLeft(int count = 1, Elasticity? elasticity = null)
            => ShiftLeft(DefaultStartIndex, count, elasticity);

        // TODO: TBD: when shift left/right are both done, they should also update the Length, especially when Elasticity comes into play
        /// <inheritdoc />
        /// <see cref="DefaultStartIndex"/>
        /// <see cref="ShiftRight(int,int,Elasticity?)"/>
        public OptimizedImmutableBitArray ShiftRight(int count = 1, Elasticity? elasticity = null)
            => ShiftRight(DefaultStartIndex, count, elasticity);

        /// <inheritdoc />
        /// <see cref="Length"/>
        public bool Remove(bool item)
        {
            /* We take the circuitous route here because we will need to work with the Index
             * anyway in order to do the appropriate Shift operation should a matching Item
             * be found. */
            for (var i = 0; i < Length; i++)
            {
                if (Get(i) != item)
                {
                    continue;
                }

                // This is a key use case for ShiftRight startIndex overload.
                var shifted = ShiftRight(i, 1, Contraction);

                // Then translate the Shifted Range into this Collection.
                ListAction(b =>
                {
                    b.Clear();
                    b.AddRange(shifted._bytes);
                });

                // And adjust the Length.
                Length = shifted.Length;

                return true;
            }

            return false;
        }
    }
}
