using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Net {
    /// <summary>
    /// Represents the detail of a directory in the FTP server.
    /// </summary>
    public class FtpDirectoryInfo : FtpFileInfo {
        protected FtpDirectoryInfo() {
        }

        protected FtpDirectoryInfo(FtpFileInfo file) {
            this.IsDirectory = true;
            this.Root = file.Root;
            this.Parent = file.Parent;
            this.Name = file.Name;
            this.RelativeUri = file.RelativeUri;
            this.AbsoluteUri = file.AbsoluteUri;
            this.Length = 0;
            this.LastModifyTime = file.LastModifyTime;
        }

        private ICollection<FtpFileInfo> m_files = new List<FtpFileInfo>();
        private ICollection<FtpDirectoryInfo> m_directories = new List<FtpDirectoryInfo>();

        /// <summary>
        /// Gets all files in this directory.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FtpFileInfo> GetFiles() {
            return this.m_files;
        }

        /// <summary>
        /// Gets all subdirectories in this directory.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FtpDirectoryInfo> GetDirectories() {
            return this.m_directories;
        }

        /// <summary>
        /// Recreates all child files and directories from the detailed information returned from a FTP server.
        /// </summary>
        /// <param name="detail"></param>
        /// <exception cref="System.ArgumentException"><paramref name="detail"/> is null or empty.</exception>
        public void ParseChildren(string detail) {
            if(string.IsNullOrWhiteSpace(detail)) {
                throw new ArgumentException("The detail string is null or empty.", "detail");
            }

            this.m_files.Clear();
            this.m_directories.Clear();

            FtpFileInfo file = null;
            foreach(string line in detail.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)) {
                if((file = FtpFileInfo.Create(this, line)) == null) {
                    continue;
                }

                if(file.IsDirectory) {
                    this.m_directories.Add(new FtpDirectoryInfo(file));
                } else {
                    this.m_files.Add(file);
                }
            }
        }

        /// <summary>
        /// Creates a root FTP directory.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="lastModifiedTime"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="uri"/> is null or not represents a FTP URI or is not a absolute URI.</exception>
        public static FtpDirectoryInfo CreateRoot(Uri uri, DateTime? lastModifiedTime) {
            if(uri == null) {
                throw new ArgumentException("The URI is null.", "uri");
            }
            if(!string.Equals(uri.Scheme, Uri.UriSchemeFtp, StringComparison.OrdinalIgnoreCase)) {
                throw new ArgumentException("The URI not represents a FTP URI.", "uri");
            }
            if(!uri.IsAbsoluteUri) {
                throw new ArgumentException("The URI not represents a absolute FTP URI.", "uri");
            }

            FtpDirectoryInfo root = new FtpDirectoryInfo {
                IsDirectory = true,
                Root = null,
                Parent = null,
                Name = Path.GetFileName(uri.LocalPath),
                RelativeUri = uri.MakeRelativeUri(uri),
                AbsoluteUri = uri,
                Length = 0,
                LastModifyTime = (lastModifiedTime ?? DateTime.MinValue),
            };
            root.Root = root;
            return root;
        }
    }
}
