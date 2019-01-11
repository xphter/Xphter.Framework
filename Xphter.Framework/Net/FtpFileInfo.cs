using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xphter.Framework.Net {
    /// <summary>
    /// Represents the detail of a file in the FTP server.
    /// </summary>
    public class FtpFileInfo {
        protected FtpFileInfo() {
        }

        /// <summary>
        /// The directory flag in the MS-DOS format.
        /// </summary>
        public const String DIRECTORY_FLAG_WINDOWS = "<DIR>";

        /// <summary>
        /// The directory flag in the Unix format.
        /// </summary>
        public const String DIRECTORY_FLAG_UNIX = "d";

        /// <summary>
        /// The current directory name.
        /// </summary>
        public const String CURRENT_DIRECTORY_NAME = ".";

        /// <summary>
        /// The parent directory name.
        /// </summary>
        public const String PARENT_DIRECTORY_NAME = "..";

        /// <summary>
        /// Gets a value to indicate whether this is a directory.
        /// </summary>
        public bool IsDirectory {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the root directory, the RelativeUri property is relatived to this directory.
        /// </summary>
        public FtpDirectoryInfo Root {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the parent directory.
        /// </summary>
        public FtpDirectoryInfo Parent {
            get;
            protected set;
        }

        /// <summary>
        /// Gets file name.
        /// </summary>
        public string Name {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the URI relative to the root directory.
        /// </summary>
        public Uri RelativeUri {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the absolute URI of this file.
        /// </summary>
        public Uri AbsoluteUri {
            get;
            protected set;
        }

        /// <summary>
        /// Gets file length.
        /// </summary>
        public long Length {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the time when this file was last modified.
        /// </summary>
        public DateTime LastModifyTime {
            get;
            protected set;
        }

        private static int IndexOfWhiteSpace(string detail, int ordinal) {
            int index = 0;
            Match match = null;
            Regex regex = new Regex(@"\s+");
            for(int i = 0; i < ordinal; i++) {
                if(!(match = regex.Match(detail, index)).Success) {
                    throw new ArgumentOutOfRangeException("There are not so much continuous white spaces in the detail information string.", "ordinal");
                }

                index = match.Index + match.Length;
            }
            return index;
        }

        /// <summary>
        /// Parese the MS-DOS format: MM-dd-yy hh:mmtt length name
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        private static FtpFileInfo ParseMsDosFormat(string detail) {
            FtpFileInfo info = new FtpFileInfo();
            string[] parts = detail.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            info.LastModifyTime = DateTime.ParseExact(string.Format("{0} {1}", parts[0], parts[1]), "MM-dd-yy hh:mmtt", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
            if(string.Equals(DIRECTORY_FLAG_WINDOWS, parts[2], StringComparison.OrdinalIgnoreCase)) {
                info.IsDirectory = true;
            } else {
                info.Length = long.Parse(parts[2]);
            }
            info.Name = detail.Substring(IndexOfWhiteSpace(detail, 3));

            return info;
        }

        /// <summary>
        /// Parses the UNIX format: privilege count user group length MMM dd HH:mm|yyyy name
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        private static FtpFileInfo ParseUnixFormat(string detail) {
            FtpFileInfo info = new FtpFileInfo();
            string[] parts = detail.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            info.IsDirectory = detail.StartsWith(DIRECTORY_FLAG_UNIX, StringComparison.OrdinalIgnoreCase);
            info.Length = long.Parse(parts[4]);
            if(parts[7].Contains(':')) {
                info.LastModifyTime = DateTime.ParseExact(string.Format("{0} {1} {2}", parts[5], parts[6], parts[7]), new string[] {
                        "MMM d HH:mm",
                        "MMM d H:mm",
                        "MMM d HH:m",
                        "MMM d H:m",
                        "MMM dd HH:mm",
                        "MMM dd H:mm",
                        "MMM dd HH:m",
                        "MMM dd H:m",
                    }, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
            } else {
                info.LastModifyTime = DateTime.ParseExact(string.Format("{0} {1} {2}", parts[5], parts[6], parts[7]), new string[] {
                        "MMM d yy",
                        "MMM d yyyy",
                        "MMM dd yy",
                        "MMM dd yyyy",
                    }, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
            }
            if(info.LastModifyTime > DateTime.Now) {
                info.LastModifyTime = info.LastModifyTime.AddYears(-1);
            }
            info.Name = detail.Substring(IndexOfWhiteSpace(detail, 8));

            return info;
        }

        /// <summary>
        /// Creates a FtpFileInfo object from the detailed information returned from the FTP server.
        /// If the file represents the current directory or parent directory, then return null.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="detail"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"><paramref name="parent"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="detail"/> is null or empty.</exception>
        public static FtpFileInfo Create(FtpDirectoryInfo parent, string detail) {
            if(parent == null) {
                throw new ArgumentException("The parent directory is null.", "parent");
            }
            if(string.IsNullOrWhiteSpace(detail)) {
                throw new ArgumentException("The detailed information is null or empty.", "detail");
            }

            FtpFileInfo info = null;
            if(Regex.IsMatch(detail, @"^\d")) {
                //MS-DOS format
                info = ParseMsDosFormat(detail);
            } else {
                //Unix format
                info = ParseUnixFormat(detail);
            }

            if(string.Equals(info.Name, CURRENT_DIRECTORY_NAME) || string.Equals(info.Name, PARENT_DIRECTORY_NAME)) {
                info = null;
            } else {
                info.Root = parent.Root;
                info.Parent = parent;
                if(parent.RelativeUri.OriginalString.Length > 0) {
                    info.RelativeUri = new Uri(string.Format("{0}/{1}", parent.RelativeUri, info.Name), UriKind.Relative);
                } else {
                    info.RelativeUri = new Uri(info.Name, UriKind.Relative);
                }
                info.AbsoluteUri = new Uri(parent.AbsoluteUri, info.Name);
                //info.RelativeUri = new Uri(Uri.UnescapeDataString(parent.Root.AbsoluteUri.MakeRelativeUri(info.AbsoluteUri).ToString()), UriKind.Relative);
            }
            return info;
        }

        #region Object Members

        /// <inheritdoc />
        public override int GetHashCode() {
            return this.AbsoluteUri.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }
            if(object.ReferenceEquals(this, obj)) {
                return true;
            }
            if(!(obj is FtpFileInfo)) {
                return false;
            }

            return this.AbsoluteUri.Equals(((FtpFileInfo) obj).AbsoluteUri);
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.AbsoluteUri.ToString();
        }

        #endregion
    }
}
