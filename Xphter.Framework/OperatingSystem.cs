using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xphter.Framework {
    /// <summary>
    /// Provides the information of operating system.
    /// </summary>
    public static class OperatingSystem {
        #region OS Names

        /// <summary>
        /// Represents the names of common operating systems.
        /// </summary>
        public static class Names {
            public const string Windows = "windows";
            public const string Linux = "linux";
            public const string Unix = "unix";
            public const string Mac = "mac";
            public const string Android = "android";
            public const string Ios = "ios";
            public const string WindowsPhone = "windows phone";
            public const string Blankberry = "blankberry";
        }

        #endregion

        #region Windows Version

        /// <summary>
        /// Gets whether current operating system is windows 95.
        /// </summary>
        public static bool IsWindows95 {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows 95[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows 98.
        /// </summary>
        public static bool IsWindows98 {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows 98[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows me.
        /// </summary>
        public static bool IsWindowsMe {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows Me[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows NT workstation.
        /// </summary>
        public static bool IsWindowsNTWorkstation {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows NT Workstation[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows 2000 professional.
        /// </summary>
        public static bool IsWindows2000Professional {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows 2000 professional[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows XP.
        /// </summary>
        public static bool IsWindowsXP {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows XP[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows vista.
        /// </summary>
        public static bool IsWindowsVista {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows Vista[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows 7.
        /// </summary>
        public static bool IsWindows7 {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows NT 6\.[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows NT server.
        /// </summary>
        public static bool IsWindowsServerNT {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows NT Server[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows 2000 server.
        /// </summary>
        public static bool IsWindowsServer2000 {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows 2000 Server[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows server 2003.
        /// </summary>
        public static bool IsWindowsServer2003 {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows Server 2003[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        /// <summary>
        /// Gets whether current operating system is windows server 2008.
        /// </summary>
        public static bool IsWindowsServer2008 {
            get {
                return Regex.IsMatch(Environment.OSVersion.VersionString, @"^Microsoft Windows Server 2008[\w\d\. ]+$", RegexOptions.IgnoreCase);
            }
        }

        #endregion
    }
}