using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// Provides a tool for invoking windows API.
    /// </summary>
    public static class WindowsAPI {
        /// <summary>
        /// The value of invalid handle.
        /// </summary>
        public const short INVALID_HANDLE_VALUE = -1;

        #region CloseHandle

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);

        #endregion

        #region ExitWindowsEx

        /// <summary>
        /// Logs off the interactive user, shuts down the system, or shuts down and restarts the system. It sends the WM_QUERYENDSESSION message to all applications to determine if they can be terminated.
        /// </summary>
        /// <param name="flags">The shutdown type.</param>
        /// <param name="reason">The reason for initiating the shutdown.</param>
        /// <returns>True indicates that the shutdown has been initiated, otherwise the function fails.</returns>
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ExitWindowsEx(ExitWindowsExFlags flags, ExitWindowsExReason reason);

        /// <summary>
        /// Gets whether current operating system supports ExitWindowsEx function.
        /// </summary>
        public static bool IsSupportExitWindowsEx {
            get {
                return OperatingSystem.IsWindows95 ||
                       OperatingSystem.IsWindows98 ||
                       OperatingSystem.IsWindowsMe ||
                       OperatingSystem.IsWindowsNTWorkstation ||
                       OperatingSystem.IsWindows2000Professional ||
                       OperatingSystem.IsWindowsXP ||
                       OperatingSystem.IsWindowsVista ||
                       OperatingSystem.IsWindows7 ||
                       OperatingSystem.IsWindowsServerNT ||
                       OperatingSystem.IsWindowsServer2000 ||
                       OperatingSystem.IsWindowsServer2003 ||
                       OperatingSystem.IsWindowsServer2008;
            }
        }

        /// <summary>
        /// The shutdown type.
        /// </summary>
        [Flags]
        public enum ExitWindowsExFlags {
            /// <summary>
            /// Shuts down all processes running in the logon session of the process that called the ExitWindowsEx function. Then it logs the user off.
            /// </summary>
            EWX_LOGOFF = 0x00,

            /// <summary>
            /// Shuts down the system and turns off the power. The system must support the power-off feature. 
            /// </summary>
            EWX_POWEROFF = 0x08,

            /// <summary>
            /// Shuts down the system and then restarts the system. 
            /// </summary>
            EWX_REBOOT = 0x02,

            /// <summary>
            /// Shuts down the system and then restarts it, as well as any applications that have been registered for restart using the RegisterApplicationRestart function. These application receive the WM_QUERYENDSESSION message with lParam set to the ENDSESSION_CLOSEAPP value. 
            /// </summary>
            EWX_RESTARTAPPS = 0x40,

            /// <summary>
            /// Shuts down the system to a point at which it is safe to turn off the power. All file buffers have been flushed to disk, and all running processes have stopped. 
            /// </summary>
            EWX_SHUTDOWN = 0x01,

            /// <summary>
            /// This flag has no effect if terminal services is enabled. Otherwise, the system does not send the WM_QUERYENDSESSION and WM_ENDSESSION messages. This can cause applications to lose data. Therefore, you should only use this flag in an emergency.
            /// </summary>
            EWX_FORCE = 0x04,

            /// <summary>
            /// Forces processes to terminate if they do not respond to the WM_QUERYENDSESSION or WM_ENDSESSION message within the timeout interval.
            /// </summary>
            EWX_FORCEIFHUNG = 0x10,
        }

        /// <summary>
        /// The reason for initiating the shutdown.
        /// </summary>
        [Flags]
        public enum ExitWindowsExReason : uint {
            SHTDN_REASON_MAJOR_OTHER = 0x00000000,
            SHTDN_REASON_MAJOR_NONE = 0x00000000,
            SHTDN_REASON_MAJOR_HARDWARE = 0x00010000,
            SHTDN_REASON_MAJOR_OPERATINGSYSTEM = 0x00020000,
            SHTDN_REASON_MAJOR_SOFTWARE = 0x00030000,
            SHTDN_REASON_MAJOR_APPLICATION = 0x00040000,
            SHTDN_REASON_MAJOR_SYSTEM = 0x00050000,
            SHTDN_REASON_MAJOR_POWER = 0x00060000,
            SHTDN_REASON_MAJOR_LEGACY_API = 0x00070000,

            SHTDN_REASON_MINOR_OTHER = 0x00000000,
            SHTDN_REASON_MINOR_NONE = 0x000000ff,
            SHTDN_REASON_MINOR_MAINTENANCE = 0x00000001,
            SHTDN_REASON_MINOR_INSTALLATION = 0x00000002,
            SHTDN_REASON_MINOR_UPGRADE = 0x00000003,
            SHTDN_REASON_MINOR_RECONFIG = 0x00000004,
            SHTDN_REASON_MINOR_HUNG = 0x00000005,
            SHTDN_REASON_MINOR_UNSTABLE = 0x00000006,
            SHTDN_REASON_MINOR_DISK = 0x00000007,
            SHTDN_REASON_MINOR_PROCESSOR = 0x00000008,
            SHTDN_REASON_MINOR_NETWORKCARD = 0x00000009,
            SHTDN_REASON_MINOR_POWER_SUPPLY = 0x0000000a,
            SHTDN_REASON_MINOR_CORDUNPLUGGED = 0x0000000b,
            SHTDN_REASON_MINOR_ENVIRONMENT = 0x0000000c,
            SHTDN_REASON_MINOR_HARDWARE_DRIVER = 0x0000000d,
            SHTDN_REASON_MINOR_OTHERDRIVER = 0x0000000e,
            SHTDN_REASON_MINOR_BLUESCREEN = 0x0000000F,
            SHTDN_REASON_MINOR_SERVICEPACK = 0x00000010,
            SHTDN_REASON_MINOR_HOTFIX = 0x00000011,
            SHTDN_REASON_MINOR_SECURITYFIX = 0x00000012,
            SHTDN_REASON_MINOR_SECURITY = 0x00000013,
            SHTDN_REASON_MINOR_NETWORK_CONNECTIVITY = 0x00000014,
            SHTDN_REASON_MINOR_WMI = 0x00000015,
            SHTDN_REASON_MINOR_SERVICEPACK_UNINSTALL = 0x00000016,
            SHTDN_REASON_MINOR_HOTFIX_UNINSTALL = 0x00000017,
            SHTDN_REASON_MINOR_SECURITYFIX_UNINSTALL = 0x00000018,
            SHTDN_REASON_MINOR_MMC = 0x00000019,
            SHTDN_REASON_MINOR_SYSTEMRESTORE = 0x0000001a,
            SHTDN_REASON_MINOR_TERMSRV = 0x00000020,
            SHTDN_REASON_MINOR_DC_PROMOTION = 0x00000021,
            SHTDN_REASON_MINOR_DC_DEMOTION = 0x00000022,

            SHTDN_REASON_FLAG_USER_DEFINED = 0x40000000,
            SHTDN_REASON_FLAG_PLANNED = 0x80000000,
        }

        #endregion

        #region LockWorkStation

        /// <summary>
        /// Locks the workstation's display. Locking a workstation protects it from unauthorized use.
        /// </summary>
        /// <returns>True indicates that the operation has been initiated, otherwise the function fails.</returns>
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LockWorkStation();

        /// <summary>
        /// Gets whether current operating system supports LockWorkStation function.
        /// </summary>
        public static bool IsSupportLockWorkStation {
            get {
                return OperatingSystem.IsWindows2000Professional ||
                       OperatingSystem.IsWindowsXP ||
                       OperatingSystem.IsWindowsVista ||
                       OperatingSystem.IsWindows7 ||
                       OperatingSystem.IsWindowsServer2000 ||
                       OperatingSystem.IsWindowsServer2003 ||
                       OperatingSystem.IsWindowsServer2008;
            }
        }

        #endregion

        #region CreateFile

        /// <summary>
        /// Creates or opens a file or I/O device. 
        /// </summary>
        /// <param name="lpFileName">The name of the file or device to be created or opened. </param>
        /// <param name="dwDesiredAccess">The requested access to the file or device, which can be summarized as read, write, both or neither zero.</param>
        /// <param name="dwShareMode"></param>
        /// <param name="lpSecurityAttributes"></param>
        /// <param name="dwCreationDisposition"></param>
        /// <param name="dwFlagsAndAttributes"></param>
        /// <param name="hTemplateFile"></param>
        /// <returns></returns>
        [DllImport("Kernel32.dll")]
        public static extern IntPtr CreateFile(string lpFileName,
            FileAccessMode dwDesiredAccess,
            FileShareMode dwShareMode,
            IntPtr lpSecurityAttributes,
            FileCreationMode dwCreationDisposition,
            FileFlagAndAttribute dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        /// <summary>
        /// Represents the mode to access a file.
        /// </summary>
        [Flags]
        public enum FileAccessMode : uint {
            None = 0x00,

            Read = 0x80000000,

            Write = 0x40000000,
        }

        /// <summary>
        /// Represents the mode to share a file.
        /// </summary>
        [Flags]
        public enum FileShareMode : uint {
            /// <summary>
            /// Prevents other processes from opening a file or device if they request delete, read, or write access.
            /// </summary>
            None = 0x00,

            /// <summary>
            /// Enables subsequent open operations on a file or device to request read access.
            /// </summary>
            Read = 0x01,

            /// <summary>
            /// Enables subsequent open operations on a file or device to request write access.
            /// </summary>
            Write = 0x02,

            /// <summary>
            /// Enables subsequent open operations on a file or device to request delete access.
            /// </summary>
            Delete = 0x04,
        }

        /// <summary>
        /// Represents the mode how to create a new file, open or rewrite a existing file.
        /// </summary>
        public enum FileCreationMode : uint {
            /// <summary>
            /// Creates a new file, only if it does not already exist.
            /// </summary>
            CreateNew = 0x01,

            /// <summary>
            /// Creates a new file, always.
            /// </summary>
            CreateAlways = 0x02,

            /// <summary>
            /// Opens a file or device, only if it exists.
            /// </summary>
            OpenExisting = 0x03,

            /// <summary>
            /// Opens a file, always.
            /// </summary>
            OpenAlways = 0x04,

            /// <summary>
            /// Opens a file and truncates it so that its size is zero bytes, only if it exists.
            /// </summary>
            TruncateExisting = 0x05,
        }

        [Flags]
        public enum FileFlagAndAttribute : uint {
            /// <summary>
            /// The file should be archived. Applications use this attribute to mark files for backup or removal.
            /// </summary>
            AttributeArchive = 0x20,

            /// <summary>
            /// The file or directory is encrypted. For a file, this means that all data in the file is encrypted. For a directory, this means that encryption is the default for newly created files and subdirectories. 
            /// </summary>
            AttributeEncrypted = 0x4000,

            /// <summary>
            /// The file is hidden. Do not include it in an ordinary directory listing.
            /// </summary>
            AttributeHidden = 0x02,

            /// <summary>
            /// The file does not have other attributes set. This attribute is valid only if used alone.
            /// </summary>
            AttributeNormal = 0x80,

            /// <summary>
            /// The data of a file is not immediately available. This attribute indicates that file data is physically moved to offline storage. This attribute is used by Remote Storage, the hierarchical storage management software. Applications should not arbitrarily change this attribute.
            /// </summary>
            AttributeOffline = 0x1000,

            /// <summary>
            /// The file is read only. Applications can read the file, but cannot write to or delete it.
            /// </summary>
            AttributeReadOnly = 0x01,

            /// <summary>
            /// The file is part of or used exclusively by an operating system.
            /// </summary>
            AttributeSystem = 0x04,

            /// <summary>
            /// The file is being used for temporary storage.
            /// </summary>
            AttributeTemporary = 0x100,

            /// <summary>
            /// The file is being opened or created for a backup or restore operation. 
            /// </summary>
            FlagBackupSemantics = 0x02000000,

            /// <summary>
            /// The file is to be deleted immediately after all of its handles are closed, which includes the specified handle and any other open or duplicated handles.
            /// </summary>
            FlagDeleteOnClose = 0x04000000,

            /// <summary>
            /// The file or device is being opened with no system caching for data reads and writes. This flag does not affect hard disk caching or memory mapped files.
            /// </summary>
            FlagNoBuffering = 0x20000000,

            /// <summary>
            /// The file data is requested, but it should continue to be located in remote storage. It should not be transported back to local storage. This flag is for use by remote storage systems.
            /// </summary>
            FlagOpenNoRecall = 0x00100000,

            /// <summary>
            /// Normal reparse point processing will not occur; CreateFile will attempt to open the reparse point. When a file is opened, a file handle is returned, whether or not the filter that controls the reparse point is operational.
            /// </summary>
            FlagOpenReparsePoint = 0x00200000,

            /// <summary>
            /// The file or device is being opened or created for asynchronous I/O.
            /// </summary>
            FlagOverlapped = 0x40000000,

            /// <summary>
            /// Access will occur according to POSIX rules. This includes allowing multiple files with names, differing only in case, for file systems that support that naming. Use care when using this option, because files created with this flag may not be accessible by applications that are written for MS-DOS or 16-bit Windows.
            /// </summary>
            FlagPosixSemantics = 0x0100000,

            /// <summary>
            /// Access is intended to be random. The system can use this as a hint to optimize file caching.
            /// </summary>
            FlagRandomAccess = 0x10000000,

            /// <summary>
            /// The file or device is being opened with session awareness. If this flag is not specified, then per-session devices (such as a device using RemoteFX USB Redirection) cannot be opened by processes running in session 0. This flag has no effect for callers not in session 0. This flag is supported only on server editions of Windows.
            /// </summary>
            FlagSessionAware = 0x00800000,

            /// <summary>
            /// Access is intended to be sequential from beginning to end. The system can use this as a hint to optimize file caching.
            /// </summary>
            FlagSequentialScan = 0x08000000,

            /// <summary>
            /// Write operations will not go through any intermediate cache, they will go directly to disk.
            /// </summary>
            FlagWriteThrough = 0x80000000,
        }

        #endregion

        #region LogonUser

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, LogonType dwLogonType, LogonProvider dwLogonProvider, ref IntPtr token);

        public enum LogonType : uint {
            /// <summary>
            /// This logon type is intended for users who will be interactively using the computer, such as a user being logged on by a terminal server, remote shell, or similar process.
            /// This logon type has the additional expense of caching logon information for disconnected operations; therefore, it is inappropriate for some client/server applications, such as a mail server.
            /// </summary>
            Interactive = 0x02,

            /// <summary>
            /// This logon type is intended for high performance servers to authenticate plaintext passwords.
            /// The LogonUser function does not cache credentials for this logon type.
            /// </summary>
            Network = 0x03,

            /// <summary>
            /// This logon type is intended for batch servers, where processes may be executing on behalf of a user without their direct intervention.
            /// This type is also for higher performance servers that process many plaintext authentication attempts at a time, such as mail or web servers.
            /// </summary>
            Batch = 0x04,

            /// <summary>
            /// Indicates a service-type logon. The account provided must have the service privilege enabled.
            /// </summary>
            Service = 0x05,

            /// <summary>
            /// GINAs are no longer supported.
            /// </summary>
            Unlock = 0x07,

            /// <summary>
            /// This logon type preserves the name and password in the authentication package, which allows the server to make connections to other network servers while impersonating the client.
            /// A server can accept plaintext credentials from a client, call LogonUser, verify that the user can access the system across the network, and still communicate with other servers.
            /// </summary>
            NetworkCleartext = 0x08,

            /// <summary>
            /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
            /// The new logon session has the same local identifier but uses different credentials for other network connections.
            /// This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
            /// </summary>
            NewCredentials = 0x09,
        }

        public enum LogonProvider : uint {
            /// <summary>
            /// Use the standard logon provider for the system. The default security provider is negotiate, unless you pass NULL for the domain name and the user name is not in UPN format. In this case, the default provider is NTLM.
            /// </summary>
            Default = 0x00,
            Winnt35 = 0x01,

            /// <summary>
            /// Use the NTLM logon provider.
            /// </summary>
            Winnt40 = 0x02,

            /// <summary>
            /// Use the negotiate logon provider.
            /// </summary>
            Winnt50 = 0x03,
            Virtual = 0x04,
        }

        #endregion
    }
}
