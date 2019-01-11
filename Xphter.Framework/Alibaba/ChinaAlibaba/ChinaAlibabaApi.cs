using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaApi : IChinaAlibabaApi {
        public string ProtocalVersion {
            get;
            set;
        }

        public string ApiVersion {
            get;
            set;
        }

        public string ApiNamespace {
            get;
            set;
        }

        public string ApiName {
            get;
            set;
        }

        public bool NeedHttps {
            get;
            set;
        }

        #region IChinaAlibabaApi Members

        public bool NeedAuthroized {
            get;
            set;
        }

        public bool NeedTimestamp {
            get;
            set;
        }

        public bool NeedSignature {
            get;
            set;
        }

        public Encoding Encoding {
            get;
            set;
        }

        public string GetRequestUri(string appKey) {
            return this.NeedHttps ?
                AlibabaHelper.GetChinaApiHttpsUri(this.ProtocalVersion, this.ApiVersion, this.ApiNamespace, this.ApiName, appKey) :
                AlibabaHelper.GetChinaApiHttpUri(this.ProtocalVersion, this.ApiVersion, this.ApiNamespace, this.ApiName, appKey);
        }

        #endregion
    }
}
