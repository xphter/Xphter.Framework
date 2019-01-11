using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xphter.Framework.Web {
    /// <summary>
    /// Represents the device type of the client running browser.
    /// </summary>
    [Flags]
    public enum ClientDeviceType {
        /// <summary>
        /// Unknown device type.
        /// </summary>
        Unknown = 0x00,

        /// <summary>
        /// PC: windows, linux, unix, mac.
        /// </summary>
        PersonalComputer = 0x02,

        /// <summary>
        /// Phone: android, ios, windows phone, blackberry.
        /// </summary>
        Phone = 0x04,

        /// <summary>
        /// Tablet: android, windows, ios.
        /// </summary>
        Tablet = 0x08,

        /// <summary>
        /// All device types.
        /// </summary>
        All = PersonalComputer | Phone | Tablet,
    }
}
