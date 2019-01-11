using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xphter.Framework.IO {
    /// <summary>
    /// Provides functions to access files.
    /// </summary>
    public static class DirectoryUtility {
        #region Search Files

        /// <summary>
        /// Searches files in the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(string path, string pattern) {
            return SearchFiles(new DirectoryInfo(path), new Regex(pattern), SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Searches files in the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(string path, string pattern, SearchOption option) {
            return SearchFiles(new DirectoryInfo(path), new Regex(pattern), option);
        }

        /// <summary>
        /// Searches files in the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(string path, Regex regex) {
            return SearchFiles(new DirectoryInfo(path), regex, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Searches files in the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="regex"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(string path, Regex regex, SearchOption option) {
            return SearchFiles(new DirectoryInfo(path), regex, option);
        }

        /// <summary>
        /// Searches files in the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(this DirectoryInfo directory, string pattern) {
            return SearchFiles(directory, new Regex(pattern), SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Searches files in the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="pattern"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(this DirectoryInfo directory, string pattern, SearchOption option) {
            return SearchFiles(directory, new Regex(pattern), option);
        }

        /// <summary>
        /// Searches files in the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(this DirectoryInfo directory, Regex regex) {
            return SearchFiles(directory, regex, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Searches files in the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="regex"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(this DirectoryInfo directory, Regex regex, SearchOption option) {
            return directory.EnumerateFiles("*", option).Where((item) => regex.IsMatch(item.Name));
        }

        #endregion

        #region Search Directories

        /// <summary>
        /// Searches files in the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectories(string path, string pattern) {
            return SearchDirectories(new DirectoryInfo(path), new Regex(pattern), SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Searches files in the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectories(string path, string pattern, SearchOption option) {
            return SearchDirectories(new DirectoryInfo(path), new Regex(pattern), option);
        }

        /// <summary>
        /// Searches files in the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectories(string path, Regex regex) {
            return SearchDirectories(new DirectoryInfo(path), regex, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Searches files in the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="regex"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectories(string path, Regex regex, SearchOption option) {
            return SearchDirectories(new DirectoryInfo(path), regex, option);
        }

        /// <summary>
        /// Searches files in the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectories(this DirectoryInfo directory, string pattern) {
            return SearchDirectories(directory, new Regex(pattern), SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Searches files in the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="pattern"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectories(this DirectoryInfo directory, string pattern, SearchOption option) {
            return SearchDirectories(directory, new Regex(pattern), option);
        }

        /// <summary>
        /// Searches files in the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="regex"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectories(this DirectoryInfo directory, Regex regex) {
            return SearchDirectories(directory, regex, SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// Searches files in the specified directory.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="regex"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectories(this DirectoryInfo directory, Regex regex, SearchOption option) {
            return directory.EnumerateDirectories("*", option).Where((item) => regex.IsMatch(item.Name));
        }

        #endregion

        /// <summary>
        /// Get directory name of the specified directory path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="path"/> is null.</exception>
        public static string GetName(string path) {
            if(path == null) {
                throw new ArgumentNullException("path");
            }
            if(path.Length == 0) {
                return string.Empty;
            }
            if(path.Length == 1 &&
                (path[0] == Path.DirectorySeparatorChar ||
                    path[0] == Path.AltDirectorySeparatorChar)) {
                return string.Empty;
            }

            int startIndex = 0, endIndex = 0;

            endIndex = (path = Path.GetFullPath(path)).Length - 1;
            if(path.EndsWith(Path.DirectorySeparatorChar.ToString()) ||
                path.EndsWith(Path.AltDirectorySeparatorChar.ToString())) {
                --endIndex;
            }

            for(int i = endIndex; i >= 0; i--) {
                if(path[i] == Path.DirectorySeparatorChar ||
                    path[i] == Path.AltDirectorySeparatorChar) {
                    startIndex = i + 1;
                    break;
                }
            }

            return endIndex > startIndex ? path.Substring(startIndex, endIndex - startIndex + 1) : string.Empty;
        }

        /// <summary>
        /// Appends directory separator to the end of <paramref name="path"/> if necessary.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string Normalize(string path) {
            if(path == null) {
                throw new ArgumentNullException("path");
            }
            if(path.Length == 0) {
                return string.Empty;
            }

            int index = path.Length - 1;
            if(path[index] == Path.DirectorySeparatorChar ||
                path[index] == Path.AltDirectorySeparatorChar) {
                return path;
            } else {
                return path + Path.DirectorySeparatorChar;
            }
        }
    }
}
