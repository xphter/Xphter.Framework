using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Xphter.Framework.IO;
using Xphter.Framework.Reflection;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Represents the information in a Sitemap file.
    /// Sitemap SPECIFICATION：http://www.sitemaps.org/protocol.html.
    /// </summary>
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SitemapInfo {
        public SitemapInfo()
            : this(null) {
        }

        public SitemapInfo(IEnumerable<SitemapItemInfo> items) {
            this.Items = items != null ? new List<SitemapItemInfo>(items) : new List<SitemapItemInfo>();
        }

        /// <summary>
        /// Maximum number of items in a sitemap file.
        /// </summary>
        public const int MAX_ITEMS_COUNT = 50000;

        /// <summary>
        /// Maximum size of a sitemap file.
        /// </summary>
        public const int MAX_FILE_SIZE = 10 * 1024 * 1024;

        /// <summary>
        /// Gets or sets all items in a sitemap file.
        /// </summary>
        [XmlElement("url")]
        public List<SitemapItemInfo> Items {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the item in a sitemap file.
    /// </summary>
    public class SitemapItemInfo {
        [XmlElement("loc")]
        public string Location {
            get;
            set;
        }

        [XmlIgnore]
        public SitemapItemType? ItemType {
            get;
            set;
        }

        [XmlIgnore]
        public DateTime? LastModifyTime {
            get;
            set;
        }

        [XmlElement("lastmod", IsNullable = false)]
        public string LastModifyTimeValue {
            get {
                return this.LastModifyTime.HasValue ? this.LastModifyTime.Value.ToString("yyyy-MM-dd") : null;
            }
            set {
                throw new InvalidOperationException();
            }
        }

        [XmlElement("changefreq")]
        public SitemapItemChangeFrequency? ChangeFrequency {
            get;
            set;
        }

        [XmlIgnore]
        public bool ChangeFrequencySpecified {
            get {
                return this.ChangeFrequency.HasValue;
            }
        }

        private float? m_priority;
        [XmlElement("priority")]
        public float? Priority {
            get {
                return this.m_priority;
            }
            set {
                this.m_priority = value.HasValue ? (float?) Math.Max(0, Math.Min(1, value.Value)) : null;
            }
        }

        [XmlIgnore]
        public bool PrioritySpecified {
            get {
                return this.Priority.HasValue;
            }
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Location;
        }
    }

    /// <summary>
    /// Represents the information in a Sitemap Index file.
    /// Sitemap SPECIFICATION：http://www.sitemaps.org/protocol.html.
    /// </summary>
    [XmlRoot("sitemapindex", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SitemapIndexInfo {
        public SitemapIndexInfo()
            : this(null) {
        }

        public SitemapIndexInfo(IEnumerable<SitemapIndexItemInfo> items) {
            this.Items = items != null ? new List<SitemapIndexItemInfo>(items) : new List<SitemapIndexItemInfo>();
        }

        /// <summary>
        /// Maximum number of items in a sitemap index file.
        /// </summary>
        public const int MAX_ITEMS_COUNT = 50000;

        /// <summary>
        /// Maximum size of a sitemap index file.
        /// </summary>
        public const int MAX_FILE_SIZE = 10 * 1024 * 1024;

        /// <summary>
        /// Gets or sets all items in a sitemap index file.
        /// </summary>
        [XmlElement("sitemap")]
        public List<SitemapIndexItemInfo> Items {
            get;
            set;
        }
    }

    /// <summary>
    /// Represents the item in a sitemap index file.
    /// </summary>
    public class SitemapIndexItemInfo {
        [XmlElement("loc")]
        public string Location {
            get;
            set;
        }

        [XmlIgnore]
        public DateTime? LastModifyTime {
            get;
            set;
        }

        [XmlElement("lastmod", IsNullable = false)]
        public string LastModifyTimeValue {
            get {
                return this.LastModifyTime.HasValue ? this.LastModifyTime.Value.ToString("yyyy-MM-dd") : null;
            }
            set {
                throw new InvalidOperationException();
            }
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Location;
        }
    }

    /// <summary>
    /// Represents the URL type of a sitemap item.
    /// </summary>
    [Flags]
    public enum SitemapItemType {
        [XmlEnum("")]
        Unknown = 0x00,

        [XmlEnum("pc")]
        PersonalComputer = 0x01,

        [XmlEnum("mobile")]
        Mobile = 0x02,

        [XmlEnum("pc,mobile")]
        AutoAdaptive = PersonalComputer | SitemapItemType.Mobile,

        [XmlEnum("htmladapt")]
        HtmlAdaptive = 0x04,
    }

    public enum SitemapItemChangeFrequency {
        [XmlEnum("never")]
        Never,

        [XmlEnum("yearly")]
        Yearly,

        [XmlEnum("monthly")]
        Monthly,

        [XmlEnum("weekly")]
        Weekly,

        [XmlEnum("daily")]
        Daily,

        [XmlEnum("hourly")]
        Hourly,

        [XmlEnum("always")]
        Always,
    }

    /// <summary>
    /// Provides functions to serialize sitemap(index) info to a file.
    /// </summary>
    public interface ISitemapSerializer {
        /// <summary>
        /// Gets the unique identifier of this serializer.
        /// </summary>
        string ID {
            get;
        }

        /// <summary>
        /// Gets the name of this serializer.
        /// </summary>
        string Name {
            get;
        }

        /// <summary>
        /// Serializes sitemap info to a file.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="path"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="info"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">number of items in <paramref name="info"/> or size of <paramref name="file"/> is out of range.</exception>
        void Serialize(SitemapInfo info, string path);

        /// <summary>
        /// Serializes sitemap index info to a file.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="path"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="info"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">number of items in <paramref name="info"/> or size of <paramref name="file"/> is out of range.</exception>
        void Serialize(SitemapIndexInfo info, string path);
    }

    /// <summary>
    /// Provides a default implementation of ISitemapSerializer interface.
    /// </summary>
    public class DefaultSitemapSerializer : ISitemapSerializer {
        protected virtual void CheckArguments(SitemapInfo info, string path) {
            if(info == null) {
                throw new ArgumentNullException("info");
            }
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("path is null or empty.", "path");
            }
            if(info.Items.Count > SitemapInfo.MAX_ITEMS_COUNT) {
                throw new ArgumentOutOfRangeException("info", string.Format("number of items in a sitemap file greater than {0}.", SitemapInfo.MAX_ITEMS_COUNT));
            }

            FileUtility.CreateFolder(path);
        }

        protected virtual void CheckArguments(SitemapIndexInfo info, string path) {
            if(info == null) {
                throw new ArgumentNullException("info");
            }
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("path is null or empty.", "path");
            }
            if(info.Items.Count > SitemapIndexInfo.MAX_ITEMS_COUNT) {
                throw new ArgumentOutOfRangeException("info", string.Format("number of items in a sitemap index file greater than {0}.", SitemapIndexInfo.MAX_ITEMS_COUNT));
            }

            FileUtility.CreateFolder(path);
        }

        protected virtual void CheckFileSize(SitemapInfo info, string path) {
            if(new FileInfo(path).Length > SitemapInfo.MAX_FILE_SIZE) {
                throw new ArgumentOutOfRangeException("info", string.Format("size of a sitemap file greater than {0}.", SitemapInfo.MAX_FILE_SIZE));
            }
        }

        protected virtual void CheckFileSize(SitemapIndexInfo info, string path) {
            if(new FileInfo(path).Length > SitemapIndexInfo.MAX_FILE_SIZE) {
                throw new ArgumentOutOfRangeException("info", string.Format("size of a sitemap index file greater than {0}.", SitemapIndexInfo.MAX_FILE_SIZE));
            }
        }

        #region ISitemapSerializer Members

        /// <inheritdoc />
        public virtual string ID {
            get {
                return "default";
            }
        }

        /// <inheritdoc />
        public virtual string Name {
            get {
                return "默认";
            }
        }

        /// <inheritdoc />
        public virtual void Serialize(SitemapInfo info, string path) {
            this.CheckArguments(info, path);

            XmlSerializer serializer = new XmlSerializer(typeof(SitemapInfo));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, "http://www.sitemaps.org/schemas/sitemap/0.9");
            using(TextWriter writer = new StreamWriter(path, false, Encoding.UTF8)) {
                serializer.Serialize(writer, info, namespaces);
            }

            this.CheckFileSize(info, path);
        }

        /// <inheritdoc />
        public virtual void Serialize(SitemapIndexInfo info, string path) {
            this.CheckArguments(info, path);

            XmlSerializer serializer = new XmlSerializer(typeof(SitemapIndexInfo));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, "http://www.sitemaps.org/schemas/sitemap/0.9");
            using(TextWriter writer = new StreamWriter(path, false, Encoding.UTF8)) {
                serializer.Serialize(writer, info, namespaces);
            }

            this.CheckFileSize(info, path);
        }

        #endregion
    }

    /// <summary>
    /// Implements ISitemapSerializer interface for baidu.com.
    /// </summary>
    public class BaiduSitemapSerializer : DefaultSitemapSerializer {
        #region ISitemapSerializer Members

        /// <inheritdoc />
        public override string ID {
            get {
                return "baidu.com";
            }
        }

        /// <inheritdoc />
        public override string Name {
            get {
                return "baidu.com";
            }
        }

        /// <inheritdoc />
        public override void Serialize(SitemapInfo info, string path) {
            this.CheckArguments(info, path);

            using(XmlWriter writer = XmlWriter.Create(path, new XmlWriterSettings {
                Encoding = Encoding.UTF8,
                Indent = true,
                CloseOutput = true,
            })) {
                writer.WriteStartDocument();
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                writer.WriteAttributeString("xmlns", "mobile", null, "http://www.baidu.com/schemas/sitemap-mobile/1/");

                foreach(SitemapItemInfo item in info.Items) {
                    writer.WriteStartElement("url");

                    if(item.Location != null) {
                        writer.WriteElementString("loc", item.Location);
                    }
                    if(item.ItemType.HasValue) {
                        string value = null;
                        switch(item.ItemType.Value) {
                            case SitemapItemType.Mobile:
                                value = "mobile";
                                break;
                            case SitemapItemType.AutoAdaptive:
                                value = "pc,mobile";
                                break;
                            case SitemapItemType.HtmlAdaptive:
                                value = "htmladapt";
                                break;
                        }
                        if(value != null) {
                            writer.WriteStartElement("mobile", "mobile", "http://www.baidu.com/schemas/sitemap-mobile/1/");
                            writer.WriteAttributeString("type", value);
                            writer.WriteEndElement();
                        }
                    }
                    if(item.LastModifyTime.HasValue) {
                        writer.WriteElementString("lastmod", item.LastModifyTime.Value.ToString("yyyy-MM-dd"));
                    }
                    if(item.ChangeFrequency.HasValue) {
                        string value = null;
                        switch(item.ChangeFrequency.Value) {
                            case SitemapItemChangeFrequency.Never:
                                value = "never";
                                break;
                            case SitemapItemChangeFrequency.Yearly:
                                value = "yearly";
                                break;
                            case SitemapItemChangeFrequency.Monthly:
                                value = "monthly";
                                break;
                            case SitemapItemChangeFrequency.Weekly:
                                value = "weekly";
                                break;
                            case SitemapItemChangeFrequency.Daily:
                                value = "daily";
                                break;
                            case SitemapItemChangeFrequency.Hourly:
                                value = "hourly";
                                break;
                        }
                        if(value != null) {
                            writer.WriteElementString("changefreq", value);
                        }
                    }
                    if(item.Priority.HasValue) {
                        writer.WriteElementString("priority", item.Priority.Value.ToString());
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            this.CheckFileSize(info, path);
        }

        #endregion
    }

    /// <summary>
    /// Provides functions to manager ISitemapSerializer objects.
    /// </summary>
    public static class SitemapSerializerManager {
        /// <summary>
        /// Initializes SitemapSerializerManager.
        /// </summary>
        /// <param name="assemblies"></param>
        public static void Initialize(params Assembly[] assemblies) {
            Initialize((IEnumerable<Assembly>) assemblies);
        }

        /// <summary>
        /// Initializes SitemapSerializerManager.
        /// </summary>
        /// <param name="assemblies"></param>
        public static void Initialize(IEnumerable<Assembly> assemblies) {
            g_serializer = TypeUtility.LoadInstances<ISitemapSerializer>(assemblies).ToArray();
        }

        private static IEnumerable<ISitemapSerializer> g_serializer;

        /// <summary>
        /// Gets all registered sitemap serializer.
        /// </summary>
        public static IEnumerable<ISitemapSerializer> SitemapSerializers {
            get {
                return g_serializer;
            }
        }

        /// <summary>
        /// Gets the sitemap serializer of the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ISitemapSerializer GetSitemapSerializer(string id) {
            if(string.IsNullOrEmpty(id)) {
                return null;
            }

            return g_serializer.Where((item) => string.Equals(item.ID, id)).FirstOrDefault();
        }
    }
}
