using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    internal class ChinaAlibabaOrderList : IChinaAlibabaOrderList {
        public ChinaAlibabaOrderList() {
            this.modelList = new ChinaAlibabaOrderInfo[0];
        }

        [ObfuscationAttribute]
        public int totalCount;

        [ObfuscationAttribute]
        public string lastStartRow;

        [ObfuscationAttribute]
        public ChinaAlibabaOrderInfo[] modelList;

        [ObfuscationAttribute]
        public int realPrePageSize;

        #region IChinaAlibabaOrderListResult Members

        int IChinaAlibabaOrderList.TotalCount {
            get {
                return this.totalCount;
            }
        }

        string IChinaAlibabaOrderList.LastStartRow {
            get {
                return this.lastStartRow;
            }
        }

        IEnumerable<IChinaAlibabaOrderInfo> IChinaAlibabaOrderList.OrderlList {
            get {
                return this.modelList;
            }
        }

        int IChinaAlibabaOrderList.RealPrePageSize {
            get {
                return this.realPrePageSize;
            }
        }

        #endregion
    }
}
