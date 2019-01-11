using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Messaging {
    /// <summary>
    /// Provides data of MessageProcessed event.
    /// </summary>
    public class MessageProcessedEventArgs : EventArgs {
        internal MessageProcessedEventArgs(IMessage message, IMessageHandler handler, Exception error) {
            this.Message = message;
            this.Handler = handler;
            this.Error = error;
        }

        /// <summary>
        /// Gets the processed message.
        /// </summary>
        public IMessage Message {
            get;
            private set;
        }

        /// <summary>
        /// Gets current message hander.
        /// </summary>
        public IMessageHandler Handler {
            get;
            private set;
        }

        /// <summary>
        /// Gets the occurred exception, null indicates there is no error.
        /// </summary>
        public Exception Error {
            get;
            private set;
        }

        /// <inheritdoc />
        public override string ToString() {
            return this.Message.ToString();
        }
    }
}
