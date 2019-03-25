using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xphter.Framework.Collections;

namespace Xphter.Framework {
    /// <summary>
    /// 提供操作整数的工具方法。
    /// </summary>
    public static class IntegerUtility {
        private static string g_uppercaseChineseNumbers = "零壹贰叁肆伍陆柒捌玖";
        private static string[] g_uppercaseChinesePlaces = new string[] {
            "","拾","佰","仟",
            "万","拾","佰","仟",
            "亿","拾","佰","仟",
            "万","拾","佰","仟", /* 没有使用兆，而是使用更通俗叫法，例如：叁拾万亿 */
        };

        /// <summary>
        /// Converts the character to a integer.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static int GetDigitFromChar(char c) {
            int digit = 0;

            switch(c) {
                case '0':
                    digit = 0;
                    break;
                case '1':
                    digit = 1;
                    break;
                case '2':
                    digit = 2;
                    break;
                case '3':
                    digit = 3;
                    break;
                case '4':
                    digit = 4;
                    break;
                case '5':
                    digit = 5;
                    break;
                case '6':
                    digit = 6;
                    break;
                case '7':
                    digit = 7;
                    break;
                case '8':
                    digit = 8;
                    break;
                case '9':
                    digit = 9;
                    break;
            }

            return digit;
        }

        private static string ToUppercaseChinese(string text) {
            string sign = string.Empty;
            if(text.StartsWith("-")) {
                sign = "负";
                text = text.Substring(1);
            }
            if(text.Length > g_uppercaseChinesePlaces.Length) {
                throw new ArgumentOutOfRangeException("text", string.Format("{0} is too big.", text));
            }

            text = Regex.Replace(text, @"^0+", string.Empty);
            if(text.Length == 0) {
                return sign + g_uppercaseChineseNumbers[0].ToString();
            }

            string content = null;
            int d0 = 0, d1 = 0, d2 = 0, d3 = 0;
            ICollection<string> contents = new List<string>();

            int[] digits = text.Reverse().Select((item) => GetDigitFromChar(item)).ToArray();
            for(int i = 0; i < digits.Length; i += 4) {
                d0 = digits[i];
                d1 = i + 1 < digits.Length ? digits[i + 1] : 0;
                d2 = i + 2 < digits.Length ? digits[i + 2] : 0;
                d3 = i + 3 < digits.Length ? digits[i + 3] : 0;
                if(d0 == 0 && d1 == 0 && d2 == 0 && d3 == 0) {
                    continue;
                }

                if(d0 != 0) {
                    content = g_uppercaseChineseNumbers[d0] + g_uppercaseChinesePlaces[i];
                } else {
                    content = g_uppercaseChinesePlaces[i];
                }
                if(i + 1 < digits.Length) {
                    if(d1 != 0) {
                        content = g_uppercaseChineseNumbers[d1] + g_uppercaseChinesePlaces[i + 1] + content;
                    } else if(d0 != 0 && d2 != 0) {
                        content = g_uppercaseChineseNumbers[0] + content;
                    }

                    if(i + 2 < digits.Length) {
                        if(d2 != 0) {
                            content = g_uppercaseChineseNumbers[d2] + g_uppercaseChinesePlaces[i + 2] + content;
                        } else if(d0 != 0 || d1 != 0) {
                            content = g_uppercaseChineseNumbers[0] + content;
                        }

                        if(i + 3 < digits.Length) {
                            if(d3 != 0) {
                                content = g_uppercaseChineseNumbers[d3] + g_uppercaseChinesePlaces[i + 3] + content;
                            }
                        }
                    }
                }

                contents.Add(content);
            }

            return sign + contents.Reverse().StringJoin(string.Empty);
        }

        /// <summary>
        /// 获取<paramref name="input"/>二进制表示形式中，去掉前导零后所占的位数。
        /// </summary>
        /// <param name="input">要获取位数的整数</param>
        /// <param name="size">整数类型的字节大小</param>
        /// <returns><paramref name="input"/>二进制表示形式中，去掉前导零后的位数。</returns>
        private static int GetBitCount(ulong input, int size) {
            if(input == 0x00) {
                return 0;
            }

            int zeroCount = 0;

            input <<= (sizeof(ulong) - size) * 8;
            while((input & 0x8000000000000000) == 0x00) {
                ++zeroCount;
                input <<= 1;
            }

            return size * 8 - zeroCount;
        }

