using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Xphter.Framework.Security {
    public class IdentityImpersonationScope : IDisposable {
        public IdentityImpersonationScope(string userName, string password)
            : this(null, userName, password, WindowsAPI.LogonType.Interactive, WindowsAPI.LogonProvider.Default) {
        }

        public IdentityImpersonationScope(string domainName, string userName, string password)
            : this(domainName, userName, password, WindowsAPI.LogonType.Interactive, WindowsAPI.LogonProvider.Default) {
        }

        public IdentityImpersonationScope(string domainName, string userName, string password, WindowsAPI.LogonType logonType, WindowsAPI.LogonProvider logonProvider) {
            if(string.IsNullOrWhiteSpace(userName)) {
                throw new ArgumentException("userName is null or empty.", "userName");
            }

            if(!WindowsAPI.LogonUser(userName, !string.IsNullOrWhiteSpace(domainName) ? domainName : ".", password, logonType, logonProvider, ref this.m_token)) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            try {
                this.m_context = WindowsIdentity.Impersonate(this.m_token);
            } catch(Exception ex) {
                try {
                    WindowsAPI.CloseHandle(this.m_token);
                } finally {
                    throw ex;
                }
            }
        }

        private IntPtr m_token;
        private WindowsImpersonationContext m_context;

        #region IDisposable Members

        protected bool m_disposed;

        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }

            this.m_disposed = true;

            WindowsAPI.CloseHandle(this.m_token);
            if(this.m_context != null) {
                this.m_context.Dispose();
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~IdentityImpersonationScope() {
            this.Disposing(false);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        #endregion
    }
}
