using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaMemberInfo : IChinaAlibabaReturnValue {
        string MemberID {
            get;
        }

        bool IsEnabled {
            get;
        }

        bool IsTP {
            get;
        }

        bool HaveSite {
            get;
        }

        bool IsPersonalTP {
            get;
        }

        bool IsEnterpriseTP {
            get;
        }

        bool IsMarketTP {
            get;
        }

        bool IsETCTP {
            get;
        }

        bool HaveDistribution {
            get;
        }

        bool IsDistribution {
            get;
        }

        bool IsMobileBind {
            get;
        }

        bool IsEmailBind {
            get;
        }

        bool HavePrecharge {
            get;
        }

        bool IsPrecharge {
            get;
        }

        bool IsCreditProtection {
            get;
        }

        bool IsPublishedCompany {
            get;
        }

        bool IsAlipayBind {
            get;
        }

        string PersonalFileAddress {
            get;
        }

        string WinportUrl {
            get;
        }

        string DomainUrl {
            get;
        }

        string CompanyUrl {
            get;
        }

        DateTime? CreateTime {
            get;
        }

        DateTime? LastLoginTime {
            get;
        }

        string CompanyName {
            get;
        }

        string CompanyIndustry {
            get;
        }

        string CompanyProduct {
            get;
        }

        string HomepageUrl {
            get;
        }

        string ContactsName {
            get;
        }

        bool ContactsSex {
            get;
        }

        string ContactsDepartment {
            get;
        }

        string ContactsJob {
            get;
        }

        string ContactsEmail {
            get;
        }

        string ContactsTelephone {
            get;
        }

        string ContactsFax {
            get;
        }

        string ContactsMobilePhone {
            get;
        }

        string ContactsAddress {
            get;
        }
    }
}
