using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    internal class WeChatServerInfo : WeChatReturnValue, IWeChatServerInfo {
        [ObfuscationAttribute]
        public string[] ip_list {
            get;
            set;
        }

        #region IWeChatServerInfo Members

        public IEnumerable<string> IpList {
            get {
                return this.ip_list;
            }
        }

        #endregion
    }
}
