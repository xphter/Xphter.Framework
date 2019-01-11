using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Messaging {
    /// <summary>
    /// Represents common message.
    /// </summary>
    public interface IMessage {
        /// <summary>
        /// Gets a description of this message.
        /// </summary>
        string Description {
            get;
        }
    }
}
