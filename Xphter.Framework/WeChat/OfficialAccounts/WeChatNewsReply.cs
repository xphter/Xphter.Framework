using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    [XmlRoot("xml")]
    public class WeChatNewsReply : WeChatMessageReply {
        public WeChatNewsReply() {
            this.Articles = new List<WeChatNewsReplyItem>();
        }

        public const int MAX_ARTICLES_COUNT = 10;

        public override string MsgType {
            get {
                return WeChatMessageTypes.NEWS;
            }
            set {
                throw new NotSupportedException();
            }
        }

        public int ArticleCount {
            get {
                return this.Articles != null ? this.Articles.Count : 0;
            }
            set {
                throw new NotSupportedException();
            }
        }

        [XmlArrayItem("item")]
        public List<WeChatNewsReplyItem> Articles {
            get;
            set;
        }

        public override string ResultContent {
            get {
                if(this.Articles == null) {
                    this.Articles = new List<WeChatNewsReplyItem>(0);
                } else if(this.Articles.Count > MAX_ARTICLES_COUNT) {
                    this.Articles = new List<WeChatNewsReplyItem>(this.Articles.Take(MAX_ARTICLES_COUNT));
                }
                return base.ResultContent;
            }
        }
    }
}
