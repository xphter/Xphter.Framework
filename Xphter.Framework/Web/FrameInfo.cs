using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Represents a frame in a web page.
    /// </summary>
    public class FrameInfo {
        /// <summary>
        /// Initialize a instance of FrameInfo class.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <exception cref="System.ArgumentException"><paramref name="address"/> is null or empty.</exception>
        public FrameInfo(string address, string width, string height) {
            if(string.IsNullOrWhiteSpace(address)) {
                throw new ArgumentException("address is null or empty.", "address");
            }

            this.Address = address;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets page address.
        /// </summary>
        public string Address {
            get;
            private set;
        }

        /// <summary>
        /// Gets page with.
        /// </summary>
        public string Width {
            get;
            private set;
        }

        /// <summary>
        /// Gets page height.
        /// </summary>
        public string Height {
            get;
            private set;
        }
    }
}
