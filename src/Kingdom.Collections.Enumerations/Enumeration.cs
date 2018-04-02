using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Kingdom.Collections
{
    using static BindingFlags;
    using static ImmutableBitArray.Elasticity;

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
    public abstract class Enumeration
        : IEquatable<ImmutableBitArray>
            , IEquatable<object>
            , IComparable<ImmutableBitArray>
            , IComparable
    {
        /// <summary>
        /// Gets the DebuggerDisplayName.
        /// </summary>
        protected internal abstract string DebuggerDisplayName { get; }

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
            get { return _bits; }
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
        public bool IsZero
        {
            get { return Bits.All(x => !x); }
        }

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

        ////TODO: TBD: inspect for Properties as well as Fields? i.e. especially for so-called 'lazy' initialized? barring the fact we pretty much want to initialize as soon as at least one are to be referenced, and initialize for ordinals, bitwise, etc
        /// <summary>
        /// Defines an appropriate set of PublicStaticDeclaredOnly.
        /// </summary>
        /// <see cref="Public"/>
        /// <see cref="Static"/>
        /// <see cref="DeclaredOnly"/>
        protected const BindingFlags PublicStaticDeclaredOnly = Public | Static | DeclaredOnly;

        /// <summary>
        /// Name backing field.
        /// </summary>
        private string _name;

        /// <summary>
        /// Gets or sets the Name of the enum based on the <see cref="Type"/>.
        /// </summary>
        public string Name
        {
            get
            {
                TryResolveName(ref _name);
                return _name;
            }
            protected internal set { _name = value; }
        }

        /// <summary>
        /// Returns the declaring types.
        /// </summary>
        /// <param name="declaringTypes"></param>
        /// <returns></returns>
        protected static IEnumerable<Type> GetDeclaringTypes(params Type[] declaringTypes)
        {
            var types = declaringTypes
                .SelectMany(type => type.GetNestedTypes(PublicStaticDeclaredOnly))
                .Where(type => type.IsClass && type.IsStatic()).ToList();

            declaringTypes.ToList().ForEach(type => types.Insert(0, type));

            return types;
        }

        private IEnumerable<Type> GetDeclaringTypes()
        {
            var declaringTypes = GetType().GetEnumeratedValueTypes().ToArray();
            return GetDeclaringTypes(declaringTypes);
        }

        /// <summary>
        /// Tries to resolve the Enumerated Name.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TryResolveName(ref string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return true;
            }

            //TODO: may also want to restrict the field type(s) to those of the declaring type
            var field = GetDeclaringTypes()
                .SelectMany(type => type.GetFields(PublicStaticDeclaredOnly))
                .FirstOrDefault(info => ReferenceEquals(this, info.GetValue(null)));

            value = field?.Name ?? string.Empty;

            return field != null
                   && !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// DisplayName backing field.
        /// </summary>
        private string _displayName;

        /// <summary>
        /// Gets the DisplayName.
        /// </summary>
        /// <see cref="Name"/>
        public string DisplayName
        {
            get
            {
                TryResolveHumanReadableCamelCase(ref _displayName, Name);
                return _displayName;
            }
            protected set { _displayName = value; }
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

            var category = fi.GetCustomAttribute<CategoryAttribute>(false);

            value = category == null ? string.Empty : category.Category;

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
        /// Tries to resolve a DisplayName.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool TryResolveHumanReadableCamelCase(ref string value, string name)
        {
            if (value != null)
            {
                return true;
            }

            value = name.GetHumanReadableCamelCase();
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// In order for things to work out well, especially with serialization, to from
        /// data store, through object relational mapping, and things of this nature, the
        /// proposed length should be given in multiples of bits per byte (or 8).
        /// </summary>
        /// <param name="proposed"></param>
        /// <returns></returns>
        public static int NormalizeLength(int proposed)
        {
            const int bitsPerByte = 8;
            return bitsPerByte
                   * (proposed % bitsPerByte == 0
                       ? proposed
                       : (proposed / bitsPerByte + 1));
        }

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
        {
            Bits = bits;
            DisplayName = displayName;
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
        /// Returns the result after comparing Bits with the <paramref name="obj"/>. This may
        /// be either a <see cref="BitArray"/> or a <see cref="byte"/> array.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            var @enum = obj as Enumeration;
            return @enum != null
                ? CompareTo(@enum.Bits)
                : CompareTo(obj as ImmutableBitArray);
        }

        /// <summary>
        /// Returns whether <paramref name="a"/> Equals <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static bool Equals(byte[] a, byte[] b)
        {
            return !(a == null || b == null)
                   && (ReferenceEquals(a, b) || a.SequenceEqual(b));
        }

        /// <summary>
        /// Returns whether the <see cref="Bits"/> Equals the <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public bool Equals(ImmutableBitArray other)
        {
            return other != null && Bits.Equals(other);
        }

        /// <summary>
        /// Returns whether this object Equals an <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <inheritdoc cref="object" />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Enumeration yes, must also be same type.
            var @enum = obj as Enumeration;
            return @enum != null && @enum.GetType() == GetType()
                ? Equals(@enum.Bits)
                : Equals(obj as ImmutableBitArray);
        }

        /// <summary>
        /// Returns the hash code corresponding to the <see cref="ByteString"/>.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            // This is as good of a hash code as any.
            return ByteString.GetHashCode();
        }
    }
}
