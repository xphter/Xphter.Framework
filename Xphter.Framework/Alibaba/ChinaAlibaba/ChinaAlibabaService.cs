using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;
using Xphter.Framework.Net;

namespace Xphter.Framework.Alibaba.ChinaAlibaba {
    public class ChinaAlibabaService : IChinaAlibabaService {
        public ChinaAlibabaService(string appKey, string appSecret, string refreshToken, DateTime? refreshTokenExpiredTime, IChinaAlibabaApiFactory apiFacgory) {
            if(appKey == null) {
                throw new ArgumentNullException("appKey", "appKey is null.");
            }
            if(appSecret == null) {
                throw new ArgumentNullException("appSecret", "appSecret is null.");
            }
            if(apiFacgory == null) {
                throw new ArgumentNullException("basicRequestsProvider", "basic requests provider is null.");
            }

            this.m_appKey = appKey;
            this.m_appSecret = appSecret;
            this.m_apiFacgory = apiFacgory;
            this.m_timestampBase = new DateTime(1970, 1, 1);
            this.m_refreshToken = refreshToken;
            this.m_refreshTokenExpiredTime = refreshTokenExpiredTime;

            this.m_timeout = DEFAULT_TIMEOUT;
            this.m_defaultEncoding = Encoding.UTF8;
            this.m_jsonSerializer = new JavaScriptSerializer {
                MaxJsonLength = 100 * 1024 * 1024,
            };
            this.m_bufferManager = BufferManager.CreateBufferManager(BUFFER_POOL_SIZE, BUFFER_POOL_ITEM_SIZE);

            this.m_checkAccessTokenEvent = new AutoResetEvent(true);
            this.m_checkRefreshTokenEvent = new AutoResetEvent(true);

            this.Initialize();
        }

        private const int BUFFER_POOL_SIZE = 10;
        private const int BUFFER_POOL_ITEM_SIZE = 1024;
        private const int MAX_POSTPONE_REFRESH_TOKEN_DAYS = 29;
        private const int MAX_POSTPONE_ACCESS_TOKEN_MINUTES = 20;
        private const int DEFAULT_TIMEOUT = 30000;

        private const string TIMESTAMP_ARGUMENT_NAME = "_aop_timestamp";
        private const string SIGNATURE_ARGUMENT_NAME = "_aop_signature";
        private const string ACCESS_TOKEN_ARGUMENT_NAME = "access_token";

        private string m_appKey;
        private string m_appSecret;
        private IChinaAlibabaApiFactory m_apiFacgory;

        private int m_timeError;
        private DateTime m_timestampBase;

        private string m_refreshToken;
        private DateTime? m_refreshTokenExpiredTime;
        private string m_accessToken;
        private DateTime? m_accessTokenExpiredTime;

        private string m_aliID;
        private string m_memberID;

        private Encoding m_defaultEncoding;
        private BufferManager m_bufferManager;
        private JavaScriptSerializer m_jsonSerializer;

        private AutoResetEvent m_checkAccessTokenEvent;
        private AutoResetEvent m_checkRefreshTokenEvent;

        private int m_timeout;
        /// <summary>
        /// Gets or sets the timeout of invoking alibaba service in milliseconds.
        /// </summary>
        public int Timeout {
            get {
                return this.m_timeout;
            }
            set {
                if(value <= 0) {
                    value = DEFAULT_TIMEOUT;
                }
                this.m_timeout = value;
            }
        }

        private void ChangeTokens(IChinaAlibabaAccessTokenReturnValue token) {
            this.m_accessToken = token.AccessToken;
            this.m_accessTokenExpiredTime = DateTime.Now.AddSeconds(token.AccessTokenLifeTime);
            this.m_aliID = token.AlibabaID;
            this.m_memberID = token.MemberID;
        }

        private void ChangeTokens(IChinaAlibabaRefreshTokenReturnValue token) {
            this.m_refreshToken = token.RefreshToken;
            this.m_refreshTokenExpiredTime = token.RefreshTokenExpiredTime;
            this.ChangeTokens((IChinaAlibabaAccessTokenReturnValue) token);

            this.OnAuthorizationChanged();
        }

