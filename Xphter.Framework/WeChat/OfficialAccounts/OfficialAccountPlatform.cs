using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Script.Serialization;
using Xphter.Framework.Collections;
using Xphter.Framework.Net;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class OfficialAccountPlatform : IOfficialAccountService {
        public OfficialAccountPlatform(string token, string originalID, string appID, string appSecret, IWeChatMessageFactory messageFactory, IWeChatMessageHandlerFactory handlerFactory) {
            if(token == null) {
                throw new ArgumentNullException("token", "token is null.");
            }
            if(originalID == null) {
                throw new ArgumentNullException("originalID", "originalID is null.");
            }

            this.m_token = token;
            this.m_originalID = originalID;
            this.m_appID = appID;
            this.m_appSecret = appSecret;
            this.m_messageFactory = messageFactory ?? new DefaultWeChatMessageFactory();
            this.m_handlerFactory = handlerFactory ?? new DefaultWeChatMessageHandlerFactory();

            this.m_accessTokenLock = new object();
            this.m_accessTokenEvent = new AutoResetEvent(true);
            this.m_jsonSerializer = new JavaScriptSerializer();
            this.m_bufferManager = BufferManager.CreateBufferManager(BUFFER_POOL_SIZE, BUFFER_POOL_ITEM_SIZE);
        }

        private const int BUFFER_POOL_SIZE = 10;
        private const int BUFFER_POOL_ITEM_SIZE = 1024;

        private string m_token;
        private string m_originalID;
        private string m_appID;
        private string m_appSecret;
        private int m_timeout = 30000;
        private IWeChatMessageFactory m_messageFactory;
        private IWeChatMessageHandlerFactory m_handlerFactory;

        private string m_accessToken;
        private int m_accessTokenLifetime;
        private DateTime m_accessTokenCreateTime;
        private object m_accessTokenLock;
        private AutoResetEvent m_accessTokenEvent;
        private BufferManager m_bufferManager;
        private JavaScriptSerializer m_jsonSerializer;

        private HttpWebRequest CreateRequest(string url, string contentType, string method) {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            if(contentType != null) {
                request.ContentType = contentType;
            }
            request.Method = method ?? WebRequestMethods.Http.Get;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Timeout = this.m_timeout;
            request.ReadWriteTimeout = this.m_timeout;
            return request;
        }

        private WeChatException DetectWeChatException(IWeChatReturnValue value) {
            if(value != null && value.ErrorCode != 0) {
                return new WeChatException(value.ErrorMessage, value.ErrorCode);
            } else {
                return null;
            }
        }

        private void SendHttpRequest<TReturn>(HttpWebRequest request, Action<Exception, TReturn> callback) where TReturn : IWeChatReturnValue {
            Exception syncError = null;

            try {
                NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                    string content = null;
                    Exception error = null;
                    byte[] buffer = this.m_bufferManager.TakeBuffer(BUFFER_POOL_ITEM_SIZE);

                    try {
                        using(HttpWebResponse response = (HttpWebResponse) ((HttpWebRequest) ar.AsyncState).EndGetResponse(ar)) {
                            content = response.GetResponseString(Encoding.UTF8, buffer);
                        }
                    } catch(Exception ex) {
                        error = ex;
                    } finally {
                        this.m_bufferManager.ReturnBuffer(buffer);
                    }

                    TReturn value = default(TReturn);
                    if(error == null) {
                        try {
                            value = WeChatReturnValue.Create<TReturn>(content);
                            error = this.DetectWeChatException(value);
                        } catch(Exception ex) {
                            error = ex;
                        }
                    }

                    callback(error, value);
                }, request), request, this.m_timeout);
            } catch(Exception ex) {
                syncError = ex;
            }

            if(syncError != null) {
                callback(syncError, default(TReturn));
            }
        }

        private void SendGetRequest<TReturn>(string url, Action<Exception, TReturn> callback) where TReturn : IWeChatReturnValue {
            HttpWebRequest request = this.CreateRequest(url, null, null);
            this.SendHttpRequest<TReturn>(request, callback);
        }

        private void SendPostRequest<TReturn>(string url, object obj, Action<Exception, TReturn> callback) where TReturn : IWeChatReturnValue {
            byte[] data = Encoding.UTF8.GetBytes(obj is string ? (string) obj : this.m_jsonSerializer.Serialize(obj));
            HttpWebRequest request = this.CreateRequest(url, "application/json", WebRequestMethods.Http.Post);
            Exception syncError = null;

            try {
                request.ContentLength = data.Length;
                NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetRequestStream((ar) => {
                    try {
                        using(Stream writer = ((HttpWebRequest) ar.AsyncState).EndGetRequestStream(ar)) {
                            writer.Write(data, 0, data.Length);
                        }
                    } catch(Exception ex) {
                        callback(ex, default(TReturn));
                        return;
                    }

                    this.SendHttpRequest<TReturn>((HttpWebRequest) ar.AsyncState, callback);
                }, request), request, this.m_timeout);
            } catch(Exception ex) {
                syncError = ex;
            }

            if(syncError != null) {
                callback(syncError, default(TReturn));
            }
        }

        private void RefreshAccessToken(AsyncResult result, Action action) {
            if(!this.IsSupportCallWeChatService) {
                result.MarkCompleted(new NotSupportedException("AppID or AppSecret is undefined."), true);
                return;
            }

            this.m_accessTokenEvent.WaitOne();

            DateTime now = DateTime.Now;
            bool isValidAccessToken = false;

            lock(this.m_accessTokenLock) {
                isValidAccessToken = !string.IsNullOrWhiteSpace(this.m_accessToken) &&
                    (now - this.m_accessTokenCreateTime).TotalSeconds < this.m_accessTokenLifetime;
            }

            if(!isValidAccessToken) {
                this.SendGetRequest<WeChatAccessToken>(string.Format(
                    "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                    HttpUtility.UrlEncode(this.m_appID),
                    HttpUtility.UrlEncode(this.m_appSecret)), (error, token) => {
                        IWeChatAccessToken accessToken = (IWeChatAccessToken) token;
                        if(accessToken != null) {
                            lock(this.m_accessTokenLock) {
                                this.m_accessToken = accessToken.AccessToken;
                                this.m_accessTokenLifetime = accessToken.ExpiresIn / 2;
                                this.m_accessTokenCreateTime = now;
                            }
                        }
                        this.m_accessTokenEvent.Set();

                        if(error != null) {
                            result.MarkCompleted(error, false);
                        } else {
                            action();
                        }
                    });
            } else {
                this.m_accessTokenEvent.Set();

                action();
            }
        }

        private void PerformAsyncGetAction<TReturn>(AsyncResult result, Func<string, string> urlSelector, Action<Exception, TReturn> callback) where TReturn : IWeChatReturnValue {
            string url = null;
            string token = null;
            DateTime now = DateTime.Now;

            lock(this.m_accessTokenLock) {
                if(!string.IsNullOrWhiteSpace(this.m_accessToken) &&
                    (now - this.m_accessTokenCreateTime).TotalSeconds < this.m_accessTokenLifetime) {
                    token = this.m_accessToken;
                    url = urlSelector(token);
                }
            }

            if(token != null) {
                this.SendGetRequest<TReturn>(url, (error, value) => {
                    if(error != null && error is WeChatException) {
                        switch(((WeChatException) error).ErrorCode) {
                            case WeChatOfficialAccountErrorCodes.INVALID_ACCESS_TOKEN:
                            case WeChatOfficialAccountErrorCodes.ILLEGAL_ACCESS_TOKEN:
                            case WeChatOfficialAccountErrorCodes.ACCESS_TOKEN_TIMEOUT:
                                lock(this.m_accessTokenLock) {
                                    this.m_accessToken = null;
                                }

                                this.RefreshAccessToken(result, () => this.PerformAsyncGetAction<TReturn>(result, urlSelector, callback));
                                break;
                            default:
                                callback(error, value);
                                break;
                        }
                    } else {
                        callback(error, value);
                    }
                });
            } else {
                this.RefreshAccessToken(result, () => this.PerformAsyncGetAction<TReturn>(result, urlSelector, callback));
            }
        }

        private void PerformAsyncPostAction<TReturn>(AsyncResult result, Func<string, string> urlSelector, object obj, Action<Exception, TReturn> callback) where TReturn : IWeChatReturnValue {
            string url = null;
            string token = null;
            DateTime now = DateTime.Now;

            lock(this.m_accessTokenLock) {
                if(!string.IsNullOrWhiteSpace(this.m_accessToken) &&
                    (now - this.m_accessTokenCreateTime).TotalSeconds < this.m_accessTokenLifetime) {
                    token = this.m_accessToken;
                    url = urlSelector(token);
                }
            }

            if(token != null) {
                this.SendPostRequest<TReturn>(url, obj, (error, value) => {
                    if(error != null && error is WeChatException) {
                        switch(((WeChatException) error).ErrorCode) {
                            case WeChatOfficialAccountErrorCodes.INVALID_ACCESS_TOKEN:
                            case WeChatOfficialAccountErrorCodes.ILLEGAL_ACCESS_TOKEN:
                            case WeChatOfficialAccountErrorCodes.ACCESS_TOKEN_TIMEOUT:
                                lock(this.m_accessTokenLock) {
                                    this.m_accessToken = null;
                                }

                                this.RefreshAccessToken(result, () => this.PerformAsyncPostAction<TReturn>(result, urlSelector, obj, callback));
                                break;
                            default:
                                callback(error, value);
                                break;
                        }
                    } else {
                        callback(error, value);
                    }
                });
            } else {
                this.RefreshAccessToken(result, () => this.PerformAsyncPostAction<TReturn>(result, urlSelector, obj, callback));
            }
        }

        private MenuItem GetMenuItem(IWeChatMenuItem menuItem) {
            if(string.IsNullOrWhiteSpace(menuItem.Name)) {
                return null;
            }

            MenuItem item = new MenuItem {
                type = EnumUtility.GetCustomAttributes<WeChatMenuItemTypeAttribute>(menuItem.Type).First().MenuItemType,
                name = menuItem.Name,
            };
            switch(menuItem.Type) {
                case WeChatMenuItemType.View:
                    item.url = menuItem.Value;
                    break;
                default:
                    item.key = menuItem.Value;
                    break;
            }

            MenuItem childMenu = null;
            foreach(IWeChatMenuItem childItem in menuItem.ChildItems) {
                if((childMenu = this.GetMenuItem(childItem)) == null) {
                    continue;
                }

                item.sub_button.Add(childMenu);
            }


            return item;
        }

        private MenuItems GetMenuItems(IEnumerable<IWeChatMenuItem> menuItems) {
            MenuItem menuItem = null;
            MenuItems menu = new MenuItems();
            foreach(IWeChatMenuItem item in menuItems) {
                if((menuItem = this.GetMenuItem(item)) == null) {
                    continue;
                }

                menu.button.Add(menuItem);
            }
            return menu;
        }

        private void GetUsersList(AsyncResult<IWeChatUsersList> result, WeChatUsersList users, string nextOpenID) {
            this.PerformAsyncGetAction<WeChatUsersReturnValue>(result, (token) => string.Format(
                "https://api.weixin.qq.com/cgi-bin/user/get?access_token={0}&next_openid={1}",
                HttpUtility.UrlEncode(token),
                HttpUtility.UrlEncode(nextOpenID)), (error, value) => {
                    if(error == null) {
                        if(value.OpenIDList != null) {
                            users.OpenIDList.AddRange(value.OpenIDList);
                        }
                        if(string.IsNullOrEmpty(value.NextOpenID)) {
                            result.MarkCompleted(null, users);
                        } else {
                            this.GetUsersList(result, users, value.NextOpenID);
                        }
                    } else {
                        result.MarkCompleted(error);
                    }
                });
        }

        #region IOfficialAccountService Members

        public bool IsSupportCallWeChatService {
            get {
                return !string.IsNullOrWhiteSpace(this.m_appID) && !string.IsNullOrWhiteSpace(this.m_appSecret);
            }
        }

        public bool Validate(string signature, string timestamp, string nonce) {
            string data = new string[] { this.m_token, timestamp, nonce }.OrderBy((item) => item, StringComparer.Ordinal).StringJoin(string.Empty);
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            buffer = SHA1.Create().ComputeHash(buffer);
            data = BitConverter.ToString(buffer).Replace("-", string.Empty);
            return string.Equals(signature, data, StringComparison.OrdinalIgnoreCase);
        }

        public string GetBaseOAuthUri(string redirectUri, string state) {
            return WeChatHelper.GetOAuthUri(this.m_appID, redirectUri, WeChatHelper.OAUTH_SCOPE_BASE, state);
        }

        public string GetUserInfoOAuthUri(string redirectUri, string state) {
            return WeChatHelper.GetOAuthUri(this.m_appID, redirectUri, WeChatHelper.OAUTH_SCOPE_USERINFO, state);
        }

        public IAsyncResult BeginProcessMessage(byte[] data, AsyncCallback callback, object userState) {
            IWeChatMessage message = null;
            IWeChatMessageHandler handler = null;
            string postContent = Encoding.UTF8.GetString(data);
            AsyncResult<IWeChatMessageResult> result = new AsyncResult<IWeChatMessageResult>(callback, userState);

            try {
                message = this.m_messageFactory.GetMessage(postContent, this);
            } catch(Exception ex) {
                result.MarkCompleted(ex, true, null);
                return result;
            }

            if(message != null) {
                try {
                    handler = this.m_handlerFactory.GetHandler(message, this);
                } catch(Exception ex) {
                    result.MarkCompleted(ex, true, null);
                    return result;
                }

                if(handler != null && handler.Priority != WeChatMessageHandlerPriority.Disabled) {
                    handler.BeginProcessMessage(message, this, (ar) => {
                        Exception error = null;
                        IWeChatMessageResult messageResult = null;
                        try {
                            messageResult = handler.EndProcessMessage(ar);
                        } catch(Exception ex) {
                            error = ex;
                        }
                        result.MarkCompleted(error, ar.CompletedSynchronously, messageResult ?? WeChatEmptyMessageResult.Instance);
                    }, null);
                } else {
                    result.MarkCompleted(null, true, WeChatEmptyMessageResult.Instance);
                }
            } else {
                result.MarkCompleted(null, true, WeChatEmptyMessageResult.Instance);
            }

            return result;
        }

        public IWeChatMessageResult EndProcessMessage(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWeChatMessageResult>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWeChatMessageResult>) asyncResult).End();
        }

        public IAsyncResult BeginClearQuota(AsyncCallback callback, object userState) {
            AsyncResult result = new AsyncResult(callback, userState);
            this.PerformAsyncPostAction<WeChatReturnValue>(result, (token) => string.Format(
                "https://api.weixin.qq.com/cgi-bin/clear_quota?access_token={0}",
                HttpUtility.UrlEncode(token)), new {
                    appid = this.m_appID,
                }, (error, value) => {
                    result.MarkCompleted(error);
                });
            return result;
        }

        public void EndClearQuota(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult)) {
                throw new InvalidOperationException();
            }

            ((AsyncResult) asyncResult).End();
        }

        public IAsyncResult BeginGetServerInfo(AsyncCallback callback, object userState) {
            AsyncResult<IWeChatServerInfo> result = new AsyncResult<IWeChatServerInfo>(callback, userState);
            this.PerformAsyncGetAction<WeChatServerInfo>(result, (token) => string.Format(
                "https://api.weixin.qq.com/cgi-bin/getcallbackip?access_token={0}",
                HttpUtility.UrlEncode(token)), (error, value) => {
                    result.MarkCompleted(error, value);
                });
            return result;
        }

        public IWeChatServerInfo EndGetServerInfo(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWeChatServerInfo>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWeChatServerInfo>) asyncResult).End();
        }

        public IAsyncResult BeginCreateMenuItems(IEnumerable<IWeChatMenuItem> menuItems, AsyncCallback callback, object userState) {
            if(menuItems == null) {
                throw new ArgumentNullException("menuItems");
            }

            AsyncResult result = new AsyncResult(callback, userState);
            this.PerformAsyncPostAction<WeChatReturnValue>(result, (token) => string.Format(
                "https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}",
                HttpUtility.UrlEncode(token)),
                this.m_jsonSerializer.Serialize(this.GetMenuItems(menuItems)).Replace(@"\u0026", "&"),
                (error, value) => {
                    result.MarkCompleted(error);
                });
            return result;
        }

        public void EndGetCreateMenuItems(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult)) {
                throw new InvalidOperationException();
            }

            ((AsyncResult) asyncResult).End();
        }

        public IAsyncResult BeginGetOAuthAccessToken(string code, AsyncCallback callback, object userState) {
            AsyncResult<IWeChatOAuthAccessToken> result = new AsyncResult<IWeChatOAuthAccessToken>(callback, userState);

            if(this.IsSupportCallWeChatService) {
                this.SendGetRequest<WeChatOAuthAccessToken>(string.Format(
                    "https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code",
                    HttpUtility.UrlEncode(this.m_appID),
                    HttpUtility.UrlEncode(this.m_appSecret),
                    HttpUtility.UrlEncode(code)), (error, token) => {
                    result.MarkCompleted(error, token);
                });
            } else {
                result.MarkCompleted(new NotSupportedException("AppID or AppSecret is undefined."), true);
            }

            return result;
        }

        public IWeChatOAuthAccessToken EndGetOAuthAccessToken(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWeChatOAuthAccessToken>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWeChatOAuthAccessToken>) asyncResult).End();
        }

        public IAsyncResult BeginGetOAuthUserInfo(string accessToken, string openID, WeChatLanguage language, AsyncCallback callback, object userState) {
            if(string.IsNullOrWhiteSpace(accessToken)) {
                throw new ArgumentException("OAuth access token is null or empty.", "accessToken");
            }
            if(string.IsNullOrWhiteSpace(openID)) {
                throw new ArgumentException("openID is null or empty.", "openID");
            }

            AsyncResult<IWeChatOAuthUserInfo> result = new AsyncResult<IWeChatOAuthUserInfo>(callback, userState);
            this.SendGetRequest<WeChatOAuthUserInfo>(string.Format(
                "https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang={2}",
                HttpUtility.UrlEncode(accessToken),
                HttpUtility.UrlEncode(openID),
                HttpUtility.UrlEncode(EnumUtility.GetDescription(language))), (error, value) => {
                    result.MarkCompleted(error, value);
                });
            return result;
        }

        public IWeChatOAuthUserInfo EndGetOAuthUserInfo(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWeChatOAuthUserInfo>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWeChatOAuthUserInfo>) asyncResult).End();
        }

        public IAsyncResult BeginSendTemplateMessage(string message, AsyncCallback callback, object userState) {
            if(string.IsNullOrWhiteSpace(message)) {
                throw new ArgumentException("message is null or empty.", "message");
            }

            AsyncResult<IWeChatTemplateMessageReturnValue> result = new AsyncResult<IWeChatTemplateMessageReturnValue>(callback, userState);
            this.PerformAsyncPostAction<WeChatTemplateMessageReturnValue>(result, (token) => string.Format(
                "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}",
                HttpUtility.UrlEncode(token)), message, (error, value) => {
                    result.MarkCompleted(error, value);
                });
            return result;
        }

        public IWeChatTemplateMessageReturnValue EndSendTemplateMessage(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWeChatTemplateMessageReturnValue>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWeChatTemplateMessageReturnValue>) asyncResult).End();
        }

        public IAsyncResult BeginSendCustomerServiceMessage(string message, AsyncCallback callback, object userState) {
            if(string.IsNullOrWhiteSpace(message)) {
                throw new ArgumentException("message is null or empty.", "message");
            }

            AsyncResult result = new AsyncResult(callback, userState);
            this.PerformAsyncPostAction<WeChatReturnValue>(result, (token) => string.Format(
                "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}",
                HttpUtility.UrlEncode(token)), message, (error, value) => {
                    result.MarkCompleted(error);
                });
            return result;
        }

        public IAsyncResult BeginSendCustomerServiceMessage(WeChatCustomerServiceMessage message, AsyncCallback callback, object userState) {
            if(message == null) {
                throw new ArgumentNullException("message");
            }

            return this.BeginSendCustomerServiceMessage(message.ResultContent, callback, userState);
        }

        public void EndSendCustomerServiceMessage(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult)) {
                throw new InvalidOperationException();
            }

            ((AsyncResult) asyncResult).End();
        }

        public IAsyncResult BeginGetUserInfo(string openID, WeChatLanguage language, AsyncCallback callback, object userState) {
            AsyncResult<IWeChatUserInfo> result = new AsyncResult<IWeChatUserInfo>(callback, userState);
            this.PerformAsyncGetAction<WeChatUserInfo>(result, (token) => string.Format(
                "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang={2}",
                HttpUtility.UrlEncode(token),
                HttpUtility.UrlEncode(openID),
                HttpUtility.UrlEncode(EnumUtility.GetDescription(language))), (error, value) => {
                    result.MarkCompleted(error, value);
                });
            return result;
        }

        public IWeChatUserInfo EndGetUserInfo(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWeChatUserInfo>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWeChatUserInfo>) asyncResult).End();
        }

        public IAsyncResult BeginGetAllUsers(AsyncCallback callback, object userState) {
            AsyncResult<IWeChatUsersList> result = new AsyncResult<IWeChatUsersList>(callback, userState);
            this.GetUsersList(result, new WeChatUsersList(), null);
            return result;
        }

        public IWeChatUsersList EndGetAllUsers(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWeChatUsersList>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWeChatUsersList>) asyncResult).End();
        }

        #endregion

        private class MenuItems {
            public MenuItems() {
                this.button = new List<MenuItem>();
            }

            public ICollection<MenuItem> button;
        }

        private class MenuItem {
            public MenuItem() {
                this.sub_button = new List<MenuItem>();
            }

            public string type;
            public string name;
            public string key;
            public string url;

            public ICollection<MenuItem> sub_button;
        }
    }
}
