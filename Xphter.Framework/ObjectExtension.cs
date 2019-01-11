using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides extension methods for Object class.
    /// </summary>
    public static class ObjectExtension {
        /// <summary>
        /// Determines whether <paramref name="current"/> is a instance of <typeparamref name="T"/> and is not a instance of subclass of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="current"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool IsCertainType<T>(this object current) {
            return current is T && !current.GetType().IsSubclassOf(typeof(T));
        }

        /// <summary>
        /// Determines whether <paramref name="current"/> is a instance of <paramref name="type"/> and is not a instance of subclass of <paramref name="type"/>.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool IsCertainType(this object current, Type type) {
            return current.GetType().Equals(type);
        }
    }
}
