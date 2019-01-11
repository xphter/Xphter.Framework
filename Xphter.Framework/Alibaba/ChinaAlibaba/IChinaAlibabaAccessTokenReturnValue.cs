using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaAccessTokenReturnValue : IChinaAlibabaReturnValue {
        string ResourceOwner {
            get;
        }

        string AlibabaID {
            get;
        }

        string MemberID {
            get;
        }

        string AccessToken {
            get;
        }

        int AccessTokenLifeTime {
            get;
        }
    }
}
