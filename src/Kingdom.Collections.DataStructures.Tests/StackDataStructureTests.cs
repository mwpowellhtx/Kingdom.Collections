using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using static Math;

    /// <summary>
    /// Provides a nominal set of <see cref="int"/> based <see cref="StackExtensionMethods"/>
    /// unit tests.
    /// </summary>
    /// <inheritdoc />
    public class StackDataStructureTests : IntegerBasedDataStructureTestsBase
    {
        protected override AddCallback Add { get; }

        protected override GetRemoveExpectedCallback GetRemoveExpected { get; }

        protected override RemoveCallback Remove { get; }

        protected override TryRemoveCallback TryRemove { get; }

        protected override GetRemoveManyExpectedCallback GetRemoveManyExpected { get; }

        protected override RemoveManyCallback RemoveMany { get; }

        protected override TryRemoveManyCallback TryRemoveMany { get; }

        public StackDataStructureTests()
        {
            Add = (s, i, j) => Verify(s).Push(i, j);
            GetRemoveExpected = (i, j) => j.Count > 0 ? j.Last() : i;
            Remove = s => s.Pop<int, IList<int>>();
            TryRemove = (IList<int> s, out int result) => s.TryPop(out result);
            GetRemoveManyExpected = (i, j, count) => new[] {i}.Concat(j).Reverse()
                .Take(Min(j.Count + 1, count > 0 ? count : 0));
            RemoveMany = (s, count) => s.PopMany<int, IList<int>>(count);
            TryRemoveMany = (IList<int> s, out IEnumerable<int> result, int count) => s.TryPopMany(out result, count);
        }
    }
}
