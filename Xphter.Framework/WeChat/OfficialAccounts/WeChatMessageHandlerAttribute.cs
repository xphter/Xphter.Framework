using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class WeChatMessageHandlerAttribute : Attribute {
        public WeChatMessageHandlerAttribute(string messageType) {
            this.MessageType = messageType;
        }

        public string MessageType {
            get;
            private set;
        }
    }
}
