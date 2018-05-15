using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using static Math;

    /// <summary>
    /// Provides a nominal set of <see cref="int"/> based <see cref="QueueExtensionMethods"/>
    /// unit tests.
    /// </summary>
    /// <inheritdoc />
    public class QueueDataStructureTests : IntegerBasedDataStructureTestsBase
    {
        protected override AddCallback Add { get; }

        protected override GetRemoveExpectedCallback GetRemoveExpected { get; }

        protected override RemoveCallback Remove { get; }

        protected override TryRemoveCallback TryRemove { get; }

        protected override GetRemoveManyExpectedCallback GetRemoveManyExpected { get; }

        protected override RemoveManyCallback RemoveMany { get; }

        protected override TryRemoveManyCallback TryRemoveMany { get; }

        public QueueDataStructureTests()
        {
            Add = (s, i, j) => Verify(s).Enqueue(i, j);
            GetRemoveExpected = (i, j) => i;
            Remove = s => s.Dequeue<int, IList<int>>();
            TryRemove = (IList<int> s, out int result) => s.TryDequeue(out result);
            GetRemoveManyExpected = (i, j, count) => new[] { i }.Concat(j)
                .Take(Min(j.Count + 1, count > 0 ? count : 0));
            RemoveMany = (s, count) => s.DequeueMany<int, IList<int>>(count);
            TryRemoveMany = (IList<int> s, out IEnumerable<int> result, int count) => s.TryDequeueMany(out result, count);
        }
    }
}
