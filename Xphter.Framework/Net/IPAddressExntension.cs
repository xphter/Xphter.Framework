using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Xphter.Framework.Net {
    public static class IPAddressExntension {
        /// <summary>
        /// Gets a value to indicate whether <paramref name="address"/> represents a private IPv4 address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool IsPrivateIPv4Address(this IPAddress address) {
            if(address == null) {
                throw new ArgumentNullException("address");
            }

            if(address.AddressFamily != AddressFamily.InterNetwork) {
                return false;
            }
            if(IPAddress.IsLoopback(address)) {
                return true;
            }

            byte[] data = address.GetAddressBytes();

            return
                // 10.0.0.0 -- 10.255.255.255
                data[0] == 10 ||

                // 172.16.0.0 -- 172.31.255.255
                data[0] == 172 && data[1] >= 16 && data[1] <= 31 ||

                // 192.168.0.0 -- 192.168.255.255
                data[0] == 192 && data[1] == 168;
        }
    }
}
