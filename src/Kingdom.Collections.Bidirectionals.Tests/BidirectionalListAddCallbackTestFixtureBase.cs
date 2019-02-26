using System;

namespace Kingdom.Collections
{
    using Xunit;

    /// <summary>
    /// Provides a set of Tests for the <see cref="IBidirectionalList{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BidirectionalListAddCallbackTestFixtureBase<T> : BidirectionalListTestFixtureBase<T>
        where T : IEquatable<T>
    {
        protected override void ConnectCallbacks(IBidirectionalList<T> list
            , BidirectionalCallback<T> onCallingBack, BidirectionalCallback<T> onCalledBack)
        {
            Assert.NotNull(onCallingBack);
            Assert.NotNull(onCalledBack);

            list.AddingItem += onCallingBack;
            list.AddedItem += onCalledBack;
        }

        [Fact]
        public void AddCallbacksWorkCorrectly() => VerifyListCallbacks((ref IBidirectionalList<T> list, T x) =>
        {
            list.AddingItem += y => Assert.Equal(x, y);
            list.AddedItem += y => Assert.Equal(x, y);
            list.Add(x);
        });

        [Fact]
        public void InsertCallbacksWorkProperly() => VerifyListCallbacks((ref IBidirectionalList<T> list, T x) =>
        {
            list.AddingItem += y => Assert.Equal(x, y);
            list.AddedItem += y => Assert.Equal(x, y);
            var count = list.Count;
            list.Insert(0, x);
            Assert.Equal(count + 1, list.Count);
        });

        [Fact]
        public void IndexerCallbacksWorkProperly() => VerifyListCallbacks((ref IBidirectionalList<T> list, T x) =>
        {
            var expectedItem = x;
            bool? callingBackCalled = null, calledBackCalled = null;
            list.AddingItem += y =>
            {
                callingBackCalled = true;
                Assert.Equal(expectedItem, y);
            };
            list.AddedItem += y =>
            {
                calledBackCalled = true;
                Assert.Equal(expectedItem, y);
            };
            var count = list.Count;
            list[Index] = x;
            Assert.Equal(count, list.Count);
            Assert.True(callingBackCalled);
            Assert.True(calledBackCalled);
        });

        /// <summary>
        /// Verifies that the Constructors all fall through to the expected sEvent Callbacks.
        /// </summary>
        [Fact]
        public void ValuesCtorCallbacksWorkProperly() => VerifyListCallbacks((ref IBidirectionalList<T> list, T x) =>
        {
            bool? callingBackCalled = null, calledBackCalled = null;

            void OnAdding(T y)
            {
                callingBackCalled = callingBackCalled ?? true;
                Assert.Equal(y, x);
                OnCallingBack(y);
            }

            void OnAdded(T y)
            {
                calledBackCalled = calledBackCalled ?? true;
                Assert.Equal(y, x);
                OnCalledBack(y);
            }

            // We should have received an instance.
            Assert.NotNull(list);

            // ReSharper disable once ArgumentsStyleNamedExpression because we make it abundantly clearer.
            // But we will replace it an exercise the Ctor at the same time.
            list = new BidirectionalList<T>(new[] {x}, onAdded: OnAdded, onAdding: OnAdding);

            Assert.True(callingBackCalled);
            Assert.True(calledBackCalled);
        });
    }
}
