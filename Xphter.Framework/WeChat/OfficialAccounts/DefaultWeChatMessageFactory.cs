using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class DefaultWeChatMessageFactory : IWeChatMessageFactory {
        public DefaultWeChatMessageFactory() {
            this.m_serializer = new XmlSerializer(typeof(WeChatMessage));
        }

        private XmlSerializer m_serializer;

        #region IWeChatMessageFactory Members

        public IWeChatMessage GetMessage(string data, IOfficialAccountService service) {
            if(string.IsNullOrWhiteSpace(data)) {
                return null;
            }

            WeChatMessage message = null;
            using(TextReader reader = new StringReader(data)) {
                message = (WeChatMessage) this.m_serializer.Deserialize(reader);
            }
            return message;
        }

        #endregion

        [XmlRoot("xml")]
        public class WeChatMessage : IWeChatMessage, IWeChatCommonMessage, IWeChatTextMessage, IWeChatImageMessage, IWeChatVoiceMessage, IWeChatVideoMessage, IWeChatLocationMessage, IWeChatLinkMessage, IWeChatEventMessage, IWeChatScanEvent, IWeChatMenuEvent {
            public string ToUserName {
                get;
                set;
            }

            public string FromUserName {
                get;
                set;
            }

            public int CreateTime {
                get;
                set;
            }

            public string MsgType {
                get;
                set;
            }

            #region Common Message Members

            public long MsgId {
                get;
                set;
            }

            public string Content {
                get;
                set;
            }

            public string MediaId {
                get;
                set;
            }

            public string PicUrl {
                get;
                set;
            }

            public string Format {
                get;
                set;
            }

            public string Recognition {
                get;
                set;
            }

            public string ThumbMediaId {
                get;
                set;
            }

            public float Location_X {
                get;
                set;
            }

            public string Location_Y {
                get;
                set;
            }

            public int Scale {
                get;
                set;
            }

            public string Label {
                get;
                set;
            }

            public float Title {
                get;
                set;
            }

            public string Description {
                get;
                set;
            }

            public string Url {
                get;
                set;
            }

            #endregion

            #region Event Message Members

            public string Event {
                get;
                set;
            }

            public string EventKey {
                get;
                set;
            }

            public string Ticket {
                get;
                set;
            }

            #endregion
        }
    }
}
