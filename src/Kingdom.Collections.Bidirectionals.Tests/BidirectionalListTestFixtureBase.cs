using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    using Xunit;

    public abstract class BidirectionalListTestFixtureBase<T> : TestFixtureBase
        where T : IEquatable<T>
    {
        /// <summary>
        /// 0
        /// </summary>
        protected const int Index = 0;

        /// <summary>
        /// Override to Get a NewItem of <typeparamref name="T"/> for use throughout the
        /// Test Cases.
        /// </summary>
        protected abstract T NewItem { get; }

        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        protected IBidirectionalList<T> TargetList { get; set; } = new BidirectionalList<T> { };

        /// <summary>
        /// Default Protected Constructor.
        /// </summary>
        protected BidirectionalListTestFixtureBase() => InitializeCollection(TargetList);

        private void InitializeCollection(ICollection<T> collection)
        {
            collection.Add(NewItem);
            collection.Add(NewItem);
            collection.Add(NewItem);
        }

        protected abstract void ConnectCallbacks(IBidirectionalList<T> list
            , BidirectionalCallback<T> onCallingBack, BidirectionalCallback<T> onCalledBack);

        protected override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                TargetList = null;
            }

            base.Dispose(disposing);
        }

        protected delegate void VerifyListCallback(ref IBidirectionalList<T> list, T item);

        protected T ExpectedItem { get; set; }

        protected int Order { get; set; }

        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        protected IList<int> CalledBackCallOrders { get; } = new List<int> { };

        // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
        protected IList<int> CallingBackCallOrders { get; } = new List<int> { };

        protected void OnCalledBack(T _) => CalledBackCallOrders.Add(++Order);

        protected void OnCallingBack(T _) => CallingBackCallOrders.Add(++Order);

        /// <summary>
        /// Override to Prepare the <paramref name="list"/> with the
        /// <paramref name="expectedItem"/>. The Default operation is essentially a No-Op.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="expectedItem"></param>
        protected virtual void PrepareList(IList<T> list, T expectedItem)
        {
        }

        /// <summary>
        /// Verifies whether the Quality of the List Action being Approached, or having been
        /// Approached, works correctly. It is up to the Caller to provide more context as to
        /// what Approaching or Approached actually means.
        /// </summary>
        /// <param name="callback"></param>
        protected void VerifyListCallbacks(VerifyListCallback callback)
        {
            Assert.NotNull(callback);

            ExpectedItem = NewItem;

            Order = 0;

            CallingBackCallOrders.Clear();
            CalledBackCallOrders.Clear();

            ConnectCallbacks(TargetList, OnCallingBack, OnCalledBack);

            var list = TargetList;

            PrepareList(list, ExpectedItem);

            callback.Invoke(ref list, ExpectedItem);

            // Replace the TargetList instance if necessary.
            if (!ReferenceEquals(TargetList, list))
            {
                TargetList = list;
            }

            /* We should be able to determine qualitatively that not only Adding and Added
             * happened, but also whether Calling did in fact occur prior to Called. */

            Assert.True(CallingBackCallOrders.Count > 0);
            Assert.True(CalledBackCallOrders.Count > 0);

            Assert.All(CallingBackCallOrders, o => Assert.True(o > 0));
            Assert.All(CalledBackCallOrders, o => Assert.True(o > 0));

            Assert.Equal(CallingBackCallOrders.Count, CalledBackCallOrders.Count);

            Assert.All(CallingBackCallOrders.Zip(CalledBackCallOrders
                    , (x, y) => new {CallingBackCallOrder = x, CalledBackCallOrder = y})
                , pair => Assert.True(pair.CalledBackCallOrder > pair.CallingBackCallOrder));
        }
    }
}
