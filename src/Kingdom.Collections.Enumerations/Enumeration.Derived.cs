using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingdom.Collections
{
    using static Type;
    using static BitConverter;
    using static BindingFlags;

    /// <summary>
    /// Enumeration implementation.
    /// </summary>
    /// <typeparam name="TDerived"></typeparam>
    /// <remarks>From 17.4.5.1 Static field initialization, If a static constructor exists in the
    /// class, execution of the static field initializers occurs immediately prior to executing
    /// that static constructor. Otherwise, (and this is the key), the static field initializers
    /// are executed at an implementation-dependent time prior to the first use of a static field
    /// of that class. I am not sure the documentation refers to initialization of static
    /// constructors with respect to other static constructors, whereas this use case is a little
    /// bit different animal than even that. It is best to be as declarative as possible so as not
    /// to confuse the issue behind mysterious runtime issues.</remarks>
    /// <see cref="!:http://www.ecma-international.org/publications/standards/Ecma-334.htm"/>
    /// <inheritdoc cref="Enumeration"/>
    public abstract partial class Enumeration<TDerived>
        : Enumeration
            , IComparable<TDerived>
            , IEquatable<TDerived>
        where TDerived : Enumeration<TDerived>
    {
        /// <summary>
        /// Gets the DebuggerDisplayName.
        /// </summary>
        /// <see cref="BaseEnumeration.DisplayName"/>
        /// <see cref="Enumeration.Bits"/>
        /// <remarks>This one really should be sealed, but for the fact that we further
        /// extend from here in order to achieve Ordinal Enumerations.</remarks>
        /// <inheritdoc cref="Enumeration"/>
        protected internal override string DebuggerDisplayName
        {
            get
            {
                string GetDisplayName()
                {
                    try
                    {
                        return DisplayName;
                    }
                    catch
                    {
                        return null;
                    }
                }

                var displayName = GetDisplayName();
                //var byteString = Bits.ToByteString(false).TrimRight(zero);
                var byteString = Bits.ToByteString(false);
                return string.IsNullOrEmpty(displayName)
                    ? $@"0x{byteString}"
                    : $@"{displayName} (0x{byteString})";
            }
        }

        /// <summary>
        /// Gets the Default <see cref="Enumeration"/>.
        /// </summary>
        public static TDerived Default => Values.First();

        /// <summary>
        /// Returns the bytes for <paramref name="ordinal"/> in little endian manner.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        /// <remarks>This says ordinal, which is necessary in support of the derived class, but
        /// this is necessary here in support of a baseline minimum of bytes under certain use
        /// cases.</remarks>
        protected static IEnumerable<byte> GetOrdinalBytes(int ordinal = default(int))
        {
            var result = GetBytes(ordinal);

            // The underlying bit array wants little endian.
            foreach (var b in IsLittleEndian ? result : result.Reverse())
            {
                yield return b;
            }
        }

        /// <summary>
        /// Verifies that the <paramref name="bytes"/> have at least some value, even if it is of
        /// a default value.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static IEnumerable<byte> VerifyBytes(params byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                bytes = GetOrdinalBytes().ToArray();
            }

            foreach (var b in bytes)
            {
                yield return b;
            }
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="bytes"></param>
        /// <inheritdoc />
        protected Enumeration(params byte[] bytes)
            : base(VerifyBytes(bytes).ToArray())
        {
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="displayName"></param>
        /// <inheritdoc />
        protected Enumeration(byte[] bytes, string displayName)
            : base(VerifyBytes(bytes).ToArray(), displayName)
        {
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="bits"></param>
        /// <inheritdoc />
        protected Enumeration(ImmutableBitArray bits)
            : base(bits)
        {
        }

        /// <summary>
        /// Values backing field.
        /// </summary>
        private static IEnumerable<TDerived> _values;

        /// <summary>
        /// Gets the enumerated Values from the <typeparamref name="TDerived"/> type.
        /// </summary>
        public static IEnumerable<TDerived> Values => _values ?? (_values = GetValues());

        /// <summary>
        /// Returns the enumerated Values.
        /// </summary>
        /// <param name="ignoreNulls"></param>
        /// <returns></returns>
        public static IEnumerable<TDerived> GetValues(bool ignoreNulls = true)
        {
            // Keep the formatting and implementation this way for troubleshooting purposes.
            var declaringTypes = GetDeclaringTypes(typeof(TDerived)).Reverse().ToArray();

            // TODO: TBD: determine how to treat Null values... at the moment, it seems as though Null is being ignored...
            foreach (var values in declaringTypes
                .Select(t => t.GetFields(PublicStaticDeclaredOnly))
                .Select(fis => fis.Select(info => info.GetValue(null)).ToArray()))
            {
                foreach (var value in values.Where(x => !ignoreNulls || x != null))
                {
                    yield return (TDerived) value;
                }

                //// TODO: TBD: this may work, but I'm not sure, if memory serves, there was a problem with this approach?
                //foreach (var value in values.OfType<TDerived>())
                //{
                //    yield return value;
                //}
            }
        }

        /// <summary>
        /// Gets the NestedClassTypes.
        /// </summary>
        private static IEnumerable<Type> NestedStaticClassTypes
            => typeof(TDerived).GetNestedTypes(PublicStaticDeclaredOnly)
                .Where(type => type.IsClass && type.IsStatic()).ToArray();

        /// <summary>
        /// Returns the comparison between <paramref name="a"/> and <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        protected static int CompareTo(TDerived a, TDerived b)
        {
            if (a != null && b == null)
            {
                return 1;
            }

            return a?.Bits.CompareTo(b.Bits)
                   ?? (b == null ? 0 : -1);
        }

        /// <summary>
        /// Returns the comparison between this object and the <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public virtual int CompareTo(TDerived other) => CompareTo(this as TDerived, other);

        /// <summary>
        /// Returns whether <paramref name="a"/> Equals <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equals(TDerived a, TDerived b)
            => !(a == null || b == null)
               && (ReferenceEquals(a, b)
                   || a.Bits.Equals(b.Bits));

        /// <summary>
        /// Returns whether this instance Equals the <paramref name="other"/> one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public virtual bool Equals(TDerived other) => Equals(this as TDerived, other);

        /// <summary>
        /// Returns whether this object is GreaterThan an <paramref name="other"/> one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool GreaterThan(TDerived other) => CompareTo((TDerived) this, other) > 0;

        /// <summary>
        /// Returns whether this object is LessThan an <paramref name="other"/> one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool LessThan(TDerived other) => CompareTo((TDerived) this, other) < 0;

        /// <summary>
        /// Returns whether this object is GreaterThanOrEqual an <paramref name="other"/> one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool GreaterThanOrEqual(TDerived other) => CompareTo((TDerived) this, other) >= 0;

        /// <summary>
        /// Returns whether this object is LessThanOrEqual an <paramref name="other"/> one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool LessThanOrEqual(TDerived other) => CompareTo((TDerived) this, other) <= 0;

        /// <summary>
        /// Returns the string representation of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Returns the <typeparamref name="TDerived"/> instance derived from the
        /// <paramref name="bytes"/>.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// <see cref="FromBitArray"/>
        public static TDerived FromBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes), $"bytes values are required for an {typeof(TDerived)}");
            }

            if (bytes.Length > 0)
            {
                // Make sure that the bytes are dealt with in LSB order.
                return FromBitArray(ImmutableBitArray.FromBytes(bytes, false));
            }

            throw new ArgumentException($"bytes values are required for an {typeof(TDerived)}", nameof(bytes));
        }

        /// <summary>
        /// BitsLookup backing field.
        /// </summary>
        private static IDictionary<TDerived, TDerived> _bitsLookup;

        /// <summary>
        /// Gets the BitsLookup.
        /// </summary>
        private static IDictionary<TDerived, TDerived> BitsLookup
            => _bitsLookup
               ?? (_bitsLookup = Values.ToDictionary(x => x, x => x));

        /// <summary>
        /// Returns the <typeparamref name="TDerived"/> derived instance corresponding to the
        /// <paramref name="bits"/>.
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        /// <see cref="FromBytesCtor"/>
        public static TDerived FromBitArray(ImmutableBitArray bits)
        {
            // This seems a bit redundant since the value is its own key...
            var @default = FromBytesCtor(bits.ToBytes(false).ToArray());

            //TODO: dictionary would be preferred, but for some reason it's not "catching" on the correct values...
            // We either want the enumerated value itself or the factory created version.
            var result = Values.FirstOrDefault(x => x.Equals(@default));

            return result ?? @default;
        }

        /// <summary>
        /// Returns a new instance generated from the <typeparamref name="TDerived"/> bytes
        /// constructor. Whether the constructor is used directly or not, it is required to
        /// facilitate complete operation of the bitwise enumeration.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static TDerived FromBytesCtor(IEnumerable bytes)
        {
            var derivedType = typeof(TDerived);
            const BindingFlags flags = Instance | NonPublic;
            //TODO: might want to break this out into an extension method
            var ctor = derivedType.GetConstructor(flags, DefaultBinder, new[] {typeof(byte[])}, null);
            var instance = ctor?.Invoke(new object[] {bytes});
            return instance as TDerived;
        }

        /// <summary>
        /// NamedLookup backing field.
        /// </summary>
        private static IDictionary<string, TDerived> _namedLookup;

        /// <summary>
        /// Gets the NamedLookup.
        /// </summary>
        protected static IDictionary<string, TDerived> NamedLookup
            => _namedLookup
               ?? (_namedLookup = Values.ToDictionary(x => x.Name, x => x));

        /// <summary>
        /// Returns the <see cref="Enumerable.FirstOrDefault{T}(IEnumerable{T})"/> value
        /// from <see cref="Values"/> for the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TDerived FromName(string name) 
            => NamedLookup.TryGetValue(name, out var result) ? result : null;

        /// <summary>
        /// DisplayNamedLookup backing field.
        /// </summary>
        private static IDictionary<string, TDerived> _displayNamedLookup;

        /// <summary>
        /// Gets the DisplayNamedLookup.
        /// </summary>
        private static IDictionary<string, TDerived> DisplayNamedLookup
            => _displayNamedLookup
               ?? (_displayNamedLookup = Values.ToDictionary(x => x.DisplayName, x => x));

        /// <summary>
        /// Returns the <see cref="Enumerable.FirstOrDefault{T}(IEnumerable{T})"/> value
        /// from <see cref="Values"/> for the given <paramref name="displayName"/>.
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static TDerived FromDisplayName(string displayName)
            => DisplayNamedLookup.TryGetValue(displayName, out var result) ? result : null;
    }
}
