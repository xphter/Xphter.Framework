using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides utilities to process array.
    /// </summary>
    public static class ArrayUtility {
        /// <summary>
        /// Creates a array which contains the specified elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static T[] Repeat<T>(T item, int length) {
            if(length < 0) {
                throw new ArgumentOutOfRangeException("length", "length less than zero.");
            }

            T[] result = new T[length];

            for(int i = 0; i < length; i++) {
                result[i] = item;
            }

            return result;
        }
    }
}
