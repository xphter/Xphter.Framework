using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Messaging {
    /// <summary>
    /// Represents a async message handler.
    /// </summary>
    public interface IMessageAsyncHandler : IMessageHandler {
        /// <summary>
        /// Begins a async operation to process the specified message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        /// <param name="userState"></param>
        /// <returns></returns>
        IAsyncResult BeginProcessMessage(IMessage message, AsyncCallback callback, object userState);

        /// <summary>
        /// Ends the async operation.
        /// </summary>
        /// <param name="asyncResult"></param>
        void EndProcessMessage(IAsyncResult asyncResult);
    }
}
