using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kingdom.Collections
{
    //using static BindingFlags;
    using static Elasticity;

    /* ReSharper disable once UseNameofExpression
     Yes, in fact, do not want this to utilize string interpolation.
     Nor the nameof operator, etc, for that matter. */
    /// <summary>
    /// 
    /// </summary>
    /// <inheritdoc cref="IEquatable{T}" />
    /// <inheritdoc cref="IComparable{T}" />
    /// <inheritdoc cref="IComparable" />
    [DebuggerDisplay("{DebuggerDisplayName}")]
    public abstract class Enumeration : BaseEnumeration, IEquatable<ImmutableBitArray>, IComparable<ImmutableBitArray>
    {
        ///// <summary>
        ///// Gets the DebuggerDisplayName.
        ///// </summary>
        //protected internal abstract string DebuggerDisplayName { get; }

        /// <summary>
        /// Gets the ByteString associated with the long byte array <see cref="Bits"/>
        /// representation.
        /// </summary>
        protected string ByteString { get; private set; }

        /// <summary>
        /// Gets the SetBitCounts, the number of <see cref="Bits"/> that are actually set.
        /// </summary>
        protected long SetBitCount { get; private set; }

        /// <summary>
        /// Bits backing field.
        /// </summary>
        private ImmutableBitArray _bits;

        /// <summary>
        /// Gets the Bits involved in the Enumeration.
        /// </summary>
        public ImmutableBitArray Bits
        {
            get => _bits;
            protected set
            {
                _bits = value == null
                    ? new ImmutableBitArray(64)
                    : (ImmutableBitArray) value.Clone();
                // Make sure that we treat the bytes as LSB throughout.
                var bytes = _bits.ToBytes(false).ToArray();
                ByteString = bytes.ToByteString();
                SetBitCount = Enumerable.Range(0, _bits.Length).Count(x => _bits[x]);
            }
        }

        /// <summary>
        /// Returns whether IsZero.
        /// </summary>
        public bool IsZero => Bits.All(x => !x);

        /// <summary>
        /// Initializes the <see cref="Bits" /> in the <paramref name="values"/>. Assumes that the
        /// intended order of initialization has been resolved approaching a call to this helper
        /// method.
        /// </summary>
        /// <param name="values"></param>
        protected static void InitializeBits<T>(IEnumerable<T> values)
            where T : Enumeration
        {
            InitializeBits(values, 0x1, 0);
        }

        /// <summary>
        /// Initializes the <see cref="Bits" /> in the <paramref name="values"/>. Assumes that the
        /// intended order of initialization has been resolved approaching a call to this helper
        /// method.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="value"></param>
        /// <param name="shiftCount"></param>
        protected static void InitializeBits<T>(IEnumerable<T> values, uint value, int shiftCount)
            where T : Enumeration
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var starting = ImmutableBitArray.FromInts(new[] {value})
                .ShiftLeft(shiftCount, Expansion);

            values.ToList().Aggregate(starting, (b, v) =>
            {
                v.Bits = b;
                return b.ShiftLeft(elasticity: Expansion);
            });
        }

        /// <summary>
        /// Call ths method during the derived constructor when there is any question as to the
        /// consistency of the <see cref="BitArray.Length"/> of the <see cref="Bits"/>. The
        /// <paramref name="values"/> collection is not necessarily the same one as was passed
        /// into the <see cref="InitializeBits{T}(IEnumerable{T})"/> method.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="length">Default length is 8×32, or 0x100, or 256 bits.</param>
        /// <remarks>This method cannot be called from the scope of this class static constructor
        /// due to the fact that derived enumerated fields will not have been initialized at that
        /// moment. Therefore, this is the next best thing to helping ourselves out.</remarks>
        protected static void InitializeBitsLengths<T>(IEnumerable<T> values,
            int length = 8 * 32)
            where T : Enumeration
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (values == null || !values.Any())
            {
                throw new ArgumentNullException(nameof(values));
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var valuesList = values.ToList();
            // ReSharper disable once PossibleMultipleEnumeration
            var firstValue = values.First();

            if (firstValue.Bits.Length < length
                && !valuesList.Any(x => x.Bits.Length > length))
            {
                firstValue.Bits.Length = length;
            }

            // ReSharper disable once PossibleMultipleEnumeration
            var maxLength = values.Max(v => v.Bits.Length);
            valuesList.ForEach(v => v.Bits.Length = maxLength);
        }

        /// <summary>
        /// CategoryName backing field.
        /// </summary>
        private string _categoryName;

        /// <summary>
        /// Gets the CategoryName. Depends upon a field being optionally decorated
        /// by the <see cref="CategoryAttribute"/> attribute.
        /// </summary>
        /// <see cref="CategoryAttribute"/>
        public string CategoryName
        {
            get
            {
                TryResolveCategoryName(ref _categoryName);
                return _categoryName;
            }
        }

        /// <summary>
        /// Tries to resolve the CategoryName.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <see cref="CategoryAttribute"/>
        private bool TryResolveCategoryName(ref string value)
        {
            if (value != null)
            {
                return true;
            }

            var fi = GetDeclaringTypes().SelectMany(type => type.GetFields())
                .SingleOrDefault(info => ReferenceEquals(this, info.GetValue(null)));

            var category = fi?.GetCustomAttribute<CategoryAttribute>(false);

            value = category?.Category;

            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// CategoryDisplayName backing field.
        /// </summary>
        private string _categoryDisplayName;

        /// <summary>
        /// Gets the CategoryDisplayName.
        /// </summary>
        /// <see cref="CategoryName"/>
        public string CategoryDisplayName
        {
            get
            {
                TryResolveHumanReadableCamelCase(ref _categoryDisplayName, CategoryName);
                return _categoryDisplayName;
            }
        }

        /// <summary>
        /// 8
        /// </summary>
        private const int BitsPerByte = 8;

        /// <summary>
        /// In order for things to work out well, especially with serialization, to from
        /// data store, through object relational mapping, and things of this nature, the
        /// proposed length should be given in multiples of bits per byte (or 8).
        /// </summary>
        /// <param name="proposed"></param>
        /// <returns></returns>
        public static int NormalizeLength(int proposed)
            => BitsPerByte
               * (proposed % BitsPerByte == 0
                   ? proposed
                   : proposed / BitsPerByte + 1);

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="displayName"></param>
        /// <inheritdoc />
        protected Enumeration(IEnumerable<byte> bytes, string displayName = null)
            : this(new ImmutableBitArray(bytes), displayName)
        {
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="displayName"></param>
        protected Enumeration(ImmutableBitArray bits, string displayName = null)
            : base(displayName)
        {
            Bits = bits;
        }

        /// <summary>
        /// Returns the comparison of Bits with <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public int CompareTo(ImmutableBitArray other)
        {
            return other == null ? 1 : Bits.CompareTo(other);
        }

        /// <summary>
        /// Returns the result after comparing Bits with the <paramref name="other"/>.
        /// This may be either a <see cref="BitArray"/> or a <see cref="byte"/> array.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public override int CompareTo(object other)
            => other is Enumeration value
                ? CompareTo(value.Bits)
                : CompareTo(other as ImmutableBitArray);

        /// <summary>
        /// Returns whether <paramref name="a"/> Equals <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static bool Equals(byte[] a, byte[] b)
            => !(a == null || b == null)
               && (ReferenceEquals(a, b) || a.SequenceEqual(b));

        /// <summary>
        /// Returns whether the <see cref="Bits"/> Equals the <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public bool Equals(ImmutableBitArray other) => other != null && Bits.Equals(other);

        /// <summary>
        /// Returns whether this object Equals an <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc cref="object" />
        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            // Enumeration yes, must also be same type.
            return ReferenceEquals(this, other)
                   || (other is Enumeration value && value.GetType() == GetType()
                       ? Equals(value.Bits)
                       : Equals(other as ImmutableBitArray));
        }

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        /// <inheritdoc />
        public override int GetHashCode() => ByteString.GetHashCode();
    }
}
