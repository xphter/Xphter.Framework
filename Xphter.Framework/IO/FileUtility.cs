using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Xphter.Framework.IO {
    /// <summary>
    /// Provides functions to access files.
    /// </summary>
    public static class FileUtility {
        public const int MIN_FILE_STREAM_BUFFER_SIZE = 8;

        private static int g_defaultFileStreamBufferSize = 4096;
        /// <summary>
        /// Gets or sets the default size of the buffer to create a FileStream.
        /// </summary>
        public static int DefaultFileStreamBufferSize {
            get {
                return g_defaultFileStreamBufferSize;
            }
            set {
                g_defaultFileStreamBufferSize = Math.Max(8, value);
            }
        }

        /// <summary>
        /// Creates the folder of the specified file path if it is not existing.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is null or empty.</exception>
        public static void CreateFolder(string path) {
            if(string.IsNullOrWhiteSpace(path)) {
                return;
            }

            string folder = Path.GetDirectoryName(path);
            if(!string.IsNullOrEmpty(folder) && !Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
        }

        /// <summary>
        /// Creates a FileStream from the specified path, return NULL if fail.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <returns></returns>
        public static FileStream CreateFileStream(string path, FileMode mode, FileAccess access, FileShare share) {
            return CreateFileStream(path, mode, access, share, DefaultFileStreamBufferSize, false);
        }

        /// <summary>
        /// Creates a FileStream from the specified path, return NULL if fail.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        public static FileStream CreateFileStream(string path, FileMode mode, FileAccess access, FileShare share, bool isAsync) {
            return CreateFileStream(path, mode, access, share, DefaultFileStreamBufferSize, isAsync);
        }

        /// <summary>
        /// Creates a FileStream from the specified path, return NULL if fail.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <param name="bufferSize"></param>
        /// <param name="isAsync"></param>
        /// <returns></returns>
        public static FileStream CreateFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool isAsync) {
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("path is null or empty.", "path");
            }

            WindowsAPI.FileAccessMode accessMode = WindowsAPI.FileAccessMode.None;
            switch(access) {
                case FileAccess.Read:
                    accessMode = WindowsAPI.FileAccessMode.Read;
                    break;
                case FileAccess.Write:
                    accessMode = WindowsAPI.FileAccessMode.Write;
                    break;
                case FileAccess.ReadWrite:
                    accessMode = WindowsAPI.FileAccessMode.Read | WindowsAPI.FileAccessMode.Write;
                    break;
            }

            WindowsAPI.FileShareMode shareMode = WindowsAPI.FileShareMode.None;
            switch(share) {
                case FileShare.Read:
                    shareMode = WindowsAPI.FileShareMode.Read;
                    break;
                case FileShare.Write:
                    shareMode = WindowsAPI.FileShareMode.Write;
                    break;
                case FileShare.ReadWrite:
                    shareMode = WindowsAPI.FileShareMode.Read | WindowsAPI.FileShareMode.Write;
                    break;
                case FileShare.Delete:
                    shareMode = WindowsAPI.FileShareMode.Delete;
                    break;
            }

            WindowsAPI.FileCreationMode creationMode = WindowsAPI.FileCreationMode.CreateAlways;
            switch(mode) {
                case FileMode.Create:
                    creationMode = WindowsAPI.FileCreationMode.CreateAlways;
                    break;
                case FileMode.CreateNew:
                    creationMode = WindowsAPI.FileCreationMode.CreateNew;
                    break;
                case FileMode.Open:
                case FileMode.Append:
                    creationMode = WindowsAPI.FileCreationMode.OpenExisting;
                    break;
                case FileMode.OpenOrCreate:
                    creationMode = WindowsAPI.FileCreationMode.OpenAlways;
                    break;
                case FileMode.Truncate:
                    creationMode = WindowsAPI.FileCreationMode.TruncateExisting;
                    break;
            }

            WindowsAPI.FileFlagAndAttribute flagAndAttribute = WindowsAPI.FileFlagAndAttribute.AttributeNormal;
            if(isAsync) {
                flagAndAttribute |= WindowsAPI.FileFlagAndAttribute.FlagOverlapped;
            }

            FileStream stream = null;
            IntPtr ptr = WindowsAPI.CreateFile(path, accessMode, shareMode, IntPtr.Zero, creationMode, flagAndAttribute, IntPtr.Zero);
            SafeFileHandle handle = new SafeFileHandle(ptr, true);

            if(!handle.IsInvalid) {
                stream = new FileStream(handle, access, bufferSize, isAsync);
            } else {
                handle.Dispose();
            }

            return stream;
        }

        /// <summary>
        ///  Creates a new file, writes the specified byte array to the file, and then
        ///  closes the file. If the target file already exists, it is overwritten.
        ///  If the target directory is not existing then create it.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is null or empty.</exception>
        public static void WriteAllBytes(string path, byte[] data) {
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("Fiel path is null or empty.", "path");
            }

            CreateFolder(path);
            File.WriteAllBytes(path, data ?? new byte[0]);
        }

        /// <summary>
        ///  Begins then operation to creates a new file, writes the specified byte array to the file, and then
        ///  closes the file. If the target file already exists, it is overwritten.
        ///  If the target directory is not existing then create it.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="bufferSize"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginWriteAllBytes(string path, byte[] data, int bufferSize, AsyncCallback callback, object userState) {
            return BeginWriteAllBytes(path, data, 0, data.Length, bufferSize, callback, userState);
        }

        /// <summary>
        ///  Begins then operation to creates a new file, writes the specified byte array to the file, and then
        ///  closes the file. If the target file already exists, it is overwritten.
        ///  If the target directory is not existing then create it.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="bufferSize"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginWriteAllBytes(string path, byte[] data, int offset, int length, int bufferSize, AsyncCallback callback, object userState) {
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("Fiel path is null or empty.", "path");
            }
            if(data == null) {
                throw new ArgumentNullException("data");
            }

            Exception syncError = null;
            AsyncResult result = new AsyncResult(callback, userState);

            try {
                CreateFolder(path);
            } catch(Exception ex) {
                syncError = ex;
            }
            if(syncError != null) {
                result.MarkCompleted(syncError, true);
                return result;
            }

            FileStream writer = null;
            try {
                writer = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, bufferSize, true);
            } catch(Exception ex) {
                syncError = ex;
            }
            if(syncError != null) {
                result.MarkCompleted(syncError, true);
                return result;
            }

            if(writer.IsAsync) {
                try {
                    writer.BeginWrite(data, offset, length, (ar) => {
                        Exception asyncError = null;

                        try {
                            using(FileStream fs = (FileStream) ar.AsyncState) {
                                fs.EndWrite(ar);
                            }
                        } catch(Exception ex) {
                            asyncError = ex;
                        }

                        result.MarkCompleted(asyncError, false);
                    }, writer);
                } catch(Exception ex) {
                    syncError = ex;
                }

                if(syncError != null) {
                    result.MarkCompleted(syncError, true);
                }
            } else {
                try {
                    using(writer) {
                        writer.Write(data, offset, length);
                    }
                } catch(Exception ex) {
                    syncError = ex;
                }

                result.MarkCompleted(syncError, true);
            }

            return result;
        }

        /// <summary>
        ///  Begins then operation to writes the specified byte array to the file, and then closes the file.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginWriteAllBytes(FileStream writer, byte[] data, int offset, int length, AsyncCallback callback, object userState) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }

            Exception syncError = null;
            AsyncResult result = new AsyncResult(callback, userState);

            if(writer.IsAsync) {
                try {
                    writer.BeginWrite(data, offset, length, (ar) => {
                        Exception asyncError = null;

                        try {
                            using(FileStream fs = (FileStream) ar.AsyncState) {
                                fs.EndWrite(ar);
                            }
                        } catch(Exception ex) {
                            asyncError = ex;
                        }

                        result.MarkCompleted(asyncError, false);
                    }, writer);
                } catch(Exception ex) {
                    syncError = ex;
                }

                if(syncError != null) {
                    result.MarkCompleted(syncError, true);
                }
            } else {
                try {
                    using(writer) {
                        writer.Write(data, offset, length);
                    }
                } catch(Exception ex) {
                    syncError = ex;
                }

                result.MarkCompleted(syncError, true);
            }

            return result;
        }

        /// <summary>
        /// Ends the async operation to writes bytes array to a file.
        /// </summary>
        /// <param name="asyncResult"></param>
        public static void EndWriteAllBytes(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult)) {
                throw new InvalidOperationException();
            }

            ((AsyncResult) asyncResult).End();
        }

        /// <summary>
        ///  Creates a new file, writes the specified string to the file, and then
        ///  closes the file. If the target file already exists, it is overwritten.
        ///  If the target directory is not existing then create it.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is null or empty.</exception>
        public static void WriteAllText(string path, string contents) {
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("Fiel path is null or empty.", "path");
            }

            CreateFolder(path);
            File.WriteAllText(path, contents ?? string.Empty);
        }

        /// <summary>
        ///  Creates a new file, writes the specified string to the file using the specified encoding, and then
        ///  closes the file. If the target file already exists, it is overwritten.
        ///  If the target directory is not existing then create it.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        /// <param name="encoding"></param>
        /// <exception cref="System.ArgumentException"><paramref name="path"/> is null or empty.</exception>
        public static void WriteAllText(string path, string contents, Encoding encoding) {
            if(string.IsNullOrWhiteSpace(path)) {
                throw new ArgumentException("Fiel path is null or empty.", "path");
            }

            CreateFolder(path);
            File.WriteAllText(path, contents ?? string.Empty, encoding);
        }

        /// <summary>
        ///  Begins the async operation to creates a new file, writes the specified text contents to the file, and then
        ///  closes the file. If the target file already exists, it is overwritten.
        ///  If the target directory is not existing then create it.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="contents"></param>
        /// <param name="encoding"></param>
        /// <param name="encodingBuffer"></param>
        /// <param name="writeBufferSize"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginWriteAllText(string path, string contents, Encoding encoding, byte[] encodingBuffer, int writeBufferSize, AsyncCallback callback, object userState) {
            int bufferLength = 0;
            Exception syncError = null;
            AsyncResult result = new AsyncResult(callback, userState);

            try {
                bufferLength = encoding.GetBytes(contents ?? string.Empty, 0, contents != null ? contents.Length : 0, encodingBuffer, 0);
            } catch(Exception ex) {
                syncError = ex;
            }
            if(syncError != null) {
                result.MarkCompleted(syncError, true);
                return result;
            }

            FileUtility.BeginWriteAllBytes(path, encodingBuffer, 0, bufferLength, writeBufferSize, (ar) => {
                Exception asyncError = null;
                try {
                    FileUtility.EndWriteAllBytes(ar);
                } catch(Exception ex) {
                    asyncError = ex;
                }

                result.MarkCompleted(asyncError, ar.CompletedSynchronously);
            }, null);

            return result;
        }

        /// <summary>
        /// Ends the async operation to writes text contents to a file.
        /// </summary>
        /// <param name="asyncResult"></param>
        public static void EndWriteAllText(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult)) {
                throw new InvalidOperationException();
            }

            ((AsyncResult) asyncResult).End();
        }

        /// <summary>
        /// Reads bytes content from a strem.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="reader"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="isSync"></param>
        private static void ReadBytes(AsyncResult<int> result, Stream reader, byte[] buffer, int offset, int length, bool isSync) {
            try {
                reader.BeginRead(buffer, offset, buffer.Length - offset, (ar) => {
                    int count = 0;
                    Exception error = null;

                    try {
                        count = reader.EndRead(ar);
                    } catch(Exception ex) {
                        error = ex;
                    }

                    if(error != null || count == 0) {
                        try {
                            reader.Close();
                        } catch(Exception ex) {
                            if(error == null) {
                                error = ex;
                            }
                        }

                        result.MarkCompleted(error, isSync && ar.CompletedSynchronously, length + count);
                    } else {
                        ReadBytes(result, reader, buffer, offset + count, length + count, isSync && ar.CompletedSynchronously);
                    }
                }, null);
            } catch(Exception ex) {
                try {
                    reader.Close();
                } catch {
                }

                result.MarkCompleted(ex, isSync, length);
            }
        }

        /// <summary>
        ///  Begins then operation to reads content from the file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="buffer"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginReadAllBytes(string path, byte[] buffer, AsyncCallback callback, object userState) {
            return BeginReadAllBytes(path, buffer, 0, callback, userState);
        }

        /// <summary>
        /// Begins then operation to reads content from the file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginReadAllBytes(string path, byte[] buffer, int offset, AsyncCallback callback, object userState) {
            FileInfo file = new FileInfo(path);
            if(!file.Exists) {
                throw new FileNotFoundException("the file is not existing", path);
            }
            if(file.Length > buffer.Length - offset) {
                throw new ArgumentException("The buffer length minus offset is less than file length.", "buffer");
            }

            Exception error = null;
            FileStream reader = null;
            AsyncResult<int> result = new AsyncResult<int>(callback, userState);

            try {
                reader = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, true);
            } catch(Exception ex) {
                error = ex;
            }

            if(error == null) {
                if(reader.IsAsync) {
                    ReadBytes(result, reader, buffer, offset, 0, true);
                } else {
                    int count = 0, length = 0;

                    try {
                        using(reader) {
                            while((count = reader.Read(buffer, offset + length, buffer.Length - offset - length)) > 0) {
                                length += count;
                            }
                        }
                    } catch(Exception ex) {
                        error = ex;
                    }

                    result.MarkCompleted(error, true, length);
                }
            } else {
                result.MarkCompleted(error, true);
            }

            return result;
        }

        /// <summary>
        /// Ends the async operation to reads bytes from a file.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static int EndReadAllBytes(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<int>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<int>) asyncResult).End();
        }

        /// <summary>
        ///  Begins the async operation to read text contents from the file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="buffer"></param>
        /// <param name="encoding"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        public static IAsyncResult BeginReadAllText(string path, byte[] buffer, Encoding encoding, AsyncCallback callback, object userState) {
            AsyncResult<string> result = new AsyncResult<string>(callback, userState);

            BeginReadAllBytes(path, buffer, 0, (ar) => {
                int length = 0;
                string text = null;
                Exception error = null;

                try {
                    length = EndReadAllBytes(ar);
                } catch(Exception ex) {
                    error = ex;
                }

                if(error == null) {
                    try {
                        text = encoding.GetString(buffer, 0, length);
                    } catch(Exception ex) {
                        error = ex;
                    }
                }

                result.MarkCompleted(error, ar.CompletedSynchronously, text);
            }, null);

            return result;
        }

        /// <summary>
        /// Ends the async operation to read text contents from a file.
        /// </summary>
        /// <param name="asyncResult"></param>
        /// <returns></returns>
        public static string EndReadAllText(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<string>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<string>) asyncResult).End();
        }
    }
}
