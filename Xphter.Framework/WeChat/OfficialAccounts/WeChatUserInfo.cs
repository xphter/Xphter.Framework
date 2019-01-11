using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    internal class WeChatUserInfo : WeChatReturnValue, IWeChatUserInfo {
        private bool m_isSubscribed;
        [ObfuscationAttribute]
        public string subscribe {
            get {
                return this.m_isSubscribed ? "1" : "0";
            }
            set {
                switch(value) {
                    case "0":
                        this.m_isSubscribed = false;
                        break;
                    default:
                        this.m_isSubscribed = true;
                        break;
                }
            }
        }

        [ObfuscationAttribute]
        public int subscribe_time {
            get;
            set;
        }

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

        private WeChatLanguage m_language;
        [ObfuscationAttribute]
        public string language {
            get {
                return EnumUtility.GetDescription(this.m_language);
            }
            set {
                this.m_language = EnumUtility.GetValue<WeChatLanguage>(value);
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
        [ObfuscationAttribute]
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

        [ObfuscationAttribute]
        public string remark {
            get;
            set;
        }

        [ObfuscationAttribute]
        public int groupid {
            get;
            set;
        }

        public override string ToString() {
            return this.nickname;
        }

        #region IWeChatUserInfo Members

        bool IWeChatUserInfo.IsSubscribed {
            get {
                return this.m_isSubscribed;
            }
        }

        private DateTime? m_subscribeTime;
        DateTime IWeChatUserInfo.SubscribeTime {
            get {
                if(!this.m_subscribeTime.HasValue) {
                    this.m_subscribeTime = WeChatHelper.TimestampToLocalTime(this.subscribe_time);
                }
                return this.m_subscribeTime.Value;
            }
        }

        string IWeChatUserInfo.OpenID {
            get {
                return this.openid;
            }
        }

        string IWeChatUserInfo.UnionID {
            get {
                return this.unionid;
            }
        }

        string IWeChatUserInfo.Nickname {
            get {
                return this.nickname;
            }
        }

        bool? IWeChatUserInfo.Sex {
            get {
                return this.m_sex;
            }
        }

        WeChatLanguage IWeChatUserInfo.Language {
            get {
                return this.m_language;
            }
        }

        string IWeChatUserInfo.Country {
            get {
                return this.country;
            }
        }

        string IWeChatUserInfo.Province {
            get {
                return this.province;
            }
        }

        string IWeChatUserInfo.City {
            get {
                return this.city;
            }
        }

        string IWeChatUserInfo.Remark {
            get {
                return this.remark;
            }
        }

        int IWeChatUserInfo.GroupID {
            get {
                return this.groupid;
            }
        }

        string IWeChatUserInfo.AvatarImageUri {
            get {
                return this.m_headimgurl;
            }
        }

        string IWeChatUserInfo.GetAvatarImageUri(WeChatAvatarImageSize size) {
            return this.m_avatarUri != null ? this.m_avatarUri + (int) size : null;
        }

        #endregion
    }
}
