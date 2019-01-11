using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    [XmlRoot("xml")]
    public class WeChatTextReply : WeChatMessageReply {
        public override string MsgType {
            get {
                return WeChatMessageTypes.TEXT;
            }
            set {
                throw new NotSupportedException();
            }
        }

        public string Content {
            get;
            set;
        }

        public override string ToString() {
            return this.Content;
        }
    }
}
