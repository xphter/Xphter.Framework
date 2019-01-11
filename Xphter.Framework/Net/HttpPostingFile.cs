using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xphter.Framework.IO;

namespace Xphter.Framework.Net {
    /// <summary>
    /// Represents a upload file used by FormValuesSerializer class.
    /// FormValuesSerializer class will serialize the actual file content of this object.
    /// </summary>
    public class HttpPostingFile {
        /// <summary>
        /// Initializes a new instance of HttpPostingFile class.
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="System.ArgumentException"><paramref name="filePath"/> is null or empty or not represents a local file.</exception>
        public HttpPostingFile(string filePath) {
            if(string.IsNullOrWhiteSpace(filePath)) {
                throw new ArgumentException("filePath is null or empty.", "filePath");
            }
            if(!PathUtility.IsValidLocalPath(filePath)) {
                throw new ArgumentException("filePath not represents a local file.", "filePath");
            }

            this.FilePath = filePath;
        }

        /// <summary>
        /// Initializes a new instance of HttpPostingFile class.
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="fileName"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="fileContent"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="fileName"/> is null or empty.</exception>
        public HttpPostingFile(byte[] fileContent, string fileName) {
            if(fileContent == null) {
                throw new ArgumentNullException("fileContent");
            }
            if(string.IsNullOrWhiteSpace(fileName)) {
                throw new ArgumentException("fileName is null or empty.", "fileName");
            }

            this.FileContent = fileContent;
            this.FilePath = fileName;
        }

        /// <summary>
        /// Gets the full path of posting file.
        /// </summary>
        public string FilePath {
            get;
            private set;
        }

        /// <summary>
        /// Gets the file content.
        /// </summary>
        public byte[] FileContent {
            get;
            private set;
        }
    }
}
