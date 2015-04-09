using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SSModelNS
{
    public class SSModel
    {
        private StringSocket socket;

        public SSModel()
        {
            socket = null;
        }


        public void Connect(string hostname, int port, string name, string ss_name)
        {
            {
                if (socket == null) // Assumes you are not connected to a server already.
                {
                    IPAddress ipaddress;
                    if (IPAddress.TryParse(hostname, out ipaddress)) // See if the hostname is an IP
                    {
                        hostname = Dns.GetHostByAddress(ipaddress).HostName; // Overwrite hostname with the IP.
                    }
                    try
                    {
                        // Try to establish a connection through StringSocket.
                        TcpClient client = new TcpClient(hostname, port);
                        socket = new StringSocket(client.Client, ASCIIEncoding.Default);
                        socket.BeginSend("connect " + name + ss_name + "\n", (e, p) => { }, null); // Send a protocol specified message to connect
                        socket.BeginReceive(LineReceived, null); // Wait for the server to respond with a match start protocol message.
                    }
                    catch (SocketException) // Could not connect to the server.
                    {
                        //ConnectionNotFoundEvent(); // Notify the user a connection could not be established.
                    }
                }

            }
        }


        private void LineReceived(String s, Exception e, object p)
        {
            String received = "";
            String command = "";

            command = received.Substring(0, received.IndexOf("\n"));
            received = received.Substring(received.IndexOf("\n"));


            if (command.Substring(0, command.IndexOf(" ")) == "connected")
            {

            }
            else if (command.Substring(0, command.IndexOf(" ")) == "cell")
            {


            }
            else if (command.Substring(0, command.IndexOf(" ")) == "error")
            {

            }


        }





    }
}
