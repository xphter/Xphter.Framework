using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Xphter.Framework.IO;
using System.IO;

namespace Xphter.Framework.Xml {
    /// <summary>
    /// Represents a XmlResolver for analyse XHTML document.
    /// </summary>
    public class XhtmlUrlResolver : XmlUrlResolver {
        /// <summary>
        /// Initialize a new instance of XhtmlUrlResolver class.
        /// </summary>
        /// <param name="resourceFolder">The folder used to store local XML resource.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="resourceFolder"/> is null or empty.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="resourceFolder"/> is not a valid local path. <paramref name="resourceFolder"/> not represents a directory path.</exception>
        public XhtmlUrlResolver(string resourceFolder) {
            if(string.IsNullOrWhiteSpace(resourceFolder)) {
                throw new ArgumentNullException("resourceFolder", "resourceFolder is null or empty.");
            }
            if(!PathUtility.IsValidLocalPath(resourceFolder)) {
                throw new ArgumentException("resourceFolder is not a valid local path.", "resourceFolder");
            }
            if(File.Exists(resourceFolder)) {
                throw new ArgumentException("resourceFolder not represents a directory path.", "resourceFolder");
            }

            //create resource folder
            string path = Path.GetFullPath(resourceFolder);
            if(!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            //copy resource files
            if(!File.Exists(Path.Combine(path, "xhtml1-strict.dtd"))) {
                File.WriteAllText(Path.Combine(path, "xhtml1-strict.dtd"), Properties.Resources.xhtml1_strict, Encoding.UTF8);
            }
            if(!File.Exists(Path.Combine(path, "xhtml1-transitional.dtd"))) {
                File.WriteAllText(Path.Combine(path, "xhtml1-transitional.dtd"), Properties.Resources.xhtml1_transitional, Encoding.UTF8);
            }
            if(!File.Exists(Path.Combine(path, "xhtml1-frameset.dtd"))) {
                File.WriteAllText(Path.Combine(path, "xhtml1-frameset.dtd"), Properties.Resources.xhtml1_frameset, Encoding.UTF8);
            }
            if(!File.Exists(Path.Combine(path, "xhtml-lat1.ent"))) {
                File.WriteAllBytes(Path.Combine(path, "xhtml-lat1.ent"), Properties.Resources.xhtml_lat1);
            }
            if(!File.Exists(Path.Combine(path, "xhtml-special.ent"))) {
                File.WriteAllBytes(Path.Combine(path, "xhtml-special.ent"), Properties.Resources.xhtml_special);
            }
            if(!File.Exists(Path.Combine(path, "xhtml-symbol.ent"))) {
                File.WriteAllBytes(Path.Combine(path, "xhtml-symbol.ent"), Properties.Resources.xhtml_symbol);
            }

            //create resource dictionary
            this.m_resource = new Dictionary<string, Uri>(6);
            this.m_resource["http://www.w3.org/tr/xhtml1/dtd/xhtml1-strict.dtd"] = new Uri(Path.Combine(path, "xhtml1-strict.dtd"));
            this.m_resource["http://www.w3.org/tr/xhtml1/dtd/xhtml1-transitional.dtd"] = new Uri(Path.Combine(path, "xhtml1-transitional.dtd"));
            this.m_resource["http://www.w3.org/tr/xhtml1/dtd/xhtml1-frameset.dtd"] = new Uri(Path.Combine(path, "xhtml1-frameset.dtd"));
            this.m_resource["http://www.w3.org/tr/xhtml1/dtd/xhtml-lat1.ent"] = new Uri(Path.Combine(path, "xhtml-lat1.ent"));
            this.m_resource["http://www.w3.org/tr/xhtml1/dtd/xhtml-special.ent"] = new Uri(Path.Combine(path, "xhtml-special.ent"));
            this.m_resource["http://www.w3.org/tr/xhtml1/dtd/xhtml-symbol.ent"] = new Uri(Path.Combine(path, "xhtml-symbol.ent"));

            this.ResourceFolder = path;
        }

        /// <summary>
        /// Gets the folder used to store local XML resource.
        /// </summary>
        public string ResourceFolder {
            get;
            private set;
        }

        /// <summary>
        /// Gets whether the local XML resource has been deleted.
        /// </summary>
        public bool HasDeleted {
            get;
            private set;
        }

        /// <summary>
        /// The dictionary for mapping a remote XML resource uri to a local uri.
        /// </summary>
        protected IDictionary<string, Uri> m_resource;

        /// <inheritdoc />
        public override Uri ResolveUri(Uri baseUri, string relativeUri) {
            if(relativeUri != null && this.m_resource.ContainsKey(relativeUri.ToLower())) {
                return this.m_resource[relativeUri.ToLower()];
            } else {
                return base.ResolveUri(baseUri, relativeUri);
            }
        }

        /// <summary>
        /// Deletes all local XML resource, then this object can not used to resolve XML resource.
        /// </summary>
        public void Delete() {
            foreach(Uri uri in this.m_resource.Values) {
                File.Delete(uri.LocalPath);
            }
            this.m_resource.Clear();
            this.HasDeleted = true;
        }
    }
}
