using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaMemberInfo : IChinaAlibabaMemberInfo {
        [ObfuscationAttribute]
        public string memberID;

        [ObfuscationAttribute]
        public string status;

        [ObfuscationAttribute]
        public bool isTP;

        [ObfuscationAttribute]
        public bool haveSite;

        [ObfuscationAttribute]
        public bool isPersonalTP;

        [ObfuscationAttribute]
        public bool isEnterpriseTP;

        [ObfuscationAttribute]
        public bool isMarketTP;

        [ObfuscationAttribute]
        public bool isETCTP;

        [ObfuscationAttribute]
        public bool haveDistribution;

        [ObfuscationAttribute]
        public bool isDistribution;

        [ObfuscationAttribute]
        public bool isMobileBind;

        [ObfuscationAttribute]
        public bool isEmailBind;

        [ObfuscationAttribute]
        public bool havePrecharge;

        [ObfuscationAttribute]
        public bool isPrecharge;

        [ObfuscationAttribute]
        public bool isCreditProtection;

        [ObfuscationAttribute]
        public bool isPublishedCompany;

        [ObfuscationAttribute]
        public bool isAlipayBind;

        [ObfuscationAttribute]
        public string personalFileAddress;

        [ObfuscationAttribute]
        public string winportAddress;

        [ObfuscationAttribute]
        public string domainAddress;

        [ObfuscationAttribute]
        public string companyAddress;

        [ObfuscationAttribute]
        public string createTime;

        [ObfuscationAttribute]
        public string lastLogin;

        [ObfuscationAttribute]
        public string companyName;

        [ObfuscationAttribute]
        public string industry;

        [ObfuscationAttribute]
        public string product;

        [ObfuscationAttribute]
        public string homepageUrl;

        [ObfuscationAttribute]
        public string sellerName;

        [ObfuscationAttribute]
        public string sex;

        [ObfuscationAttribute]
        public string department;

        [ObfuscationAttribute]
        public string openJobTitle;

        [ObfuscationAttribute]
        public string email;

        [ObfuscationAttribute]
        public string telephone;

        [ObfuscationAttribute]
        public string fax;

        [ObfuscationAttribute]
        public string mobilePhone;

        [ObfuscationAttribute]
        public string addressLocation;

        public override int GetHashCode() {
            return this.memberID.GetHashCode();
        }

        public override bool Equals(object obj) {
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is ChinaAlibabaMemberInfo)) {
                return false;
            }

            return string.Equals(this.memberID, ((ChinaAlibabaMemberInfo) obj).memberID);
        }

        public override string ToString() {
            return this.companyName ?? this.sellerName;
        }

        #region IChinaAlibabaMemberInfo Members

        string IChinaAlibabaMemberInfo.MemberID {
            get {
                return this.memberID;
            }
        }

        bool IChinaAlibabaMemberInfo.IsEnabled {
            get {
                return string.Equals(this.status, "enabled", StringComparison.OrdinalIgnoreCase);
            }
        }

        bool IChinaAlibabaMemberInfo.IsTP {
            get {
                return this.isTP;
            }
        }

        bool IChinaAlibabaMemberInfo.HaveSite {
            get {
                return this.haveSite;
            }
        }

        bool IChinaAlibabaMemberInfo.IsPersonalTP {
            get {
                return this.isPersonalTP;
            }
        }

        bool IChinaAlibabaMemberInfo.IsEnterpriseTP {
            get {
                return this.isEnterpriseTP;
            }
        }

        bool IChinaAlibabaMemberInfo.IsMarketTP {
            get {
                return this.isMarketTP;
            }
        }

        bool IChinaAlibabaMemberInfo.IsETCTP {
            get {
                return this.isETCTP;
            }
        }

        bool IChinaAlibabaMemberInfo.HaveDistribution {
            get {
                return this.haveDistribution;
            }
        }

        bool IChinaAlibabaMemberInfo.IsDistribution {
            get {
                return this.isDistribution;
            }
        }

        bool IChinaAlibabaMemberInfo.IsMobileBind {
            get {
                return this.isMobileBind;
            }
        }

        bool IChinaAlibabaMemberInfo.IsEmailBind {
            get {
                return this.isEmailBind;
            }
        }

        bool IChinaAlibabaMemberInfo.HavePrecharge {
            get {
                return this.havePrecharge;
            }
        }

        bool IChinaAlibabaMemberInfo.IsPrecharge {
            get {
                return this.isPrecharge;
            }
        }

        bool IChinaAlibabaMemberInfo.IsCreditProtection {
            get {
                return this.isCreditProtection;
            }
        }

        bool IChinaAlibabaMemberInfo.IsPublishedCompany {
            get {
                return this.isPublishedCompany;
            }
        }

        bool IChinaAlibabaMemberInfo.IsAlipayBind {
            get {
                return this.isAlipayBind;
            }
        }

        string IChinaAlibabaMemberInfo.PersonalFileAddress {
            get {
                return this.personalFileAddress;
            }
        }

        string IChinaAlibabaMemberInfo.WinportUrl {
            get {
                return this.winportAddress;
            }
        }

        string IChinaAlibabaMemberInfo.DomainUrl {
            get {
                return this.domainAddress;
            }
        }

        string IChinaAlibabaMemberInfo.CompanyUrl {
            get {
                return this.companyAddress;
            }
        }

        DateTime? IChinaAlibabaMemberInfo.CreateTime {
            get {
                return !string.IsNullOrWhiteSpace(this.createTime) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.createTime) : null;
            }
        }

        DateTime? IChinaAlibabaMemberInfo.LastLoginTime {
            get {
                return !string.IsNullOrWhiteSpace(this.lastLogin) ? (DateTime?) AlibabaHelper.AlibabaTimeToLocalTime(this.lastLogin) : null;
            }
        }

        string IChinaAlibabaMemberInfo.CompanyName {
            get {
                return this.companyName;
            }
        }

        string IChinaAlibabaMemberInfo.CompanyIndustry {
            get {
                return this.industry;
            }
        }

        string IChinaAlibabaMemberInfo.CompanyProduct {
            get {
                return this.product;
            }
        }

        string IChinaAlibabaMemberInfo.HomepageUrl {
            get {
                return this.homepageUrl;
            }
        }

        string IChinaAlibabaMemberInfo.ContactsName {
            get {
                return this.sellerName;
            }
        }

        bool IChinaAlibabaMemberInfo.ContactsSex {
            get {
                return string.Equals(this.sex, "先生", StringComparison.OrdinalIgnoreCase);
            }
        }

        string IChinaAlibabaMemberInfo.ContactsDepartment {
            get {
                return this.department;
            }
        }

        string IChinaAlibabaMemberInfo.ContactsJob {
            get {
                return this.openJobTitle;
            }
        }

        string IChinaAlibabaMemberInfo.ContactsEmail {
            get {
                return this.email;
            }
        }

        string IChinaAlibabaMemberInfo.ContactsTelephone {
            get {
                return this.telephone;
            }
        }

        string IChinaAlibabaMemberInfo.ContactsFax {
            get {
                return this.fax;
            }
        }

        string IChinaAlibabaMemberInfo.ContactsMobilePhone {
            get {
                return this.mobilePhone;
            }
        }

        string IChinaAlibabaMemberInfo.ContactsAddress {
            get {
                return this.addressLocation;
            }
        }

        #endregion
    }
}
