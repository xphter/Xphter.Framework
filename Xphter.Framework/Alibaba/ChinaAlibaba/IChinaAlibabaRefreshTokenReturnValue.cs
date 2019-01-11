using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaRefreshTokenReturnValue : IChinaAlibabaAccessTokenReturnValue {
        string RefreshToken {
            get;
        }

        DateTime RefreshTokenExpiredTime {
            get;
        }
    }
}
