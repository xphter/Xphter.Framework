using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public abstract class WeChatMessageReply : IWeChatMessageResult {
        public virtual string ToUserName {
            get;
            set;
        }

        public virtual string FromUserName {
            get;
            set;
        }

        public virtual int CreateTime {
            get;
            set;
        }

        public abstract string MsgType {
            get;
            set;
        }

        #region IWeChatMessageResult Members

        public virtual string ContentType {
            get {
                return "text/xml";
            }
        }

        public virtual string ResultContent {
            get {
                StringBuilder content = new StringBuilder();
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add(string.Empty, string.Empty);
                using(XmlWriter writer = XmlWriter.Create(content, new XmlWriterSettings {
                    Indent = true,
                    OmitXmlDeclaration = true,
                })) {
                    new XmlSerializer(this.GetType()).Serialize(writer, this, namespaces);
                }
                return content.ToString();
            }
        }

        #endregion
    }
}
