using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    public partial class OptimizedImmutableBitArray
    {
        internal const int BitCount = 8;

        internal IEnumerable<byte> InternalBytes() => from b in _bytes select b;
    }
}
