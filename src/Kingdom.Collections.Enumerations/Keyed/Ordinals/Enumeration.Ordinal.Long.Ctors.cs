namespace Kingdom.Collections
{
    /// <summary>
    /// Provides a <see cref="long"/> based <see cref="OrdinalEnumeration{TKey,T}"/> base class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract partial class LongOrdinalEnumeration<T> : OrdinalEnumeration<long, T>
        where T : LongOrdinalEnumeration<T>
    {
        /// <summary>
        /// Protected Default Constructor.
        /// </summary>
        protected LongOrdinalEnumeration()
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected LongOrdinalEnumeration(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        protected LongOrdinalEnumeration(string name, string displayName)
            : base(name, displayName)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="ordinal"></param>
        protected LongOrdinalEnumeration(long ordinal)
            : base(ordinal)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="name"></param>
        protected LongOrdinalEnumeration(long ordinal, string name)
            : base(ordinal, name)
        {
        }

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        protected LongOrdinalEnumeration(long ordinal, string name, string displayName)
            : base(ordinal, name, displayName)
        {
        }
    }
}
