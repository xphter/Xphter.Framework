using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaApiFactory {
        IChinaAlibabaApi GetSystemTime {
            get;
        }

        IChinaAlibabaApi GetRefreshToken {
            get;
        }

        IChinaAlibabaApi GetAccessToken {
            get;
        }

        IChinaAlibabaApi PostponeRefreshToken {
            get;
        }

        IChinaAlibabaApi GetMemberInfo {
            get;
        }

        IChinaAlibabaApi GetOrderList {
            get;
        }

        IChinaAlibabaApi GetOrderInfo {
            get;
        }
    }
}
