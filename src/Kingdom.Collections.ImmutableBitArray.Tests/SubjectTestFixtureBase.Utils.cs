﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using static BitConverter;

    public abstract partial class SubjectTestFixtureBase<T>
    {
        protected static IEnumerable<TItem> GetRange<TItem>(params TItem[] values) => values;

        // TODO: TBD: this is a bit self-defeating? in the sense I am potentially re-writing the code under test? and/or maybe a method, extension, should be exposed internally?
        protected static IEnumerable<byte> GetEndianAwareBytes(uint x)
            => IsLittleEndian ? GetBytes(x) : GetBytes(x).Reverse();

        protected static OptimizedImmutableBitArray CreateBitArray(params uint[] values)
            => new OptimizedImmutableBitArray(values);

        protected static OptimizedImmutableBitArray CreateBitArrayWithArray(params byte[] bytes)
            => new OptimizedImmutableBitArray(bytes);
    }
}