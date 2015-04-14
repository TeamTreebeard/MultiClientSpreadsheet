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

        public event Action ConnectionNotFoundEvent;

        public event Action<String, String> updateCellEvent;
        private int cellCount = 0;

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
                        ConnectionNotFoundEvent();
                    }
                }

            }
        }


        private void LineReceived(String s, Exception e, object p)
        {
            String info = "";
            String command = "";

            command = s.Substring(0, command.IndexOf(" "));

            if (command == "connected")
            {

            }
            else if (command == "cell")
            {
                info = s.Substring(5);
                updateCellEvent(s.Substring(5, s.IndexOf(" ")), s.Substring(s.IndexOf(" ")+1));
            }
            else if (command == "error")
            {
                info = s.Substring(6, 1);
                if (Convert.ToInt32(info) == 0)
                {
                    //do error stuff?
                }
                else if(Convert.ToInt32(info) == 1)
                {
                    //do error stuff?
                }
                else if (Convert.ToInt32(info) == 2)
                {
                    //do error stuff?
                }
                else if (Convert.ToInt32(info) == 3)
                {
                    //do error stuff?
                }
                if (Convert.ToInt32(info) == 4)
                {
                    //do error stuff?
                }
            }

            socket.BeginReceive(LineReceived, null);
        }

        public void sendUndo()
        {
            socket.BeginSend("undo\n", (e, p) => { }, socket);
        }


        public void sendCell(string name, string contents)
        {

            socket.BeginSend("cell " + name + " " + contents + "\n", (e, o) => { }, socket);
            socket.BeginReceive(LineReceived, null);
        }
    }
}
