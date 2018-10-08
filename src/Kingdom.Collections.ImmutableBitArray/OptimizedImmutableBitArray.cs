using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using static BitConverter;

    /// <inheritdoc />
    public partial class OptimizedImmutableBitArray : IElasticImmutableBitArray<OptimizedImmutableBitArray>
    {
        private readonly List<byte> _bytes;

        /// <inheritdoc />
        public OptimizedImmutableBitArray(params uint[] values)
            : this(values.SelectMany(x => IsLittleEndian ? GetBytes(x) : GetBytes(x).Reverse()))
        {
        }

        /// <summary>
        /// <see cref="byte"/> Array oriented Constructor.
        /// </summary>
        public OptimizedImmutableBitArray(params byte[] bytes)
        {
            _bytes = bytes.ToList();
            _length = _bytes.Count * 8;
        }

        /// <summary>
        /// <see cref="byte"/> oriented Constructor.
        /// </summary>
        public OptimizedImmutableBitArray(IEnumerable<byte> bytes)
        {
            _bytes = bytes.ToList();
            _length = _bytes.Count * 8;
        }

        /// <summary>
        /// <see cref="bool"/> oriented Constructor.
        /// </summary>
        /// <param name="bits"></param>
        public OptimizedImmutableBitArray(IEnumerable<bool> bits)
        {
            _bytes = new List<byte>();
            _length = 0;

            // ReSharper disable once PossibleMultipleEnumeration
            Length = bits.Count();

            // Emplace each of the new set of Bits.
            for (var i = 0; i < Length; i++)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                Set(i, bits.ElementAt(i));
            }
        }

        /// <inheritdoc />
        public IEnumerator<bool> GetEnumerator() => ListFunc(b => GetBooleans(b, Length).GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // TODO: TBD: start with Shift Left/Right base-zero address.
        // TODO: TBD: would be interesting to add a base address however
        // TODO: TBD: next: what to do about shifting, starting position, etc
        // TODO: TBD: this is by far the hardest part about the whole procedure...

        /// <inheritdoc />
        public bool IsReadOnly => false;

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
        /// <see cref="IEnumerable{T}"/>
        /// <see cref="Get(int)"/>
        /// <see cref="Length"/>
        public bool Contains(bool item) => this.Any(x => x == item);

        /// <inheritdoc />
        /// <see cref="Length"/>
        /// <see cref="Get(int)"/>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// <paramref name="arrayIndex"/> exceeds <see cref="Length"/>.</exception>
        public void CopyTo(bool[] array, int arrayIndex)
        {
            // ReSharper disable once InconsistentNaming
            var this_Length = Length;

            if (arrayIndex < 0 || arrayIndex >= this_Length)
            {
                /* This would be trapped by the call to Get as well, but let's
                 * arrest the issue early for CopyTo just the same. */
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex
                    , $"'{typeof(OptimizedImmutableBitArray).FullName}.{nameof(CopyTo)}'"
                      + $" argument {nameof(arrayIndex)} '{arrayIndex}' out of range.");
            }

            // ReSharper disable once InconsistentNaming
            var array_Length = array.Length;

            // Note the introduction of a second index Jay.
            for (int i = arrayIndex, j = 0; i < this_Length && j < array_Length; i++, j++)
            {
                // This is a bit safer approach and easier to reason about as well.
                array[j] = Get(i);
            }
        }

        /// <inheritdoc />
        public object Clone() => new OptimizedImmutableBitArray(_bytes.Select(x => x));
    }
}
