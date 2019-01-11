using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xphter.Framework.Net;

namespace Xphter.Framework.Net.Tests {
    [TestClass()]
    public class NetworkRequestAsyncTimeoutTests {
        [TestMethod()]
        public void RegisterTest_WebRequest() {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://localhost:44444/test.jpg");
            request.Method = WebRequestMethods.Http.Get;

            NetworkRequestAsyncTimeout.RegisterRequest(request.BeginGetResponse(null, null), request, 1);
        }

        [TestMethod()]
        public void RegisterConnect_Socket() {
            Exception error = null;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            NetworkRequestAsyncTimeout.RegisterConnect(socket.BeginConnect(new IPEndPoint(IPAddress.Parse("192.168.1.222"), 44444), (ar) => {
                try {
                    socket.EndConnect(ar);
                } catch(Exception ex) {
                    error = ex;
                }
            }, null), socket, 10000);

            Thread.Sleep(30000);

            Assert.IsNotNull(error);
        }   

        [TestMethod()]
        public void RegisterConnect_TcpClient() {
            Exception error = null;
            TcpClient client = new TcpClient(AddressFamily.InterNetwork);

            NetworkRequestAsyncTimeout.RegisterConnect(client.BeginConnect(IPAddress.Parse("192.168.1.222"), 44444, (ar) => {
                try {
                    client.EndConnect(ar);
                } catch(Exception ex) {
                    error = ex;
                }
            }, null), client, 1000);

            Thread.Sleep(30000);

            Assert.IsNotNull(error);
        }
    }
}
