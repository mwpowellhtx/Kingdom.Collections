using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using static Math;

    /// <summary>
    /// Provides a nominal set of <see cref="int"/> based <see cref="DequeExtensionMethods"/>
    /// unit tests. We will only focus on one of the Deque, or Double-ended Queue, permutations
    /// for test coverage purposes.
    /// </summary>
    /// <inheritdoc />
    public class DequeAddFrontRemoveBackDataStructureTests : IntegerBasedDataStructureTestsBase
    {
        protected override AddCallback Add { get; }

        protected override GetRemoveExpectedCallback GetRemoveExpected { get; }

        protected override RemoveCallback Remove { get; }

        protected override TryRemoveCallback TryRemove { get; }

        protected override GetRemoveManyExpectedCallback GetRemoveManyExpected { get; }

        protected override RemoveManyCallback RemoveMany { get; }

        protected override TryRemoveManyCallback TryRemoveMany { get; }

        public DequeAddFrontRemoveBackDataStructureTests()
        {
            Add = (s, i, j) => Verify(s).EnqueueFront(i, j);
            GetRemoveExpected = (i, j) => i;
            Remove = s => s.DequeueBack<int, IList<int>>();
            TryRemove = (IList<int> s, out int result) => s.TryDequeueBack(out result);
            GetRemoveManyExpected = (i, j, count) => new[] {i}.Concat(j)
                .Take(Min(j.Count + 1, count > 0 ? count : 0));
            RemoveMany = (s, count) => s.DequeueBackMany<int, IList<int>>(count);
            TryRemoveMany = (IList<int> s, out IEnumerable<int> result, int count) => s.TryDequeueBackMany(out result, count);
        }
    }
}
