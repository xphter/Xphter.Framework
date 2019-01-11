using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xphter.Framework.IO {
    /// <summary>
    /// Provides functions to access file path.
    /// </summary>
    public static class PathUtility {
        /// <summary>
        /// The maximum length of a file path.
        /// </summary>
        public const int MAX_PATH_LENGTH = 260;

        /// <summary>
        /// The regex used to validate local file path validity.
        /// </summary>
        private static Regex g_filePathRegex;

        /// <summary>
        /// The regex used to validate local file name validity.
        /// </summary>
        private static Regex g_invalidFileNameRegex;

        private static char[] g_directorySeparators = new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };
        private static char[] g_extensionSeparators = new char[] { '.' };

        /// <summary>
        /// Create a regex used to validate file path validity.
        /// The regex should only used to validate local file path string.
        /// </summary>
        /// <returns>A regex used to validate file path validity.</returns>
        private static Regex CreateFilePathRegex() {
            StringBuilder invalidPathChars = new StringBuilder();
            foreach(char c in Path.GetInvalidPathChars()) {
                invalidPathChars.AppendFormat("\\u{0:X4}", (int) c);
            }
            invalidPathChars.AppendFormat("\\u{0:X4}", (int) Path.VolumeSeparatorChar);
            invalidPathChars.AppendFormat("\\u{0:X4}", (int) Path.DirectorySeparatorChar);
            invalidPathChars.AppendFormat("\\u{0:X4}", (int) Path.AltDirectorySeparatorChar);

            StringBuilder invalidFileNameChars = new StringBuilder();
            foreach(char c in Path.GetInvalidFileNameChars()) {
                invalidFileNameChars.AppendFormat("\\u{0:X4}", (int) c);
            }
            invalidFileNameChars.AppendFormat("\\u{0:X4}", (int) Path.VolumeSeparatorChar);
            invalidFileNameChars.AppendFormat("\\u{0:X4}", (int) Path.DirectorySeparatorChar);
            invalidFileNameChars.AppendFormat("\\u{0:X4}", (int) Path.AltDirectorySeparatorChar);

            /*
             * regex pattern format:
             * {0} - System volume separator char
             * {1} - System directory separator char
             * {2} - System Alternate directory separator char
             * {3} - Invalid path chars
             * {4} - Invalid file name chars
             * */
            string pattern = string.Format(
                "^(?:" +
                    "(?:" +
                        "(?:[a-z]{0})?" +
                        "(?:[{1}{2}][^{3}]+)*" +
                        "(?:[{1}{2}][^{4}]*)?" +
                    ")" +
                "|" +
                    "(?:" +
                        "(?:[^{3}]+[{1}{2}])*" +
                        "[^{4}]*" +
                    ")" +
                ")$",
              string.Format("\\u{0:X4}", (int) Path.VolumeSeparatorChar),
              string.Format("\\u{0:X4}", (int) Path.DirectorySeparatorChar),
              string.Format("\\u{0:X4}", (int) Path.AltDirectorySeparatorChar),
              invalidPathChars,
              invalidFileNameChars);

            return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        }

        /// <summary>
        /// Create a regex used to validate file name validity.
        /// </summary>
        /// <returns></returns>
        private static Regex CreateInvalidFileNameRegex() {
            return new Regex(@"^\s*(?:con|prn|aux|nul|com\d|lpt\d)\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// Determines whether the specified string is a valid local file path.
        /// </summary>
        /// <param name="path">The string will be check.</param>
        /// <returns>Return true if <paramref name="path"/> is a valid file path, otherwise return false.</returns>
        public static bool IsValidLocalPath(string path) {
            if(string.IsNullOrWhiteSpace(path)) {
                return false;
            }

            bool result = false;

            if(g_filePathRegex == null) {
                g_filePathRegex = CreateFilePathRegex();
            }
            if(g_invalidFileNameRegex == null) {
                g_invalidFileNameRegex = CreateInvalidFileNameRegex();
            }
            if(result = g_filePathRegex.IsMatch(path)) {
                string[] parts = path.Split(g_directorySeparators, StringSplitOptions.RemoveEmptyEntries);
                if(parts.Length > 0) {
                    for(int i = 0; i < parts.Length - 1; i++) {
                        if(g_invalidFileNameRegex.IsMatch(parts[i])) {
                            result = false;
                            break;
                        }
                    }

                    if(result) {
                        result = !g_invalidFileNameRegex.IsMatch(parts[parts.Length - 1].Split(g_extensionSeparators, StringSplitOptions.RemoveEmptyEntries)[0]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Determines the difference between two path.
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns>If <paramref name="path1"/> is empty or <paramref name="path2"/> not starts with <paramref name="path1"/>, then return <paramref name="path2"/>; else return the relative path thats append to <paramref name="path1"/> yield <paramref name="path2"/>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="path1"/> or <paramref name="path2"/> is null or not a valid local path</exception>
        public static string MakeRelativePath(string path1, string path2) {
            if(path1 == null) {
                throw new ArgumentException("path1 is null.", "path1");
            }
            if(path2 == null) {
                throw new ArgumentException("path2 is null.", "path2");
            }

            if(path1.Length == 0) {
                return path2;
            }

            path1 = path1.ToLower().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            path2 = path2.ToLower().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            if(string.Equals(path1, path2)) {
                return string.Empty;
            }

            if(!path1.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                path1 += Path.DirectorySeparatorChar;
            }
            if(string.Equals(path1, path2 + Path.DirectorySeparatorChar)) {
                return string.Empty;
            }

            if(path2.StartsWith(path1)) {
                return path2.Remove(0, path1.Length);
            } else {
                return path2;
            }
        }
    }
}
