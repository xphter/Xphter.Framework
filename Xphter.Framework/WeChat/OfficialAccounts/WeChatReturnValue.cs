using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    internal class WeChatReturnValue : IWeChatReturnValue {
        static WeChatReturnValue() {
            g_serializer = new JavaScriptSerializer();
        }

        private static JavaScriptSerializer g_serializer;

        [ObfuscationAttribute]
        public int errcode {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string errmsg {
            get;
            set;
        }

        public static T Create<T>(string data) where T : IWeChatReturnValue {
            if(string.IsNullOrWhiteSpace(data)) {
                return default(T);
            }
            
            return g_serializer.Deserialize<T>(data);
        }

        public override string ToString() {
            return this.errmsg;
        }

        #region IWeChatReturnValue Members

        int IWeChatReturnValue.ErrorCode {
            get {
                return this.errcode;
            }
        }

        string IWeChatReturnValue.ErrorMessage {
            get {
                return this.errmsg;
            }
        }

        #endregion
    }
}
