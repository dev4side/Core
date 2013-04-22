﻿using System.Collections.Generic;

namespace Core.Common.Collection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryWithDefaultValue<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public TValue DefaultValue { get; set; }

        public TValue this[TKey key]
        {
            get 
            { 
                TValue value = DefaultValue;
                if(base.TryGetValue(key, out value))
                    return value;
                return DefaultValue;
            }
        }
    }
}
