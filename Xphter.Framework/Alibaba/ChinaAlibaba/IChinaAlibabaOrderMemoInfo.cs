using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaOrderMemoInfo {
        long ID {
            get;
        }

        DateTime? ModifiedTime {
            get;
        }

        DateTime? CreateTime {
            get;
        }

        long OrderID {
            get;
        }

        string MemberID {
            get;
        }

        string RemarkText {
            get;
        }

        string RemarkIcon {
            get;
        }
    }
}
