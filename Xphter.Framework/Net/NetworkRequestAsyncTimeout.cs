using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Xphter.Framework.Net {
    /// <summary>
    /// Registers timeout handler of web request.
    /// </summary>
    public static class NetworkRequestAsyncTimeout {
        /// <summary>
        /// Register a timeout handler.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="request"></param>
        /// <param name="timeout"></param>
        public static void RegisterRequest(IAsyncResult result, WebRequest request, int timeout) {
            if(timeout < 0) {
                return;
            }

            WaitHandle wait = result.AsyncWaitHandle;
            if(wait == null) {
                return;
            }

            AsyncOperationTimeoutState state = new AsyncOperationTimeoutState {
                Request = request,
            };
            state.Handle = ThreadPool.RegisterWaitForSingleObject(wait, (userState, isTimeout) => {
                AsyncOperationTimeoutState ts = (AsyncOperationTimeoutState) userState;
                WebRequest webRequst = (WebRequest) ts.Request;

                try {
                    if(isTimeout) {
                        webRequst.Abort();
                    }
                } finally {
                    if(ts.Handle != null) {
                        ts.Handle.Unregister(null);
                    }
                }
            }, state, timeout, true);
        }

        /// <summary>
        /// Register a timeout handler for connect operation.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="client"></param>
        /// <param name="timeout"></param>
        public static void RegisterConnect(IAsyncResult result, Socket client, int timeout) {
            if(timeout < 0) {
                return;
            }

            WaitHandle wait = result.AsyncWaitHandle;
            if(wait == null) {
                return;
            }

            AsyncOperationTimeoutState state = new AsyncOperationTimeoutState {
                Request = client,
            };
            state.Handle = ThreadPool.RegisterWaitForSingleObject(wait, (userState, isTimeout) => {
                AsyncOperationTimeoutState ts = (AsyncOperationTimeoutState) userState;
                Socket socket = (Socket) ts.Request;

                try {
                    if(isTimeout) {
                        socket.Close();
                    }
                } finally {
                    if(ts.Handle != null) {
                        ts.Handle.Unregister(null);
                    }
                }
            }, state, timeout, true);
        }

        /// <summary>
        /// Register a timeout handler for connect operation.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="client"></param>
        /// <param name="timeout"></param>
        public static void RegisterConnect(IAsyncResult result, TcpClient client, int timeout) {
            if(timeout < 0) {
                return;
            }

            WaitHandle wait = result.AsyncWaitHandle;
            if(wait == null) {
                return;
            }

            AsyncOperationTimeoutState state = new AsyncOperationTimeoutState {
                Request = client,
            };
            state.Handle = ThreadPool.RegisterWaitForSingleObject(wait, (userState, isTimeout) => {
                AsyncOperationTimeoutState ts = (AsyncOperationTimeoutState) userState;
                TcpClient tcpClient = (TcpClient) ts.Request;

                try {
                    if(isTimeout) {
                        tcpClient.Close();
                    }
                } finally {
                    if(ts.Handle != null) {
                        ts.Handle.Unregister(null);
                    }
                }
            }, state, timeout, true);
        }

        /// <summary>
        /// Provides the user state object of ThreadPool.RegisterWaitForSingleObject method.
        /// </summary>
        private class AsyncOperationTimeoutState {
            /// <summary>
            /// The registered wait handle.
            /// </summary>
            public RegisteredWaitHandle Handle;

            /// <summary>
            /// The actual request.
            /// </summary>
            public object Request;
        }
    }
}
