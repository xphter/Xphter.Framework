using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.WeChat.OfficialAccounts {
    public class AggregateWeChatMessageHandler : IWeChatMessageHandler {
        public AggregateWeChatMessageHandler()
            : this(WeChatMessageHandlerPriority.Normal) {
        }

        public AggregateWeChatMessageHandler(WeChatMessageHandlerPriority priority) {
            this.Priority = priority;
            this.m_handlers = Enumerable.Empty<IWeChatMessageHandler>();
        }

        private IEnumerable<IWeChatMessageHandler> m_handlers;

        public void AddHanlder(IWeChatMessageHandler handler) {
            if(handler == null) {
                throw new ArgumentNullException("handler");
            }
            if(handler.Priority == WeChatMessageHandlerPriority.Disabled) {
                return;
            }

            this.m_handlers = this.m_handlers.Concat(new IWeChatMessageHandler[] { handler }).OrderByDescending((item) => item.Priority).ToArray();
        }

        private void ProcessMessage(IWeChatMessage message, IOfficialAccountService service, Queue<IWeChatMessageHandler> queue, AsyncResult<IWeChatMessageResult> result, bool completedSynchronously) {
            if(queue.Count == 0) {
                result.MarkCompleted(null, completedSynchronously, null);
                return;
            }

            IWeChatMessageHandler handler = queue.Dequeue();
            handler.BeginProcessMessage(message, service, (ar) => {
                Exception error = null;
                IWeChatMessageResult messageResult = null;
                bool synchronously = completedSynchronously && ar.CompletedSynchronously;

                try {
                    messageResult = handler.EndProcessMessage(ar);
                } catch(Exception ex) {
                    error = ex;
                }

                if(error != null) {
                    result.MarkCompleted(error, synchronously, null);
                } else if(messageResult != null) {
                    result.MarkCompleted(null, synchronously, messageResult);
                } else {
                    this.ProcessMessage(message, service, queue, result, synchronously);
                }
            }, null);
        }

        #region IWeChatMessageHandler Members

        public WeChatMessageHandlerPriority Priority {
            get;
            private set;
        }

        public IAsyncResult BeginProcessMessage(IWeChatMessage message, IOfficialAccountService service, AsyncCallback callback, object userState) {
            AsyncResult<IWeChatMessageResult> result = new AsyncResult<IWeChatMessageResult>(callback, userState);
            this.ProcessMessage(message, service, new Queue<IWeChatMessageHandler>(this.m_handlers), result, true);
            return result;
        }

        public IWeChatMessageResult EndProcessMessage(IAsyncResult asyncResult) {
            if(!(asyncResult is AsyncResult<IWeChatMessageResult>)) {
                throw new InvalidOperationException();
            }

            return ((AsyncResult<IWeChatMessageResult>) asyncResult).End();
        }

        #endregion
    }
}
