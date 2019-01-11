using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.IO {
    /// <summary>
    /// Represents a directory or file in a ZIP archive.
    /// </summary>
    public class ZipItemInfo {
        /// <summary>
        /// Initialize ZipItemInfo class.
        /// </summary>
        static ZipItemInfo() {
            Type t = typeof(Package).Assembly.GetType("MS.Internal.IO.Zip.ZipFileInfo");
            NameProperty = t.GetProperty("Name", BindingFlags.Instance | BindingFlags.NonPublic);
            FolderFlagProperty = t.GetProperty("FolderFlag", BindingFlags.Instance | BindingFlags.NonPublic);
            GetStreamMethod = t.GetMethod("GetStream", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Initialize a new instance of ZipItemInfo class.
        /// </summary>
        internal ZipItemInfo() {
        }

        protected static readonly PropertyInfo NameProperty;
        internal static readonly PropertyInfo FolderFlagProperty;
        protected static readonly MethodInfo GetStreamMethod;

        protected object m_internalZipFileInfo;
        /// <summary>
        /// Gets or sets the internal instance of MS.Internal.IO.Zip.ZipFileInfo class.
        /// </summary>
        internal object InternalZipFileInfo {
            get {
                return m_internalZipFileInfo;
            }
            set {
                m_internalZipFileInfo = value;
                this.FullName = (string) NameProperty.GetValue(value, null);
                if((bool) FolderFlagProperty.GetValue(value, null)) {
                    this.Name = Path.GetFileName(Path.GetDirectoryName(this.FullName));
                } else {
                    this.Name = Path.GetFileName(this.FullName);
                }
            }
        }

        /// <summary>
        /// Gets the name of the directory or file.
        /// </summary>
        public string Name {
            get;
            private set;
        }

        /// <summary>
        /// Gets the full path of the directory or file.
        /// </summary>
        public string FullName {
            get;
            private set;
        }

        /// <summary>
        /// Gets the owner ZIP archive of the directory or file.
        /// </summary>
        public ZipArchive OwnerArchive {
            get;
            internal set;
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.FullName.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is ZipItemInfo)) {
                return false;
            }
            return string.Equals(this.FullName, ((ZipItemInfo) obj).FullName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.FullName;
        }
    }
}
