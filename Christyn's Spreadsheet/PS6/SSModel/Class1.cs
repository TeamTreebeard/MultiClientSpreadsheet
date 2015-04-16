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
        public Boolean connected;

        public event Action ConnectionNotFoundEvent;

        public event Action<String, String> updateCellEvent;
        public event Action<String> cellErrorEvent;

        public SSModel()
        {
            socket = null;
            connected = false;
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
                        socket.BeginSend("connect " + name +" "+ ss_name + "\n", (e, p) => { }, null); // Send a protocol specified message to connect
                        socket.BeginReceive(LineReceived, socket); // Wait for the server to respond with a match start protocol message.
                    }
                    catch (SocketException) // Could not connect to the server.
                    {
                        ConnectionNotFoundEvent();
                    }
                }

            }
        }
        
        //parses the string received from the server to determine what action to take
        //The first word is interpreted as the command.
        //If the command = connected, prepares the client to receive incoming cells
        //if the command = cell, updates the spreadsheet with that cell
        //if the command is error, takes the appropriate action for the error number
        //otherwise the client will ignore the string
        private void LineReceived(String s, Exception e, object p)
        {
            String info = "";
            String command = "";

            command = s.Substring(0, s.IndexOf(" "));
            if (command == "connected")
            {
                //don't really need to do anything with this info, but we have successfully connected to a SS so set identifier to true.
                connected = true;
                System.Diagnostics.Debug.Write(connected);

            }
            else if (command == "cell")
            {
                info = s.Substring(5);
                updateCellEvent(info.Substring(0, info.IndexOf(" ")), info.Substring(info.IndexOf(" ")+1));
            }
            else if (command == "error")
            {
                info = s.Substring(6, 1);
                if (Convert.ToInt32(info) == 0)
                {
                    //thanks for that SUPER HELPFUL ERROR - "General error"
                    System.Diagnostics.Debug.Write(info);
                }
                else if(Convert.ToInt32(info) == 1)
                {
                    cellErrorEvent(s.Substring(8));
                }
                else if (Convert.ToInt32(info) == 2)
                {
                    //thanks for ANOTHER SUPER HELPFUL ERROR - invalid command
                }
                else if (Convert.ToInt32(info) == 3)
                {
                    //can't perform in current state?
                }
                else if (Convert.ToInt32(info) == 4)
                {
                    registerUser(s.Substring(8));
                    System.Diagnostics.Debug.Write("here we are!");
                }
            }

            else
            {
                System.Diagnostics.Debug.Write(s);
            }

            socket.BeginReceive(LineReceived, null);
        }

        public void sendUndo()
        {
            try
            {
                socket.BeginSend("undo\nkdjfkjldsfkgjhd", (e, p) => { }, socket);
            }
            catch(SocketException)
            {
                // no undo should happen, we're probably not connected right now.
            }
        }


        public void sendCell(string name, string contents)
        {

            socket.BeginSend("cell " + name + " " + contents + "\n", (e, o) => { }, socket);
            socket.BeginReceive(LineReceived, null);
        }

        public void registerUser(string name)
        {
            socket.BeginSend("register " + name + "\n", (e, o) => { }, socket);
            socket.BeginReceive(LineReceived, null);
        }

        public void saveSheet()
        {
            socket.BeginSend("save\n", (e, o) => { }, socket);
        }
    }
}
