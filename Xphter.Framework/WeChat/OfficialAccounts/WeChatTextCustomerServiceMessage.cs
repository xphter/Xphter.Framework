using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Web.JavaScript;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class WeChatTextCustomerServiceMessage : WeChatCustomerServiceMessage {
        public WeChatTextCustomerServiceMessage(string text) {
            this.m_text = new TextContent {
                Content = text,
            };
        }

        public override string MsgType {
            get {
                return WeChatMessageTypes.TEXT;
            }
        }

        private TextContent m_text;
        [JsonPropertyName("text")]
        public TextContent Text {
            get {
                return this.m_text;
            }
        }

        public class TextContent {
            [JsonPropertyName("content")]
            public string Content {
                get;
                internal set;
            }
        }
    }
}
