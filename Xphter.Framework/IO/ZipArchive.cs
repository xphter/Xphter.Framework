using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using Xphter.Framework.Collections;

namespace Xphter.Framework.IO {
    /// <summary>
    /// Represents a ZIP archive.
    /// </summary>
    public class ZipArchive : IDisposable {
        /// <summary>
        /// Initialize ZipArchive class.
        /// </summary>
        static ZipArchive() {
            Type t = typeof(Package).Assembly.GetType("MS.Internal.IO.Zip.ZipArchive");
            OpenOnFileMethod = t.GetMethod("OpenOnFile", BindingFlags.Static | BindingFlags.NonPublic);
            OpenOnStreamMethod = t.GetMethod("OpenOnStream", BindingFlags.Static | BindingFlags.NonPublic);
            GetFilesMethod = t.GetMethod("GetFiles", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Initialize a new instance of ZipArchive class.
        /// </summary>
        private ZipArchive() {
            this.m_directories = new List<ZipDirectoryInfo>();
            this.m_files = new List<ZipFileInfo>();
        }

        private static readonly MethodInfo OpenOnFileMethod;
        private static readonly MethodInfo OpenOnStreamMethod;
        private static readonly MethodInfo GetFilesMethod;

        private ICollection<ZipDirectoryInfo> m_directories;
        private ICollection<ZipFileInfo> m_files;
        private object m_internalZipArchive;

        /// <summary>
        /// Gets the mode when open the archive.
        /// </summary>
        public FileMode OpenMode {
            get;
            private set;
        }

        /// <summary>
        /// Gets the access mode when open the archive.
        /// </summary>
        public FileAccess OpenAccess {
            get;
            private set;
        }

        /// <summary>
        /// Analyzes the specified instance of MS.Internal.IO.Zip.ZipArchive.
        /// </summary>
        /// <param name="internalZipArchive"></param>
        private void Analyze(object internalZipArchive) {
            this.m_internalZipArchive = internalZipArchive;

            IEnumerable<object> items = ((IEnumerable) GetFilesMethod.Invoke(internalZipArchive, null)).Cast<object>();
            IDictionary<string, ZipDirectoryInfo> directories = items.Where((item) => (bool) ZipItemInfo.FolderFlagProperty.GetValue(item, null)).Select((item) => new ZipDirectoryInfo {
                InternalZipFileInfo = item,
            }).ToDictionary((item) => Path.GetDirectoryName(item.FullName), (item) => item);
            IDictionary<string, ZipFileInfo> files = items.Where((item) => !(bool) ZipItemInfo.FolderFlagProperty.GetValue(item, null)).Select((item) => new ZipFileInfo {
                InternalZipFileInfo = item,
            }).ToDictionary((item) => item.FullName, (item) => item);

            string directoryName = null;
            foreach(KeyValuePair<string, ZipDirectoryInfo> pair in directories) {
                pair.Value.OwnerArchive = this;
                if(string.IsNullOrEmpty(directoryName = Path.GetDirectoryName(pair.Key))) {
                    this.m_directories.Add(pair.Value);
                } else {
                    (pair.Value.Parent = directories[directoryName]).Directories.Add(pair.Value);
                }
            }
            foreach(KeyValuePair<string, ZipFileInfo> pair in files) {
                pair.Value.OwnerArchive = this;
                if(string.IsNullOrEmpty(directoryName = Path.GetDirectoryName(pair.Key))) {
                    this.m_files.Add(pair.Value);
                } else {
                    (pair.Value.Directory = directories[directoryName]).Files.Add(pair.Value);
                }
            }
        }

        /// <summary>
        /// Creates a ZipArchive object from the specified file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <param name="streaming"></param>
        /// <returns></returns>
        public static ZipArchive CreateFromFile(string path, FileMode mode, FileAccess access, FileShare share, bool streaming) {
            ZipArchive archive = new ZipArchive {
                OpenMode = mode,
                OpenAccess = access,
            };
            archive.Analyze(OpenOnFileMethod.Invoke(null, new object[]{
                path,
                mode,
                access,
                share,
                streaming,
            }));
            return archive;
        }

        /// <summary>
        /// Creates a ZipArchive object from the specified stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="streaming"></param>
        /// <returns></returns>
        public static ZipArchive CreateFromStream(Stream stream, FileMode mode, FileAccess access, bool streaming) {
            ZipArchive archive = new ZipArchive {
                OpenMode = mode,
                OpenAccess = access,
            };
            archive.Analyze(OpenOnStreamMethod.Invoke(null, new object[]{
                stream,
                mode,
                access,
                streaming,
            }));
            return archive;
        }

        /// <summary>
        /// Gets the subdirectories of the current archive.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ZipDirectoryInfo> GetDirectories() {
            return this.m_directories;
        }

        /// <summary>
        /// Gets a file list from the current archive.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ZipFileInfo> GetFiles() {
            return this.m_files;
        }

        /// <summary>
        /// Gets all files in this archive.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ZipFileInfo> GetAllFiles() {
            ZipDirectoryInfo directory = null;
            List<ZipFileInfo> files = new List<ZipFileInfo>(this.GetFiles());
            Queue<ZipDirectoryInfo> queue = new Queue<ZipDirectoryInfo>(this.GetDirectories());
            while(queue.Count > 0) {
                directory = queue.Dequeue();
                files.AddRange(directory.GetFiles());
                foreach(ZipDirectoryInfo item in directory.GetDirectories()) {
                    queue.Enqueue(item);
                }
            }
            return files;
        }

        /// <summary>
        /// Extracts all directories and files to the specified directory.
        /// </summary>
        /// <param name="directoryName"></param>
        public void Extract(string directoryName) {
            if(!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
            }

            foreach(ZipDirectoryInfo item in this.m_directories) {
                item.Save(directoryName);
            }
            foreach(ZipFileInfo item in this.m_files) {
                item.Save(directoryName);
            }
        }

        #region IDisposable Members

        /// <inheritdoc />
        public void Dispose() {
            if(this.m_internalZipArchive != null) {
                ((IDisposable) this.m_internalZipArchive).Dispose();
            }
        }

        #endregion
    }
}
