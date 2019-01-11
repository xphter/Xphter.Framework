using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xphter.Framework.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Represents a ICP beian number like: 粤B2-20090059、京ICP备11041704号、京ICP证000007号.
    /// </summary>
    public class IcpNumber {
        /// <summary>
        /// Initialize IcpNumber class.
        /// </summary>
        static IcpNumber() {
            g_provinces = "京津冀晋蒙辽吉黑沪申苏浙皖闽赣鲁豫鄂楚湘粤桂琼渝蜀川黔贵滇云藏陕秦陇甘青宁新港澳台";
            g_provincesPinyin = new Dictionary<string, string> { { "AO", "澳" }, { "CHUAN", "川" }, { "CHU", "楚" }, { "DIAN", "滇" }, { "E", "鄂" }, { "GANG", "港" }, { "GAN", "甘" }, { "GUI", "贵" }, { "HEI", "黑" }, { "HU", "沪" }, { "JING", "京" }, { "JIN", "晋" }, { "JI", "吉" }, { "LIAO", "辽" }, { "LONG", "陇" }, { "LU", "鲁" }, { "MENG", "蒙" }, { "MIN", "闽" }, { "NING", "宁" }, { "QIONG", "琼" }, { "QIAN", "黔" }, { "QING", "青" }, { "QIN", "秦" }, { "SHEN", "申" }, { "SHAN", "陕" }, { "SHU", "蜀" }, { "SU", "苏" }, { "TAI", "台" }, { "WAN", "皖" }, { "XIANG", "湘" }, { "XIN", "新" }, { "YUE", "粤" }, { "YUN", "云" }, { "YU", "渝" }, { "ZHE", "浙" } };
            g_othersPinyin = new Dictionary<string, string> { { "BEI", "备" }, { "HAO", "号" }, { "ZHENG", "证" } };

            g_a2Regex = new Regex(string.Format(@"^\s*([{0}]A2\-\d+)号?(\-\d+)?\s*$", g_provinces), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_bRegex = new Regex(string.Format(@"^\s*([{0}]B\-\d+\-\d+\-\d+)号?(\-\d+)?\s*$", g_provinces), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_b2Regex = new Regex(string.Format(@"^\s*([{0}]B2\-\d+)号?(\-\d+)?\s*$", g_provinces), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_b1b2Regex = new Regex(string.Format(@"^\s*([{0}]B1\.B2\-\d+)号?(\-\d+)?\s*$", g_provinces), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_icpBeiRegex = new Regex(string.Format(@"^\s*([{0}]ICP备\d+)号?(\-\d+)?\s*$", g_provinces), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            g_icpZhengRegex = new Regex(string.Format(@"^\s*([{0}]ICP证\d+)号?(\-\d+)?\s*$", g_provinces), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Initialize a instance of  class.
        /// </summary>
        /// <param name="icpNumberString"></param>
        public IcpNumber(string icpNumberString) {
            if(!IsWellFormedIcpNumberString(icpNumberString)) {
                throw new FormatException(string.Format("{0} is not a valid ICP number.", icpNumberString));
            }

            Match match = null;
            string subjectPostfix = null;
            Group subjectGroup = null, siteGroup = null;

            match = g_a2Regex.Match(icpNumberString);
            if(match == null || !match.Success) {
                match = g_bRegex.Match(icpNumberString);
                if(match == null || !match.Success) {
                    match = g_b2Regex.Match(icpNumberString);
                    if(match == null || !match.Success) {
                        match = g_b1b2Regex.Match(icpNumberString);
                        if(match == null || !match.Success) {
                            match = g_icpBeiRegex.Match(icpNumberString);
                            if(match == null || !match.Success) {
                                match = g_icpZhengRegex.Match(icpNumberString);
                                subjectPostfix = "号";
                            } else {
                                subjectPostfix = "号";
                            }
                        }
                    }
                }
            } else {
                subjectPostfix = "号";
            }

            subjectGroup = match.Groups[1];
            siteGroup = match.Groups[2];

            this.OriginalString = icpNumberString;
            this.NumberType = siteGroup.Success ? IcpNumberType.SiteNumber : IcpNumberType.SubjectNumber;
            this.SubjectNumber = subjectGroup.Value + subjectPostfix;
            this.SiteNumber = siteGroup.Success ? this.SubjectNumber + siteGroup.Value : null;
        }

        private static string g_provinces;
        private static IDictionary<string, string> g_provincesPinyin;
        private static IDictionary<string, string> g_othersPinyin;

        /// <summary>
        /// Like: 粤A2-20044005号(huawei.com)
        /// </summary>
        private static Regex g_a2Regex;

        /// <summary>
        /// Like: 辽B-2-4-20080039(vsread.com)
        /// </summary>
        private static Regex g_bRegex;

        /// <summary>
        /// Like: 浙B2-20080224(taobao.com)
        /// </summary>
        private static Regex g_b2Regex;

        /// <summary>
        /// Like: 皖B1.B2-20070020(218.cc)
        /// </summary>
        private static Regex g_b1b2Regex;

        /// <summary>
        /// Like: 京ICP备11041704号(jd.com)
        /// </summary>
        private static Regex g_icpBeiRegex;

        /// <summary>
        /// Like: 京ICP证030173号(baidu.com)
        /// </summary>
        private static Regex g_icpZhengRegex;

        /// <summary>
        /// Gets the original ICP number string that was passed to the IcpNumber constructor.
        /// </summary>
        public string OriginalString {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of this ICP number.
        /// </summary>
        public IcpNumberType NumberType {
            get;
            private set;
        }

        /// <summary>
        /// Gets the subject number in this ICP number.
        /// </summary>
        public string SubjectNumber {
            get;
            private set;
        }

        /// <summary>
        /// Gets the site number in this ICP number, return null if it only contains a subject number.
        /// </summary>
        public string SiteNumber {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a IcpNumber with the string.
        /// </summary>
        /// <param name="icpNumberString"></param>
        /// <returns></returns>
        public static bool IsWellFormedIcpNumberString(string icpNumberString) {
            return !string.IsNullOrWhiteSpace(icpNumberString) &&
                (g_a2Regex.IsMatch(icpNumberString) ||
                g_bRegex.IsMatch(icpNumberString) ||
                g_b2Regex.IsMatch(icpNumberString) ||
                g_b1b2Regex.IsMatch(icpNumberString) ||
                g_icpBeiRegex.IsMatch(icpNumberString) ||
                g_icpZhengRegex.IsMatch(icpNumberString));
        }

        /// <summary>
        /// Gets a normalize ICP number string from the specified pinyin string.
        /// </summary>
        /// <param name="pinyin"></param>
        /// <returns></returns>
        public static string FromPinyin(string pinyin) {
            if(pinyin == null) {
                throw new ArgumentNullException("pinyin");
            }

            if(string.IsNullOrWhiteSpace(pinyin = pinyin.Trim())) {
                return pinyin;
            }

            if(pinyin.StartsWith("CANG", StringComparison.OrdinalIgnoreCase) || pinyin.StartsWith("ZANG", StringComparison.OrdinalIgnoreCase)) {
                pinyin = "藏" + pinyin.Substring(4);
            } else {
                foreach(KeyValuePair<string, string> item in g_provincesPinyin) {
                    if(pinyin.StartsWith(item.Key, StringComparison.OrdinalIgnoreCase)) {
                        pinyin = item.Value + pinyin.Substring(item.Key.Length);
                        break;
                    }
                }
            }
            return pinyin.Replace(g_othersPinyin, true);
        }

        /// <summary>
        /// Transform a ICP number string to pinyin.
        /// </summary>
        /// <param name="icpNumberString"></param>
        /// <returns></returns>
        public static string ToPinyin(string icpNumberString) {
            if(icpNumberString == null) {
                throw new ArgumentNullException("icpNumberString");
            }

            if(string.IsNullOrWhiteSpace(icpNumberString = icpNumberString.Trim())) {
                return icpNumberString;
            }

            if(icpNumberString.StartsWith("藏", StringComparison.OrdinalIgnoreCase)) {
                return "ZANG" + ChineseUtility.GetPinyin(icpNumberString.Substring(1)).ToUpper();
            } else {
                return ChineseUtility.GetPinyin(icpNumberString).ToUpper();
            }
        }
    }

    /// <summary>
    /// Represents all types of ICP number.
    /// </summary>
    public enum IcpNumberType {
        [Description("主体备案号")]
        SubjectNumber,

        [Description("网站备案号")]
        SiteNumber,
    }
}
