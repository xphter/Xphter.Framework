using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaOrderSnapshotImageInfo {
        string BigImageUrl {
            get;
        }

        string SmallImageUrl {
            get;
        }

        string MiddleImageUrl {
            get;
        }
    }
}
