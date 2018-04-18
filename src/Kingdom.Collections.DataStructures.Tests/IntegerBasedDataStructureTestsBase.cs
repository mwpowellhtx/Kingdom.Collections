using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using NUnit.Framework;

    /// <summary>
    /// The Collection Patterns are pretty clear. Each of the Extensions can Add items to the
    /// Collection. Conversely, each of the Patterns can Remove items from the Collection. It
    /// does not matter whether we are talking about front, back, many, tried, etc. The Patterns
    /// are all Consistent. With that being established, the usage of this test suite basically
    /// involves simply defining each of the respective Callbacks. You have to provide the test
    /// cases hooks as well satisfying both the compiler and the Unit Test framework Test Case
    /// discovery, but this is a minor point by comparison. The hard work of actually defining
    /// the test cases is also provided.
    /// </summary>
    public abstract class IntegerBasedDataStructureTestsBase : DataStructureTestsBase<int, List<int>>
    {
        protected delegate IList<int> AddCallback(List<int> subject, int item, params int[] additionalItems);

        protected abstract AddCallback Add { get; }

        protected delegate int RemoveCallback(IList<int> subject);

        protected delegate int GetRemoveExpectedCallback(int item, IList<int> additionalItems);

        protected abstract GetRemoveExpectedCallback GetRemoveExpected { get; }

        protected abstract RemoveCallback Remove { get; }

        protected delegate bool TryRemoveCallback(IList<int> subject, out int result);

        protected abstract TryRemoveCallback TryRemove { get; }

        protected delegate IEnumerable<int> GetRemoveManyExpectedCallback(int item, IList<int> additionalItems
            , int count);

        protected abstract GetRemoveManyExpectedCallback GetRemoveManyExpected { get; }

        protected delegate IEnumerable<int> RemoveManyCallback(IList<int> subject, int count);

        protected abstract RemoveManyCallback RemoveMany { get; }

        protected delegate bool TryRemoveManyCallback(IList<int> subject, out IEnumerable<int> result, int count);

        protected abstract TryRemoveManyCallback TryRemoveMany { get; }

        protected static IEnumerable<TestCaseData> ProtectedBasicTestCases
        {
            get
            {
                yield return new TestCaseData(1, new ItemList());
                yield return new TestCaseData(1, new ItemList(2));
                yield return new TestCaseData(1, new ItemList(2, 3));
                yield return new TestCaseData(1, new ItemList(2, 3, 4));
            }
        }

        protected abstract IEnumerable<TestCaseData> BasicTestCases { get; }

        [Test]
        [TestCaseSource(nameof(BasicTestCases))]
        public virtual void Verify_can_Add(int item, ItemList additionalItems)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            Assert.That(sub, Has.Count.EqualTo(additionalItems.Count + 1));
            var expected = new[] {item}.Concat(additionalItems).Reverse().ToList();
            CollectionAssert.AreEqual(expected, sub);
        }

        [Test]
        [TestCaseSource(nameof(BasicTestCases))]
        public void Verify_can_Remove(int item, ItemList additionalItems)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            Assert.That(sub, Has.Count.EqualTo(additionalItems.Count + 1));
            var expected = GetRemoveExpected(item, additionalItems);
            Assert.That(Remove(sub), Is.EqualTo(expected));
        }


        [Test]
        public void Verify_that_Remove_empty_throws()
        {
            Assert.That(() => Remove(Subject), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        [TestCaseSource(nameof(BasicTestCases))]
        public void Verify_that_TryRemove_correct(int item, ItemList additionalItems)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            Assert.That(sub, Has.Count.EqualTo(additionalItems.Count + 1));
            var expected = GetRemoveExpected(item, additionalItems);
            int actual;
            Assert.That(TryRemove(sub, out actual), Is.True);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Verify_that_TryRemove_empty_false()
        {
            int actual;
            Assert.That(TryRemove(Verify(Subject), out actual), Is.False);
            Assert.That(actual, Is.EqualTo(default(int)));
        }

        protected static IEnumerable<TestCaseData> ProtectedManyItemsTestCases
        {
            get
            {
                yield return new TestCaseData(1, new ItemList(), 2);
                yield return new TestCaseData(1, new ItemList(2), 2);
                yield return new TestCaseData(1, new ItemList(2, 3), 2);
                yield return new TestCaseData(1, new ItemList(2, 3, 4), 2);
                yield return new TestCaseData(1, new ItemList(), -2);
                yield return new TestCaseData(1, new ItemList(2), -2);
                yield return new TestCaseData(1, new ItemList(2, 3), -2);
                yield return new TestCaseData(1, new ItemList(2, 3, 4), -2);
            }
        }

        protected abstract IEnumerable<TestCaseData> ManyItemsTestCases { get; }


        [Test]
        [TestCaseSource(nameof(ManyItemsTestCases))]
        public void Verify_that_RemoveMany_correct(int item, ItemList additionalItems, int count)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            var actual = RemoveMany(sub, count).ToArray();
            var expected = GetRemoveManyExpected(item, additionalItems, count).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [Test]
        [TestCaseSource(nameof(ManyItemsTestCases))]
        public void Verify_that_TryRemoveMany_correct(int item, ItemList additionalItems, int count)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            IEnumerable<int> actual;
            Assert.That(TryRemoveMany(sub, out actual, count), Is.EqualTo(count > 0));
            var expected = GetRemoveManyExpected(item, additionalItems, count).ToArray();
            CollectionAssert.AreEqual(expected, actual.ToArray());
        }
    }
}
