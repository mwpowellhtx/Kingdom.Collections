using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using static BitConverter;
    using static Math;
    using static Elasticity;

    /// <inheritdoc />
    public class OptimizedImmutableBitArray : IElasticImmutableBitArray<OptimizedImmutableBitArray>
    {
        // TODO: TBD: maintaining bit array as a collection of bytes makes the most sense here because underneath in the IL, bool is actually stored in the space of a byte, so this is the most efficient representation.
        private List<byte> _bytes;

        /// <inheritdoc />
        public OptimizedImmutableBitArray(params uint[] values)
            : this(values.SelectMany(x => IsLittleEndian ? GetBytes(x) : GetBytes(x).Reverse()))
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public OptimizedImmutableBitArray(params byte[] bytes)
        {
            _bytes = bytes.ToList();
            _length = _bytes.Count * 8;
        }

        /// <summary>
        /// 
        /// </summary>
        public OptimizedImmutableBitArray(IEnumerable<byte> bytes)
        {
            _bytes = bytes.ToList();
            _length = _bytes.Count * 8;
        }

        private void ListAction(Action<List<byte>> action) => action.Invoke(_bytes);

        private TResult ListFunc<TResult>(Func<List<byte>, TResult> func) => func(_bytes);

        private static IEnumerable<bool> GetBooleans(IEnumerable<byte> bytes, int length)
        {
            var shifts = Enumerable.Range(0, BitCount - 1).ToArray();
            // TODO: TBD: may want to Take(Length) of the bits here...
            return bytes.SelectMany(b => shifts.Select(shift => ((1 << shift) & b) != 0)).Take(length).ToArray();
        }

        /// <inheritdoc />
        public IEnumerator<bool> GetEnumerator() => ListFunc(bytes => GetBooleans(bytes, Length)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private delegate byte CalculateBinaryCallback(byte x, byte y);

        private static IEnumerable<byte> CalculateBinaryOperator(IEnumerable<byte> a
            , IEnumerable<byte> b, CalculateBinaryCallback calc)
        {
            // Ensuring that A and B are both the same size.
            void NormalizeArray(IEnumerable<byte> x, ref IEnumerable<byte> y)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if (x.Count()
                    // ReSharper disable once PossibleMultipleEnumeration
                    > y.Count())
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    y = y.Concat(new byte[
                        // ReSharper disable once PossibleMultipleEnumeration
                        x.Count()
                        // ReSharper disable once PossibleMultipleEnumeration
                        - y.Count()]).ToArray();
                }
            }

            // ReSharper disable once PossibleMultipleEnumeration
            NormalizeArray(a, ref b);
            // ReSharper disable once PossibleMultipleEnumeration
            NormalizeArray(b
                // ReSharper disable once PossibleMultipleEnumeration
                , ref a);

            // Then simply Zip the two collections and apply the Calculation.
            // ReSharper disable once PossibleMultipleEnumeration
            return a.Zip(b, (x, y) => calc(x, y)).ToArray();
        }

        private static IEnumerable<T> GetRange<T>(params T[] items) => items;

        /// <inheritdoc />
        public OptimizedImmutableBitArray And(OptimizedImmutableBitArray other) => And(GetRange(other));

        /// <inheritdoc />
        public OptimizedImmutableBitArray Or(OptimizedImmutableBitArray other) => Or(GetRange(other));

        /// <inheritdoc />
        public OptimizedImmutableBitArray Xor(OptimizedImmutableBitArray other) => Xor(GetRange(other));

        /// <summary>
        /// Performs the Bitwise And operation on the current instance, <paramref name="other"/>,
        /// and <paramref name="others"/> instances. Returns a new instance containing the result.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="others"></param>
        /// <returns>A new instance containing the result.</returns>
        public OptimizedImmutableBitArray And(OptimizedImmutableBitArray other
            , params OptimizedImmutableBitArray[] others)
            => And(GetRange(other).Concat(others).ToArray());

        /// <summary>
        /// Performs the Bitwise Or operation on the current instance, <paramref name="other"/>,
        /// and <paramref name="others"/> instances. Returns a new instance containing the result.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="others"></param>
        /// <returns>A new instance containing the result.</returns>
        public OptimizedImmutableBitArray Or(OptimizedImmutableBitArray other
            , params OptimizedImmutableBitArray[] others)
            => Or(GetRange(other).Concat(others).ToArray());

        /// <summary>
        /// Performs the Bitwise Exclusive Or operation on the current instance,
        /// <paramref name="other"/>, and <paramref name="others"/> instances. Returns a new
        /// instance containing the result.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="others"></param>
        /// <returns>A new instance containing the result.</returns>
        public OptimizedImmutableBitArray Xor(OptimizedImmutableBitArray other
            , params OptimizedImmutableBitArray[] others)
            => Xor(GetRange(other).Concat(others).ToArray());

        /// <inheritdoc />
        public OptimizedImmutableBitArray And(IEnumerable<OptimizedImmutableBitArray> others)
        {
            var bytes = others.Aggregate(_bytes.ToArray()
                , (g, other) => CalculateBinaryOperator(
                    g, other._bytes, (x, y) => (byte) (x & y)).ToArray());

            return new OptimizedImmutableBitArray(bytes);
        }

        /// <inheritdoc />
        public OptimizedImmutableBitArray Or(IEnumerable<OptimizedImmutableBitArray> others)
        {
            var bytes = others.Aggregate(_bytes.ToArray()
                , (g, other) => CalculateBinaryOperator(
                    g, other._bytes, (x, y) => (byte) (x | y)).ToArray());

            return new OptimizedImmutableBitArray(bytes);
        }

        /// <inheritdoc />
        public OptimizedImmutableBitArray Xor(IEnumerable<OptimizedImmutableBitArray> others)
        {
            var bytes = others.Aggregate(_bytes.ToArray()
                , (g, other) => CalculateBinaryOperator(
                    g, other._bytes, (x, y) => (byte) (x ^ y)).ToArray());

            return new OptimizedImmutableBitArray(bytes);
        }

        /// <inheritdoc />
        public OptimizedImmutableBitArray Not() => new OptimizedImmutableBitArray(_bytes.Select(x => (byte) ~x));

        // TODO: TBD: start with Shift Left/Right base-zero address.
        // TODO: TBD: would be interesting to add a base address however
        // TODO: TBD: next: what to do about shifting, starting position, etc
        // TODO: TBD: this is by far the hardest part about the whole procedure...

        internal const int BitCount = 8;

        private static byte MergeAll(params byte[] values)
            => values.Aggregate(default(byte), (g, y) => (byte) (g | y));

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
                        (byte) ((bytes[current] & mask) << (BitCount - shift))
                        , (byte) ((bytes[previous] & ~mask) >> shift)
                    );
                }
            }
        }

        // TODO: TBD: want to separate these a little bit along the lines of Elasticity...
        /// <inheritdoc />
        public OptimizedImmutableBitArray ShiftLeft(int count = 1, Elasticity elasticity = None)
        {
            var bytes = new List<byte>();

            bytes.AddRange(_bytes);

            var padCount = count / BitCount + 1;

            // Go ahead and Prepend the Bytes.
            bytes.InsertRange(0, new byte[padCount]);

            // TODO: TBD: could potentially leverage some code here for both Shift Left/Right, but let's get it on its feet and testing first, then consider what to do for further optimizations.
            CalculateShiftedBytes(bytes, count);

            IEnumerable<T> WithOrWithoutExpansion<T>(IEnumerable<T> values)
                => elasticity.Contains(Expansion)
                    ? values.Take(_bytes.Count)
                    : values;

            return new OptimizedImmutableBitArray(WithOrWithoutExpansion(bytes));
        }

        // TODO: TBD: when shift left/right are both done, they should also update the Length, especially when Elasticity comes into play
        /// <inheritdoc />
        public OptimizedImmutableBitArray ShiftRight(int count = 1, Elasticity elasticity = None)
        {
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
                        (byte) ((bytes[current] & ~mask) >> shift)
                        , (byte) ((bytes[next] & mask) << (BitCount - shift))
                    );
                }
            }

            // There is a strong similarity with Shift Left notwithstanding Reversed Bytes and Bits.
            IEnumerable<T> WithOrWithoutContraction<T>(IEnumerable<T> values)
                => values.ReverseTake(
                    elasticity.Contains(Contraction)
                        ? Max(0, _bytes.Count - padCount - (count % BitCount == 0 ? 0 : 1))
                        : _bytes.Count
                ).Reverse();

            // Make sure to also Reverse the Bits on the way out as well.
            return new OptimizedImmutableBitArray(WithOrWithoutContraction(bytes));
        }

        //// TODO: TBD: this is one attempt; I think the approach almost fits the bill but for what we do with boundary use cases.
        ///// <inheritdoc />
        //public OptimizedImmutableBitArray ShiftRight(int count = 1, Elasticity elasticity = None)
        //{
        //    byte ReverseBits(byte x)
        //    {
        //        byte ReverseTwoForOne(byte y, int i)
        //            => (byte) (y & i << (BitCount - i)
        //                       | y & (BitCount - i) >> (BitCount - i));

        //        var index = -1;

        //        return MergeAll(ReverseTwoForOne(x, ++index)
        //            , ReverseTwoForOne(x, ++index)
        //            , ReverseTwoForOne(x, ++index)
        //            , ReverseTwoForOne(x, ++index)
        //        );
        //    }

        //    var bytes = new List<byte>();

        //    /* Make sure we Reverse not only the Bytes but also the Bits
        //     in order for the following Shift to be consistent. */
        //    bytes.AddRange(_bytes.Select(ReverseBits).Reverse());

        //    // TODO: TBD: we'll need to be careful how this works pulling Bits/Bytes to the Right
        //    // TODO: TBD: would it leave us with potentially unanticipated buffer space on the Right?
        //    var padCount = count / BitCount + 1;

        //    // Go ahead and Prepend the Bytes.
        //    bytes.InsertRange(0, new byte[padCount]);

        //    // We will leverage the Same Shift Calculation that we use during the Shift Left operation.
        //    CalculateShiftedBytes(bytes, count);

        //    // There is a strong similarity with Shift Left notwithstanding Reversed Bytes and Bits.
        //    IEnumerable<T> WithOrWithoutContraction<T>(IEnumerable<T> values)
        //        => values.Take(
        //            elasticity.Contains(Contraction)
        //                ? Max(0, _bytes.Count - padCount - (count % BitCount == 0 ? 0 : 1))
        //                : _bytes.Count
        //        ).Reverse();

        //    // Make sure to also Reverse the Bits on the way out as well.
        //    return new OptimizedImmutableBitArray(WithOrWithoutContraction(bytes).Select(ReverseBits));
        //}

        /// <summary>
        /// Returns whether the Bit at <paramref name="index"/> is Set to <value>true</value>.
        /// The default behavior involves no <see cref="Elasticity"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <remarks>The default behavior involves no <see cref="Elasticity"/>.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Occurs when <paramref name="index"/>
        /// is Less Than Zero or Greater Than or Equal to <see cref="Length"/>.</exception>
        /// <inheritdoc />
        public bool Get(int index)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index)
                    , index, $"Argument '{nameof(index)}' value '{index}' out of range.");
            }

            return ListFunc(b => b[index / 8] & (1 << (index % 8))) != 0;
        }

        // TODO: TBD: ditto Elastic Set / capture this interface as well...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="elasticity"></param>
        /// <returns></returns>
        public bool Get(int index, Elasticity elasticity)
        // TODO: TBD: what to do in the use case for Expansion ...
            => (elasticity.Contains(Expansion) || index < Length) && Get(index);

        /// <summary>
        /// Sets the Bit at <paramref name="index"/> to <paramref name="value"/>.
        /// The default behavior involves no <see cref="Elasticity"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <remarks>The default behavior involves no <see cref="Elasticity"/>.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is
        /// Less Than Zero or Greater Than Or Equal To <see cref="Length"/>.</exception>
        /// <inheritdoc />
        public void Set(int index, bool value)
        {
            if (index < 0 || index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index)
                    , index, $"Argument '{nameof(index)}' value '{index}' out of range");
            }

            /* We do not actually need the Functional result.
             * This is just a convenience method given the approach we are taking here. */
            ListFunc(b =>
            {
                var mask = (byte) (1 << (index % 8));
                return value ? (b[index / 8] |= mask) : (b[index / 8] &= (byte) ~mask);
            });
        }

        // TODO: TBD: consolidate the Set interface to the Elasticity one with default Elasticity...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="elasticity"></param>
        public void Set(int index, bool value, Elasticity elasticity)
        {
            if (elasticity.Contains(Expansion)
                && index >= Length
                && _bytes.Count < index / BitCount + 1)
            {
                _bytes.AddRange(new byte[index / BitCount + 1]);
                // TODO: TBD: or simply pass it through Length ?
                _length = index + 1;
            }

            Set(index, value);
        }

        /// <inheritdoc />
        public void SetAll(bool value)
            => ListAction(a =>
            {
                var len = Length;

                // Set all but the Last Byte.
                for (var i = 0; i < len - 1; i++)
                {
                    a[i] = value ? byte.MaxValue : byte.MinValue;
                }

                ListFunc(b =>
                {
                    var mask = MakeMask(0, len % 8);
                    return value ? (b[len / 8] |= mask) : (b[len / 8] &= (byte) ~mask);
                });
            });

        /// <inheritdoc />
        /// <see cref="Get(int)"/>
        /// <see cref="Set(int,bool)"/>
        public bool this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }

        internal IEnumerable<byte> InternalBytes() => from b in _bytes select b;

        /// <inheritdoc />
        public IEnumerable<byte> ToBytes(bool msb = true)
            => msb ? _bytes.ToArray().Reverse() : _bytes;

        private static IEnumerable<uint> ToInts(IEnumerable<byte> bytes, bool msb = true)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            var values = bytes.ToArray();

            const int size = sizeof(uint);

            // When there are Any we are guaranteed at least One result.
            while (values.Any())
            {
                // This is a byte faster than looping even though we are talking about a handful of bytes.
                if (values.Length < size)
                {
                    values = values.Concat(new byte[size - values.Length]).ToArray();
                }

                // The Array is ordered front to back in LSB, whereas the conversion may want MSB.
                yield return ToUInt32(IsLittleEndian
                    ? values.Take(size).ToArray()
                    : values.Take(size).Reverse().ToArray(), 0);

                // Carry on with the Next iteration if necessary.
                values = values.Skip(size).ToArray();
            }
        }

        // TODO: TBD: need a ctor corresponding to this one, to/from uints ...
        // TODO: TBD: I think the idea of "msb" in this instance was perhaps misinformed, is better to track with the IsLittleEndian ...
        /// <inheritdoc />
        public IEnumerable<uint> ToInts(bool msb = true) => ToInts(_bytes, msb);

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <summary>
        /// Not actually used, yet. We may have usage for this later on, however.
        /// </summary>
        /// <param name="shifts"></param>
        /// <returns></returns>
        /// <remarks>Marking it <see cref="ObsoleteAttribute"/> for now, however, hold onto it
        /// for the time being. It may be useful later on.</remarks>
        [Obsolete]
        private static byte MakeMask(params int[] shifts)
            => shifts.Aggregate(default(byte)
                , (g, shift) => (byte) (g | (byte) (1 << shift))
            );

        /// <summary>
        /// Returns the <see cref="byte"/> following appropriate masking shifting from
        /// <paramref name="iShift"/> through <paramref name="jShift"/>. Not to be confused
        /// with actual Bit Positions for purposes of indexing and so forth, per se.
        /// </summary>
        /// <param name="iShift"></param>
        /// <param name="jShift"></param>
        /// <returns></returns>
        internal static byte MakeMask(int iShift, int jShift)
        {
            var mask = default(byte);

            for (; iShift <= jShift; iShift++)
            {
                mask |= (byte) (1 << iShift);
            }

            return mask;
        }

        /// <summary>
        /// <see cref="Length"/> backing field.
        /// </summary>
        /// <see cref="Length"/>
        private int _length;

        /* TODO: TBD: for now, Length responds with Elasticity (default); future direction,
         * Elasticity should be a property of the bit array itself, which would potentially
         * constrain the behavior accordingly as to whether Contraction/Expansion is permitted. */

        /// <summary>
        /// Gets or Sets the Length of the BitArray.
        /// Getting the Length refers to <see cref="Count"/>.
        /// Setting the Length sets the actual length to the nearest <see cref="byte"/>.
        /// </summary>
        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <see cref="Max(byte,byte)"/>
        /// <see cref="MakeMask(int,int)"/>
        public int Length
        {
            get => _length;
            set
            {
                // Do nothing use case.
                if (value == _length)
                {
                    return;
                }

                // Values Less Than Zero should Never Ever be allowed past this point.
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value)
                        , value, $"Negative value '{value}' is invalid");
                }

                // In any event we want the DesiredCount.
                var desiredCount = value / BitCount + 1;

                /* Either Iterates Removing the Undesired Bytes,
                 or Adds a Range of Bytes to the end wholesale. */

                while (_bytes.Count != desiredCount)
                {
                    if (_bytes.Count < desiredCount)
                    {
                        _bytes.AddRange(new byte[desiredCount - _bytes.Count]);
                    }
                    else if (_bytes.Count > desiredCount)
                    {
                        _bytes.RemoveAt(_bytes.Count - 1);
                    }
                }

                /* Only in the event we have a Mid-Byte Length do we shave any undesired Bits
                 * from the Byte. Should work regardless whether Expanding or Contracting. */
                if (value % BitCount is int bitIndex && bitIndex != 0)
                {
                    _bytes[desiredCount - 1] = (byte) (_bytes[desiredCount - 1] & MakeMask(0, bitIndex - 1));
                }

                // Then assign the Length after we have our ducks in a row.
                _length = value;
            }
        }

        /// <inheritdoc />
        public void Clear() => ListAction(b =>
        {
            b.Clear();
            _length = 0;
        });

        /// <inheritdoc />
        /// <see cref="Length"/>
        /// <see cref="Set(int,bool)"/>
        public void Add(bool item) => Set(++Length - 1, item);

        /// <inheritdoc />
        /// <see cref="Length"/>
        public bool Remove(bool item)
        {
            /* We take the circuitous route here because we will need to work with the Index
             * anyway in order to do the appropriate Shift operation should a matching Item
             * be found. */
            for (var i = 0; i < Length; i++)
            {
                // TODO: TBD: after identifying the position of Item ...
                // TODO: TBD: this is a function shifting right one bit overlapping with that position.
                if (Get(i) == item)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        /// <see cref="Length"/>
        /// <see cref="Get(int)"/>
        public bool Contains(bool item)
        {
            /* This is more accurate since there may be Bits beyond Length
             * which we want to otherwise Ignore. */
            for (var i = 0; i < Length; i++)
            {
                if (Get(i) == item)
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        /// <see cref="Length"/>
        /// <see cref="Get(int)"/>
        public void CopyTo(bool[] array, int arrayIndex)
        {
            // Note the introduction of a second index Jay.
            for (int i = arrayIndex, j = 0; i < Length && j < array.Length; i++, j++)
            {
                // This is a bit safer approach and easier to reason about as well.
                array[j] = Get(i);
            }
        }

        /// <inheritdoc />
        public int Count => ListFunc(b => b.Count * 8);

        private static bool Equals(IEnumerable<byte> a, IEnumerable<byte> b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var minCount = Min(a.Count()
                // ReSharper disable once PossibleMultipleEnumeration
                , b.Count());

            bool NonZero(byte x) => x != 0;

            bool HasMoreNonZero(IEnumerable<byte> x) => x.Skip(minCount).Any(NonZero);

            bool Equals(byte x, byte y) => x == y;

            // ReSharper disable once PossibleMultipleEnumeration
            return a.Take(minCount)
                       // ReSharper disable once PossibleMultipleEnumeration
                       .Zip(b.Take(minCount), Equals).All(z => z)
                   // ReSharper disable once PossibleMultipleEnumeration
                   && !(HasMoreNonZero(a)
                        // ReSharper disable once PossibleMultipleEnumeration
                        || HasMoreNonZero(b));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equals(OptimizedImmutableBitArray a, OptimizedImmutableBitArray b)
            => Equals(a?._bytes, b?._bytes);

        /// <inheritdoc />
        public bool Equals(OptimizedImmutableBitArray other)
            => other != null && Equals(_bytes, other._bytes);

        /// <summary>
        /// Returns the Comparison of <paramref name="a"/> with <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int CompareTo(IList<byte> a, IList<byte> b)
        {
            const int greater = 1;

            if (a != null && b == null)
            {
                return greater;
            }

            const int lesser = -1;

            if (b != null && a == null)
            {
                return lesser;
            }

            // Accounts for the longer BitArray.
            bool IsLongerAndGreater(IEnumerable<byte> l, IEnumerable<byte> r)
            // ReSharper disable once PossibleMultipleEnumeration
                => l.Count()
                   // ReSharper disable once PossibleMultipleEnumeration
                   > r.Count()
                   // ReSharper disable once PossibleMultipleEnumeration
                   && l.Skip(
                       // ReSharper disable once PossibleMultipleEnumeration
                       r.Count()).Any(x => x != 0);

            // If any Bits at all are set beyond the Other, consider that one Greater.
            if (IsLongerAndGreater(a, b))
            {
                return greater;
            }

            if (IsLongerAndGreater(b, a))
            {
                return lesser;
            }

            /* Comparing each Byte is insufficient in this case. What we actually want to
             * determine is whether there are any bits set in a greater position that are not set
             * in the base instance. */

            // Compare both ends of the equation for Lesser or Greater outcomes.
            int? CompareSameLength(IEnumerable<byte> x, IEnumerable<byte> y, int result)
            {
                int i;

                // Nothing to do in this loop other than determine the boundary Index.
                // ReSharper disable once PossibleMultipleEnumeration
                for (i = x.Count() - 1;
                    // ReSharper disable once PossibleMultipleEnumeration
                    i >= 0 && x.ElementAt(i) == 0;
                    i--)
                {
                }

                // ReSharper disable once InvertIf
                if (i >= 0)
                {
                    // We may return early when there are any Bits set in B beyond the Boundary Byte.
                    // ReSharper disable once PossibleMultipleEnumeration
                    if (y.Skip(i + 1).Any(z => z != 0))
                    {
                        return result;
                    }

                    // ReSharper disable once PossibleMultipleEnumeration
                    if (x.ElementAt(i)
                            // ReSharper disable once PossibleMultipleEnumeration
                            .CompareTo(y.ElementAt(i)) is int zz && zz != 0)
                    {
                        return zz / Abs(zz);
                    }
                }

                int? Indeterminate() => null;

                // Otherwise, we must examine the Boundary Byte itself.
                return Indeterminate();
            }

            const int equal = 0;

            return CompareSameLength(a, b, lesser)
                   ?? CompareSameLength(b, a, greater)
                   ?? equal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int CompareTo(OptimizedImmutableBitArray a, OptimizedImmutableBitArray b)
            => CompareTo(a?._bytes, b?._bytes);

        /// <inheritdoc />
        public int CompareTo(OptimizedImmutableBitArray other) => CompareTo(_bytes, other?._bytes);

        /// <inheritdoc />
        public object Clone() => new OptimizedImmutableBitArray(_bytes.Select(x => x));
    }
}
