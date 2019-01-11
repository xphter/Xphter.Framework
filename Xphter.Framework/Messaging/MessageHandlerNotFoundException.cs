using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Xphter.Framework.Messaging {
    /// <summary>
    /// Thrown when can't find a handler to process a message.
    /// </summary>
    public class MessageHandlerNotFoundException : Exception {
        public MessageHandlerNotFoundException(IMessage currentMessage)
            : base() {
            this.CurrentMessage = currentMessage;
        }

        public MessageHandlerNotFoundException(string message, IMessage currentMessage)
            : base(message) {
            this.CurrentMessage = currentMessage;
        }

        protected MessageHandlerNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public MessageHandlerNotFoundException(string message, Exception innerException, IMessage currentMessage)
            : base(message, innerException) {
            this.CurrentMessage = currentMessage;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public IMessage CurrentMessage {
            get;
            set;
        }
    }
}
