﻿using System;
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
        public event Action<String> genericErrorEvent;
        public event Action<String> commandErrorEvent;
        public event Action<String> usedNameEvent;
        public event Action<String> noSpreadSheetEvent;

        private Boolean registering = false;
        private string h_name, u_name, s_name;
        private int t_port;

        public SSModel()
        {
            socket = null;
            connected = false;
            h_name = "";
            t_port = -1;
            u_name = "";
            s_name = "";
        }


        public void Connect(string hostname, int port, string name, string ss_name)
        {
            h_name = hostname;
            t_port = port;
            u_name = name;
            s_name = ss_name;

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
                    socket.BeginSend("connect " + name + " " + ss_name + "\n", (e, p) => { }, null); // Send a protocol specified message to connect
                    socket.BeginReceive(LineReceived, socket); // Wait for the server to respond with a match start protocol message.
                }
                catch (SocketException) // Could not connect to the server.
                {
                    ConnectionNotFoundEvent();
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
                    info = s.Substring(10);
                    //don't really need to do anything with this info, but we have successfully connected to a SS so set identifier to true.
                    int receives = Convert.ToInt32(info);
                    for (int i = 0; i < receives; i++)
                    {
                        socket.BeginReceive(LineReceived, null);
                    }
                    registering = false;
                    connected = true;
                    return;

                }
                else if (command == "cell")
                {

                    info = s.Substring(5);
                    updateCellEvent(info.Substring(0, info.IndexOf(" ")), info.Substring(info.IndexOf(" ") + 1));
                }
                else if (command == "error")
                {
                    info = s.Substring(6, 1);
                    if (Convert.ToInt32(info) == 0)
                    {
                        //thanks for that SUPER HELPFUL ERROR - "General error"
                        genericErrorEvent(s.Substring(8));
                    }
                    else if (Convert.ToInt32(info) == 1)
                    {
                        cellErrorEvent(s.Substring(8));
                    }
                    else if (Convert.ToInt32(info) == 2)
                    {
                        commandErrorEvent(s.Substring(8));
                        //thanks for ANOTHER SUPER HELPFUL ERROR - invalid command
                    }
                    else if (Convert.ToInt32(info) == 3)
                    {
                        noSpreadSheetEvent(s.Substring(8));
                    }
                    else if (Convert.ToInt32(info) == 4)
                    {
                        if (!registering)
                        {
                            registerUser(s.Substring(8));
                            return;
                        }
                        else
                        {
                            usedNameEvent(s.Substring(8));
                            registering = false;
                        }
                    }
                }

                else
                {

                }
            

            socket.BeginReceive(LineReceived, null);
        }

        public void sendUndo()
        {
            try
            {
                socket.BeginSend("undo\n", (e, p) => { }, socket);
            }
            catch(Exception)
            {
                // no undo should happen, we're probably not connected right now.
            }
        }


        public void sendCell(string name, string contents)
        {

            try
            {
                System.Diagnostics.Debug.Write("Sending....");
                socket.BeginSend("cell " + name + " " + contents + "\n", (e, o) => { }, socket);
                socket.BeginReceive(LineReceived, null);
            }
            catch(Exception)
            {
                //boop nothing
            }
        }

        public void registerUser(string name) {
            try { 
            socket.BeginSend("connect sysadmin default\n", (e, o) => { }, socket);
            socket.BeginSend("register " +name+ "\n", (e, o) => { }, socket);
            registering = true;
            socket.Close();
            socket = null;
            connected = false;
            Connect(h_name, t_port, u_name, s_name);
            }
            catch(Exception)
            {

            }
        }

        public void closeSocket()
        {
            try
            {
                socket.Close();
                socket = null;
                connected = false;
            }
            catch(Exception)
            {

            }
        }
    }
}
