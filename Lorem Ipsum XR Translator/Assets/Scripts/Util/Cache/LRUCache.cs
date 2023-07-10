using System;
using System.Collections.Generic;


namespace Util.Cache
{
    /// <summary>
    /// Implementation of a Least Recently Used (LRU) Cache.
    /// The cache has a maximum capacity and removes the least recently used items first when the capacity is reached.
    /// </summary>
    /// <typeparam name="K">Type of the keys in the cache.</typeparam>
    /// <typeparam name="V">Type of the values in the cache.</typeparam>
    public class LRUCache<K, V> : ICache<K, V>
    {
        /// <summary>
        /// Inner representation of a cache entry.
        /// </summary>
        private class Item
        {
            /// <summary>
            /// Gets or sets the key of the entry.
            /// </summary>
            public K Key { get; set; }

            /// <summary>
            /// Gets or sets the value of the entry.
            /// </summary>
            public V Value { get; set; }
        }

        // Maximum number of items that can be stored in the cache.
        private readonly int _cap;

        // Dictionary stores the key and corresponding node in the linked list for O(1) access.
        private readonly Dictionary<K, LinkedListNode<Item>> _cache;

        // LinkedList keeps track of the LRU order with most recently used items at the end.
        private readonly LinkedList<Item> _lruList;

        /// <summary>
        /// Initializes a new instance of the <see cref="LRUCache{K, V}"/> class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The maximum number of items that the cache can hold.</param>
        public LRUCache(int capacity)
        {
            this._cap = capacity;
            this._cache = new Dictionary<K, LinkedListNode<Item>>(capacity);
            this._lruList = new LinkedList<Item>();
        }

        /// <summary>
        /// Retrieves the value associated with the specified key from the cache.
        /// </summary>
        /// <param name="key">The key of the item to retrieve.</param>
        /// <returns>The value associated with the key if the key exists; otherwise, the default value for type <typeparamref name="V"/>.</returns>
        public V Get(K key)
        {
            if (this._cache.TryGetValue(key, out var node))
            {
                var value = node.Value.Value;
                this._lruList.Remove(node);
                this._lruList.AddLast(node);
                return value;
            }
            return default(V);
        }

        /// <summary>
        /// Adds or updates the value associated with the specified key in the cache.
        /// If the cache has reached its capacity, it will remove the least recently used item before performing the addition.
        /// </summary>
        /// <param name="key">The key of the item to add or update.</param>
        /// <param name="val">The value to associate with the key.</param>
        public void Put(K key, V val)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key can't be null!!");
            }

            if (this._cache.Count >= this._cap)
            {
                // Remove the least recently used item from the cache when it's full.
                var lastNode = this._lruList.First;
                this._cache.Remove(lastNode.Value.Key);
                this._lruList.RemoveFirst();
            }


            var newItem = new LinkedListNode<Item>(new Item { Key = key, Value = val });
            this._lruList.AddLast(newItem);
            this._cache[key] = newItem;
        }

        /// <summary>
        /// Gets the number of key-value pairs contained in the cache.
        /// </summary>
        /// <returns>The number of key-value pairs contained in the cache.</returns>
        public int Size()
        {
            return this._cache.Count;
        }
    }
}
