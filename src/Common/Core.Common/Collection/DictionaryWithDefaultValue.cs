using System.Collections.Generic;

namespace Core.Common.Collection
{
    /// <summary>
    /// Represents a collection of keys and values with default value.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class DictionaryWithDefaultValue<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public TValue DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key. If the specified key is not found, a get operation throws a System.Collections.Generic.KeyNotFoundException, and a set operation creates a new element with the specified key.</returns>
        public new TValue this[TKey key]
        {
            get 
            { 
                TValue value = DefaultValue;
                return base.TryGetValue(key, out value) ? value : DefaultValue;
            }
        }
    }
}
