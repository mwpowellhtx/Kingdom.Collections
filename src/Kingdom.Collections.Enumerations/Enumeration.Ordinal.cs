using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    /// <summary>
    /// Provides an Ordinal based <see cref="Enumeration{T}"/>. This may seem a little backwards,
    /// but this will depend on the underlying Bit Array logic to keep track of the Ordinal.
    /// </summary>
    /// <typeparam name="TDerived"></typeparam>
    /// <inheritdoc cref="Enumeration{T}" />
    public abstract class OrdinalEnumeration<TDerived>
        : Enumeration<TDerived>
            , IEquatable<int>
            , IEquatable<int?>
            , IComparable<int>
        where TDerived : OrdinalEnumeration<TDerived>
    {
        /// <summary>
        /// Gets the DebuggerDisplayName.
        /// </summary>
        /// <see cref="Enumeration.DisplayName"/>
        /// <see cref="Ordinal"/>
        /// <inheritdoc />
        protected internal sealed override string DebuggerDisplayName
            => $"@{DisplayName} (Ordinal: {Ordinal})";

        /// <summary>
        /// Gets the Ordinal value.
        /// </summary>
        public int Ordinal
        {
            get { return Bits == null ? 0 : (int) Bits.ToInts().SingleOrDefault(); }
            protected set { Bits = ImmutableBitArray.FromInts(new[] {(uint) value}); }
        }

        /// <summary>
        /// Initializes the <see cref="Ordinal"/> Values.
        /// </summary>
        /// <param name="values"></param>
        protected static void InitializeOrdinals(IEnumerable<TDerived> values)
        {
            // This might be too much it effectively initializes across the aggregate.
            values.ToArray().Aggregate(0, (o, v) => v.Ordinal = ++o);
        }

        /// <summary>
        /// Protected Default Constructor
        /// </summary>
        /// <inheritdoc />
        protected OrdinalEnumeration()
            : this(0)
        {
        }

        /// <summary>
        /// Protected Constructor
        /// </summary>
        /// <param name="ordinal"></param>
        /// <inheritdoc />
        protected OrdinalEnumeration(int ordinal)
            : base(GetOrdinalBytes(ordinal).ToArray())
        {
        }

        /// <summary>
        /// Returns the comparison of <paramref name="a"/> with <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int CompareTo(int a, int b)
        {
            return a.CompareTo(b);
        }

        /// <summary>
        /// Returns the comparison of <see cref="Ordinal"/> with <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public int CompareTo(int other)
        {
            return CompareTo(Ordinal, other);
        }

        /// <summary>
        /// Returns whether <see cref="Ordinal"/> Equals an <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public bool Equals(int other)
        {
            return Ordinal == other;
        }

        /// <summary>
        /// Returns whether <see cref="Ordinal"/> Equals an <paramref name="other"/>.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public bool Equals(int? other)
        {
            return other == Ordinal;
        }

        /// <summary>
        /// Returns the <see cref="Enumerable.FirstOrDefault{T}(IEnumerable{T})"/> value from
        /// <see cref="Enumeration{T}.Values"/> for the given <paramref name="ordinal"/>.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public static TDerived FromOrdinal(int ordinal)
        {
            var values = GetValues();
            return values.FirstOrDefault(v => v.Ordinal.Equals(ordinal));
        }

        /// <summary>
        /// Gets the EnumeratedValues, which, from an Ordinal perspective, includes just itself.
        /// </summary>
        /// <inheritdoc />
        public override IEnumerable<TDerived> EnumeratedValues
        {
            get { yield return (TDerived) this; }
        }
    }
}
