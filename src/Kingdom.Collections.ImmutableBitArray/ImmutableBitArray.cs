using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    /// <summary>
    /// Provides a &quot;morally equivalent&quot; (in huge quotes, so called) implementation of
    /// <see cref="ImmutableBitArray"/>, similar to the framework <see cref="BitArray"/>. This
    /// became necessary because we noticed that the framework implementation is not immutable
    /// in critical places, and it is a fairly simple thing to implement. Admitedly, immutable
    /// is perhaps not the best name when idempotence is closer to what I wanted to achieve here.
    /// Especially concerning operations such as twos complement, which should most definitely
    /// leave the original instance unchanged.
    /// </summary>
    /// <inheritdoc cref="IImmutableBitArray{T}"/>
    /// <see cref="!:http://en.wikipedia.org/wiki/Idempotence"/>
    /// <see cref="!:http://en.wikipedia.org/wiki/Immutable_object"/>
    public class ImmutableBitArray : IImmutableBitArray<ImmutableBitArray>
    {
        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// Values backing field.
        /// </summary>
        protected List<bool> _values;

        private void SetValues<T>(IEnumerable<T> values, Func<int> getSize,
            Func<IEnumerable<T>, int, int, bool> testValue)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var count = values.Count();
            var size = getSize();
            if ((long) count * size > int.MaxValue)
            {
                throw new ArgumentException("values bit length exceeds Int32.MaxValue", nameof(values));
            }

            _values = new List<bool>();
            var innerRange = Enumerable.Range(0, size).ToArray();
            for (var i = 0; i < count; i++)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                _values.AddRange(innerRange.Select(j => testValue(values, i, j)));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableBitArray"/> class that contains
        /// bit values copied from the specified <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="ImmutableBitArray"/> to copy.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null</exception>
        public ImmutableBitArray(ImmutableBitArray other)
            : this(other._values)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableBitArray"/> class that contains
        /// bit values copied from the specified array of <see cref="bool"/>s.
        /// </summary>
        /// <param name="values">An array of <see cref="bool"/>s to copy</param>
        /// <exception cref="ArgumentNullException"><paramref name="values"/> is null</exception>
        public ImmutableBitArray(IEnumerable<bool> values)
        {
            SetValues(values.ToArray(), () => 1, (arr, i, j) => arr.ElementAt(i));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableBitArray"/> class that contains
        /// bit values copied from the specified array of bytes in LSB.
        /// </summary>
        /// <param name="bytes">An array of bytes containing the values to copy, where each byte
        /// represents eight consecutive bits.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="bytes"/> is greater
        /// than <see cref="int.MaxValue"/>.</exception>
        public ImmutableBitArray(IEnumerable<byte> bytes)
        {
            SetValues(bytes, () => sizeof(byte) * 8, (arr, i, j) => (arr.ElementAt(i) & 1 << j) != 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableBitArray"/> class that contains
        /// bit values copied from the specified array of 32-bit integers in LSB.
        /// </summary>
        /// <param name="uints">An array of integers containing the values to copy, where each
        /// integer represents 32 consecutive bits.</param>
        /// <exception cref="ArgumentNullException"><paramref name="uints"/> is null.</exception>
        /// <exception cref="ArgumentException">The length of <paramref name="uints"/> is greater
        /// than <see cref="int.MaxValue"/>.</exception>
        public ImmutableBitArray(IEnumerable<uint> uints)
        {
            SetValues(uints, () => sizeof(uint) * 8
                , (arr, i, j) => (arr.ElementAt(i) & 1 << j) != 0);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableBitArray"/> class that can hold
        /// the specified number of bit values, which are initially set to false.
        /// </summary>
        /// <param name="length">The number of bit values in the new <see cref="ImmutableBitArray"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than
        /// zero.</exception>
        /// <inheritdoc />
        public ImmutableBitArray(int length)
            : this(length, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableBitArray"/> class that can hold
        /// the specified number of bit values, which are initially set to the specified value.
        /// </summary>
        /// <param name="length">The number of bit values in the new <see cref="ImmutableBitArray"/>.</param>
        /// <param name="defaultValue">The <see cref="bool"/> value to assign to each bit.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is less than
        /// zero.</exception>
        public ImmutableBitArray(int length, bool defaultValue)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            _values = Enumerable.Range(0, length).Select(_ => defaultValue).ToList();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the entire <see cref="ImmutableBitArray"/>.</returns>
        /// <inheritdoc />
        public IEnumerator<bool> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> for the entire <see cref="ImmutableBitArray"/>.</returns>
        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <inheritdoc />
        public void Add(bool value)
        {
            _values.Add(value);
        }

        /// <summary>
        /// Clears the Values.
        /// </summary>
        /// <inheritdoc />
        public void Clear()
        {
            _values.Clear();
        }

        /// <summary>
        /// Returns whether Contains the <paramref name="item"/>.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public bool Contains(bool item)
        {
            return _values.Any(x => x == item);
        }

        /// <summary>
        /// Copies the entire <see cref="ImmutableBitArray"/> to a compatible one-dimensional
        /// <see cref="Array"/>, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the
        /// destination of the elements copied from <see cref="ImmutableBitArray"/>. The
        /// <see cref="Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based <paramref name="arrayIndex"/> in array at
        /// which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less
        /// than zero.</exception>
        /// <exception cref="ArgumentException"><paramref name="array"/> is multidimensional;
        /// or, the number of elements in the source <see cref="ImmutableBitArray"/> is greater than
        /// the available space from <paramref name="arrayIndex"/> to the end of the destination
        /// <paramref name="array"/>.</exception>
        /// <exception cref="InvalidCastException">The type of the source <see cref="ImmutableBitArray"/>
        /// cannot be cast automatically to the type of the destination
        /// <paramref name="array"/>.</exception>
        /// <inheritdoc />
        public void CopyTo(bool[] array, int arrayIndex)
        {
            _values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the <paramref name="item"/> from the Collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public bool Remove(bool item)
        {
            var i = _values.IndexOf(item);

            if (i < 0)
            {
                return false;
            }

            _values.RemoveAt(i);
            return true;
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <returns>The number of elements contained in the <see cref="ImmutableBitArray"/>.</returns>
        /// <inheritdoc />
        public int Count => _values.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="ImmutableBitArray"/> is read-only.
        /// </summary>
        /// <returns>This property is always false.</returns>
        /// <inheritdoc />
        public bool IsReadOnly => false;

        private static void VerifyLength(IList<bool> values, int expectedLength)
        {
            while (expectedLength < values.Count)
            {
                values.RemoveAt(expectedLength);
            }

            while (expectedLength > values.Count)
            {
                values.Add(false);
            }
        }

        /// <summary>
        /// Gets or sets the number of elements in the <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <returns>The number of elements in the <see cref="ImmutableBitArray"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The property is set to a value that
        /// is less than zero.</exception>
        /// <seealso cref="Get"/>
        /// <seealso cref="Set"/>
        /// <inheritdoc />
        public int Length
        {
            get { return _values.Count; }
            set { VerifyLength(_values, value); }
        }

        private static ImmutableBitArray BitwiseFunc(IEnumerable<IList<bool>> valuesArr,
            Func<IList<bool>, bool> func)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            var maxCount = valuesArr.Max(arr => arr.Count);
            var values = new List<bool>();
            for (var index = 0; index < maxCount; index++)
            {
                var i = index;
                // ReSharper disable once PossibleMultipleEnumeration
                var slice = valuesArr.Select(arr => i < arr.Count && arr[i]).ToList();
                values.Add(func(slice));
            }

            return new ImmutableBitArray(values);
        }

        /// <summary>
        /// Performs an immutable bitwise AND operation on the elements in the current
        /// <see cref="ImmutableBitArray"/> against the corresponding elements in the specified
        /// <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="ImmutableBitArray"/> with which to perform the bitwise AND
        /// operation.</param>
        /// <returns> An immutable instance containing the result of the bitwise AND operation on
        /// the elements in the current <see cref="ImmutableBitArray"/> against the corresponding elements
        /// in the specified <see cref="ImmutableBitArray"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        /// <inheritdoc />
        public ImmutableBitArray And(ImmutableBitArray other) => And(new[] {other});

        /// <summary>
        /// Performs an immutable bitwise AND operation on the elements in the current
        /// <see cref="ImmutableBitArray"/> against the corresponding elements in the specified
        /// <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <param name="others">The <see cref="ImmutableBitArray"/>(s) with which to perform the
        /// bitwise AND operation.</param>
        /// <returns> An immutable instance containing the result of the bitwise AND operation on
        /// the elements in the current <see cref="ImmutableBitArray"/> against the corresponding
        /// elements in the specified <see cref="ImmutableBitArray"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="others"/> is null.</exception>
        /// <inheritdoc />
        public ImmutableBitArray And(IEnumerable<ImmutableBitArray> others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            return BitwiseFunc(new[] {this}.Concat(others).Select(o => o._values)
                .ToList(), slice => slice.All(x => x));
        }

        /// <summary>
        /// Performs the bitwise OR operation on the elements in the current
        /// <see cref="ImmutableBitArray"/> against the corresponding elements in the specified
        /// <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="ImmutableBitArray"/> with which to perform the bitwise OR
        /// operation.</param>
        /// <returns>The current instance containing the result of the bitwise OR operation on     
        /// the elements in the current <see cref="ImmutableBitArray"/> against the corresponding elements
        /// in the specified <see cref="ImmutableBitArray"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        /// <inheritdoc />
        public ImmutableBitArray Or(ImmutableBitArray other)
        {
            return Or(new[] {other});
        }

        /// <summary>
        /// Performs the bitwise OR operation on the elements in the current
        /// <see cref="ImmutableBitArray"/> against the corresponding elements in the specified
        /// <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <param name="others">The <see cref="ImmutableBitArray"/>(s) with which to perform the
        /// bitwise OR operation.</param>
        /// <returns>The current instance containing the result of the bitwise OR operation on     
        /// the elements in the current <see cref="ImmutableBitArray"/> against the corresponding
        /// elements in the specified <see cref="ImmutableBitArray"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="others"/> is null.</exception>
        /// <inheritdoc />
        public ImmutableBitArray Or(IEnumerable<ImmutableBitArray> others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            return BitwiseFunc(new[] {this}.Concat(others).Select(x => x._values)
                .ToList(), slide => slide.Any(x => x));
        }

        /// <summary>
        /// Performs the bitwise exclusive OR operation on the elements in the current
        /// <see cref="ImmutableBitArray"/> against the corresponding elements in the specified
        /// <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <param name="other">The <see cref="ImmutableBitArray"/> with which to perform the bitwise
        /// exclusive OR operation.</param>
        /// <returns>The current instance containing the result of the bitwise exclusive OR
        /// operation on the elements in the current <see cref="ImmutableBitArray"/> against the
        /// corresponding elements in the specified <see cref="ImmutableBitArray"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="other"/> is null.</exception>
        /// <inheritdoc />
        public ImmutableBitArray Xor(ImmutableBitArray other) => Xor(new[] {other});

        /// <summary>
        /// Performs the bitwise exclusive OR operation on the elements in the current
        /// <see cref="ImmutableBitArray"/> against the corresponding elements in the specified
        /// <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <param name="others">The <see cref="ImmutableBitArray"/> with which to perform the
        /// bitwise exclusive OR operation.</param>
        /// <returns>The current instance containing the result of the bitwise exclusive OR
        /// operation on the elements in the current <see cref="ImmutableBitArray"/> against the
        /// corresponding elements in the specified <see cref="ImmutableBitArray"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="others"/> is null.</exception>
        /// <inheritdoc />
        public ImmutableBitArray Xor(IEnumerable<ImmutableBitArray> others)
        {
            if (others == null)
            {
                throw new ArgumentNullException(nameof(others));
            }

            return BitwiseFunc(new[] {this}.Concat(others).Select(x => x._values)
                .ToList(), slice =>
            {
                // http://en.wikipedia.org/wiki/Exclusive_or
                var count = slice.Count(x => x);
                // XOR means this: false = all false or all true; true is a mixed result
                return !(count == 0 || count == slice.Count);
            });
        }

        /// <summary>
        /// Inverts all the bit values in the current <see cref="ImmutableBitArray"/>, so that elements
        /// set to true are changed to false, and elements set to false are changed to true.
        /// </summary>
        /// <returns>The current instance with inverted bit values.</returns>
        /// <inheritdoc />
        public ImmutableBitArray Not() => new ImmutableBitArray(_values.Select(x => !x).ToArray());

        /// <summary>
        /// Shifts the current <see cref="ImmutableBitArray"/> left by the
        /// <paramref name="count"/> number of bits. Optionally expands the
        /// bit array depending on the value of <paramref name="elastic"/>.
        /// </summary>
        /// <param name="count">The number of bits to shift left.</param>
        /// <param name="elastic">Optionally expands the length of the bit array.</param>
        /// <returns>A new instance with the bits shifted left by the <paramref name="count"/>.</returns>
        /// <inheritdoc />
        public ImmutableBitArray ShiftLeft(int count = 1, bool elastic = false)
        {
            if (count < 0)
            {
                throw new ArgumentException("count must be greater than or equal to zero", nameof(count));
            }

            // Allow for zero count. Consistent with idempotency rules, return its clone.
            if (count == 0)
            {
                return (ImmutableBitArray) Clone();
            }

            var length = Length;
            // Here is a unique corner case of the shift left operation.
            if (!elastic && length == 0)
            {
                return new ImmutableBitArray(new bool[0]);
            }

            var values = Enumerable.Range(0, count).Select(_ => false).Concat(_values).ToArray();

            if (!elastic)
            {
                // Inelastic Shifting is great, only take the Length of the original source.
                values = values.Take(length).ToArray();
            }
            else
            {
                while (count-- > 0)
                {
                    if (values.Last())
                    {
                        break;
                    }

                    // Restrict the width when Clear Values permit us to do so.
                    values = values.Take(values.Length - 1).ToArray();
                }
            }

            return new ImmutableBitArray(values);
        }

        /// <summary>
        /// Shifts the current <see cref="ImmutableBitArray"/> right by the
        /// <paramref name="count"/> number of bits. Optionally contracts the
        /// bit array depending on the value of <paramref name="elastic"/>.
        /// </summary>
        /// <param name="count">The number of bits to shift right.</param>
        /// <param name="elastic">Optionally contracts the length of bit array.</param>
        /// <returns>A new instance with the bits shifted right by the <paramref name="count"/>.</returns>
        /// <inheritdoc />
        public ImmutableBitArray ShiftRight(int count = 1, bool elastic = false)
        {
            if (count < 0)
            {
                throw new ArgumentException("count must be greater than or equal to zero", nameof(count));
            }

            // Allow for zero count. Consistent with idempotency rules, return its clone.
            if (count == 0)
            {
                return (ImmutableBitArray) Clone();
            }

            var length = Length;
            // This one is a unique corner case of thie shift right operation.
            if (length == 0 || (elastic && count >= length))
            {
                return new ImmutableBitArray(new bool[0]);
            }

            var values = _values.Concat(Enumerable.Range(0, count).Select(_ => false));
            var skipped = values.Skip(count);
            /* TODO: TBD: not sure 'elastic' is quite the right description in this case;
             more like whether we should allow shrinkage... along similar lines as with
             ShiftLeft, the conditions of which may not be so concise as first thought...
             may need/want to re-consider what this is for ShiftRight... */
            return new ImmutableBitArray(elastic ? skipped.Take(length - count) : skipped);
        }

        /// <summary>
        /// Gets the value of the bit at a specific position in the <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <param name="index">The zero-based <paramref name="index"/> of the value to get.</param>
        /// <returns>The value of the bit at position <paramref name="index"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.
        /// There is no upper limit for <paramref name="index"/>, but does not expand the number of
        /// elements within.</exception>
        /// <inheritdoc />
        public bool Get(int index) => index < _values.Count && _values[index];

        /// <summary>
        /// Sets the bit at a specific position in the <see cref="ImmutableBitArray"/> to the specified
        /// <paramref name="value"/>.
        /// </summary>
        /// <param name="index">The zero-based <paramref name="index"/> of the bit to set.</param>
        /// <param name="value">The Boolean <paramref name="value"/> to assign to the bit.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than
        /// zero.</exception>
        /// <inheritdoc />
        public void Set(int index, bool value)
        {
            if (index > Length - 1)
            {
                Length = index + 1;
            }

            _values[index] = value;
        }

        /// <summary>
        /// Gets or sets the value of the bit at a specific position in the
        /// <see cref="ImmutableBitArray"/>. Ensures that the internals can support questions when
        /// <paramref name="index"/> is greater than the current <see cref="Length"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the value to get or set.</param>
        /// <returns>The value of the bit at position index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than
        /// zero.</exception>
        /// <inheritdoc />
        public bool this[int index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }

        /// <summary>
        /// Sets all bits in the <see cref="ImmutableBitArray"/> to the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The Boolean <paramref name="value"/> to assign to all bits.</param>
        /// <inheritdoc />
        public void SetAll(bool value)
        {
            for (var i = 0; i < _values.Count; i++)
            {
                _values[i] = value;
            }
        }

        private static bool Equals(IImmutableBitArray a, IImmutableBitArray b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a.Length == b.Length)
            {
                return a.SequenceEqual(b);
            }

            var min = Math.Min(a.Length, b.Length);
            var aRemaining = a.Skip(min);
            var bRemaining = b.Skip(min);

            // Not sure what I was thinking with the previous version.
            return a.Take(min).SequenceEqual(b.Take(min))
                   && !(aRemaining.Any(x => x) || bRemaining.Any(x => x));
        }

        /// <summary>
        /// Returns whether this instance Equals the <paramref name="other"/> instance.
        /// </summary>
        /// <param name="other">An <paramref name="other"/> instance.</param>
        /// <returns>Whether this instance Equals the <paramref name="other"/> instance.</returns>
        /// <inheritdoc />
        public bool Equals(ImmutableBitArray other) => Equals(this, other);

        private static int CompareTo(IImmutableBitArray a, IImmutableBitArray b)
        {
            if (ReferenceEquals(a, b))
            {
                return 0;
            }

            // If there are any remaining then we know that this is automatically greater.
            var min = Math.Min(a.Length, b.Length);
            var aRemaining = a.Skip(min);
            var bRemaining = b.Skip(min);

            if (aRemaining.Any(x => x))
            {
                return 1;
            }

            if (bRemaining.Any(x => x))
            {
                return -1;
            }

            // Examine the msb to the lsb in descending order. We iterate this regardless whether lengths were the same.
            for (var i = min; i >= 0; i--)
            {
                var aValue = a.ElementAt(i);
                var bValue = b.ElementAt(i);

                if (aValue && !bValue)
                {
                    return 1;
                }

                if (bValue && !aValue)
                {
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Returns the comparison of this instance with the <paramref name="other"/> instance.
        /// </summary>
        /// <param name="other">An <paramref name="other"/> instance.</param>
        /// <returns>The comparison of this instance with the <paramref name="other"/> instance.</returns>
        /// <inheritdoc />
        public int CompareTo(ImmutableBitArray other) => CompareTo(this, other);

        private IEnumerable<T> ToValues<T>(Func<int> getSize, Func<T> getDefaultValue,
            Func<int, T> getShifted, Func<T, T, T> mergeValue, bool msb)
        {
            var size = getSize();
            var defaultValue = getDefaultValue();
            var values = new List<T>();
            for (var i = 0; i < _values.Count; i++)
            {
                var j = i / size;
                if (values.Count < j + 1)
                {
                    if (msb)
                    {
                        values.Insert(0, defaultValue);
                    }
                    else
                    {
                        values.Add(defaultValue);
                    }
                }

                if (!_values[i])
                {
                    continue;
                }

                if (msb)
                {
                    // TODO: TBD: this may need to be a little bit different for Int32 vs. Byte...
                    values[0] = mergeValue(values[0], getShifted(i % size));
                }
                else
                {
                    values[j] = mergeValue(values[j], getShifted(i % size));
                }
            }

            return values;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{Byte}"/> in either <paramref name="msb"/>.
        /// </summary>
        /// <param name="msb"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public IEnumerable<byte> ToBytes(bool msb = true)
        {
            // This should be equally as quick whether MSB or not.
            return ToValues<byte>(() => sizeof(byte) * 8, () => 0,
                shift => (byte) (1 << shift), (a, b) => (byte) (a | b), msb);
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{UInt32}"/> in either <paramref name="msb"/>.
        /// </summary>
        /// <param name="msb"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public IEnumerable<uint> ToInts(bool msb = true)
        {
            // TODO: TBD: whether/how to handle msb?
            return ToValues<uint>(() => sizeof(uint) * 8, () => 0,
                shift => (uint) 1 << shift, (a, b) => a | b, msb);
        }

        /// <summary>
        /// Returns a newly created bit array based on the <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="msb"></param>
        /// <returns></returns>
        public static ImmutableBitArray FromBytes(IEnumerable<byte> bytes, bool msb = true)
        {
            bytes = (msb ? bytes.Reverse() : bytes) ?? new byte[0];
            return new ImmutableBitArray(bytes);
        }

        /// <summary>
        /// Returns a newly created bit array based on the <paramref name="uints"/>.
        /// </summary>
        /// <param name="uints"></param>
        /// <returns></returns>
        public static ImmutableBitArray FromInts(IEnumerable<uint> uints)
        {
            // TODO: TBD: whether/how to handle msb?
            uints = uints ?? new uint[0];
            return new ImmutableBitArray(uints);
        }

        /// <summary>
        /// Creates a copy of the <see cref="ImmutableBitArray"/>.
        /// </summary>
        /// <returns>A copy of the <see cref="ImmutableBitArray"/>.</returns>
        /// <inheritdoc />
        public virtual object Clone() => new ImmutableBitArray(this);
    }
}
