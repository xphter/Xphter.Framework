using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Xphter.Framework.Collections {
    /// <summary>
    /// Represents a dictionary which suports XML serializing.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class XmlSerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable {
        private const string KEY_ELEMENT_NAME = "Key";
        private const string VALUE_ELEMENT_NAME = "Value";

        #region IXmlSerializable Members

        /// <inheritdoc />
        public XmlSchema GetSchema() {
            return null;
        }

        /// <inheritdoc />
        public void ReadXml(XmlReader reader) {
            TKey key = default(TKey);
            TValue value = default(TValue);
            XmlSerializer ks = new XmlSerializer(typeof(TKey));
            XmlSerializer vs = new XmlSerializer(typeof(TValue));

            reader.ReadStartElement();
            while(reader.NodeType != XmlNodeType.EndElement) {
                reader.ReadStartElement(KEY_ELEMENT_NAME);
                key = (TKey) ks.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement(VALUE_ELEMENT_NAME);
                value = (TValue) vs.Deserialize(reader);
                reader.ReadEndElement();

                this[key] = value;
            }
            reader.ReadEndElement();
        }

        /// <inheritdoc />
        public void WriteXml(XmlWriter writer) {
            XmlSerializer ks = new XmlSerializer(typeof(TKey));
            XmlSerializer vs = new XmlSerializer(typeof(TValue));

            foreach(KeyValuePair<TKey, TValue> pair in this) {
                writer.WriteStartElement(KEY_ELEMENT_NAME);
                ks.Serialize(writer, pair.Key);
                writer.WriteEndElement();

                writer.WriteStartElement(VALUE_ELEMENT_NAME);
                vs.Serialize(writer, pair.Value);
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
