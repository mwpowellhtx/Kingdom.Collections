using System.Collections.Generic;

namespace Kingdom.Collections
{
    using Xunit;

    public partial class OptimizedImmutableBitArrayCtorTests
    {
        private static OptimizedImmutableBitArray CreateBitArray(IEnumerable<byte> bytes)
        {
            Assert.NotNull(bytes);
            return new OptimizedImmutableBitArray(bytes);
        }
    }
}
