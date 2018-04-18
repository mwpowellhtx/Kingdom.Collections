using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;
    using static Math;

    /// <summary>
    /// Provides a nominal set of <see cref="int"/> based <see cref="QueueExtensionMethods"/>
    /// unit tests.
    /// </summary>
    /// <inheritdoc />
    public class QueueDataStructureTests : IntegerBasedDataStructureTestsBase
    {
        private AddCallback _add;

        protected override AddCallback Add => _add;

        private GetRemoveExpectedCallback _getRemoveExpected;

        protected override GetRemoveExpectedCallback GetRemoveExpected => _getRemoveExpected;

        private RemoveCallback _remove;

        protected override RemoveCallback Remove => _remove;

        private TryRemoveCallback _tryRemove;

        protected override TryRemoveCallback TryRemove => _tryRemove;

        private GetRemoveManyExpectedCallback _getRemoveManyExpected;

        protected override GetRemoveManyExpectedCallback GetRemoveManyExpected => _getRemoveManyExpected;

        private RemoveManyCallback _removeMany;

        protected override RemoveManyCallback RemoveMany => _removeMany;

        private TryRemoveManyCallback _tryRemoveMany;

        protected override TryRemoveManyCallback TryRemoveMany => _tryRemoveMany;

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            _add = (s, i, j) => Verify(s).Enqueue(i, j);
            _getRemoveExpected = (i, j) => i;
            _remove = s => s.Dequeue<int, IList<int>>();
            _tryRemove = (IList<int> s, out int result) => s.TryDequeue(out result);
            _getRemoveManyExpected = (i, j, count) => new[] {i}.Concat(j)
                .Take(Min(j.Count + 1, count > 0 ? count : 0));
            _removeMany = (s, count) => s.DequeueMany<int, IList<int>>(count);
            _tryRemoveMany = (IList<int> s, out IEnumerable<int> result, int count) => s.TryDequeueMany(out result, count);
        }

        protected override IEnumerable<TestCaseData> BasicTestCases => ProtectedBasicTestCases;

        protected override IEnumerable<TestCaseData> ManyItemsTestCases => ProtectedManyItemsTestCases;
    }
}