        /// <summary>
        /// 获取<paramref name="input"/>二进制表示形式中，去掉前导零后所占的位数。
        /// </summary>
        /// <param name="input">要获取位数的字节</param>
        /// <returns><paramref name="input"/>二进制表示形式中，去掉前导零后的位数。</returns>
        public static int GetBitCount(this byte input) {
            return GetBitCount((ulong) input, sizeof(byte));
        }

        /// <summary>
        /// 获取<paramref name="input"/>二进制表示形式中，去掉前导零后所占的位数。
        /// </summary>
        /// <param name="input">要获取位数的无符号短整形</param>
        /// <returns><paramref name="input"/>二进制表示形式中，去掉前导零后的位数。</returns>
        public static int GetBitCount(this ushort input) {
            return GetBitCount((ulong) input, sizeof(ushort));
        }

        /// <summary>
        /// 获取<paramref name="input"/>二进制表示形式中，去掉前导零后所占的位数。
        /// </summary>
        /// <param name="input">要获取位数的无符号整形</param>
        /// <returns><paramref name="input"/>二进制表示形式中，去掉前导零后的位数。</returns>
        public static int GetBitCount(this uint input) {
            return GetBitCount((ulong) input, sizeof(uint));
        }

        /// <summary>
        /// 获取<paramref name="input"/>二进制表示形式中，去掉前导零后的位数。
        /// </summary>
        /// <param name="input">要获取位数的无符号长整形</param>
        /// <returns><paramref name="input"/>二进制表示形式中，去掉前导零后的位数。</returns>
        public static int GetBitCount(this ulong input) {
            return GetBitCount((ulong) input, sizeof(ulong));
        }

        /// <summary>
        /// Converts a Int16 into the bytes array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="buffer"/> is too small.</exception>
        public static void ToBytes(this short value, byte[] buffer, int offset) {
            if(buffer == null) {
                throw new ArgumentNullException("buffer");
            }
            if(offset + sizeof(short) > buffer.Length) {
                throw new ArgumentOutOfRangeException("buffer");
            }

            if(BitConverter.IsLittleEndian) {
                for(var i = 0; i < sizeof(short); i++) {
                    buffer[offset + i] = (byte) (value >> 8 * i);
                }
            } else {
                for(var i = sizeof(short) - 1; i >= 0; i--) {
                    buffer[offset + sizeof(short) - 1 - i] = (byte) (value >> 8 * i);
                }
            }
        }

        /// <summary>
        /// Converts a Int32 into the bytes array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="buffer"/> is too small.</exception>
        public static void ToBytes(this int value, byte[] buffer, int offset) {
            if(buffer == null) {
                throw new ArgumentNullException("buffer");
            }
            if(offset + sizeof(int) > buffer.Length) {
                throw new ArgumentOutOfRangeException("buffer");
            }

            if(BitConverter.IsLittleEndian) {
                for(var i = 0; i < sizeof(int); i++) {
                    buffer[offset + i] = (byte) (value >> 8 * i);
                }
            } else {
                for(var i = sizeof(int) - 1; i >= 0; i--) {
                    buffer[offset + sizeof(int) - 1 - i] = (byte) (value >> 8 * i);
                }
            }
        }

        /// <summary>
        /// Converts a Int64 into the bytes array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="buffer"/> is too small.</exception>
        public static void ToBytes(this long value, byte[] buffer, int offset) {
            if(buffer == null) {
                throw new ArgumentNullException("buffer");
            }
            if(offset + sizeof(long) > buffer.Length) {
                throw new ArgumentOutOfRangeException("buffer");
            }

            if(BitConverter.IsLittleEndian) {
                for(var i = 0; i < sizeof(long); i++) {
                    buffer[offset + i] = (byte) (value >> 8 * i);
                }
            } else {
                for(var i = sizeof(long) - 1; i >= 0; i--) {
                    buffer[offset + sizeof(long) - 1 - i] = (byte) (value >> 8 * i);
                }
            }
        }

