using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework {
    /// <summary>
    /// A utility used to control windows OS, ie. shutdown, reboot, log off, lock.
    /// </summary>
    public static class WindowsController {
        /// <summary>
        /// Gets whether can shutdown windows.
        /// </summary>
        public static bool CanShutdown {
            get {
                return Environment.UserInteractive && WindowsAPI.IsSupportExitWindowsEx;
            }
        }

        /// <summary>
        /// Shutdown windows.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Current process is not running in user interactive mode.</exception>
        /// <exception cref="System.NotSupportedException">Current operating system is not supports this operation.</exception>
        public static void Shutdown() {
            if(!Environment.UserInteractive) {
                throw new InvalidOperationException("The current process is not running in user interactive mode.");
            }
            if(!WindowsAPI.IsSupportExitWindowsEx) {
                throw new NotSupportedException("Current operating system is not supports this operation.");
            }
            WindowsAPI.ExitWindowsEx(WindowsAPI.ExitWindowsExFlags.EWX_SHUTDOWN | WindowsAPI.ExitWindowsExFlags.EWX_FORCEIFHUNG, WindowsAPI.ExitWindowsExReason.SHTDN_REASON_MAJOR_NONE);
        }

        /// <summary>
        /// Gets whether can reboot windows.
        /// </summary>
        public static bool CanReboot {
            get {
                return Environment.UserInteractive && WindowsAPI.IsSupportExitWindowsEx;
            }
        }

        /// <summary>
        /// Reboot windows.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Current process is not running in user interactive mode.</exception>
        /// <exception cref="System.NotSupportedException">Current operating system is not supports this operation.</exception>
        public static void Reboot() {
            if(!Environment.UserInteractive) {
                throw new InvalidOperationException("The current process is not running in user interactive mode.");
            }
            if(!WindowsAPI.IsSupportExitWindowsEx) {
                throw new NotSupportedException("Current operating system is not supports this operation.");
            }
            WindowsAPI.ExitWindowsEx(WindowsAPI.ExitWindowsExFlags.EWX_REBOOT | WindowsAPI.ExitWindowsExFlags.EWX_FORCEIFHUNG, WindowsAPI.ExitWindowsExReason.SHTDN_REASON_MAJOR_NONE);
        }

        /// <summary>
        /// Gets whether can log off windows.
        /// </summary>
        public static bool CanLogOff {
            get {
                return Environment.UserInteractive && WindowsAPI.IsSupportExitWindowsEx;
            }
        }

        /// <summary>
        /// Log off windows.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Current process is not running in user interactive mode.</exception>
        /// <exception cref="System.NotSupportedException">Current operating system is not supports this operation.</exception>
        public static void LogOff() {
            if(!Environment.UserInteractive) {
                throw new InvalidOperationException("The current process is not running in user interactive mode.");
            }
            if(!WindowsAPI.IsSupportExitWindowsEx) {
                throw new NotSupportedException("Current operating system is not supports this operation.");
            }
            WindowsAPI.ExitWindowsEx(WindowsAPI.ExitWindowsExFlags.EWX_LOGOFF | WindowsAPI.ExitWindowsExFlags.EWX_FORCEIFHUNG, WindowsAPI.ExitWindowsExReason.SHTDN_REASON_MAJOR_NONE);
        }

        /// <summary>
        /// Gets whether can lock windows.
        /// </summary>
        public static bool CanLock {
            get {
                return Environment.UserInteractive && WindowsAPI.IsSupportLockWorkStation;
            }
        }

        /// <summary>
        /// Lock windows.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Current process is not running in user interactive mode.</exception>
        /// <exception cref="System.NotSupportedException">Current operating system is not supports this operation.</exception>
        public static void Lock() {
            if(!Environment.UserInteractive) {
                throw new InvalidOperationException("The current process is not running in user interactive mode.");
            }
            if(!WindowsAPI.IsSupportLockWorkStation) {
                throw new NotSupportedException("The current operating system is not supports this operation.");
            }
            WindowsAPI.LockWorkStation();
        }
    }
}
