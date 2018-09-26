namespace Kingdom.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc />
    public interface IElasticImmutableBitArray<T> : IImmutableBitArray<T>
        where T : class, IElasticImmutableBitArray<T>
    {
        /// <summary>
        /// Returns whether the Bit at <paramref name="index"/> is Set. This operation also
        /// involves <paramref name="elasticity"/>, meaning that <paramref name="index"/>
        /// may exceed the current <see cref="IImmutableBitArray.Length"/> of the Array.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="elasticity"></param>
        /// <returns></returns>
        bool Get(int index, Elasticity elasticity);

        /// <summary>
        /// Sets the Bit at <paramref name="index"/> to the <paramref name="value"/>.
        /// This operation also involves <paramref name="elasticity"/>, meaning that
        /// <paramref name="index"/> may exceed the current
        /// <see cref="IImmutableBitArray.Length"/> of the Array.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="elasticity"></param>
        void Set(int index, bool value, Elasticity elasticity);
    }
}
