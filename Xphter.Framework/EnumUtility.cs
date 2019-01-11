using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides functions to operate Enum type.
    /// </summary>
    public static class EnumUtility {
        public static IEnumerable<T> GetCustomAttributes<T>(Enum value) where T : Attribute {
            IEnumerable<T> result = null;
            Type enumType = value.GetType();
            string name = Enum.GetName(enumType, value);
            if(name != null) {
                object[] attributes = enumType.GetField(name).GetCustomAttributes(typeof(T), false);
                result = attributes.Cast<T>().ToArray();
            } else {
                result = Enumerable.Empty<T>();
            }
            return result;
        }

        /// <summary>
        /// Gets the description of a enum value.
        /// </summary>
        /// <param name="value">A enum value.</param>
        /// <returns>The description of <paramref name="value"/>.</returns>
        public static string GetDescription(Enum value) {
            string description = null;

            Type enumType = value.GetType();
            string name = Enum.GetName(enumType, value);
            if(name != null) {
                object[] attributes = enumType.GetField(name).GetCustomAttributes(typeof(DescriptionAttribute), false);
                if(attributes != null && attributes.Length > 0) {
                    description = ((DescriptionAttribute) attributes[0]).Description;
                }
            }

            return description ?? value.ToString();
        }

        /// <summary>
        /// Gets the descriptions of a enum value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetDescriptions(Enum value) {
            Type enumType = value.GetType();
            if(enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0) {
                throw new ArgumentException(string.Format("{0} not marked by {1} attribute", enumType.FullName, typeof(FlagsAttribute).Name), "value");
            }

            object[] attributes = null;
            ICollection<string> descriptions = new List<string>();
            foreach(Enum item in Enum.GetValues(enumType)) {
                if(Convert.ToUInt64(item) == 0 || !value.HasFlag(item)) {
                    continue;
                }

                attributes = enumType.GetField(Enum.GetName(enumType, item)).GetCustomAttributes(typeof(DescriptionAttribute), false);
                if(attributes != null && attributes.Length > 0) {
                    descriptions.Add(((DescriptionAttribute) attributes[0]).Description);
                }
            }

            return descriptions;
        }

        /// <summary>
        /// Gets a enum value by the specified description.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetValue<T>(string description) {
            Type enumType = typeof(T);
            if(!enumType.IsEnum) {
                throw new ArgumentException(string.Format("{0} is not a enum", enumType.FullName, typeof(FlagsAttribute).Name), "T");
            }
            if(string.IsNullOrEmpty(description)) {
                return default(T);
            }

            T result = default(T);
            DescriptionAttribute descriptionAttribute = null;
            foreach(T item in Enum.GetValues(enumType).Cast<T>()) {
                if((descriptionAttribute = (DescriptionAttribute) enumType.GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault()) == null) {
                    continue;
                }

                if(string.Equals(descriptionAttribute.Description, description, StringComparison.OrdinalIgnoreCase)) {
                    result = item;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a enum value by the specified descriptions.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="description"></param>
        /// <returns></returns>
        public static T GetValue<T>(IEnumerable<string> descriptions) {
            Type enumType = typeof(T);
            if(!enumType.IsEnum) {
                throw new ArgumentException(string.Format("{0} is not a enum", enumType.FullName, typeof(FlagsAttribute).Name), "T");
            }
            if(enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0) {
                throw new ArgumentException(string.Format("{0} not marked by {1} attribute", enumType.FullName, typeof(FlagsAttribute).Name), "T");
            }
            if(descriptions == null || (descriptions = descriptions.Where((item) => !string.IsNullOrEmpty(item)).Select((item) => item.ToLower()).ToArray()).Count() == 0) {
                return default(T);
            }

            ulong result = Convert.ToUInt64(default(T));
            DescriptionAttribute descriptionAttribute = null;
            foreach(T item in Enum.GetValues(enumType).Cast<T>()) {
                if((descriptionAttribute = (DescriptionAttribute) enumType.GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault()) == null) {
                    continue;
                }

                if(descriptions.Contains(descriptionAttribute.Description.ToLower())) {
                    result |= Convert.ToUInt64(item);
                }
            }

            return (T) Enum.ToObject(enumType, result);
        }
    }
}
