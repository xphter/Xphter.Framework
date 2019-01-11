using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace Xphter.Framework.Web.Caching {
    /// <summary>
    /// Provides extensions of System.Web.Caching.Cache class.
    /// </summary>
    public static class CacheExtensions {
        /// <summary>
        /// Inserts an item into the Cache object with a cache key to reference its location, using the specified priority.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="priority"></param>
        public static void Insert(this Cache cache, string key, object value, CacheItemPriority priority) {
            cache.Insert(key, value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, priority, null);
        }

        /// <summary>
        /// Inserts an item into the Cache object with a cache key to reference its location, using the specified dependency and priority.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="denpendency"></param>
        /// <param name="priority"></param>
        public static void Insert(this Cache cache, string key, object value, CacheDependency denpendency, CacheItemPriority priority) {
            cache.Insert(key, value, denpendency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, priority, null);
        }

        /// <summary>
        /// Inserts an item into the Cache object with a cache key to reference its location, using the specified dependency and priority and remove callback.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="denpendency"></param>
        /// <param name="priority"></param>
        /// <param name="onRemoveCallback"></param>
        public static void Insert(this Cache cache, string key, object value, CacheDependency denpendency, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback) {
            cache.Insert(key, value, denpendency, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, priority, onRemoveCallback);
        }

        /// <summary>
        /// Gets the internal data item in a IExpirableWrapper object from the cache object. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetExpirable<T>(this Cache cache, string key) {
            T item = default(T);
            object obj = cache[key];
            IExpirableWrapper<T> wrapper = null;

            if(obj != null && obj is IExpirableWrapper<T>) {
                wrapper = (IExpirableWrapper<T>) obj;
                if(wrapper.IsExpired) {
                    cache.Remove(key);
                } else {
                    item = wrapper.DataItem;
                }
            }

            return item;
        }
    }
}
