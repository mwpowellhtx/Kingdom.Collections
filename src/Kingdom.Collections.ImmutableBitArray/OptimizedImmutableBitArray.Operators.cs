using System.Collections.Generic;

namespace Kingdom.Collections
{
    using static Elasticity;

    public partial class OptimizedImmutableBitArray
    {
        /// <summary>
        /// Returns the result of <see cref="And(OptimizedImmutableBitArray)"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static OptimizedImmutableBitArray operator &(OptimizedImmutableBitArray a
            , OptimizedImmutableBitArray b) => a.And(b);

        /// <summary>
        /// Returns the result of <see cref="Or(OptimizedImmutableBitArray)"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static OptimizedImmutableBitArray operator |(OptimizedImmutableBitArray a
            , OptimizedImmutableBitArray b) => a.Or(b);

        /// <summary>
        /// Returns the result of <see cref="Xor(OptimizedImmutableBitArray)"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static OptimizedImmutableBitArray operator ^(OptimizedImmutableBitArray a
            , OptimizedImmutableBitArray b) => a.Xor(b);

        /// <summary>
        /// Returns the result of <see cref="And(IEnumerable{OptimizedImmutableBitArray})"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static OptimizedImmutableBitArray operator &(OptimizedImmutableBitArray a
            , IEnumerable<OptimizedImmutableBitArray> others) => a.And(others);

        /// <summary>
        /// Returns the result of <see cref="Or(IEnumerable{OptimizedImmutableBitArray})"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static OptimizedImmutableBitArray operator |(OptimizedImmutableBitArray a
            , IEnumerable<OptimizedImmutableBitArray> others) => a.Or(others);

        /// <summary>
        /// Returns the result of <see cref="Xor(IEnumerable{OptimizedImmutableBitArray})"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="others"></param>
        /// <returns></returns>
        public static OptimizedImmutableBitArray operator ^(OptimizedImmutableBitArray a
            , IEnumerable<OptimizedImmutableBitArray> others) => a.Xor(others);

        /// <summary>
        /// Returns the result of <see cref="ShiftLeft(int,Elasticity)"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static OptimizedImmutableBitArray operator <<(OptimizedImmutableBitArray a, int count)
            => a.ShiftLeft(count, None);

        /// <summary>
        /// Returns the result of <see cref="ShiftRight(int,Elasticity)"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static OptimizedImmutableBitArray operator >>(OptimizedImmutableBitArray a, int count)
            => a.ShiftRight(count, None);
    }
}
