using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a key-value pair which suports XML serializing.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class XmlSerializableKeyValuePair<TKey, TValue> : IXmlSerializable {
        /// <summary>
        /// Initialize a instance of XmlSerializableKeyValuePair class.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public XmlSerializableKeyValuePair() {
        }

        /// <summary>
        /// Initialize a instance of XmlSerializableKeyValuePair class.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public XmlSerializableKeyValuePair(TKey key, TValue value) {
            this.Key = key;
            this.Value = value;
        }

        private const string KEY_ELEMENT_NAME = "Key";
        private const string VALUE_ELEMENT_NAME = "Value";

        /// <summary>
        /// Gets or sets key.
        /// </summary>
        public TKey Key {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets value.
        /// </summary>
        public TValue Value {
            get;
            set;
        }

        public override string ToString() {
            return string.Format("Key: {0}, Value: {1}", this.Key, this.Value);
        }

        #region IXmlSerializable Members

        /// <inheritdoc />
        public XmlSchema GetSchema() {
            return null;
        }

        /// <inheritdoc />
        public void ReadXml(XmlReader reader) {
            XmlSerializer ks = new XmlSerializer(typeof(TKey));
            XmlSerializer vs = new XmlSerializer(typeof(TValue));

            reader.ReadStartElement();

            reader.ReadStartElement(KEY_ELEMENT_NAME);
            this.Key = (TKey) ks.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement(VALUE_ELEMENT_NAME);
            this.Value = (TValue) vs.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadEndElement();
        }

        /// <inheritdoc />
        public void WriteXml(XmlWriter writer) {
            XmlSerializer ks = new XmlSerializer(typeof(TKey));
            XmlSerializer vs = new XmlSerializer(typeof(TValue));

            writer.WriteStartElement(KEY_ELEMENT_NAME);
            ks.Serialize(writer, this.Key);
            writer.WriteEndElement();

            writer.WriteStartElement(VALUE_ELEMENT_NAME);
            vs.Serialize(writer, this.Value);
            writer.WriteEndElement();
        }

        #endregion
    }
}
