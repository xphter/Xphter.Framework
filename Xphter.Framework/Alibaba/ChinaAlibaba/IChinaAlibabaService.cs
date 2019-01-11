using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    /// <summary>
    /// Provides functions of open.1688.com.
    /// </summary>
    public interface IChinaAlibabaService : IDisposable {
        /// <summary>
        /// Gets ali-ID of the authorized member.
        /// </summary>
        string AuthorizedAliID {
            get;
        }

        /// <summary>
        /// Gets member-ID of the authorized member.
        /// </summary>
        string AuthorizedMemberID {
            get;
        }

        /// <summary>
        /// occurred when the authorization info(refresh token...) has changed.
        /// </summary>
        event EventHandler<ChinaAlibabaAuthorizationChangedEventArgs> AuthorizationChanged;

        IAsyncResult BeginGetIsAuthorized(AsyncCallback callback, object userState);

        bool EndGetIsAuthorized(IAsyncResult asyncResult);

        IAsyncResult BeginAuthorize(string code, string redirectUri, AsyncCallback callback, object userState);

        void EndAuthorize(IAsyncResult asyncResult);

        IAsyncResult BeginGetCurrentTime(AsyncCallback callback, object userState);

        DateTime EndGetCurrentTime(IAsyncResult asyncResult);

        IAsyncResult BeginInvokeApi(IChinaAlibabaApi api, object arguments, AsyncCallback callback, object userState);

        string EndInvokeApi(IAsyncResult asyncResult);

        IAsyncResult BeginInvokeApi<TReturn>(IChinaAlibabaApi api, IChinaAlibabaApiArguments arguments, AsyncCallback callback, object userState);

        TReturn EndInvokeApi<TReturn>(IAsyncResult asyncResult);

        IAsyncResult BeginGetMemberInfo(string memberID, AsyncCallback callback, object userState);

        IChinaAlibabaMemberInfo EndGetMemberInfo(IAsyncResult asyncResult);

        IAsyncResult BeginGetOrderList(ChinaAlibabaGetOrderListArguments arguments, AsyncCallback callback, object userState);

        IChinaAlibabaOrderList EndGetOrderList(IAsyncResult asyncResult);

        IAsyncResult BeginGetOrderInfo(ChinaAlibabaGetOrderInfoArguments arguments, AsyncCallback callback, object userState);

        IChinaAlibabaOrderInfo EndGetOrderInfo(IAsyncResult asyncResult);
    }

    /// <summary>
    /// Provides information of AuthorizationChanged event.
    /// </summary>
    public class ChinaAlibabaAuthorizationChangedEventArgs : EventArgs {
        public ChinaAlibabaAuthorizationChangedEventArgs(string refreshToken, DateTime refreshTokenExpiredTime) {
            this.RefreshToken = refreshToken;
            this.RefreshTokenExpiredTime = refreshTokenExpiredTime;
        }

        public string RefreshToken {
            get;
            private set;
        }

        public DateTime RefreshTokenExpiredTime {
            get;
            private set;
        }
    }
}
