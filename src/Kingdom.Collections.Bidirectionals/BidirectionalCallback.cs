namespace Kingdom.Collections
{
    /// <summary>
    /// Callback when either Adding or Removing in any way, shape, or form.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    public delegate void BidirectionalCallback<in T>(T item);
}
