using System.Collections.Generic;

namespace Kingdom.Collections
{
    /// <summary>
    /// Bidirectional List interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <inheritdoc />
    public interface IBidirectionalList<T> : IList<T>
    {
        /// <summary>
        /// Event raised prior to Adding an Item.
        /// </summary>
        event BidirectionalCallback<T> AddingItem;

        /// <summary>
        /// Event raised after an Item has been Added.
        /// </summary>
        event BidirectionalCallback<T> AddedItem;

        /// <summary>
        /// Event raised prior to Removing an Item.
        /// </summary>
        event BidirectionalCallback<T> RemovingItem;

        /// <summary>
        /// Event raised after an Item has been Removed.
        /// </summary>
        event BidirectionalCallback<T> RemovedItem;
    }
}
