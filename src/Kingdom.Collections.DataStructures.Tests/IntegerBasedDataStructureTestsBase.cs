using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using Xunit;

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
        static IntegerBasedDataStructureTestsBase()
        {
            BasicTestCases = ProtectedBasicTestCases;
            ManyItemsTestCases = ProtectedManyItemsTestCases;
        }

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

        protected static IEnumerable<object[]> ProtectedBasicTestCases
        {
            get
            {
                yield return new object[] {1, new ItemList()};
                yield return new object[] {1, new ItemList(2)};
                yield return new object[] {1, new ItemList(2, 3)};
                yield return new object[] {1, new ItemList(2, 3, 4)};
            }
        }

        public static IEnumerable<object[]> BasicTestCases { get; protected set; }

        [Theory]
        [MemberData(nameof(BasicTestCases))]
        public virtual void Verify_can_Add(int item, ItemList additionalItems)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            Assert.Equal(additionalItems.Count + 1, sub.Count);
            var expected = new[] {item}.Concat(additionalItems).Reverse().ToList();
            Assert.Equal(expected, sub);
        }

        [Theory]
        [MemberData(nameof(BasicTestCases))]
        public void Verify_can_Remove(int item, ItemList additionalItems)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            Assert.Equal(additionalItems.Count + 1, sub.Count);
            var expected = GetRemoveExpected(item, additionalItems);
            Assert.Equal(expected, Remove(sub));
        }


        [Fact]
        public void Verify_that_Remove_empty_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Remove(Subject));
        }

        [Theory]
        [MemberData(nameof(BasicTestCases))]
        public void Verify_that_TryRemove_correct(int item, ItemList additionalItems)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            Assert.Equal(additionalItems.Count + 1, sub.Count);
            var expected = GetRemoveExpected(item, additionalItems);
            // Wow for C# 7 ...
            Assert.True(TryRemove(sub, out var actual));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Verify_that_TryRemove_empty_false()
        {
            Assert.False(TryRemove(Verify(Subject), out var actual));
            Assert.Equal(default(int), actual);
        }

        protected static IEnumerable<object[]> ProtectedManyItemsTestCases
        {
            get
            {
                yield return new object[] {1, new ItemList(), 2};
                yield return new object[] {1, new ItemList(2), 2};
                yield return new object[] {1, new ItemList(2, 3), 2};
                yield return new object[] {1, new ItemList(2, 3, 4), 2};
                yield return new object[] {1, new ItemList(), -2};
                yield return new object[] {1, new ItemList(2), -2};
                yield return new object[] {1, new ItemList(2, 3), -2};
                yield return new object[] {1, new ItemList(2, 3, 4), -2};
            }
        }

        public static IEnumerable<object[]> ManyItemsTestCases { get; protected set; }


        [Theory]
        [MemberData(nameof(ManyItemsTestCases))]
        public void Verify_that_RemoveMany_correct(int item, ItemList additionalItems, int count)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            var actual = RemoveMany(sub, count).ToArray();
            var expected = GetRemoveManyExpected(item, additionalItems, count).ToArray();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ManyItemsTestCases))]
        public void Verify_that_TryRemoveMany_correct(int item, ItemList additionalItems, int count)
        {
            var sub = Add(Subject, item, additionalItems.ToArray());
            Assert.Equal(count > 0, TryRemoveMany(sub, out var actual, count));
            var expected = GetRemoveManyExpected(item, additionalItems, count);
            Assert.Equal(expected, actual);
        }
    }
}
