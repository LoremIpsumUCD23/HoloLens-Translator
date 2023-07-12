using System.Collections.Generic;

namespace Util.Cache
{
    /// <summary>
    /// Defines the interface for a generic cache with key of type K and value of type V.
    /// </summary>
    /// <typeparam name="K">Type of the keys in the cache.</typeparam>
    /// <typeparam name="V">Type of the values in the cache.</typeparam>
    public interface ICache<K, V>
    {
        /// <summary>
        /// Retrieves the value associated with the provided key from the cache.
        /// </summary>
        /// <param name="key">The key of the item to retrieve from the cache.</param>
        /// <returns>The value associated with the key if the key exists, otherwise default value of type V.</returns>
        V Get(K key);

        /// <summary>
        /// Adds or updates the value associated with the provided key in the cache.
        /// </summary>
        /// <param name="key">The key of the item to add or update in the cache.</param>
        /// <param name="val">The value to associate with the key.</param>
        void Put(K key, V val);

        /// <summary>
        /// Retrieves the current size of the cache.
        /// </summary>
        /// <returns>The number of items currently stored in the cache.</returns>
        int Size();
    }
}
