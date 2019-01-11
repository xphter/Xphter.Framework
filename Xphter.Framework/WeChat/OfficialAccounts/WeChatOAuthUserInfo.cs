using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    internal class WeChatOAuthUserInfo : WeChatReturnValue, IWeChatOAuthUserInfo {
        [ObfuscationAttribute]
        public string openid {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string unionid {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string nickname {
            get;
            set;
        }

        private bool? m_sex;
        [ObfuscationAttribute]
        public string sex {
            get {
                return this.m_sex.HasValue ? this.m_sex.Value ? "1" : "2" : "0";
            }
            set {
                switch(value) {
                    case "1":
                        this.m_sex = true;
                        break;
                    case "2":
                        this.m_sex = false;
                        break;
                    default:
                        this.m_sex = null;
                        break;
                }
            }
        }

        [ObfuscationAttribute]
        public string country {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string province {
            get;
            set;
        }

        [ObfuscationAttribute]
        public string city {
            get;
            set;
        }

        private string m_avatarUri;
        private string m_headimgurl;
        public string headimgurl {
            get {
                return this.m_headimgurl;
            }
            set {
                if(!string.IsNullOrWhiteSpace(this.m_headimgurl = value)) {
                    this.m_avatarUri = value.Substring(0, value.Length - 1);
                } else {
                    this.m_avatarUri = null;
                }
            }
        }

        public string[] privilege {
            get;
            set;
        }

        public override string ToString() {
            return this.nickname;
        }

        #region IWeChatOAuthUserInfo Members

        string IWeChatOAuthUserInfo.OpenID {
            get {
                return this.openid;
            }
        }

        string IWeChatOAuthUserInfo.UnionID {
            get {
                return this.unionid;
            }
        }

        string IWeChatOAuthUserInfo.Nickname {
            get {
                return this.nickname;
            }
        }

        bool? IWeChatOAuthUserInfo.Sex {
            get {
                return this.m_sex;
            }
        }

        string IWeChatOAuthUserInfo.Country {
            get {
                return this.country;
            }
        }

        string IWeChatOAuthUserInfo.Province {
            get {
                return this.province;
            }
        }

        string IWeChatOAuthUserInfo.City {
            get {
                return this.city;
            }
        }

        IEnumerable<string> IWeChatOAuthUserInfo.Privileges {
            get {
                return this.privilege;
            }
        }

        string IWeChatOAuthUserInfo.GetAvatarImageUri(WeChatAvatarImageSize size) {
            return this.m_avatarUri != null ? this.m_avatarUri + (int) size : null;
        }

        #endregion
    }
}
