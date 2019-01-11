using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xphter.Framework.Collections;

namespace Xphter.Framework {
    /// <summary>
    /// Represents a domain name.
    /// </summary>
    public class DomainName {
        /// <summary>
        /// Initialize DomainName class.
        /// </summary>
        static DomainName() {
            g_regex = new Regex(
                string.Format(@"^{0}+(?:[\.。]{0}+)+$", @"[\u4E00-\u9FA5a-zA-z0-9\-]"),
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string[] emptyStringArray = new string[0];
            g_gTLDs = new string[] {
                "arpa", "com", "info", "net", "org", "biz", "name", "pro",
                "aero", "cat", "coop", "edu", "gov", "int", "jobs", "club",
                "mil", "mobi", "museum", "travel", "asia", "tel", "catholic",
                "post", "xxx", "中国", "中國", "网络", "wang", "公司", "香港","台湾", "台灣" };
            g_ccTLDs = new Dictionary<string, string[]> {
                { /* 中国 */ "cn" , new string[] {
                    "bj", "fj", "sh", "jx", "tj", "sd", "cq", "ha", "he",
                    "hb", "sx", "hn", "nm", "gd", "ln", "gx", "jl", "hi",
                    "hl", "sc", "js", "gz", "zj", "yn", "ah", "xz", "sn",
                    "tw", "gs", "hk", "qh", "mo", "nx", "xj" } },

                { /* 阿森松岛 */ "ac" , emptyStringArray },
                { /* 安道尔 */ "ad" , emptyStringArray },
                { /* 阿拉伯联合酋长国 */ "ae" , emptyStringArray },
                { /* 阿富汗 */ "af" , emptyStringArray },
                { /* 安提瓜和巴布达 */ "ag" , emptyStringArray },
                { /* 安圭拉 */ "ai" , emptyStringArray },
                { /* 阿尔巴尼亚 */ "al" , emptyStringArray },
                { /* 亚美尼亚 */ "am" , emptyStringArray },
                { /* 荷属安的列斯群岛 */ "an" , emptyStringArray },
                { /* 安哥拉 */ "ao" , emptyStringArray },
                { /* 南极洲 */ "aq" , emptyStringArray },
                { /* 阿根廷 */ "ar" , emptyStringArray },
                { /* 美属萨摩亚 */ "as" , emptyStringArray },
                { /* 奥地利 */ "at" , emptyStringArray },
                { /* 澳大利亚 */ "au" , emptyStringArray },
                { /* 阿鲁巴 */ "aw" , emptyStringArray },
                { /* 阿塞拜疆 */ "az" , emptyStringArray },
                { /* 波斯尼亚和黑塞哥维那 */ "ba" , emptyStringArray },
                { /* 巴巴多斯 */ "bb" , emptyStringArray },
                { /* 孟加拉国 */ "bd" , emptyStringArray },
                { /* 比利时 */ "be" , emptyStringArray },
                { /* 布基纳法索 */ "bf" , emptyStringArray },
                { /* 保加利亚 */ "bg" , emptyStringArray },
                { /* 巴林 */ "bh" , emptyStringArray },
                { /* 布隆迪 */ "bi" , emptyStringArray },
                { /* 贝宁 */ "bj" , emptyStringArray },
                { /* 百慕大 */ "bm" , emptyStringArray },
                { /* 文莱 */ "bn" , emptyStringArray },
                { /* 玻利维亚 */ "bo" , emptyStringArray },
                { /* 巴西 */ "br" , emptyStringArray },
                { /* 巴哈马 */ "bs" , emptyStringArray },
                { /* 不丹 */ "bt" , emptyStringArray },
                { /* 布维岛 */ "bv" , emptyStringArray },
                { /* 博茨瓦纳 */ "bw" , emptyStringArray },
                { /* 白俄罗斯 */ "by" , emptyStringArray },
                { /* 伯利兹 */ "bz" , emptyStringArray },
                { /* 加拿大 */ "ca" , emptyStringArray },
                { /* 可可群岛 */ "cc" , emptyStringArray },
                { /* 刚果民主共和国 */ "cd" , emptyStringArray },
                { /* 中非共和国 */ "cf" , emptyStringArray },
                { /* 刚果 */ "cg" , emptyStringArray },
                { /* 瑞士 */ "ch" , emptyStringArray },
                { /* 科特迪瓦 */ "ci" , emptyStringArray },
                { /* 库克群岛 */ "ck" , emptyStringArray },
                { /* 智利 */ "cl" , emptyStringArray },
                { /* 喀麦隆 */ "cm" , emptyStringArray },
                { /* 哥伦比亚 */ "co" , emptyStringArray },
                { /* 哥斯达黎加 */ "cr" , emptyStringArray },
                { /* 古巴 */ "cu" , emptyStringArray },
                { /* 佛得角 */ "cv" , emptyStringArray },
                { /* 圣诞岛 */ "cx" , emptyStringArray },
                { /* 塞浦路斯 */ "cy" , emptyStringArray },
                { /* 捷克共和国 */ "cz" , emptyStringArray },
                { /* 德国 */ "de" , emptyStringArray },
                { /* 吉布提 */ "dj" , emptyStringArray },
                { /* 丹麦 */ "dk" , emptyStringArray },
                { /* 多米尼克 */ "dm" , emptyStringArray },
                { /* 多米尼加共和国 */ "do" , emptyStringArray },
                { /* 阿尔及利亚 */ "dz" , emptyStringArray },
                { /* 厄瓜多尔 */ "ec" , emptyStringArray },
                { /* 爱沙尼亚 */ "ee" , emptyStringArray },
                { /* 埃及 */ "eg" , emptyStringArray },
                { /* 西撒哈拉 */ "eh" , emptyStringArray },
                { /* 厄立特里亚 */ "er" , emptyStringArray },
                { /* 西班牙 */ "es" , emptyStringArray },
                { /* 埃塞俄比亚 */ "et" , emptyStringArray },
                { /* 欧洲联盟 */ "eu" , emptyStringArray },
                { /* 芬兰 */ "fi" , emptyStringArray },
                { /* 斐济 */ "fj" , emptyStringArray },
                { /* 福克兰群岛 */ "fk" , emptyStringArray },
                { /* 密克罗尼西亚联邦 */ "fm" , emptyStringArray },
                { /* 法罗群岛 */ "fo" , emptyStringArray },
                { /* 法国 */ "fr" , emptyStringArray },
                { /* 加蓬 */ "ga" , emptyStringArray },
                { /* 格林纳达 */ "gd" , emptyStringArray },
                { /* 格鲁吉亚 */ "ge" , emptyStringArray },
                { /* 法属圭亚那 */ "gf" , emptyStringArray },
                { /* 格恩西岛 */ "gg" , emptyStringArray },
                { /* 加纳 */ "gh" , emptyStringArray },
                { /* 直布罗陀 */ "gi" , emptyStringArray },
                { /* 格陵兰 */ "gl" , emptyStringArray },
                { /* 冈比亚 */ "gm" , emptyStringArray },
                { /* 几内亚 */ "gn" , emptyStringArray },
                { /* 瓜德罗普 */ "gp" , emptyStringArray },
                { /* 赤道几内亚 */ "gq" , emptyStringArray },
                { /* 希腊 */ "gr" , emptyStringArray },
                { /* 南乔治亚岛和南桑德韦奇岛 */ "gs" , emptyStringArray },
                { /* 危地马拉 */ "gt" , emptyStringArray },
                { /* 关岛 */ "gu" , emptyStringArray },
                { /* 几内亚比绍 */ "gw" , emptyStringArray },
                { /* 圭亚那 */ "gy" , emptyStringArray },
                { /* 香港 */ "hk" , emptyStringArray },
                { /* 赫德和麦克唐纳群岛 */ "hm" , emptyStringArray },
                { /* 洪都拉斯 */ "hn" , emptyStringArray },
                { /* 克罗地亚 */ "hr" , emptyStringArray },
                { /* 海地 */ "ht" , emptyStringArray },
                { /* 匈牙利 */ "hu" , emptyStringArray },
                { /* 印度尼西亚 */ "id" , emptyStringArray },
                { /* 爱尔兰 */ "ie" , emptyStringArray },
                { /* 以色列 */ "il" , emptyStringArray },
                { /* 马恩岛 */ "im" , emptyStringArray },
                { /* 印度 */ "in" , emptyStringArray },
                { /* 英属印度洋地区 */ "io" , emptyStringArray },
                { /* 伊拉克 */ "iq" , emptyStringArray },
                { /* 伊朗 */ "ir" , emptyStringArray },
                { /* 冰岛 */ "is" , emptyStringArray },
                { /* 意大利 */ "it" , emptyStringArray },
                { /* 泽西岛 */ "je" , emptyStringArray },
                { /* 牙买加 */ "jm" , emptyStringArray },
                { /* 约旦 */ "jo" , emptyStringArray },
                { /* 日本 */ "jp" , emptyStringArray },
                { /* 肯尼亚 */ "ke" , emptyStringArray },
                { /* 吉尔吉斯斯坦 */ "kg" , emptyStringArray },
                { /* 柬埔寨 */ "kh" , emptyStringArray },
                { /* 基里巴斯 */ "ki" , emptyStringArray },
                { /* 科摩罗 */ "km" , emptyStringArray },
                { /* 圣基茨和尼维斯 */ "kn" , emptyStringArray },
                { /* 朝鲜 */ "kp" , emptyStringArray },
                { /* 韩国 */ "kr" , emptyStringArray },
                { /* 科威特 */ "kw" , emptyStringArray },
                { /* 开曼群岛 */ "ky" , emptyStringArray },
                { /* 哈萨克斯坦 */ "kz" , emptyStringArray },
                { /* 老挝 */ "la" , emptyStringArray },
                { /* 黎巴嫩 */ "lb" , emptyStringArray },
                { /* 圣卢西亚 */ "lc" , emptyStringArray },
                { /* 列支敦士登 */ "li" , emptyStringArray },
                { /* 斯里兰卡 */ "lk" , emptyStringArray },
                { /* 利比里亚 */ "lr" , emptyStringArray },
                { /* 莱索托 */ "ls" , emptyStringArray },
                { /* 立陶宛 */ "lt" , emptyStringArray },
                { /* 卢森堡 */ "lu" , emptyStringArray },
                { /* 拉脱维亚 */ "lv" , emptyStringArray },
                { /* 利比亚 */ "ly" , emptyStringArray },
                { /* 摩洛哥 */ "ma" , emptyStringArray },
                { /* 摩纳哥 */ "mc" , emptyStringArray },
                { /* 摩尔多瓦 */ "md" , emptyStringArray },
                { /* 黑山 */ "me" , emptyStringArray },
                { /* 马达加斯加 */ "mg" , emptyStringArray },
                { /* 马绍尔群岛 */ "mh" , emptyStringArray },
                { /* 马其顿 */ "mk" , emptyStringArray },
                { /* 马里 */ "ml" , emptyStringArray },
                { /* 缅甸 */ "mm" , emptyStringArray },
                { /* 蒙古 */ "mn" , emptyStringArray },
                { /* 澳门 */ "mo" , emptyStringArray },
                { /* 北马里亚纳群岛 */ "mp" , emptyStringArray },
                { /* 马提尼克岛 */ "mq" , emptyStringArray },
                { /* 毛里塔尼亚 */ "mr" , emptyStringArray },
                { /* 蒙特塞拉特岛 */ "ms" , emptyStringArray },
                { /* 马耳他 */ "mt" , emptyStringArray },
                { /* 毛里求斯 */ "mu" , emptyStringArray },
                { /* 马尔代夫 */ "mv" , emptyStringArray },
                { /* 马拉维 */ "mw" , emptyStringArray },
                { /* 墨西哥 */ "mx" , emptyStringArray },
                { /* 马来西亚 */ "my" , emptyStringArray },
                { /* 莫桑比克 */ "mz" , emptyStringArray },
                { /* 纳米比亚 */ "na" , emptyStringArray },
                { /* 新喀里多尼亚 */ "nc" , emptyStringArray },
                { /* 尼日尔 */ "ne" , emptyStringArray },
                { /* 诺福克岛 */ "nf" , emptyStringArray },
                { /* 尼日利亚 */ "ng" , emptyStringArray },
                { /* 尼加拉瓜 */ "ni" , emptyStringArray },
                { /* 荷兰 */ "nl" , emptyStringArray },
                { /* 挪威 */ "no" , emptyStringArray },
                { /* 尼泊尔 */ "np" , emptyStringArray },
                { /* 瑙鲁 */ "nr" , emptyStringArray },
                { /* 纽埃岛 */ "nu" , emptyStringArray },
                { /* 新西兰 */ "nz" , emptyStringArray },
                { /* 阿曼 */ "om" , emptyStringArray },
                { /* 巴拿马 */ "pa" , emptyStringArray },
                { /* 秘鲁 */ "pe" , emptyStringArray },
                { /* 法属波利尼西亚 */ "pf" , emptyStringArray },
                { /* 巴布亚新几内亚 */ "pg" , emptyStringArray },
                { /* 菲律宾 */ "ph" , emptyStringArray },
                { /* 巴基斯坦 */ "pk" , emptyStringArray },
                { /* 波兰 */ "pl" , emptyStringArray },
                { /* 圣皮埃尔岛及密客隆岛 */ "pm" , emptyStringArray },
                { /* 皮特凯恩群岛 */ "pn" , emptyStringArray },
                { /* 波多黎各 */ "pr" , emptyStringArray },
                { /* 巴勒斯坦 */ "ps" , emptyStringArray },
                { /* 葡萄牙 */ "pt" , emptyStringArray },
                { /* 帕劳 */ "pw" , emptyStringArray },
                { /* 巴拉圭 */ "py" , emptyStringArray },
                { /* 卡塔尔 */ "qa" , emptyStringArray },
                { /* 留尼汪 */ "re" , emptyStringArray },
                { /* 罗马尼亚 */ "ro" , emptyStringArray },
                { /* 俄罗斯 */ "ru" , emptyStringArray },
                { /* 卢旺达 */ "rw" , emptyStringArray },
                { /* 沙特阿拉伯 */ "sa" , emptyStringArray },
                { /* 所罗门群岛 */ "sb" , emptyStringArray },
                { /* 塞舌尔 */ "sc" , emptyStringArray },
                { /* 苏丹 */ "sd" , emptyStringArray },
                { /* 瑞典 */ "se" , emptyStringArray },
                { /* 新加坡 */ "sg" , emptyStringArray },
                { /* 圣赫勒拿岛 */ "sh" , emptyStringArray },
                { /* 斯洛文尼亚 */ "si" , emptyStringArray },
                { /* 斯瓦尔巴岛和扬马延岛 */ "sj" , emptyStringArray },
                { /* 斯洛伐克 */ "sk" , emptyStringArray },
                { /* 塞拉利昂 */ "sl" , emptyStringArray },
                { /* 圣马力诺 */ "sm" , emptyStringArray },
                { /* 塞内加尔 */ "sn" , emptyStringArray },
                { /* 索马里 */ "so" , emptyStringArray },
                { /* 苏里南 */ "sr" , emptyStringArray },
                { /* 南苏丹（预计） */ "ss" , emptyStringArray },
                { /* 圣多美和普林西比 */ "st" , emptyStringArray },
                { /* 萨尔瓦多 */ "sv" , emptyStringArray },
                { /* 叙利亚 */ "sy" , emptyStringArray },
                { /* 斯威士兰 */ "sz" , emptyStringArray },
                { /* 特克斯和凯科斯群岛 */ "tc" , emptyStringArray },
                { /* 乍得 */ "td" , emptyStringArray },
                { /* 法属南部领土 */ "tf" , emptyStringArray },
                { /* 多哥 */ "tg" , emptyStringArray },
                { /* 泰国 */ "th" , emptyStringArray },
                { /* 塔吉克斯坦 */ "tj" , emptyStringArray },
                { /* 托克劳 */ "tk" , emptyStringArray },
                { /* 东帝汶（新域名） */ "tl" , emptyStringArray },
                { /* 土库曼斯坦 */ "tm" , emptyStringArray },
                { /* 突尼斯 */ "tn" , emptyStringArray },
                { /* 汤加 */ "to" , emptyStringArray },
                { /* 东帝汶（旧域名，尚未停用） */ "tp" , emptyStringArray },
                { /* 土耳其 */ "tr" , emptyStringArray },
                { /* 特立尼达和多巴哥 */ "tt" , emptyStringArray },
                { /* 图瓦卢 */ "tv" , emptyStringArray },
                { /* 台湾 */ "tw" , emptyStringArray },
                { /* 坦桑尼亚 */ "tz" , emptyStringArray },
                { /* 乌克兰 */ "ua" , emptyStringArray },
                { /* 乌干达 */ "ug" , emptyStringArray },
                { /* 英国 */ "uk" , emptyStringArray },
                { /* 美国本土外小岛屿 */ "um" , emptyStringArray },
                { /* 美国 */ "us" , emptyStringArray },
                { /* 乌拉圭 */ "uy" , emptyStringArray },
                { /* 乌兹别克斯坦 */ "uz" , emptyStringArray },
                { /* 梵蒂冈 */ "va" , emptyStringArray },
                { /* 圣文森特和格林纳丁斯 */ "vc" , emptyStringArray },
                { /* 委内瑞拉 */ "ve" , emptyStringArray },
                { /* 英属维尔京群岛 */ "vg" , emptyStringArray },
                { /* 美属维尔京群岛 */ "vi" , emptyStringArray },
                { /* 越南 */ "vn" , emptyStringArray },
                { /* 瓦努阿图 */ "vu" , emptyStringArray },
                { /* 瓦利斯和富图纳群岛 */ "wf" , emptyStringArray },
                { /* 萨摩亚 */ "ws" , emptyStringArray },
                { /* 也门 */ "ye" , emptyStringArray },
                { /* 马约特岛 */ "yt" , emptyStringArray },
                { /* 塞尔维亚和黑山 */ "yu" , emptyStringArray },
                { /* 耶纽 */ "yr" , emptyStringArray },
                { /* 南非 */ "za" , emptyStringArray },
                { /* 赞比亚 */ "zm" , emptyStringArray },
                { /* 津巴布韦 */ "zw" , emptyStringArray },
            };
        }

        /// <summary>
        /// Initialize a instance of DomainName class.
        /// </summary>
        /// <param name="domainNameString"></param>
        public DomainName(string domainNameString) {
            if(!IsWellFormedDomainNameString(domainNameString)) {
                throw new FormatException(string.Format("{0} is not a valid domain name.", domainNameString));
            }

            this.OriginalString = domainNameString;
            domainNameString = domainNameString.ToLower().Trim();
            if(Uri.IsWellFormedUriString(domainNameString, UriKind.Absolute)) {
                domainNameString = new Uri(domainNameString, UriKind.Absolute).Host;
            }
            string[] segments = domainNameString.Split('.', '。');

            string www = "www";
            switch(segments.Length) {
                case 2:
                    this.m_segments = segments;
                    break;
                case 3:
                    if(www.Equals(segments[0])) {
                        // like: www.a.com
                        this.m_segments = segments;
                    } else if(g_gTLDs.Contains(segments[2])) {
                        // like: a.b.com
                        this.m_segments = segments;
                    } else if(g_ccTLDs.Keys.Contains(segments[2]) &&
                        (g_gTLDs.Contains(segments[1]) ||
                        g_ccTLDs[segments[2]].Contains(segments[1]) ||
                        g_ccTLDs.Keys.Contains(segments[1]))) {
                        // like: a.edu.cn, a.ha.cn, a.co.uk
                        this.m_segments = new string[] {
                                segments[0],
                                string.Format("{0}.{1}", segments[1], segments[2]),
                        };
                    } else {
                        // other cases
                        this.m_segments = segments;
                    }
                    break;
                default:
                    if(g_ccTLDs.Keys.Contains(segments[segments.Length - 1]) &&
                        (g_gTLDs.Contains(segments[segments.Length - 2]) ||
                        g_ccTLDs[segments[segments.Length - 1]].Contains(segments[segments.Length - 2]) ||
                        g_ccTLDs.Keys.Contains(segments[segments.Length - 2]))) {
                        // like: a.b.edu.cn, www.b.ha.cn, www.c.co.uk
                        this.m_segments = new string[segments.Length - 1];
                        Array.Copy(segments, this.m_segments, this.m_segments.Length - 1);
                        this.m_segments[this.m_segments.Length - 1] = string.Format("{0}.{1}", segments[segments.Length - 2], segments[segments.Length - 1]);
                    } else {
                        // other cases
                        this.m_segments = segments;
                    }
                    break;
            }
        }

        /// <summary>
        /// The regular expression to analyze domian name string.
        /// </summary>
        private static Regex g_regex;

        /// <summary>
        /// Generic top-level domains.
        /// </summary>
        private static string[] g_gTLDs;

        /// <summary>
        /// Country code top-level domains, key is national domain, value is area domains.
        /// </summary>
        private static IDictionary<string, string[]> g_ccTLDs;

        /// <summary>
        /// The segements of this domain name.
        /// </summary>
        private string[] m_segments;

        /// <summary>
        /// Gets the original domain name number string that was passed to the DomainName constructor.
        /// </summary>
        public string OriginalString {
            get;
            private set;
        }

        /// <summary>
        /// Gets level of this domain name.
        /// </summary>
        public int Level {
            get {
                return this.m_segments.Length - 1;
            }
        }

        /// <summary>
        /// Gets the segement of the specified level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="level"/> is less than zero or out of segments range.</exception>
        public string GetSegment(int level) {
            if(level < 0 || level >= this.m_segments.Length) {
                throw new ArgumentOutOfRangeException("level");
            }

            return this.m_segments[this.m_segments.Length - 1 - level];
        }

        /// <summary>
        /// Gets right part of this domain name from the specified level.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="level"/> is less than zero or out of segments range.</exception>
        public string GetRightPart(int level) {
            if(level < 0 || level >= this.m_segments.Length) {
                throw new ArgumentOutOfRangeException("level");
            }

            return this.m_segments.Where((item, index) => index >= this.m_segments.Length - 1 - level).StringJoin('.');
        }

        /// <summary>
        /// Indicates whether the string is well-formed by attempting to construct a DomainName with the string.
        /// </summary>
        /// <param name="domainNameString"></param>
        /// <returns></returns>
        public static bool IsWellFormedDomainNameString(string domainNameString) {
            if(string.IsNullOrWhiteSpace(domainNameString)) {
                return false;
            }
            domainNameString = domainNameString.Trim();
            if(Uri.IsWellFormedUriString(domainNameString, UriKind.Absolute) &&
                new Uri(domainNameString, UriKind.Absolute).HostNameType == UriHostNameType.Dns) {
                return true;
            }
            if(g_regex.IsMatch(domainNameString)) {
                return true;
            }
            return false;
        }
    }
}
