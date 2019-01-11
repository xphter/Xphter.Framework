using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    internal class WeChatUsersReturnValue : WeChatReturnValue, IWeChatUsersReturnValue {
        [ObfuscationAttribute]
        public int total {
            get;
            set;
        }

        [ObfuscationAttribute]
        public int count {
            get;
            set;
        }

        [ObfuscationAttribute]
        public Data data {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string next_openid {
            get;
            set;
        }

        public class Data {
            [ObfuscationAttribute]
            public string[] openid {
                get;
                set;
            }
        }

        #region IWeChatUsersReturnValue Members

        public int TotalCount {
            get {
                return this.total;
            }
        }

        public int CurrentCount {
            get {
                return this.count;
            }
        }

        public IEnumerable<string> OpenIDList {
            get {
                return this.data != null ? this.data.openid : null;
            }
        }

        public string NextOpenID {
            get {
                return this.next_openid;
            }
        }

        #endregion
    }
}
