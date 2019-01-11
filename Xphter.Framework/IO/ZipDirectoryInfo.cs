using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xphter.Framework.Collections;

namespace Xphter.Framework.IO {
    /// <summary>
    /// Represents a directory in a ZIP archive.
    /// </summary>
    public class ZipDirectoryInfo : ZipItemInfo {
        /// <summary>
        /// Initialize a new instance of ZipDirectoryInfo class.
        /// </summary>
        internal ZipDirectoryInfo()
            : base() {
            this.Directories = new List<ZipDirectoryInfo>();
            this.Files = new List<ZipFileInfo>();
        }

        /// <summary>
        /// Gets the instance of parent directory.
        /// </summary>
        public ZipDirectoryInfo Parent {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the subdirectories of the current directory.
        /// </summary>
        internal ICollection<ZipDirectoryInfo> Directories {
            get;
            private set;
        }

        /// <summary>
        /// Gets the file list of the current directory.
        /// </summary>
        internal ICollection<ZipFileInfo> Files {
            get;
            private set;
        }

        /// <summary>
        /// Gets the subdirectories of the current directory.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ZipDirectoryInfo> GetDirectories() {
            return this.Directories;
        }

        /// <summary>
        /// Gets a file list from the current directory.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ZipFileInfo> GetFiles() {
            return this.Files;
        }

        /// <summary>
        /// Saves the directory and all child items to the specified directory.
        /// </summary>
        /// <param name="directoryName"></param>
        public void Save(string directoryName) {
            string path = Path.Combine(directoryName ?? string.Empty, this.FullName);
            if(!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }

            foreach(ZipDirectoryInfo item in this.Directories) {
                item.Save(directoryName);
            }
            foreach(ZipFileInfo item in this.Files) {
                item.Save(directoryName);
            }
        }
    }
}