        private HttpWebRequest CreateRequest(string url, string method, Encoding encoding) {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.KeepAlive = false;
            request.ContentType = "application/x-www-form-urlencoded;charset=" + (encoding ?? this.m_defaultEncoding).WebName;
            request.Method = method ?? WebRequestMethods.Http.Get;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Timeout = this.m_timeout;
            request.ReadWriteTimeout = this.m_timeout;
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            return request;
        }

        private AlibabaException GetAlibabaException(WebException webException) {
            AlibabaException alibabaException = null;
            byte[] buffer = this.m_bufferManager.TakeBuffer(BUFFER_POOL_ITEM_SIZE);

            try {
                using(HttpWebResponse response = (HttpWebResponse) webException.Response) {
                    if(response != null) {
                        string content = response.GetResponseString(buffer);

                        ChinaAlibabaErrorInfo info = this.m_jsonSerializer.Deserialize<ChinaAlibabaErrorInfo>(content);
                        alibabaException = new AlibabaException(info.error_description ?? info.error_message, (int) response.StatusCode, webException);
                    } else {
                        alibabaException = new AlibabaException(webException.Message, 500, webException);
                    }
                }
            } catch(Exception ex) {
                alibabaException = new AlibabaException(string.Format("Error occurred when parse error response: {0}", ex.Message), 500, ex);
            } finally {
                this.m_bufferManager.ReturnBuffer(buffer);
            }

            return alibabaException;
        }

        private void ProcessReturnValue<TReturn>(Exception error, string content, Action<Exception, TReturn> callback) {
            TReturn value = default(TReturn);
            if(error == null && content != null) {
                try {
                    value = this.m_jsonSerializer.Deserialize<TReturn>(content);
                } catch(Exception ex) {
                    error = new AlibabaException(string.Format("Error occurred when parse response content: {0}", ex.Message), 500, ex);
                }
            }
            callback(error, value);
        }

        private void SendHttpRequest(HttpWebRequest request, Action<Exception, string> callback) {
            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse((ar) => {
                string content = null;
                Exception error = null;
                byte[] buffer = this.m_bufferManager.TakeBuffer(BUFFER_POOL_ITEM_SIZE);

                try {
                    using(HttpWebResponse response = (HttpWebResponse) request.EndGetResponse(ar)) {
                        content = response.GetResponseString(buffer);
                    }
                } catch(WebException ex) {
                    error = this.GetAlibabaException(ex);
                } catch(Exception ex) {
                    error = new AlibabaException(string.Format("Error occurred when send HTTP request: {0}", ex.Message), 500, ex);
                } finally {
                    this.m_bufferManager.ReturnBuffer(buffer);
                }

                callback(error, content);
            }, null), request, this.m_timeout);
        }

