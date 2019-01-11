using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Messaging {
    /// <summary>
    /// Represents a message handler.
    /// </summary>
    public interface IMessageHandler {
        /// <summary>
        /// Gets types of all messages which this handler can process.
        /// </summary>
        IEnumerable<Type> MessageTypes {
            get;
        }

        /// <summary>
        /// Process the specified message.
        /// </summary>
        /// <param name="message"></param>
        void ProcessMessage(IMessage message);
    }
}
