using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Represents the information in a RSS file.
    /// RSS 2.0 SPECIFICATION：http://feedvalidator.org/docs/rss2.html.
    /// </summary>
    [XmlRoot("rss")]
    public class RssInfo {
        public RssInfo() {
            this.Version = "2.0";
        }

        [XmlAttribute("version")]
        public string Version {
            get;
            set;
        }

        [XmlElement("channel")]
        public RssChannelInfo Channel {
            get;
            set;
        }
    }

    public class RssChannelInfo {
        [XmlElement("title")]
        public string Title {
            get;
            set;
        }

        [XmlElement("link")]
        public string Link {
            get;
            set;
        }

        [XmlElement("description")]
        public string Description {
            get;
            set;
        }

        [XmlElement("language")]
        public string Language {
            get;
            set;
        }

        [XmlElement("copyright")]
        public string Copyright {
            get;
            set;
        }

        [XmlElement("category")]
        public List<RssCategoryInfo> Categories {
            get;
            set;
        }

        [XmlElement("generator")]
        public string Generator {
            get;
            set;
        }

        [XmlElement("docs")]
        public string Docs {
            get;
            set;
        }

         [XmlElement("ttl")]
        public int? Ttl {
            get;
            set;
        }

        [XmlElement("image")]
        public RssImageInfo Image {
            get;
            set;
        }

        [XmlElement("lastBuildDate")]
        public DateTime? LastBuildDate {
            get;
            set;
        }

        [XmlElement("pubDate")]
        public DateTime? PubDate {
            get;
            set;
        }

        [XmlElement("managingEditor")]
        public string ManagingEditor {
            get;
            set;
        }

        [XmlElement("webMaster")]
        public string WebMaster {
            get;
            set;
        }

        [XmlElement("item")]
        public List<RssItemInfo> Items {
            get;
            set;
        }
    }

    public class RssCategoryInfo {
        [XmlAttribute("domain")]
        public string Domain {
            get;
            set;
        }

        [XmlText]
        public string Value {
            get;
            set;
        }
    }

    public class RssImageInfo {
        [XmlElement("url")]
        public string Url {
            get;
            set;
        }

        [XmlElement("title")]
        public string Title {
            get;
            set;
        }

        [XmlElement("link")]
        public string Link {
            get;
            set;
        }

        [XmlElement("description")]
        public string Description {
            get;
            set;
        }

        [XmlElement("width")]
        public int? Width {
            get;
            set;
        }

        [XmlElement("height")]
        public int? Height {
            get;
            set;
        }
    }

    public class RssItemInfo {
        [XmlElement("title")]
        public string Title {
            get;
            set;
        }

        [XmlElement("link")]
        public string Link {
            get;
            set;
        }

        [XmlElement("description")]
        public string Description {
            get;
            set;
        }

        [XmlElement("pubDate")]
        public DateTime? PubDate {
            get;
            set;
        }

        [XmlElement("guid")]
        public string Guid {
            get;
            set;
        }

        [XmlElement("author")]
        public string Author {
            get;
            set;
        }

        [XmlElement("source")]
        public string Source {
            get;
            set;
        }

        [XmlElement("category")]
        public List<RssCategoryInfo> Categories {
            get;
            set;
        }

        [XmlElement("comments")]
        public string Comments {
            get;
            set;
        }

        [XmlElement("enclosure")]
        public RssEnclosureInfo Enclosure {
            get;
            set;
        }
    }

    public class RssEnclosureInfo {
        [XmlAttribute("url")]
        public string Url {
            get;
            set;
        }

        [XmlAttribute("type")]
        public string Type {
            get;
            set;
        }

        [XmlAttribute("length")]
        public int Length {
            get;
            set;
        }
    }
}
