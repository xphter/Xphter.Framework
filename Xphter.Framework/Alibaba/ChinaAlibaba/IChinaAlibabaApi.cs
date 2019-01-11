using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public interface IChinaAlibabaApi {
        bool NeedAuthroized {
            get;
        }

        bool NeedTimestamp {
            get;
        }

        bool NeedSignature {
            get;
        }

        Encoding Encoding {
            get;
        }

        string GetRequestUri(string appKey);
    }
}