        /// <summary>
        /// Converts a UInt16 into the bytes array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="buffer"/> is too small.</exception>
        public static void ToBytes(this ushort value, byte[] buffer, int offset) {
            ToBytes((short) value, buffer, offset);
        }

        /// <summary>
        /// Converts a UInt32 into the bytes array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="buffer"/> is too small.</exception>
        public static void ToBytes(this uint value, byte[] buffer, int offset) {
            ToBytes((int) value, buffer, offset);
        }

        /// <summary>
        /// Converts a UInt64 into the bytes array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="buffer"/> is too small.</exception>
        public static void ToBytes(this ulong value, byte[] buffer, int offset) {
            ToBytes((long) value, buffer, offset);
        }

        /// <summary>
        /// Converts a part of byte array from host byte order to network byte order.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="offset"/> is less than zero or <paramref name="length"/> is less than zero or <paramref name="buffer"/> is too small.</exception>        
        public static void HostToNetworkOrder(byte[] buffer, int offset, int length) {
            if(buffer == null) {
                throw new ArgumentNullException("buffer");
            }
            if(offset < 0) {
                throw new ArgumentOutOfRangeException("offset");
            }
            if(length < 0) {
                throw new ArgumentOutOfRangeException("length");
            }
            if(offset + length > buffer.Length) {
                throw new ArgumentOutOfRangeException("length", "buffer is too small.");
            }
            if(length < 2 || !BitConverter.IsLittleEndian) {
                return;
            }

            Array.Reverse(buffer, offset, length);
        }

        /// <summary>
        /// Converts a part of byte array from host byte order to network byte order.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="offset"/> is less than zero or <paramref name="length"/> is less than zero or <paramref name="buffer"/> is too small.</exception>        
        public static void NetworkToHostOrder(byte[] buffer, int offset, int length) {
            HostToNetworkOrder(buffer, offset, length);
        }

        /// <summary>
        /// Gets a Int32 from a bytes array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="buffer"/> is too small.</exception>
        public static int Int32FromBytes(byte[] buffer) {
            return Int32FromBytes(buffer, 0);
        }

        /// <summary>
        /// Gets a Int32 from a bytes array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <exception cref="System.ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="buffer"/> is too small.</exception>
        public static int Int32FromBytes(byte[] buffer, int offset) {
            if(buffer == null) {
                throw new ArgumentNullException("buffer");
            }
            if(offset + 4 > buffer.Length) {
                throw new ArgumentOutOfRangeException("buffer");
            }

            return ((int) buffer[offset]) |
                ((int) buffer[offset + 1]) << 8 |
                ((int) buffer[offset + 2]) << 16 |
                ((int) buffer[offset + 3]) << 24;
        }

        /// <summary>
        /// Converts the specified integer to uppercase chinese digit.
        /// </summary>
        /// <param name="value">Must be between 0 and 9</param>
        /// <returns></returns>
        public static char ToUppercaseChineseDigit(this int value) {
            if(value < 0 || value > 9) {
                throw new ArgumentOutOfRangeException("value", "value must be between 0 and 9.");
            }

            return g_uppercaseChineseNumbers[value];
        }

        /// <summary>
        /// Converts the specified integer to uppercase chinese numbers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUppercaseChineseNumber(this int value) {
            return ToUppercaseChinese(value.ToString());
        }

        /// <summary>
        /// Converts the specified long-integer to uppercase chinese numbers.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToUppercaseChineseNumber(this long value) {
            return ToUppercaseChinese(value.ToString());
        }

        /// <summary>
        /// Gets the number of integer places of the speicifed Int32 value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetIntegerPlaces(this int value) {
            if(value == 0) {
                return 1;
            }

            return (int) Math.Floor(Math.Log10(Math.Abs(value)) + 1);
        }

        /// <summary>
        /// Gets the number of integer places of the speicifed Int64 value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int GetIntegerPlaces(this long value) {
            if(value == 0L) {
                return 1;
            }

            return (int) Math.Floor(Math.Log10(Math.Abs(value)) + 1);
        }
    }
}
