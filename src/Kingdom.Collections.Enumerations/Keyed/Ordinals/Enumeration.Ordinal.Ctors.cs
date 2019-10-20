using System;

namespace Kingdom.Collections
{
    /// <summary>
    /// Provides a <see cref="OrdinalEnumeration{TKey,T}"/> base class. Common
    /// specializations include <see cref="IntegerOrdinalEnumeration{T}"/> and
    /// <see cref="LongOrdinalEnumeration{T}"/>, for starters.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="T"></typeparam>
    public abstract partial class OrdinalEnumeration<TKey, T> : Enumeration<TKey, T>
        where T : OrdinalEnumeration<TKey, T>
        where TKey : struct, IComparable<TKey>, IEquatable<TKey>
    {
        /// <summary>
        /// Protected Default Constructor.
        /// </summary>
        protected OrdinalEnumeration()
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected OrdinalEnumeration(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        protected OrdinalEnumeration(string name, string displayName)
            : base(name, displayName)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="key"></param>
        protected OrdinalEnumeration(TKey key)
            : base(key)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        protected OrdinalEnumeration(TKey key, string name)
            : base(key, name)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        protected OrdinalEnumeration(TKey key, string name, string displayName)
            : base(key, name, displayName)
        {
        }
    }
}
