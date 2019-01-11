using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using Xphter.Framework.Collections;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Net {
    /// <summary>
    /// Provides functions for serializing a object to form values.
    /// </summary>
    public class FormValuesSerializer {
        /// <summary>
        /// The Member name of Nullable<T> type.
        /// </summary>
        private static readonly string Nullable_HasValue = ((MemberExpression) ((Expression<Func<int?, bool>>) ((obj) => obj.HasValue)).Body).Member.Name;
        private static readonly string Nullable_Value = ((MemberExpression) ((Expression<Func<int?, int>>) ((obj) => obj.Value)).Body).Member.Name;
        private static readonly Type IEnumerable_Definition = typeof(IEnumerable<>);

        private IList<FormNameValuePair> m_pairs = new List<FormNameValuePair>();
        private Encoding m_ascii = Encoding.ASCII;
        private Encoding m_utf8 = Encoding.UTF8;
        private byte[] m_buffer = new byte[1024];
        private byte[] m_prefixBoundary;
        private byte[] m_endBoundary;

        /// <summary>
        /// Writes content to a "application/x-www-form-urlencoded" stream.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="writer"></param>
        private void WriteEncodedToStream(string content, Stream writer) {
            if(string.IsNullOrEmpty(content)) {
                return;
            }

            int length = this.m_ascii.GetByteCount(content);
            if(this.m_buffer.Length <= length) {
                this.m_buffer = new byte[length * 2];
            }

            this.m_ascii.GetBytes(content, 0, content.Length, this.m_buffer, 0);
            writer.Write(this.m_buffer, 0, length);
        }

        /// <summary>
        /// Writes content to a "multipart/form-data" stream.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="writer"></param>
        private void WriteUncodedToStream(string content, Stream writer) {
            if(string.IsNullOrEmpty(content)) {
                return;
            }

            int length = this.m_utf8.GetByteCount(content);
            if(this.m_buffer.Length <= length) {
                this.m_buffer = new byte[length * 2];
            }

            this.m_utf8.GetBytes(content, 0, content.Length, this.m_buffer, 0);
            writer.Write(this.m_buffer, 0, length);
        }

        /// <summary>
        /// Writes a source stream to a destination stream.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="writer"></param>
        private void WriteToStream(Stream reader, Stream writer) {
            int count = 0;
            while((count = reader.Read(this.m_buffer, 0, this.m_buffer.Length)) > 0) {
                writer.Write(this.m_buffer, 0, count);
            }
        }

        /// <summary>
        /// Writes prefix-boundary to a "multipart/form-data" stream.
        /// </summary>
        /// <param name="writer"></param>
        private void WritePrefixBoundary(Stream writer) {
            writer.Write(this.m_prefixBoundary, 0, this.m_prefixBoundary.Length);
        }

        /// <summary>
        /// Writes end-boundary to a "multipart/form-data" stream.
        /// </summary>
        /// <param name="writer"></param>
        private void WriteEndBoundary(Stream writer) {
            writer.Write(this.m_endBoundary, 0, this.m_endBoundary.Length);
        }

        /// <summary>
        /// Finds all name-value pairs from the specified object.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        private void FindNameValuePairs(string name, object obj) {
            if(obj == null) {
                return;
            }

            int index = 0;
            Type type = obj.GetType();
            if(type.IsPrimitive || type.IsDecimal() || type.IsString() || type.IsEnum) {
                //primitive type, decimal, string, enum: use return value of ToString method
                this.m_pairs.Add(new FormNameValuePair(this, name, new StringFormValue(this, obj.ToString())));
            } else if(type.IsDateTime()) {
                //DateTime: use return value of ToString("yyyy-MM-dd HH:mm:ss") method
                this.m_pairs.Add(new FormNameValuePair(this, name, new StringFormValue(this, ((DateTime) obj).ToString("yyyy-MM-dd HH:mm:ss"))));
            } else if(Convert.IsDBNull(obj)) {
                //DBNull: ignore
            } else if(type.IsNullable()) {
                //Nullable: if it has value then find name-value pairs from it's value
                if((bool) type.GetProperty(Nullable_HasValue, BindingFlags.Instance | BindingFlags.Public).GetValue(obj, null)) {
                    this.FindNameValuePairs(name, type.GetProperty(Nullable_Value, BindingFlags.Instance | BindingFlags.Public).GetValue(obj, null));
                }
            } else if(obj is HttpPostingFile) {
                //HttpPostingFile: use file content
                string filePath = ((HttpPostingFile) obj).FilePath;
                this.m_pairs.Add(new FormNameValuePair(this, name, new FileContentFormValue(this, (HttpPostingFile) obj), HttpHelper.GetContentType(Path.GetExtension(filePath)), new Dictionary<string, string> {
                    { "filename", Path.GetFileName(filePath) },
                }));
            } else if(type.IsValueType || type.IsClass) {
                if(type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition().GetInterface(IEnumerable_Definition.FullName) != null) {
                    //Array, IEnmerable<T>: treated according to the element type
                    Type elementType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];
                    if(elementType.IsByte()) {
                        //byte[], IEnumerable<byte>: use Base64 encoding string
                        this.m_pairs.Add(new FormNameValuePair(this, name, new StringFormValue(this, Convert.ToBase64String(((IEnumerable<byte>) obj).ToArray()))));
                    } else {
                        //other types: use "name[index]=value" format
                        foreach(object item in (IEnumerable) obj) {
                            this.FindNameValuePairs(string.Format("{0}[{1}]", name, index++), item);
                        }
                    }
                } else if(type.GetInterface(typeof(IEnumerable).FullName) != null) {
                    //IEnumerable: use "name[index]=value" format of each element
                    foreach(object item in (IEnumerable) obj) {
                        this.FindNameValuePairs(string.Format("{0}[{1}]", name, index++), item);
                    }
                } else {
                    //class or structure: use "name.property=value" format of each field and property
                    foreach(FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)) {
                        if(Attribute.GetCustomAttributes(field, typeof(FormValuesIgnoreAttribute), true).Length > 0) {
                            continue;
                        }

                        this.FindNameValuePairs(string.IsNullOrEmpty(name) ? field.Name : string.Format("{0}.{1}", name, field.Name), field.GetValue(obj));
                    }
                    foreach(PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                        if(property.GetIndexParameters().Length > 0) {
                            continue;
                        }
                        if(Attribute.GetCustomAttributes(property, typeof(FormValuesIgnoreAttribute), true).Length > 0) {
                            continue;
                        }

                        this.FindNameValuePairs(string.IsNullOrEmpty(name) ? property.Name : string.Format("{0}.{1}", name, property.Name), property.GetValue(obj, null));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified data must be named.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckIsMustName(object data) {
            if(data == null) {
                return false;
            }

            Type type = data.GetType();
            return type.IsSimple() ||
                data is HttpPostingFile ||
                data is IEnumerable;
        }

        /// <summary>
        /// Determines whether need to perform URL encode of the specified posted data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Return true if need use "application/x-www-form-urlencoded" type, otherwise return false to indicate use "multipart/form-data" type.</returns>
        public static bool CheckEncodeRequirement(object data) {
            if(data == null) {
                return true;
            }

            Type type = null;
            bool result = true;
            object current = null;

            Queue<object> queue = new Queue<object>();
            queue.Enqueue(data);
            while(queue.Count > 0) {
                current = queue.Dequeue();
                type = current.GetType();

                if(type.IsPrimitive || type.IsDecimal() || type.IsString() || type.IsDateTime() || type.IsEnum || Convert.IsDBNull(current)) {
                    continue;
                }

                if(current is HttpPostingFile) {
                    result = false;
                    break;
                } else if(type.IsNullable()) {
                    if((bool) type.GetProperty(Nullable_HasValue, BindingFlags.Instance | BindingFlags.Public).GetValue(current, null)) {
                        queue.Enqueue(type.GetProperty(Nullable_Value, BindingFlags.Instance | BindingFlags.Public).GetValue(current, null));
                    }
                } else if(type.IsValueType || type.IsClass) {
                    if(type.GetInterface(typeof(IEnumerable).FullName) != null) {
                        foreach(object item in (IEnumerable) current) {
                            if(item != null) {
                                queue.Enqueue(item);
                            }
                        }
                    } else {
                        object value = null;
                        foreach(FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)) {
                            if(Attribute.GetCustomAttributes(field, typeof(FormValuesIgnoreAttribute), true).Length > 0) {
                                continue;
                            }

                            if((value = field.GetValue(current)) != null) {
                                queue.Enqueue(value);
                            }
                        }
                        foreach(PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                            if(property.GetIndexParameters().Length > 0) {
                                continue;
                            }
                            if(Attribute.GetCustomAttributes(property, typeof(FormValuesIgnoreAttribute), true).Length > 0) {
                                continue;
                            }

                            if((value = property.GetValue(current, null)) != null) {
                                queue.Enqueue(value);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Serializes the specified object to a "application/x-www-form-urlencoded" string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is simple type, you must specify a name for it.</exception>
        public string Serialize(object obj) {
            StringBuilder builder = new StringBuilder();
            this.Serialize(obj, null, builder);
            return builder.ToString();
        }

        /// <summary>
        /// Serializes the specified object to a "application/x-www-form-urlencoded" string.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is simple type, you must specify a name for it.</exception>
        public string Serialize(object obj, string name) {
            StringBuilder builder = new StringBuilder();
            this.Serialize(obj, name, builder);
            return builder.ToString();
        }

        /// <summary>
        /// Serializes the specified object to a "application/x-www-form-urlencoded" StringBuilder.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="builder"></param>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is simple type, you must specify a name for it.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder"/> is null.</exception>
        public void Serialize(object obj, StringBuilder builder) {
            this.Serialize(obj, null, builder);
        }

        /// <summary>
        /// Serializes the specified object to a "application/x-www-form-urlencoded" StringBuilder.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="builder"></param>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is simple type, you must specify a name for it.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="builder"/> is null.</exception>
        public void Serialize(object obj, string name, StringBuilder builder) {
            if(obj == null || Convert.IsDBNull(obj)) {
                return;
            }
            if(string.IsNullOrWhiteSpace(name) && CheckIsMustName(obj)) {
                throw new ArgumentException("obj is simple type, you must specify a name for it.", "obj");
            }
            if(builder == null) {
                throw new ArgumentNullException("builder");
            }

            this.m_pairs.Clear();
            this.FindNameValuePairs(name, obj);
            for(var i = 0; i < this.m_pairs.Count; i++) {
                if(i > 0) {
                    builder.Append('&');
                }
                this.m_pairs[i].WriteToStringBuilder(builder);
            }
        }

        /// <summary>
        /// Serializes the specified object to a "application/x-www-form-urlencoded" stream.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="writer"></param>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is simple type, you must specify a name for it.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public void Serialize(object obj, Stream writer) {
            this.Serialize(obj, null, writer);
        }

        /// <summary>
        /// Serializes the specified object to a "application/x-www-form-urlencoded" stream.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="writer"></param>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is simple type, you must specify a name for it.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public void Serialize(object obj, string name, Stream writer) {
            if(obj == null || Convert.IsDBNull(obj)) {
                return;
            }
            if(string.IsNullOrWhiteSpace(name) && CheckIsMustName(obj)) {
                throw new ArgumentException("obj is simple type, you must specify a name for it.", "obj");
            }
            if(writer == null) {
                throw new ArgumentNullException("writer");
            }

            this.m_pairs.Clear();
            this.FindNameValuePairs(name, obj);
            for(var i = 0; i < this.m_pairs.Count; i++) {
                if(i > 0) {
                    this.WriteEncodedToStream("&", writer);
                }
                this.m_pairs[i].WriteToEncodedStream(writer);
            }
        }

        /// <summary>
        /// Serializes the specified object to a "multipart/form-data" stream.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <param name="boundary"></param>
        /// <param name="writer"></param>
        /// <exception cref="System.ArgumentException"><paramref name="obj"/> is simple type, you must specify a name for it.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="writer"/> is null.</exception>
        public void Serialize(object obj, string name, string boundary, Stream writer) {
            if(obj == null || Convert.IsDBNull(obj)) {
                return;
            }
            if(string.IsNullOrWhiteSpace(name) && CheckIsMustName(obj)) {
                throw new ArgumentException("obj is simple type, you must specify a name for it.", "obj");
            }
            if(writer == null) {
                throw new ArgumentNullException("writer");
            }

            this.m_prefixBoundary = this.m_utf8.GetBytes(string.Format("--{0}\r\nContent-Disposition: form-data", boundary));
            this.m_endBoundary = this.m_utf8.GetBytes(string.Format("--{0}--", boundary));

            this.m_pairs.Clear();
            this.FindNameValuePairs(name, obj);

            if(this.m_pairs.Count > 0) {
                foreach(FormNameValuePair item in this.m_pairs) {
                    item.WriteToUncodedStream(writer);
                }
            }
            this.WriteEndBoundary(writer);
        }

        /// <summary>
        /// Represents a name-value pair.
        /// </summary>
        private class FormNameValuePair {
            public FormNameValuePair(FormValuesSerializer serializer, string name, IFormValue value)
                : this(serializer, name, value, null, null) {
            }

            public FormNameValuePair(FormValuesSerializer serializer, string name, IFormValue value, string contentType)
                : this(serializer, name, value, contentType, null) {
            }

            public FormNameValuePair(FormValuesSerializer serializer, string name, IFormValue value, string contentType, IDictionary<string, string> parameters) {
                this.m_name = name;
                this.m_value = value;
                this.m_contentType = contentType;
                this.m_serializer = serializer;
                this.m_parameters = new Dictionary<string, string> {
                    { "name", name },
                };
                if(parameters != null) {
                    foreach(KeyValuePair<string, string> item in parameters) {
                        this.m_parameters[item.Key] = item.Value;
                    }
                }
            }

            private string m_name;
            private IFormValue m_value;
            private string m_contentType;
            private FormValuesSerializer m_serializer;
            private IDictionary<string, string> m_parameters;

            /// <summary>
            /// Writes name-value pair to a StringBuilder.
            /// </summary>
            /// <param name="builder"></param>
            public void WriteToStringBuilder(StringBuilder builder) {
                if(string.IsNullOrEmpty(this.m_name)) {
                    return;
                }

                builder.AppendFormat("{0}={1}",
                    HttpUtility.UrlEncode(this.m_name, Encoding.UTF8),
                    HttpUtility.UrlEncode(this.m_value.GetStringContent(), Encoding.UTF8));
            }

            /// <summary>
            /// Writes name-value pair to a "application/x-www-form-urlencoded" stream.
            /// </summary>
            /// <param name="writer"></param>
            public void WriteToEncodedStream(Stream writer) {
                if(string.IsNullOrEmpty(this.m_name)) {
                    return;
                }

                this.m_serializer.WriteEncodedToStream(HttpUtility.UrlEncode(this.m_name, Encoding.UTF8), writer);
                this.m_serializer.WriteEncodedToStream("=", writer);
                this.m_serializer.WriteEncodedToStream(HttpUtility.UrlEncode(this.m_value.GetStringContent(), Encoding.UTF8), writer);
            }

            /// <summary>
            /// Writes name-value pair to a "multipart/form-data" stream.
            /// </summary>
            /// <param name="writer"></param>
            public void WriteToUncodedStream(Stream writer) {
                if(string.IsNullOrEmpty(this.m_name)) {
                    return;
                }

                this.m_serializer.WritePrefixBoundary(writer);
                foreach(KeyValuePair<string, string> pair in this.m_parameters) {
                    this.m_serializer.WriteUncodedToStream("; ", writer);
                    this.m_serializer.WriteUncodedToStream(pair.Key, writer);
                    this.m_serializer.WriteUncodedToStream("=\"", writer);
                    this.m_serializer.WriteUncodedToStream(pair.Value, writer);
                    this.m_serializer.WriteUncodedToStream("\"", writer);
                }
                this.m_serializer.WriteUncodedToStream("\r\n", writer);
                if(!string.IsNullOrWhiteSpace(this.m_contentType)) {
                    this.m_serializer.WriteUncodedToStream("Content-Type: ", writer);
                    this.m_serializer.WriteUncodedToStream(this.m_contentType, writer);
                    this.m_serializer.WriteUncodedToStream("\r\n", writer);
                }
                this.m_serializer.WriteUncodedToStream("\r\n", writer);
                this.m_value.WriteToUncodedStream(writer);
                this.m_serializer.WriteUncodedToStream("\r\n", writer);
            }

            /// <inheritdoc />
            public override string ToString() {
                return string.Format("{0}={1}", this.m_name, this.m_value);
            }
        }

        /// <summary>
        /// Represents a form value.
        /// </summary>
        private interface IFormValue {
            /// <summary>
            /// Gets string content of this form value.
            /// </summary>
            /// <returns></returns>
            string GetStringContent();

            /// <summary>
            /// Writes value content to a uncoded stream.
            /// </summary>
            /// <param name="writer"></param>
            void WriteToUncodedStream(Stream writer);
        }

        /// <summary>
        /// Represents a IFormValue to render string content.
        /// </summary>
        private class StringFormValue : IFormValue {
            public StringFormValue(FormValuesSerializer serializer, string value) {
                this.m_value = value;
                this.m_serializer = serializer;
            }

            private string m_value;
            private FormValuesSerializer m_serializer;

            /// <inheritdoc />
            public override string ToString() {
                return this.m_value;
            }

            #region IFormValue Members

            /// <inheritdoc />
            public string GetStringContent() {
                return this.m_value;
            }

            /// <inheritdoc />
            public void WriteToUncodedStream(Stream writer) {
                if(this.m_value.Length > 0) {
                    this.m_serializer.WriteUncodedToStream(this.m_value, writer);
                }
            }

            #endregion
        }

        /// <summary>
        /// Represents a IFormValue to render content of a file.
        /// </summary>
        private class FileContentFormValue : IFormValue {
            public FileContentFormValue(FormValuesSerializer serializer, HttpPostingFile file) {
                this.m_file = file;
                this.m_serializer = serializer;
            }

            private HttpPostingFile m_file;
            private FormValuesSerializer m_serializer;

            /// <inheritdoc />
            public override string ToString() {
                return "FileContent";
            }

            #region IFormValue Members

            /// <inheritdoc />
            public string GetStringContent() {
                return string.Empty;
            }

            /// <inheritdoc />
            public void WriteToUncodedStream(Stream writer) {
                if(this.m_file.FileContent != null) {
                    writer.Write(this.m_file.FileContent, 0, this.m_file.FileContent.Length);
                } else if(!string.IsNullOrWhiteSpace(this.m_file.FilePath) &&
                    File.Exists(this.m_file.FilePath)) {
                    using(Stream reader = File.OpenRead(this.m_file.FilePath)) {
                        this.m_serializer.WriteToStream(reader, writer);
                    }
                }
            }

            #endregion
        }
    }

    /// <summary>
    /// Specifies that FormValuesSerializer will not serialize the public property or public field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FormValuesIgnoreAttribute : Attribute {
        public Guid m_typeId = Guid.NewGuid();
        public override object TypeId {
            get {
                return m_typeId;
            }
        }
    }
}
