using System.Collections.Generic;

namespace Kingdom.Collections.Generic
{
    /// <summary>
    /// Provides an interface concerning Bidirectional Dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IBidirectionalDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
    }
}