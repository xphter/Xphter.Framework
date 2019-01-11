using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaRefreshTokenReturnValue : ChinaAlibabaAccessTokenReturnValue, IChinaAlibabaRefreshTokenReturnValue {
        [ObfuscationAttribute]
        public string refresh_token {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string refresh_token_timeout {
            get;
            set;
        }

        #region IChinaAlibabaRefreshTokenReturnValue Members

        string IChinaAlibabaRefreshTokenReturnValue.RefreshToken {
            get {
                return this.refresh_token;
            }
        }

        DateTime IChinaAlibabaRefreshTokenReturnValue.RefreshTokenExpiredTime {
            get {
                return AlibabaHelper.AlibabaTimeToLocalTime(this.refresh_token_timeout);
            }
        }

        #endregion
    }
}
