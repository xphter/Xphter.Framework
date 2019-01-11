using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaOrderLogisticsCompanyInfo : IChinaAlibabaOrderLogisticsCompanyInfo {
        [ObfuscationAttribute]
        public long id;

        [ObfuscationAttribute]
        public string companyName;

        [ObfuscationAttribute]
        public string companyNo;

        public override string ToString() {
            return this.companyName;
        }

        #region IChinaAlibabaOrderLogisticsCompanyInfo Members

        long IChinaAlibabaOrderLogisticsCompanyInfo.ID {
            get {
                return this.id;
            }
        }

        string IChinaAlibabaOrderLogisticsCompanyInfo.CompanyName {
            get {
                return this.companyName;
            }
        }

        string IChinaAlibabaOrderLogisticsCompanyInfo.CompanyNo {
            get {
                return this.companyNo;
            }
        }

        #endregion
    }
}
