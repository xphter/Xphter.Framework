using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.Web.JavaScript;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class WeChatNewsCustomerServiceMessage : WeChatCustomerServiceMessage {
        public WeChatNewsCustomerServiceMessage() {
            this.m_news = new NewsContent();
        }

        public const int MAX_ARTICLES_COUNT = 8;

        public override string MsgType {
            get {
                return WeChatMessageTypes.NEWS;
            }
        }

        private NewsContent m_news;
        [JsonPropertyName("news")]
        public NewsContent News {
            get {
                return this.m_news;
            }
        }

        public override string ResultContent {
            get {
                if(this.m_news.Items.Count > MAX_ARTICLES_COUNT) {
                    IEnumerable<WeChatNewsCustomerServiceMessageItem> items = new List<WeChatNewsCustomerServiceMessageItem>(this.m_news.Items.Take(MAX_ARTICLES_COUNT));

                    this.m_news.Items.Clear();
                    foreach(WeChatNewsCustomerServiceMessageItem item in items) {
                        this.m_news.Items.Add(item);
                    }
                }
                return base.ResultContent;
            }
        }

        public class NewsContent {
            public NewsContent() {
                this.m_items = new List<WeChatNewsCustomerServiceMessageItem>();
            }

            private ICollection<WeChatNewsCustomerServiceMessageItem> m_items;
            [JsonPropertyName("articles")]
            public ICollection<WeChatNewsCustomerServiceMessageItem> Items {
                get {
                    return this.m_items;
                }
            }
        }
    }
}
