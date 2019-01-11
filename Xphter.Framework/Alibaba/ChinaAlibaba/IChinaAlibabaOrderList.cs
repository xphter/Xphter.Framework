using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaOrderList {
        int TotalCount {
            get;
        }

        string LastStartRow {
            get;
        }

        IEnumerable<IChinaAlibabaOrderInfo> OrderlList {
            get;
        }

        int RealPrePageSize {
            get;
        }
    }
}