        private void SendPostRequest(string url, IDictionary<string, string> args, Encoding encoding, Action<Exception, string> callback) {
            HttpWebRequest request = this.CreateRequest(url, WebRequestMethods.Http.Post, encoding);
            string queryString = AlibabaHelper.GetQueryString(args, encoding ?? this.m_defaultEncoding);
            byte[] data = queryString.Length > 0 ? (encoding ?? this.m_defaultEncoding).GetBytes(queryString) : null;

            if(data != null) {
                request.ContentLength = data.Length;
                NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetRequestStream((ar) => {
                    Exception error = null;

                    try {
                        using(Stream writer = request.EndGetRequestStream(ar)) {
                            writer.Write(data, 0, data.Length);
                        }
                    } catch(WebException ex) {
                        error = this.GetAlibabaException(ex);
                    } catch(Exception ex) {
                        error = new AlibabaException(string.Format("Error occurred when send POST data: {0}", ex.Message), 500, ex);
                    }

                    if(error != null) {
                        callback(error, null);
                    } else {
                        this.SendHttpRequest(request, callback);
                    }
                }, null), request, this.m_timeout);
            } else {
                this.SendHttpRequest(request, callback);
            }
        }

        private void SendApiRequest(IChinaAlibabaApi api, object args, Action<Exception, string> callback) {
            string apiUri = api.GetRequestUri(this.m_appKey);
            IDictionary<string, string> arguments = AlibabaHelper.GetArgumentsDictionary(args);
            if(api.NeedAuthroized) {
                arguments[ACCESS_TOKEN_ARGUMENT_NAME] = this.m_accessToken;
            }
            if(api.NeedTimestamp) {
                arguments[TIMESTAMP_ARGUMENT_NAME] = (DateTime.Now - this.m_timestampBase).TotalMilliseconds + this.m_timeError.ToString();
            }
            if(api.NeedSignature) {
                arguments[SIGNATURE_ARGUMENT_NAME] = AlibabaHelper.GetApiSignature(apiUri, arguments, this.m_appSecret);
            }

            this.SendPostRequest(apiUri, arguments, api.Encoding, callback);
        }

        private bool CheckAccessToken(Action<Exception> callback) {
            DateTime now = DateTime.Now;
            if(!string.IsNullOrWhiteSpace(this.m_accessToken) && this.m_accessTokenExpiredTime.HasValue && (this.m_accessTokenExpiredTime.Value - now).TotalMinutes > MAX_POSTPONE_ACCESS_TOKEN_MINUTES) {
                return true;
            }

            this.m_checkAccessTokenEvent.WaitOne();
            if(!string.IsNullOrWhiteSpace(this.m_accessToken) && this.m_accessTokenExpiredTime.HasValue && (this.m_accessTokenExpiredTime.Value - now).TotalMinutes > MAX_POSTPONE_ACCESS_TOKEN_MINUTES) {
                this.m_checkAccessTokenEvent.Set();
                return true;
            }

            this.SendApiRequest<ChinaAlibabaAccessTokenReturnValue>(this.m_apiFacgory.GetAccessToken, new {
                client_id = this.m_appKey,
                client_secret = this.m_appSecret,
                grant_type = "refresh_token",
                refresh_token = this.m_refreshToken,
            }, (error, response) => {
                try {
                    if(error == null && response != null) {
                        this.ChangeTokens(response);
                    }
                } catch {
                    // ignore all errors
                } finally {
                    this.m_checkAccessTokenEvent.Set();
                }

                callback(error);
            });

            return false;
        }

        private bool CheckRefreshToken(Action<Exception> callback) {
            DateTime now = DateTime.Now;
            if(this.m_refreshTokenExpiredTime.HasValue && (this.m_refreshTokenExpiredTime.Value - now).TotalDays > MAX_POSTPONE_REFRESH_TOKEN_DAYS) {
                return true;
            }

            this.m_checkRefreshTokenEvent.WaitOne();
            if(this.m_refreshTokenExpiredTime.HasValue && (this.m_refreshTokenExpiredTime.Value - now).TotalDays > MAX_POSTPONE_REFRESH_TOKEN_DAYS) {
                this.m_checkRefreshTokenEvent.Set();
                return true;
            }

            this.SendApiRequest<ChinaAlibabaRefreshTokenReturnValue>(this.m_apiFacgory.PostponeRefreshToken, new {
                client_id = this.m_appKey,
                client_secret = this.m_appSecret,
                refresh_token = this.m_refreshToken,
            }, (error, response) => {
                try {
                    if(error == null && response != null) {
                        this.ChangeTokens(response);
                    }
                } catch {
                    // ignore all errors
                } finally {
                    this.m_checkRefreshTokenEvent.Set();
                }

                callback(error);
            });

            return false;
        }

        private bool CheckTokens(Action<Exception> callback) {
            return this.CheckAccessToken(callback) && this.CheckRefreshToken(callback);
        }

        private void PerformApiRequest(AsyncResult result, IChinaAlibabaApi api, object args, Action<Exception, string> callback) {
            if(api.NeedAuthroized && (string.IsNullOrWhiteSpace(this.m_refreshToken) || !this.m_refreshTokenExpiredTime.HasValue)) {
                result.MarkCompleted(new AlibabaException("Unauthorized", 401), true);
                return;
            }

            if(api.NeedAuthroized) {
                if(this.CheckTokens((error) => {
                    if(error != null) {
                        result.MarkCompleted(error, false);
                    } else {
                        this.SendApiRequest(api, args, callback);
                    }
                })) {
                    this.SendApiRequest(api, args, callback);
                }
            } else {
                this.SendApiRequest(api, args, callback);
            }
        }

        private void SendPostRequest<TReturn>(string url, IDictionary<string, string> args, Encoding encoding, Action<Exception, TReturn> callback) {
            this.SendPostRequest(url, args, encoding, (error, content) => {
                this.ProcessReturnValue<TReturn>(error, content, callback);
            });
        }

        private void SendApiRequest<TReturn>(IChinaAlibabaApi api, object args, Action<Exception, TReturn> callback) {
            this.SendApiRequest(api, args, (error, content) => {
                this.ProcessReturnValue<TReturn>(error, content, callback);
            });
        }

        private void PerformApiRequest<TReturn>(AsyncResult result, IChinaAlibabaApi api, IChinaAlibabaApiArguments args, Action<Exception, TReturn> callback) {
            this.PerformApiRequest(result, api, args, (error, content) => {
                this.ProcessReturnValue<TReturn>(error, content, callback);
            });
        }

        protected virtual void Initialize() {
            DateTime now = DateTime.Now;
            this.BeginGetCurrentTime((ar) => {
                try {
                    DateTime time = this.EndGetCurrentTime(ar);
                    this.m_timeError = (int) (time - now).TotalMilliseconds;
                } catch {
                }
            }, null);
        }

        protected virtual void OnAuthorizationChanged() {
            if(this.AuthorizationChanged != null) {
                this.AuthorizationChanged(this, new ChinaAlibabaAuthorizationChangedEventArgs(this.m_refreshToken, this.m_refreshTokenExpiredTime.Value));
            }
        }

        #region IChinaAlibabaService Members

        public string AuthorizedAliID {
            get {
                return this.m_aliID;
            }
        }

        public string AuthorizedMemberID {
            get {
                return this.m_memberID;
            }
        }

        public event EventHandler<ChinaAlibabaAuthorizationChangedEventArgs> AuthorizationChanged;

        public IAsyncResult BeginGetIsAuthorized(AsyncCallback callback, object userState) {
            AsyncResult<bool> result = new AsyncResult<bool>(callback, userState);

            if(string.IsNullOrWhiteSpace(this.m_refreshToken) || !this.m_refreshTokenExpiredTime.HasValue) {
                result.MarkCompleted(null, true, false);
                return result;
            }

            if(this.CheckTokens((error) => {
                result.MarkCompleted(null, false, error == null);
            })) {
                result.MarkCompleted(null, true, true);
            }

            return result;
        }

        public bool EndGetIsAuthorized(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<bool>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<bool>) asyncResult).End();
        }

        public IAsyncResult BeginAuthorize(string code, string redirectUri, AsyncCallback callback, object userState) {
            AsyncResult result = new AsyncResult(callback, userState);

            this.SendApiRequest<ChinaAlibabaRefreshTokenReturnValue>(this.m_apiFacgory.GetRefreshToken, new {
                client_id = this.m_appKey,
                client_secret = this.m_appSecret,
                grant_type = "authorization_code",
                need_refresh_token = true,
                redirect_uri = redirectUri,
                code = code,
            }, (error, response) => {
                if(error == null) {
                    this.ChangeTokens(response);
                }

                result.MarkCompleted(error, false);
            });

            return result;
        }

        public void EndAuthorize(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult)) {
                throw new InvalidOperationException();
            }

            ((AsyncResult) asyncResult).End();
        }

        public IAsyncResult BeginGetCurrentTime(AsyncCallback callback, object userState) {
            AsyncResult<DateTime> result = new AsyncResult<DateTime>(callback, userState);

            this.SendApiRequest(this.m_apiFacgory.GetSystemTime, null, (error, content) => {
                DateTime time = DateTime.MinValue;
                if(error == null) {
                    time = AlibabaHelper.AlibabaTimeToLocalTime(content.Substring(1, content.Length - 2));
                }

                result.MarkCompleted(error, false, time);
            });

            return result;
        }

        public DateTime EndGetCurrentTime(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<DateTime>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<DateTime>) asyncResult).End();
        }

        public IAsyncResult BeginInvokeApi(IChinaAlibabaApi api, object arguments, AsyncCallback callback, object userState) {
            AsyncResult<string> result = new AsyncResult<string>(callback, userState);

            this.PerformApiRequest(result, api, arguments, (error, content) => {
                result.MarkCompleted(error, content);
            });

            return result;
        }

        public string EndInvokeApi(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<string>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<string>) asyncResult).End();
        }

        public IAsyncResult BeginInvokeApi<TReturn>(IChinaAlibabaApi api, IChinaAlibabaApiArguments arguments, AsyncCallback callback, object userState) {
            AsyncResult<TReturn> result = new AsyncResult<TReturn>(callback, userState);

            this.PerformApiRequest<TReturn>(result, api, arguments, (error, value) => {
                result.MarkCompleted(error, value);
            });

            return result;
        }

        public TReturn EndInvokeApi<TReturn>(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<TReturn>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<TReturn>) asyncResult).End();
        }

        public IAsyncResult BeginGetMemberInfo(string memberID, AsyncCallback callback, object userState) {
            return this.BeginInvokeApi<Dictionary<string, ChinaAlibabaMap<ChinaAlibabaMemberInfo>>>(this.m_apiFacgory.GetMemberInfo, new ChinaAlibabaGetMemberInfoArguments {
                MemberID = memberID,
            }, callback, userState);
        }

        public IChinaAlibabaMemberInfo EndGetMemberInfo(IAsyncResult asyncResult) {
            IDictionary<string, ChinaAlibabaMap<ChinaAlibabaMemberInfo>> result = this.EndInvokeApi<Dictionary<string, ChinaAlibabaMap<ChinaAlibabaMemberInfo>>>(asyncResult);
            return result.Count > 0 ? result["result"].toReturn.FirstOrDefault() : null;
        }

        public IAsyncResult BeginGetOrderList(ChinaAlibabaGetOrderListArguments arguments, AsyncCallback callback, object userState) {
            return this.BeginInvokeApi<Dictionary<string, ChinaAlibabaOrderList>>(this.m_apiFacgory.GetOrderList, arguments, callback, userState);
        }

        public IChinaAlibabaOrderList EndGetOrderList(IAsyncResult asyncResult) {
            IDictionary<string, ChinaAlibabaOrderList> result = this.EndInvokeApi<Dictionary<string, ChinaAlibabaOrderList>>(asyncResult);
            return result.Count > 0 ? result["orderListResult"] : new ChinaAlibabaOrderList();
        }

        public IAsyncResult BeginGetOrderInfo(ChinaAlibabaGetOrderInfoArguments arguments, AsyncCallback callback, object userState) {
            return this.BeginInvokeApi<Dictionary<string, ChinaAlibabaOrderInfo>>(this.m_apiFacgory.GetOrderInfo, arguments, callback, userState);
        }

        public IChinaAlibabaOrderInfo EndGetOrderInfo(IAsyncResult asyncResult) {
            IDictionary<string, ChinaAlibabaOrderInfo> result = this.EndInvokeApi<Dictionary<string, ChinaAlibabaOrderInfo>>(asyncResult);
            return result.Count > 0 ? result["orderModel"] : null;
        }

        #endregion

        #region IDisposable Members

        ~ChinaAlibabaService() {
            this.Dispose(false);
        }

        private bool m_disposed;

        protected virtual void Dispose(bool disposing) {
            if(this.m_disposed) {
                return;
            }
            this.m_disposed = true;

            this.m_checkAccessTokenEvent.Dispose();
            this.m_checkRefreshTokenEvent.Dispose();

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        public void Close() {
            this.Dispose(true);
        }

        public void Dispose() {
            this.Dispose(true);
        }

        #endregion
    }
}
