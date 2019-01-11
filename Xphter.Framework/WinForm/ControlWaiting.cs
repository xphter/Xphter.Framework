using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Xphter.Framework.WinForm {
    /// <summary>
    /// Provides a tool to show waiting cursor of the WinForm control when perform operation.
    /// </summary>
    public class ControlWaiting : IDisposable {
        /// <summary>
        /// Initialize a new instance of ControlWaiting.
        /// </summary>
        /// <param name="control">The target control.</param>
        /// <exception cref="System.ArgumentException"><paramref name="control"/> is null or the handle has not created.</exception>
        public ControlWaiting(Control control) {
            if(control == null) {
                throw new ArgumentException("Target control is null.", "control");
            }
            if(!control.IsHandleCreated) {
                throw new ArgumentException("The control handle has not created.", "control");
            }

            this.Control = control;
            if(this.Control.InvokeRequired) {
                this.Control.BeginInvoke(new Action(delegate() {
                    this.m_currentCursor = this.Control.Cursor;
                    this.Control.Cursor = Cursors.WaitCursor;
                }));
            } else {
                this.m_currentCursor = this.Control.Cursor;
                this.Control.Cursor = Cursors.WaitCursor;
            }
        }

        /// <summary>
        /// The current cursor of the target control.
        /// </summary>
        private Cursor m_currentCursor;

        /// <summary>
        /// Gets the target control.
        /// </summary>
        public Control Control {
            get;
            private set;
        }

        #region IDisposable Members

        protected bool m_isDisposed;

        ~ControlWaiting() {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing) {
            if(this.m_isDisposed) {
                return;
            }

            if(disposing) {
            }

            if(this.Control.Created && this.Control.IsHandleCreated) {
                if(this.Control.InvokeRequired) {
                    this.Control.BeginInvoke(new Action(delegate() {
                        this.Control.Cursor = this.m_currentCursor;
                    }));
                } else {
                    this.Control.Cursor = this.m_currentCursor;
                }
            }

            this.m_isDisposed = true;
        }

        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
