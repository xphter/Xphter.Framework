using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    internal class WeChatTemplateMessageReturnValue : WeChatReturnValue, IWeChatTemplateMessageReturnValue {
        [ObfuscationAttribute]
        public int msgid {
            get;
            set;
        }

        public override string ToString() {
            return this.msgid.ToString();
        }

        #region IWeChatTemplateMessageReturnValue Members

        int IWeChatTemplateMessageReturnValue.MessageID {
            get {
                return this.msgid;
            }
        }

        #endregion
    }
}
