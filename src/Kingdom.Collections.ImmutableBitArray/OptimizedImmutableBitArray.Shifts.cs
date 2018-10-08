using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using static Math;
    using static Elasticity;

    public partial class OptimizedImmutableBitArray
    {
        private void CalculateShiftedBytes(IList<byte> bytes, int count)
        {
            var shift = count % BitCount;

            // ReSharper disable once InvertIf
            if (_bytes.Any() && shift != 0)
            {
                // Tack on One more Byte in the event the Shift Boundary is mid-Byte.
                bytes.Add(0);

                int i;
                byte mask;

                // We want to Iterate Plus One accounting for the Boundary Bytes.
                for (i = 0, mask = MakeMask(0, shift - 1); i < _bytes.Count + 1; i++)
                {
                    // Do not let the Index fool you, this is Index from the MSB end of the Bytes.
                    var current = _bytes.Count - 1 - i;
                    var previous = current - 1;

                    // Merge a portion of the Current with the opposite Portion of the Previous.
                    bytes[current] = MergeAll(
                        (byte)((bytes[current] & mask) << (BitCount - shift))
                        , (byte)((bytes[previous] & ~mask) >> shift)
                    );
                }
            }
        }

        // TODO: TBD: want to separate these a little bit along the lines of Elasticity...
        /// <inheritdoc />
        public OptimizedImmutableBitArray ShiftLeft(int count = 1, Elasticity elasticity = Elasticity.None)
        {
            // TODO: TBD: methinks that this is then a function of the StartIndex version of the same...
            var bytes = new List<byte>();

            bytes.AddRange(_bytes);

            var padCount = count / BitCount + 1;

            // Go ahead and Prepend the Bytes.
            bytes.InsertRange(0, new byte[padCount]);

            // TODO: TBD: could potentially leverage some code here for both Shift Left/Right, but let's get it on its feet and testing first, then consider what to do for further optimizations.
            CalculateShiftedBytes(bytes, count);

            IEnumerable<T> WithOrWithoutExpansion<T>(IEnumerable<T> values)
                => elasticity.Contains(Elasticity.Expansion)
                    ? values.Take(_bytes.Count)
                    : values;

            return new OptimizedImmutableBitArray(WithOrWithoutExpansion(bytes));
        }

        // TODO: TBD: when shift left/right are both done, they should also update the Length, especially when Elasticity comes into play
        /// <inheritdoc />
        public OptimizedImmutableBitArray ShiftRight(int count = 1, Elasticity elasticity = Elasticity.None)
        {
            // TODO: TBD: methinks that this is then a function of the StartIndex version of the same...
            var bytes = new List<byte>();

            bytes.AddRange(_bytes);

            var padCount = count / BitCount;

            // Go ahead and Append the Bytes.
            bytes.AddRange(new byte[padCount]);

            var shift = count % BitCount;

            //// TODO: TBD: reviewing this code compared/contrasted with the Shift Left Calculation, this one could potentially be leveraged for both after all with a bit of functional injection.
            // ReSharper disable once InvertIf
            if (_bytes.Any() && shift != 0)
            {
                int i;
                byte mask;

                // We want to Iterate Plus One accounting for the Boundary Bytes.
                for (i = 0, mask = MakeMask(0, shift - 1); i < _bytes.Count + 1; i++)
                {
                    // Do not let the Index fool you, this is Index from the MSB end of the Bytes.
                    var current = i;
                    var next = current + 1;

                    // Merge a portion of the Current with the opposite Portion of the Previous.
                    bytes[current] = MergeAll(
                        (byte)((bytes[current] & ~mask) >> shift)
                        , (byte)((bytes[next] & mask) << (BitCount - shift))
                    );
                }
            }

            // There is a strong similarity with Shift Left notwithstanding Reversed Bytes and Bits.
            IEnumerable<T> WithOrWithoutContraction<T>(IEnumerable<T> values)
                => values.ReverseTake(
                    elasticity.Contains(Elasticity.Contraction)
                        ? Max(0, _bytes.Count - padCount - (count % BitCount == 0 ? 0 : 1))
                        : _bytes.Count
                ).Reverse();

            // Make sure to also Reverse the Bits on the way out as well.
            return new OptimizedImmutableBitArray(WithOrWithoutContraction(bytes));
        }

        private static void VerifyShiftArguments(int startIndex, int count, int length, Elasticity? elasticity)
        {
            bool TryArgumentOutOfRange<T>(string argName, T argValue
                , Func<T, bool> outOfRange, Func<string> message)
            {
                if (!outOfRange(argValue))
                {
                    return false;
                }

                if (elasticity.Contains(Elasticity.Silent))
                {
                    return true;
                }

                throw new ArgumentOutOfRangeException(argName, argValue, message());
            }

            if (TryArgumentOutOfRange(nameof(startIndex), startIndex
                    , x => x < 0 || x >= length
                    // ReSharper disable once AccessToModifiedClosure
                    , () => $"'{nameof(startIndex)}' '{startIndex}' invalid.")
                || TryArgumentOutOfRange(nameof(count), count
                    , x => x < 0, () => $"'{nameof(count)}' '{count}' invalid."))
            {
                // Zero Count also returns early. There is nothing to do in that case.
            }
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

            OptimizedImmutableBitArray ShiftBooleanArray()
            {
                // tbits: this_Bits
                // pbits: Prepend Bits
                // sbits: Shift Bits
                // ibits: Intermediate Bits

                var tbits = this.ToArray();

                // Preserve these Bits for Prepending to Intermediate at the end.
                var pbits = tbits.Take(startIndex);

                // Shift the Bits within the Start Index Range.
                var sbits = tbits.Skip(startIndex);

                // Prepend the Count of Bits it takes in order to Shift Left.
                var ibits = GetRange(count, () => false).Concat(sbits);

                /* Expansion is a little bit different from Contraction. We may just accept the Intermediate
                 * Bits as-is allowing for the Elasticity request. Or, take the left-most Bits accounting for
                 * the current Bit Array Length. */
                if (!elasticity.Contains(Elasticity.Expansion))
                {
                    ibits = ibits.ToArray().Take(this_Length - startIndex);
                }

                /* Now reconstitute the Result Bits including the Append Bits. Shifting Left may Append
                 * nothing whatsoever; in other words, we may be shifting from the right-most LSB. Or,
                 * the Start Index may be more significant than that, in which case, Append those Bits. */
                return new OptimizedImmutableBitArray(pbits.Concat(ibits));
            }

            return ShiftBooleanArray();
        }

        /// <summary>
        /// Shifts the current Bit Array <paramref name="count"/> Bits to the Right. Leaves the
        /// Bits right of the <paramref name="startIndex"/> alone, while Shifting the left of the
        /// same to the Right. Applies the <paramref name="elasticity"/> specification allowing for
        /// <see cref="Elasticity.Contraction"/>. When unspecified constrains the Shifted bits to the current
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

            OptimizedImmutableBitArray ShiftBooleanArray()
            {
                // tbits: this_Bits
                // pbits: Prepend Bits
                // sbits: Shift Bits
                // ibits: Intermediate Bits

                var tbits = this.ToArray();

                // Prepend these Bits just prior to the Intermediate Result.
                var pbits = tbits.Take(startIndex);

                // Shift the Bits within the Start Index Range.
                var sbits = tbits.Skip(startIndex);

                // Prepend the Count of Bits it takes in order to Shift Right.
                IEnumerable<bool> ibits = sbits.Concat(GetRange(count, () => false)).ToArray();

                // Starting from the Shifted Bits.
                ibits = ibits.Skip(count).ToArray();

                // ReSharper disable once InvertIf
                if (elasticity.Contains(Elasticity.Contraction))
                {
                    // Contract when instructed to do so.
                    ibits = ibits.Take(Max(0, ibits.Count() - count)).ToArray();
                }

                /* Now reconstitute the Result Bits including the Prepend Bits. Shifting Right may Prepend
                 * nothing whatsoever; in other words, we may be shifting from the right-most LSB. Or, the
                 * Start Index may be more significant than that, in which case, Prepend those Bits. */
                return new OptimizedImmutableBitArray(pbits.Concat(ibits));
            }

            return ShiftBooleanArray();
        }

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
