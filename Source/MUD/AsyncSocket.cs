﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MUD
{
    public partial class Communication
    {
        class AsyncSocket
        {
            private Socket listener;

            public AsyncSocket()
            {
            }

            public void StartListening()
            {
                StartListening(8080, 20);
            }

            public void StartListening(int port, int maxBacklog)
            {
                byte[] bytes = new Byte[1024];

                // establish listener endpoint
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = null;

                // find an ipv4 address to listen on
                foreach (var adr in ipHostInfo.AddressList)
                {
                    if (adr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipAddress = adr;
                        break;
                    }
                }
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

                // create a tcp/ip socket
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // bind socket to local endpoint and listen for incoming connections
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(maxBacklog);
                    Console.WriteLine("Server up and running on {0}:{1}", ipAddress.ToString(), port);

                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            public void StopListening()
            {
                listener.Close();
            }

            private void AcceptCallback(IAsyncResult ar)
            {
                if (!_shutdown)
                {
                    // get the socket that handles the client request
                    Socket listener = (Socket)ar.AsyncState;
                    Socket socket = listener.EndAccept(ar);

                    Console.WriteLine("Client connected: {0}", socket.Handle.ToString());

                    Client client = new Client(socket);
                    client.Connected = true;
                    socket.BeginReceive(client.Buffer, 0, Constants.BufferSize, 0,
                        new AsyncCallback(ReadCallback), client);

                    client.Connect();
                    AddClient(client);
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                }
            }

            private void ReadCallback(IAsyncResult ar)
            {
                string command = string.Empty;

                Client client = (Client)ar.AsyncState;
                Socket socket = client.Socket;

                if (client.Connected)
                {
                    int bytesRead = socket.EndReceive(ar);

                    if (bytesRead > 0)
                    {
                        command = Encoding.ASCII.GetString(client.Buffer, 0, bytesRead - 2);

                        Console.WriteLine("Read {0} bytes from client. Data: {1}", command.Length, command);

                        client.Commands.Enqueue(command);

                        socket.BeginReceive(client.Buffer, 0, Constants.BufferSize, 0,
                            new AsyncCallback(ReadCallback), client);
                    }
                    else if (bytesRead == 0)
                    {
                        Console.WriteLine("Client disconnected unexpectedly");
                        client.Disconnect();

                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                }
            }
        }
    }
}