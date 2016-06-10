# ImmutableBitArray

I have been using the .NET Framework [System.Collections.BitArray](http://msdn.microsoft.com/en-us/library/system.collections.bitarray.aspx) for a little while now in order to support wide bit set enumerations. For the most part this is fine but for the fact the it is not [immutable](http://en.wikipedia.org/wiki/Immutable_object), and in a sense, [idempotent](http://en.wikipedia.org/wiki/Idempotence), especially in that bitwise operations ought to leave the root operands unchanged, and instead yield a new instance of the result.

The operations are fairly self explanatory. The goals were clear getting stared: I wanted to establish a basic moral equivalence, so-called, but for the afore mentioned immutability and idempotency concerns. I will continue adding new operations, and will continue to flesh it out, or as issues and requests are submitted, or contributors want to add to the body of effort.

Thanks and enjoy!
