using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    public partial class OptimizedImmutableBitArray
    {
        private delegate byte CalculateBinaryCallback(byte x, byte y);

        private static IEnumerable<byte> CalculateBinaryOperator(IEnumerable<byte> a
            , IEnumerable<byte> b, CalculateBinaryCallback calc)
        {
            // Ensuring that A and B are both the same size.
            void NormalizeArray(IEnumerable<byte> x, ref IEnumerable<byte> y)
            {
                // ReSharper disable once PossibleMultipleEnumeration
                if (x.Count()
                    // ReSharper disable once PossibleMultipleEnumeration
                    > y.Count())
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    y = y.Concat(new byte[
                        // ReSharper disable once PossibleMultipleEnumeration
                        x.Count()
                        // ReSharper disable once PossibleMultipleEnumeration
                        - y.Count()]).ToArray();
                }
            }

            // ReSharper disable once PossibleMultipleEnumeration
            NormalizeArray(a, ref b);
            // ReSharper disable once PossibleMultipleEnumeration
            NormalizeArray(b
                // ReSharper disable once PossibleMultipleEnumeration
                , ref a);

            // Then simply Zip the two collections and apply the Calculation.
            // ReSharper disable once PossibleMultipleEnumeration
            return a.Zip(b, (x, y) => calc(x, y)).ToArray();
        }

        /// <inheritdoc />
        public OptimizedImmutableBitArray And(OptimizedImmutableBitArray other) => And(GetRange(other));

        /// <inheritdoc />
        public OptimizedImmutableBitArray Or(OptimizedImmutableBitArray other) => Or(GetRange(other));

        /// <inheritdoc />
        public OptimizedImmutableBitArray Xor(OptimizedImmutableBitArray other) => Xor(GetRange(other));

        /// <summary>
        /// Performs the Bitwise And operation on the current instance, <paramref name="other"/>,
        /// and <paramref name="others"/> instances. Returns a new instance containing the result.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="others"></param>
        /// <returns>A new instance containing the result.</returns>
        public OptimizedImmutableBitArray And(OptimizedImmutableBitArray other
            , params OptimizedImmutableBitArray[] others)
            => And(GetRange(other).Concat(others).ToArray());

        /// <summary>
        /// Performs the Bitwise Or operation on the current instance, <paramref name="other"/>,
        /// and <paramref name="others"/> instances. Returns a new instance containing the result.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="others"></param>
        /// <returns>A new instance containing the result.</returns>
        public OptimizedImmutableBitArray Or(OptimizedImmutableBitArray other
            , params OptimizedImmutableBitArray[] others)
            => Or(GetRange(other).Concat(others).ToArray());

        /// <summary>
        /// Performs the Bitwise Exclusive Or operation on the current instance,
        /// <paramref name="other"/>, and <paramref name="others"/> instances. Returns a new
        /// instance containing the result.
        /// </summary>
        /// <param name="other"></param>
        /// <param name="others"></param>
        /// <returns>A new instance containing the result.</returns>
        public OptimizedImmutableBitArray Xor(OptimizedImmutableBitArray other
            , params OptimizedImmutableBitArray[] others)
            => Xor(GetRange(other).Concat(others).ToArray());

        /// <inheritdoc />
        public OptimizedImmutableBitArray And(IEnumerable<OptimizedImmutableBitArray> others)
        {
            var bytes = others.Aggregate(_bytes.ToArray()
                , (g, other) => CalculateBinaryOperator(
                    g, other._bytes, (x, y) => (byte) (x & y)).ToArray());

            return new OptimizedImmutableBitArray(bytes);
        }

        /// <inheritdoc />
        public OptimizedImmutableBitArray Or(IEnumerable<OptimizedImmutableBitArray> others)
        {
            var bytes = others.Aggregate(_bytes.ToArray()
                , (g, other) => CalculateBinaryOperator(
                    g, other._bytes, (x, y) => (byte) (x | y)).ToArray());

            return new OptimizedImmutableBitArray(bytes);
        }

        /// <inheritdoc />
        public OptimizedImmutableBitArray Xor(IEnumerable<OptimizedImmutableBitArray> others)
        {
            var bytes = others.Aggregate(_bytes.ToArray()
                , (g, other) => CalculateBinaryOperator(
                    g, other._bytes, (x, y) => (byte) (x ^ y)).ToArray());

            return new OptimizedImmutableBitArray(bytes);
        }

        /// <inheritdoc />
        public OptimizedImmutableBitArray Not() => new OptimizedImmutableBitArray(_bytes.Select(x => (byte) ~x));
    }
}
