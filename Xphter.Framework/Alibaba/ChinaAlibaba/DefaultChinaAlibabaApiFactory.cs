using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public class DefaultChinaAlibabaApiFactory : IChinaAlibabaApiFactory {
        #region IChinaAlibabaBasicRequestsProvider Members

        private IChinaAlibabaApi m_getSystemTime;
        public IChinaAlibabaApi GetSystemTime {
            get {
                return this.m_getSystemTime ?? (this.m_getSystemTime = new ChinaAlibabaApi {
                    ProtocalVersion = "param2",
                    ApiVersion = "1",
                    ApiNamespace = "cn.alibaba.open",
                    ApiName = "system.time.get",
                });
            }
        }

        private IChinaAlibabaApi m_getRefreshToken;
        public IChinaAlibabaApi GetRefreshToken {
            get {
                return this.m_getRefreshToken ?? (this.m_getRefreshToken = new ChinaAlibabaApi {
                    ProtocalVersion = "http",
                    ApiVersion = "1",
                    ApiNamespace = "system.oauth2",
                    ApiName = "getToken",
                    NeedHttps = true,
                });
            }
        }

        private IChinaAlibabaApi m_getAccessToken;
        public IChinaAlibabaApi GetAccessToken {
            get {
                return this.m_getAccessToken ?? (this.m_getAccessToken = new ChinaAlibabaApi {
                    ProtocalVersion = "param2",
                    ApiVersion = "1",
                    ApiNamespace = "system.oauth2",
                    ApiName = "getToken",
                    NeedHttps = true,
                });
            }
        }

        private IChinaAlibabaApi m_postponeRefreshToken;
        public IChinaAlibabaApi PostponeRefreshToken {
            get {
                return this.m_postponeRefreshToken ?? (this.m_postponeRefreshToken = new ChinaAlibabaApi {
                    ProtocalVersion = "param2",
                    ApiVersion = "1",
                    ApiNamespace = "system.oauth2",
                    ApiName = "postponeToken",
                    NeedHttps = true,
                    NeedAuthroized = true,
                });
            }
        }

        private IChinaAlibabaApi m_getMemberInfo;
        public IChinaAlibabaApi GetMemberInfo {
            get {
                return this.m_getMemberInfo ?? (this.m_getMemberInfo = new ChinaAlibabaApi {
                    ProtocalVersion = "param2",
                    ApiVersion = "1",
                    ApiNamespace = "cn.alibaba.open",
                    ApiName = "member.get",
                    NeedSignature = true,
                });
            }
        }

        private IChinaAlibabaApi m_getOrderList;
        public IChinaAlibabaApi GetOrderList {
            get {
                return this.m_getOrderList ?? (this.m_getOrderList = new ChinaAlibabaApi {
                    ProtocalVersion = "param2",
                    ApiVersion = "2",
                    ApiNamespace = "cn.alibaba.open",
                    ApiName = "trade.order.list.get",
                    NeedAuthroized = true,
                    NeedSignature = true,
                });
            }
        }

        private IChinaAlibabaApi m_getOrderInfo;
        public IChinaAlibabaApi GetOrderInfo {
            get {
                return this.m_getOrderInfo ?? (this.m_getOrderInfo = new ChinaAlibabaApi {
                    ProtocalVersion = "param2",
                    ApiVersion = "1",
                    ApiNamespace = "cn.alibaba.open",
                    ApiName = "trade.order.detail.get",
                    NeedAuthroized = true,
                    NeedSignature = true,
                });
            }
        }

        #endregion
    }
}
