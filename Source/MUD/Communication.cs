using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MUD
{
    public partial class Communication
    {
        #region Clients
        private static ConcurrentDictionary<int, Client> _clients = new ConcurrentDictionary<int, Client>();
        private static ICollection<Client> Clients { get { return _clients.Values; } }
        private static AsyncSocket server;
        private static bool _shutdown = false;
        public bool Shutdown { get { return _shutdown; } }

        private static void AddClient(Client client)
        {
            Client c;
            _clients.TryGetValue(client.SocketID, out c);
            if (c == null)
            {
                _clients.GetOrAdd(client.SocketID, client);
            }
            else
            {
                Console.WriteLine("Bad things happened: Socket ID already in use");
            }
        }

        private void RemoveClient(Client client)
        {
            Client c;
            _clients.TryRemove(client.SocketID, out c);
        }
        #endregion Clients

        private void SendToSocket(Socket socket, string data)
        {   
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            socket.BeginSend(byteData, 0, byteData.Length, 0, null, socket);
        }

        private void Interpret(Character ch, string command, string arguments)
        {

        }

        /// <summary>
        /// basic command interpretation placeholder for player input
        /// </summary>
        /// <param name="command"></param>
        private void DoCommand(Client client, string command)
        {
            // send a message to all connected clients
            if (command.IsEqual("echo"))
            {
                foreach (Client c in Clients)
                {
                    c.OutputBuffer.AppendLine("Do you hear that?..");
                }
            }
            else if (command.IsEqual("quit"))
            {
                client.Disconnect();
            }
            else if (command.IsEqual("shutdown"))
            {
                _shutdown = true;
            }
        }

        public void StartServer()
        {
            server = new AsyncSocket();
            server.StartListening();
        }

        public void EndServer()
        {
            foreach (Client client in Clients)
            {
                client.Connected = false;
                client.Socket.Shutdown(SocketShutdown.Both);
                client.Socket.Close();
            }
            _clients.Clear();
            server.StopListening();
        }

        public void ProcessClientInput()
        {
            foreach (Client client in Clients)
            {
                if (client.Connected && client.Commands.Count > 0)
                {
                    string command = client.Commands.Dequeue();
                    DoCommand(client, command);
                }
            }
        }

        public void ProcessClientOutput()
        {
            foreach (Client client in Clients)
            {
                if (client.OutputBuffer.Length > 0)
                {
                    SendToSocket(client.Socket, client.OutputBuffer.ToString());
                    client.OutputBuffer.Clear();
                }
            }
        }

        public void ProcessDisconnects()
        {
            foreach (Client client in Clients)
            {
                if (!client.Connected)
                {
                    RemoveClient(client);
                    client.Socket.Shutdown(SocketShutdown.Both);
                    client.Socket.Close();
                }
            }
        }
    }
}