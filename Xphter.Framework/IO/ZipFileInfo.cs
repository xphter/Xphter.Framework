using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Xphter.Framework.IO {
    /// <summary>
    /// Represents a file in a ZIP archive.
    /// </summary>
    public class ZipFileInfo : ZipItemInfo {
        /// <summary>
        /// Initialize a new instance of ZipFileInfo class.
        /// </summary>
        internal ZipFileInfo()
            : base() {
        }

        /// <summary>
        /// Gets the instance of parent directory.
        /// </summary>
        public ZipDirectoryInfo Directory {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a stream for reading or writing the file.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <returns></returns>
        public Stream GetStream(FileMode mode, FileAccess access) {
            return (Stream) GetStreamMethod.Invoke(this.m_internalZipFileInfo, new object[]{
                mode,
                access,
            });
        }

        /// <summary>
        /// Gets binary data content of this file.
        /// </summary>
        /// <returns></returns>
        public byte[] GetContent() {
            int count = 0;
            byte[] buffer = new byte[1024];
            using(MemoryStream writer = new MemoryStream(1024)) {
                using(Stream reader = this.GetStream(FileMode.Open, FileAccess.Read)) {
                    while((count = reader.Read(buffer, 0, buffer.Length)) > 0) {
                        writer.Write(buffer, 0, count);
                    }
                }
                return writer.ToArray();
            }
        }

        /// <summary>
        /// Saves the file to the specified directory.
        /// </summary>
        /// <param name="directoryName"></param>
        public void Save(string directoryName) {
            this.SaveAs(Path.Combine(directoryName ?? string.Empty, this.FullName));
        }

        /// <summary>
        /// Saves the file content to the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is null or empty.</exception>
        public void SaveAs(string path) {
            string directory = Path.GetDirectoryName(path);
            if(!string.IsNullOrEmpty(directory) && !System.IO.Directory.Exists(directory)) {
                System.IO.Directory.CreateDirectory(directory);
            }

            int count = 0;
            byte[] buffer = new byte[1024];
            using(Stream writer = File.Open(path ?? this.FullName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using(Stream reader = this.GetStream(FileMode.Open, FileAccess.Read)) {
                    while((count = reader.Read(buffer, 0, buffer.Length)) > 0) {
                        writer.Write(buffer, 0, count);
                    }
                }
            }
        }
    }
}
