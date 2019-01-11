using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xphter.Framework.Reflection;

namespace Xphter.Framework {
    /// <summary>
    /// Provides functions for generic objects.
    /// </summary>
    public static class ObjectUtility {
        /// <summary>
        /// Sets value to empty of all flaged properties.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="names"></param>
        public static void FilterInvisibleProperties(object target, IEnumerable<string> names) {
            if(target == null) {
                throw new ArgumentNullException("target");
            }
            if(names == null) {
                throw new ArgumentNullException("names");
            }
            if(names.Count() == 0) {
                return;
            }

            Type type = null;
            foreach(PropertyInfo item in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
                if(!names.Contains(item.Name) || !item.CanRead || !item.CanWrite || item.GetIndexParameters().Length > 0) {
                    continue;
                }
                type = item.PropertyType;
                if(type.IsArray || type.IsInterface || type.ContainsGenericParameters) {
                    continue;
                }

                if(type.IsPrimitive) {
                    if(type.IsNumber()) {
                        item.SetValue(target, 0, null);
                    } else if(type.IsBoolean()) {
                        item.SetValue(target, false, null);
                    } else if(type.IsChar()) {
                        item.SetValue(target, ' ', null);
                    }
                } else if(type.IsDecimal()) {
                    item.SetValue(target, 0M, null);
                } else if(type.IsString()) {
                    item.SetValue(target, null, null);
                } else if(type.IsEnum) {
                    item.SetValue(target, 0, null);
                } else if(type.IsDateTime()) {
                    item.SetValue(target, DateTime.MinValue, null);
                } else if(type.IsNullable() && type.GetGenericArguments()[0].IsPrimitive) {
                    item.SetValue(target, null, null);
                }
            }
        }
    }
}
