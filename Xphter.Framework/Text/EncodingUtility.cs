using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xphter.Framework.IO;

namespace Xphter.Framework.Text {
    /// <summary>
    /// Provides function to detect enconding of text files.
    /// </summary>
    public static class EncodingUtility {
        static EncodingUtility() {
            g_BOM = new byte[5][] {
                    //UTF-8 BOM
                    new byte[] {
                        0xEF,
                        0xBB,
                        0xBF,
                    },
                    //UTF-32 Little-Endian BOM
                    new byte[] {
                        0xFF,
                        0xFE,
                        0x00,
                        0x00,
                    },
                    //UTF-32 Big-Endian BOM
                    new byte[] {
                        0x00,
                        0x00,
                        0xFE,
                        0xFF,
                    },
                    //UTF-16 Little-Endian BOM
                    new byte[] {
                        0xFF,
                        0xFE,
                    },
                    //UTF-16 Big-Endian BOM
                    new byte[] {
                        0xFE,
                        0xFF,
                    },
                };
        }

        private static bool? g_isLittleEndian;
        /// <summary>
        /// Gets whether the byte order is little-endian pattern.
        /// </summary>
        public static unsafe bool IsLittleEndian {
            get {
                if(!g_isLittleEndian.HasValue) {
                    int i = 0x01;
                    g_isLittleEndian = (*(char*) &i == 0x01);
                }

                return g_isLittleEndian.Value;
            }
        }

        private static byte[][] g_BOM = null;
        /// <summary>
        /// Detect whether encoding of the specified file is UTF-8, UTF-16 or UTF-32.
        /// </summary>
        /// <param name="file">The path of file which will be detected.</param>
        /// <returns>Returns encoding of <paramref name="file"/> if encoding of it is unicode, otherwise return null.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="file"/> is null or empty or while-space.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="file"/> is not existing.</exception>
        public static Encoding DetectUnicodeEncoding(string file) {
            if(string.IsNullOrWhiteSpace(file)) {
                throw new ArgumentNullException("file");
            }
            if(!PathUtility.IsValidLocalPath(file)) {
                throw new ArgumentException("the path of file is not a valid local file path.", "file");
            }
            if(!File.Exists(file)) {
                throw new ArgumentException("file is not existing.", "file");
            }

            //read file header, four bytes
            byte[] data = new byte[4] {
                0x00,
                0x00,
                0x00,
                0x00,
            };
            using(FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                reader.Read(data, 0, data.Length);
            }

            return DetectUnicodeEncoding(data);
        }

        /// <summary>
        /// Detect whether encoding of the specified bytes data is UTF-8, UTF-16 or UTF-32.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="data"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="data"/> is too short.</exception>
        public static Encoding DetectUnicodeEncoding(byte[] data) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }
            if(data.Length < 4) {
                return null;
            }

            Encoding encoding = null;

            //compare BOM
            if(data[0] == g_BOM[0][0] &&
              data[1] == g_BOM[0][1] &&
              data[2] == g_BOM[0][2]) {
                encoding = Encoding.UTF8;
            } else if(data[0] == g_BOM[1][0] &&
              data[1] == g_BOM[1][1] &&
              data[2] == g_BOM[1][2] &&
              data[3] == g_BOM[1][3]) {
                encoding = Encoding.UTF32;
            } else if(data[0] == g_BOM[2][0] &&
              data[1] == g_BOM[2][1] &&
              data[2] == g_BOM[2][2] &&
              data[3] == g_BOM[2][3]) {
                encoding = new UTF32Encoding(true, true);
            } else if(data[0] == g_BOM[3][0] &&
              data[1] == g_BOM[3][1]) {
                encoding = Encoding.Unicode;
            } else if(data[0] == g_BOM[4][0] &&
              data[1] == g_BOM[4][1]) {
                encoding = new UnicodeEncoding(true, true);
            }

            return encoding;
        }
    }
}
