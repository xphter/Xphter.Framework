using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Web.JavaScript {
    /// <summary>
    /// Provides functions for serializing a object to JSON string and create object from a JSON string.
    /// </summary>
    public class JsonSerializer {
        /// <summary>
        /// The Member name of Nullable<T> type.
        /// </summary>
        private static readonly string Nullable_HasValue = ((MemberExpression) ((Expression<Func<int?, bool>>) ((obj) => obj.HasValue)).Body).Member.Name;
        private static readonly string Nullable_Value = ((MemberExpression) ((Expression<Func<int?, int>>) ((obj) => obj.Value)).Body).Member.Name;

        private static readonly DateTime JavaScript_Date_Ticks_Begin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Encodes the specified string, so that they can used in JavaScript environment.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// <returns>A encoded string.</returns>
        private string EncodeJsString(string input) {
            if(string.IsNullOrEmpty(input)) {
                return input;
            }

            StringBuilder result = new StringBuilder();
            foreach(char c in input) {
                switch(c) {
                    case '\0':
                        result.Append("\\0");
                        break;
                    case '\b':
                        result.Append("\\b");
                        break;
                    case '\t':
                        result.Append("\\t");
                        break;
                    case '\r':
                        result.Append("\\r");
                        break;
                    case '\n':
                        result.Append("\\n");
                        break;
                    case '\v':
                        result.Append("\\v");
                        break;
                    case '\f':
                        result.Append("\\f");
                        break;
                    case '\"':
                        result.Append("\\\"");
                        break;
                    case '\'':
                        result.Append("\\\'");
                        break;
                    case '\\':
                        result.Append("\\\\");
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Serializes the specified object to a StringBuilder object.
        /// </summary>
        /// <param name="obj">The object will be serialized.</param>
        /// <param name="builder">A StringBuilder object to receive the output JSON string.</param>
        private void SerializeToStringBuilder(object obj, StringBuilder builder) {
            if(obj == null) {
                builder.Append("null");
                return;
            }

            Type type = obj.GetType();
            if(type.IsPrimitive || type.Equals(typeof(decimal))) {
                if(obj is bool) {
                    builder.Append(obj.ToString().ToLower());
                } else if(obj is char) {
                    builder.Append(string.Format("\"{0}\"", obj.ToString()));
                } else {
                    builder.Append(obj.ToString());
                }
            } else if(type.IsNullable()) {
                if((bool) type.GetProperty(Nullable_HasValue, BindingFlags.Instance | BindingFlags.Public).GetValue(obj, null)) {
                    this.SerializeToStringBuilder(type.GetProperty(Nullable_Value, BindingFlags.Instance | BindingFlags.Public).GetValue(obj, null), builder);
                } else {
                    builder.Append("null");
                }
            } else if(type.IsEnum) {
                builder.Append(((int) obj).ToString());
            } else if(obj is string) {
                builder.Append(string.Format("\"{0}\"", this.EncodeJsString(obj.ToString())));
            } else if(obj is DateTime) {
                builder.AppendFormat("new Date({0})", (TimeZoneInfo.ConvertTimeToUtc((DateTime) obj) - JavaScript_Date_Ticks_Begin).Ticks / 10000);
            } else if(Convert.IsDBNull(obj)) {
                builder.Append("null");
            } else if(type.IsValueType || type.IsClass) {
                if(obj is IEnumerable) {
                    IEnumerator iterator = ((IEnumerable) obj).GetEnumerator();
                    builder.Append("[");

                    int count = 0;
                    while(iterator.MoveNext()) {
                        this.SerializeToStringBuilder(iterator.Current, builder);
                        builder.Append(",");
                        ++count;
                    }

                    if(count > 0) {
                        builder.Remove(builder.Length - 1, 1);
                    }
                    builder.Append("]");
                } else {
                    builder.Append("{");

                    int count = 0;
                    string propertyName = null;
                    JsonPropertyNameAttribute propertyNameAttribute = null;

                    foreach(FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)) {
                        if(Attribute.GetCustomAttributes(field, typeof(JsonIgnoreAttribute), true).Length > 0) {
                            continue;
                        }

                        if((propertyNameAttribute = (JsonPropertyNameAttribute) Attribute.GetCustomAttributes(field, typeof(JsonPropertyNameAttribute), true).FirstOrDefault()) != null) {
                            propertyName = propertyNameAttribute.PropertyName;
                        } else {
                            propertyName = field.Name;
                        }

                        builder.AppendFormat("\"{0}\":", this.EncodeJsString(propertyName));
                        this.SerializeToStringBuilder(field.GetValue(obj), builder);
                        builder.Append(",");
                        ++count;
                    }
                    foreach(PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                        if(property.GetIndexParameters().Length > 0) {
                            continue;
                        }
                        if(Attribute.GetCustomAttributes(property, typeof(JsonIgnoreAttribute), true).Length > 0) {
                            continue;
                        }

                        if((propertyNameAttribute = (JsonPropertyNameAttribute) Attribute.GetCustomAttributes(property, typeof(JsonPropertyNameAttribute), true).FirstOrDefault()) != null) {
                            propertyName = propertyNameAttribute.PropertyName;
                        } else {
                            propertyName = property.Name;
                        }

                        builder.AppendFormat("\"{0}\":", this.EncodeJsString(propertyName));
                        this.SerializeToStringBuilder(property.GetValue(obj, null), builder);
                        builder.Append(",");
                        ++count;
                    }

                    if(count > 0) {
                        builder.Remove(builder.Length - 1, 1);
                    }
                    builder.Append("}");
                }
            }
        }

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="obj">The object will be serialized.</param>
        /// <returns>A JSON string.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="obj"/> is null.</exception>
        public string Serialize(object obj) {
            StringBuilder builder = new StringBuilder();
            this.SerializeToStringBuilder(obj, builder);
            return builder.ToString();
        }

        /// <summary>
        /// Serializes the specified object to a StringBuilder object.
        /// </summary>
        /// <param name="obj">The object will be serialized.</param>
        /// <param name="builder">A StringBuilder object to receive the output JSON string.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="obj"/> is null or <paramref name="builder"/> is null.</exception>
        public void Serialize(object obj, StringBuilder builder) {
            if(builder == null) {
                throw new ArgumentNullException("builder", "builder is null.");
            }

            this.SerializeToStringBuilder(obj, builder);
        }
    }

    /// <summary>
    /// Specifies that JsonSerializer will not serialize the public property or public field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class JsonIgnoreAttribute : Attribute {
        public Guid m_typeId = Guid.NewGuid();
        public override object TypeId {
            get {
                return m_typeId;
            }
        }
    }

    /// <summary>
    /// Specifies that property name of serialized JavaScript object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class JsonPropertyNameAttribute : Attribute {
        /// <summary>
        /// Initialize a new instance of JsonPropertyNameAttribute class.
        /// </summary>
        /// <param name="propertyName"></param>
        public JsonPropertyNameAttribute(string propertyName) {
            this.m_propertyName = propertyName;
        }

        private string m_propertyName;
        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName {
            get {
                return this.m_propertyName;
            }
        }

        public Guid m_typeId = Guid.NewGuid();
        public override object TypeId {
            get {
                return m_typeId;
            }
        }
    }
}
