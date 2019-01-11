using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Reflection {
    /// <summary>
    /// Provides functions to access properties of a object.
    /// </summary>
    public static class PropertyUtility {
        /// <summary>
        /// Gets the explicit implemented property.
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TInterface"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static PropertyInfo GetExplicitImplementedProperty<TClass, TInterface>(string propertyName)
            where TClass : class
            where TInterface : class {
            Type tInterface = typeof(TInterface);
            if(!tInterface.IsInterface) {
                throw new ArgumentException("TInterface is not a interface type.", "TInterface");
            }

            Type tClass = typeof(TClass);
            if(tClass.GetInterface(tInterface.FullName) == null) {
                throw new ArgumentException("TClass not implements TInterface.", "tClass");
            }

            return tClass.GetProperty(string.Format("{0}.{1}", tInterface.FullName, propertyName), BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
