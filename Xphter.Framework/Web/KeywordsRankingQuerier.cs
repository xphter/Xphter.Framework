using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Xphter.Framework.Net;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Provide function to query keywords ranking.
    /// </summary>
    public class KeywordsRankingQuerier {
        /// <summary>
        /// Initialize a new instance of KeywordsRankingQuerier class.
        /// </summary>
        /// <param name="ranking">A IKeywordsRanking object.</param>
        /// <exception cref="System.ArgumentException"><paramref name="ranking"/> is null.</exception>
        public KeywordsRankingQuerier(IKeywordsRanking ranking) {
            if(ranking == null) {
                throw new ArgumentException("Keywords ranking object is null.", "ranking");
            }

            this.KeywordsRanking = ranking;
        }

        /// <summary>
        /// Keywords ranking provider.
        /// </summary>
        public IKeywordsRanking KeywordsRanking {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets proxy info.
        /// </summary>
        public IWebProxy Proxy {
            get;
            set;
        }

        private int m_timeout = System.Threading.Timeout.Infinite;
        /// <summary>
        /// Gets or set tiemout of quering operation.
        /// </summary>
        public int Timeout {
            get {
                return this.m_timeout;
            }
            set {
                this.m_timeout = value > 0 ? value : System.Threading.Timeout.Infinite;
            }
        }

        /// <summary>
        /// Query search result of the specified keyword.
        /// </summary>
        /// <param name="keyword">Keyword.</param>
        /// <returns>Search result of <paramref name="keyword"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="keyword"/> is null or empty.</exception>
        public ReadOnlyCollection<Uri> Query(string keyword) {
            if(string.IsNullOrWhiteSpace(keyword)) {
                throw new ArgumentException("Keyword is null or empty.", "keyword");
            }

            //create query request
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(this.KeywordsRanking.GetQueryUri(keyword.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)));
            request.Proxy = this.Proxy;
            request.Timeout = this.Timeout;
            request.ReadWriteTimeout = this.Timeout;
            this.KeywordsRanking.PrepareRequest(request);

            //get query response
            string response = HttpHelper.GetRequest(request).GetResponseString(1024 * 1024);

            //analyze query response
            return this.KeywordsRanking.Analyze(response);
        }

        /// <summary>
        /// Query the keyword ranking of the specified domain.
        /// </summary>
        /// <param name="keyword">Keyword.</param>
        /// <param name="domain">A DNS name, such as 163.com.</param>
        /// <returns>Return ranking of <paramref name="keyword"/> with <paramref name="domain"/> or return -1 if it can not fetch this ranking.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="keyword"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="domain"/> is null or empty.</exception>
        public int Query(string keyword, string domain) {
            if(string.IsNullOrWhiteSpace(domain)) {
                throw new ArgumentException("Domain name is null or empty.", "domain");
            }

            //query search result
            ReadOnlyCollection<Uri> result = this.Query(keyword);

            //search domain name
            int ranking = -1;
            for(int i = 0; i < result.Count; i++) {
                if(result[i].Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase)) {
                    ranking = i + 1;
                    break;
                }
            }

            return ranking;
        }
    }

    /// <summary>
    /// Provides keywords ranking function for KeywordsRankingQuerier class.
    /// </summary>
    public interface IKeywordsRanking {
        /// <summary>
        /// Gets the max ranking which can be queried.
        /// </summary>
        int MaxRanking {
            get;
        }

        /// <summary>
        /// Get a HTTP URI for quering the specified keywords.
        /// </summary>
        /// <param name="keywords">Keywords list.</param>
        /// <returns>HTTP URI for quering <paramref name="keywords"/>.</returns>
        string GetQueryUri(params string[] keywords);

        /// <summary>
        /// Prepares the request used to fetch query response.
        /// </summary>
        /// <param name="request">A HttpWebRequest.</param>
        void PrepareRequest(HttpWebRequest request);

        /// <summary>
        /// Analyze the query result.
        /// </summary>
        /// <param name="response">Query response.</param>
        /// <returns>Query result ranking.</returns>
        ReadOnlyCollection<Uri> Analyze(string response);
    }

    /// <summary>
    /// Baidu keywords ranking.
    /// </summary>
    public class BaiduKeywordsRanking : IKeywordsRanking {
        /// <summary>
        /// URI of Baidu.
        /// </summary>
        public const string BAIDU_URI = "http://www.baidu.com";

        /// <summary>
        /// Cookies that will be sent to server.
        /// </summary>
        private CookieContainer m_cookies;

        /// <summary>
        /// A Regex for search href attribute.
        /// </summary>
        private Regex m_hrefRegex = new Regex("href\\=\\\"([^\\\"]+)\\\"", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// A Regex for search table such as "<table....id="4"".
        /// </summary>
        private Regex m_tableRegex = new Regex("\\<table(?:\\s+[\\w\\-]+\\=\\\"[\\w\\-]+\\\")*?\\s+id\\=\\\"\\d{1,3}\\\"", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        #region IKeywordsRanking Members

        /// <inheritdoc />
        public int MaxRanking {
            get {
                return 100;
            }
        }

        /// <inheritdoc />
        public string GetQueryUri(params string[] keywords) {
            //encode keywords
            Encoding encoding = Encoding.UTF8;
            StringBuilder queryKey = new StringBuilder();
            queryKey.Append(HttpUtility.UrlEncode(keywords[0], encoding));
            for(int i = 1; i < keywords.Length; i++) {
                queryKey.AppendFormat("+{0}", HttpUtility.UrlEncode(keywords[i], encoding));
            }

            return string.Format("http://www.baidu.com/s?q1={0}&rn={1}", queryKey, this.MaxRanking);
        }

        /// <inheritdoc />
        public void PrepareRequest(HttpWebRequest request) {
            //get cookies
            if(this.m_cookies == null) {
                HttpWebRequest petitio = (HttpWebRequest) WebRequest.Create(BAIDU_URI);
                petitio.CookieContainer = this.m_cookies = new CookieContainer();
                petitio.MediaType = "text/html";
                petitio.AllowAutoRedirect = false;
                petitio.KeepAlive = true;
                petitio.Method = WebRequestMethods.Http.Get;
                using(HttpWebResponse response = (HttpWebResponse) petitio.GetResponse()) {
                }
            }

            //set headers
            request.CookieContainer = this.m_cookies;
            request.Referer = BAIDU_URI;
        }

        /// <inheritdoc />
        public ReadOnlyCollection<Uri> Analyze(string response) {
            string uri = null;
            Match match = null;
            List<Uri> result = new List<Uri>();

            MatchCollection matches = this.m_tableRegex.Matches(response);
            for(int i = 0; i < matches.Count; i++) {
                if(!(match = matches[i]).Success) {
                    continue;
                }

                match = this.m_hrefRegex.Match(response, match.Index, (i + 1 < matches.Count ? matches[i + 1].Index : response.Length - 1) - match.Index);
                if(match != null && match.Success && Uri.IsWellFormedUriString(uri = match.Groups[1].Value, UriKind.Absolute)) {
                    result.Add(new Uri(uri));
                }
            }

            return result.AsReadOnly();
        }

        #endregion
    }

    /// <summary>
    /// Google keywords ranking.
    /// </summary>
    public class GoogleKeywordsRanking : IKeywordsRanking {
        #region IKeywordsRanking Members

        /// <inheritdoc />
        public int MaxRanking {
            get {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public string GetQueryUri(params string[] keywords) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void PrepareRequest(HttpWebRequest request) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ReadOnlyCollection<Uri> Analyze(string response) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
