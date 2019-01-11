using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaAccessTokenReturnValue : IChinaAlibabaAccessTokenReturnValue {
        [ObfuscationAttribute]
        public string resource_owner {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string aliId {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string memberId {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string access_token {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string expires_in {
            get;
            set;
        }

        #region IChinaAlibabaAccessTokenReturnValue Members

        string IChinaAlibabaAccessTokenReturnValue.ResourceOwner {
            get {
                return this.resource_owner;
            }
        }

        string IChinaAlibabaAccessTokenReturnValue.AlibabaID {
            get {
                return this.aliId;
            }
        }

        string IChinaAlibabaAccessTokenReturnValue.MemberID {
            get {
                return this.memberId;
            }
        }

        string IChinaAlibabaAccessTokenReturnValue.AccessToken {
            get {
                return this.access_token;
            }
        }

        int IChinaAlibabaAccessTokenReturnValue.AccessTokenLifeTime {
            get {
                return int.Parse(this.expires_in);
            }
        }

        #endregion
    }
}
