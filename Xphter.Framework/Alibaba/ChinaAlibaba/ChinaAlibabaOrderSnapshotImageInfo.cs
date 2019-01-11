using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public class ChinaAlibabaOrderSnapshotImageInfo : IChinaAlibabaOrderSnapshotImageInfo {
        [ObfuscationAttribute]
        public string imageUrl;

        [ObfuscationAttribute]
        public string summImageUrl;

        [ObfuscationAttribute]
        public string midImageUrl;

        #region IChinaAlibabaOrderSnapshotImageInfo Members

        string IChinaAlibabaOrderSnapshotImageInfo.BigImageUrl {
            get {
                return this.imageUrl;
            }
        }

        string IChinaAlibabaOrderSnapshotImageInfo.SmallImageUrl {
            get {
                return this.summImageUrl;
            }
        }

        string IChinaAlibabaOrderSnapshotImageInfo.MiddleImageUrl {
            get {
                return this.midImageUrl;
            }
        }

        #endregion
    }
}
