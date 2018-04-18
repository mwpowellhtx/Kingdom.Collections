using System.Collections.Generic;
using System.Linq;

namespace Kingdom.Collections
{
    /// <summary>
    /// Provides the set of Double-ended Queue, or Deque, operations on <see cref="IList{T}"/>.
    /// </summary>
    /// <see cref="!:https://en.wikipedia.org/wiki/Double-ended_queue"/>
    public static class DequeExtensionMethods
    {
        /// <summary>
        /// Enqueues the <paramref name="item"/> onto the Front of the <paramref name="list"/>.
        /// May Enqueue <paramref name="additionalItems"/> onto the Front of the list in Enqueue
        /// order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="additionalItems"></param>
        /// <returns></returns>
        public static TList EnqueueFront<T, TList>(this TList list, T item, params T[] additionalItems)
            where TList : IList<T>
        {
            list.Insert(0, item);

            foreach (var additionalItem in additionalItems)
            {
                list.EnqueueFront(additionalItem);
            }

            return list;
        }

        /// <summary>
        /// Enqueues the <paramref name="item"/> onto the Back of the <paramref name="list"/>.
        /// May Enqueue <paramref name="additionalItems"/> onto the Back of the list in Enqueue
        /// order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <param name="additionalItems"></param>
        /// <returns></returns>
        public static TList EnqueueBack<T, TList>(this TList list, T item, params T[] additionalItems)
            where TList : IList<T>
        {
            list.Add(item);

            foreach (var additionalItem in additionalItems)
            {
                list.EnqueueBack(additionalItem);
            }

            return list;
        }

        /// <summary>
        /// Returns the Dequeued Item from the Front of the List.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T DequeueFront<T, TList>(this TList list)
            where TList : IList<T>
        {
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Returns the Dequeued Item from the Back of the List.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T DequeueBack<T, TList>(this TList list)
            where TList : IList<T>
        {
            var count = list.Count;
            var item = list[count - 1];
            list.RemoveAt(count - 1);
            return item;
        }

        /// <summary>
        /// Tries to Dequeue the <paramref name="item"/> from the Front of the
        /// <paramref name="list"/>. Returns whether this operation was successful.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool TryDequeueFront<T, TList>(this TList list, out T item)
            where TList : IList<T>
        {
            var count = list.Count;
            item = list.Any() ? list.DequeueFront<T, TList>() : default(T);
            return count != list.Count;
        }

        /// <summary>
        /// Tries to Dequeue the <paramref name="item"/> from the Back of the
        /// <paramref name="list"/>. Returns whether this operation was successful.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool TryDequeueBack<T, TList>(this TList list, out T item)
            where TList : IList<T>
        {
            var count = list.Count;
            item = list.Any() ? list.DequeueBack<T, TList>() : default(T);
            return count != list.Count;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of Dequeued items from the Front of the
        /// <paramref name="list"/>. The returned items will be in
        /// <see cref="DequeueFront{T,TList}"/> order and there may be as many as up to
        /// <paramref name="count"/> items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> DequeueFrontMany<T, TList>(this TList list, int count = 1)
            where TList : IList<T>
        {
            while (count-- > 0 && list.Any())
            {
                yield return list.DequeueFront<T, TList>();
            }
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> of Dequeued items from the Back of the
        /// <paramref name="list"/>. The returned items will be in
        /// <see cref="DequeueBack{T,TList}"/> order and there may be as many as up to
        /// <paramref name="count"/> items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<T> DequeueBackMany<T, TList>(this TList list, int count = 1)
            where TList : IList<T>
        {
            while (count-- > 0 && list.Any())
            {
                yield return list.DequeueBack<T, TList>();
            }
        }

        /// <summary>
        /// Tries to Dequeue as Many <typeparamref name="T"/> items as possible. May return with
        /// a <paramref name="result"/> that contains up to <paramref name="count"/> number of
        /// them. The returned <paramref name="result"/> will be in
        /// <see cref="DequeueFront{T,TList}"/> order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <param name="result"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool TryDequeueFrontMany<T, TList>(this TList list, out IEnumerable<T> result, int count = 1)
            where TList : IList<T>
        {
            result = new List<T>();

            while (count-- > 0 && list.Any())
            {
                ((IList<T>) result).Add(list.DequeueFront<T, TList>());
            }

            return result.Any();
        }

        /// <summary>
        /// Tries to Dequeue as Many <typeparamref name="T"/> items as possible. May return with
        /// a <paramref name="result"/> that contains up to <paramref name="count"/> number of
        /// them. The returned <paramref name="result"/> will be in
        /// <see cref="DequeueBack{T,TList}"/> order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TList"></typeparam>
        /// <param name="list"></param>
        /// <param name="result"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool TryDequeueBackMany<T, TList>(this TList list, out IEnumerable<T> result, int count = 1)
            where TList : IList<T>
        {
            result = new List<T>();

            while (count-- > 0 && list.Any())
            {
                ((IList<T>) result).Add(list.DequeueBack<T, TList>());
            }

            return result.Any();
        }
    }
}
