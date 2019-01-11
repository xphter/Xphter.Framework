using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xphter.Framework.Collections;

namespace Xphter.Framework.Messaging {
    /// <summary>
    /// Implements a sample message queue in current process, which has multiple producer and single consumer.
    /// </summary>
    public class MessageQueue : IDisposable {
        public MessageQueue(bool throwExceptionIfHandlerNotFound) {
            this.m_event = new AutoResetEvent(false);
            this.m_queue = new LockFreeQueue<IMessage>();

            this.m_throwExceptionIfHandlerNotFound = throwExceptionIfHandlerNotFound;
            this.m_handlers = new Dictionary<Type, ICollection<IMessageHandler>>();

            new Thread(() => this.MessageLoop()).Start();
        }

        private AutoResetEvent m_event;
        private LockFreeQueue<IMessage> m_queue;

        private bool m_throwExceptionIfHandlerNotFound;
        private IDictionary<Type, ICollection<IMessageHandler>> m_handlers;

        private IEnumerable<IMessageHandler> FindHandlers(IMessage message) {
            Type messageType = message.GetType();
            IEnumerable<IMessageHandler> result = Enumerable.Empty<IMessageHandler>();

            lock(this.m_handlers) {
                foreach(Type type in this.m_handlers.Keys) {
                    if(type.IsAssignableFrom(messageType)) {
                        result = result.Concat(this.m_handlers[type]);
                    }
                }

                result = result.ToArray();
            }

            return result;
        }

        private void ProcessMessage(IMessage message) {
            IEnumerable<IMessageHandler> handlers = this.FindHandlers(message);
            if(!handlers.Any()) {
                if(this.m_throwExceptionIfHandlerNotFound) {
                    throw new MessageHandlerNotFoundException(message);
                } else {
                    return;
                }
            }

            Exception syncError = null;
            IMessageAsyncHandler asyncHandler = null;

            foreach(IMessageHandler handler in handlers) {
                syncError = null;

                if(handler is IMessageAsyncHandler) {
                    asyncHandler = (IMessageAsyncHandler) handler;

                    asyncHandler.BeginProcessMessage(message, (ar) => {
                        Exception asyncError = null;
                        IMessage currentMessage = (IMessage) ((object[]) ar.AsyncState)[0];
                        IMessageAsyncHandler currentHandler = (IMessageAsyncHandler) ((object[]) ar.AsyncState)[1];

                        try {
                            currentHandler.EndProcessMessage(ar);
                        } catch(Exception ex) {
                            asyncError = ex;
                        }

                        this.OnMessageProcessed(new MessageProcessedEventArgs(currentMessage, currentHandler, asyncError));
                    }, new object[] { message, asyncHandler });
                } else {
                    try {
                        handler.ProcessMessage(message);
                    } catch(Exception ex) {
                        syncError = ex;
                    }

                    this.OnMessageProcessed(new MessageProcessedEventArgs(message, handler, syncError));
                }
            }
        }

        private void ProcessMessages() {
            IMessage message = null;

            while(this.m_queue.TryDequeue(out message)) {
                this.ProcessMessage(message);
            }
        }

        private void MessageLoop() {
            while(!this.m_disposed) {
                try {
                    this.m_event.WaitOne();
                } catch(ObjectDisposedException) {
                    this.ProcessMessages();

                    break;
                }

                this.ProcessMessages();
            }
        }

        #region MessageProcessed Event

        protected virtual void OnMessageProcessed(MessageProcessedEventArgs args) {
            if(this.MessageProcessed != null) {
                this.MessageProcessed(this, args);
            }
        }

        public event EventHandler<MessageProcessedEventArgs> MessageProcessed;

        #endregion

        public void RegisterHandler(IMessageHandler handler) {
            if(this.m_disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }
            if(handler == null) {
                throw new ArgumentNullException("handler");
            }
            if(handler.MessageTypes == null || !handler.MessageTypes.Where((item) => item != null).Any()) {
                throw new ArgumentException("handler can not process any messages.", "handler");
            }

            lock(this.m_handlers) {
                if(this.m_handlers.Any((item) => item.Value.Contains(handler))) {
                    throw new InvalidOperationException("handler is already existing.");
                }

                foreach(Type type in handler.MessageTypes.Where((item) => item != null)) {
                    if(!this.m_handlers.ContainsKey(type)) {
                        this.m_handlers[type] = new List<IMessageHandler>();
                    }

                    this.m_handlers[type].Add(handler);
                }
            }
        }

        public void UnregisterHandler(IMessageHandler handler) {
            if(this.m_disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }
            if(handler == null) {
                throw new ArgumentNullException("handler");
            }

            lock(this.m_handlers) {
                foreach(ICollection<IMessageHandler> handlers in this.m_handlers.Values) {
                    handlers.Remove(handler);
                }
            }
        }

        public void SendMessage(IMessage message) {
            if(this.m_disposed) {
                throw new ObjectDisposedException(this.GetType().Name);
            }
            if(message == null) {
                throw new ArgumentNullException("message");
            }

            this.m_queue.Enqueue(message);

            try {
                this.m_event.Set();
            } catch(ObjectDisposedException) {
            }
        }

        #region IDisposable Members

        private volatile bool m_disposed;

        protected virtual void Disposing(bool disposing) {
            if(this.m_disposed) {
                return;
            }

            this.m_disposed = true;

            using(this.m_event) {
                this.m_event.Set();
            }

            if(disposing) {
                GC.SuppressFinalize(this);
            }
        }

        ~MessageQueue() {
            this.Disposing(false);
        }

        public void Close() {
            this.Disposing(true);
        }

        public void Dispose() {
            this.Disposing(true);
        }

        #endregion
    }
}
