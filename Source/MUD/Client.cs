using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace MUD
{
    public class Client
    {
        public int CurrentBufferPosition { get; set; }
        public byte[] Buffer { get; set; }
        public Socket Socket { get; set; }
        public bool Connected { get; set; } = false;
        public Account Account { get; set; }
        public Character Character { get { return Account.Character; } }
        public Queue<string> Commands { get; set; }
        public StringBuilder OutputBuffer { get; set; }
        public int SocketID { get { return Socket.Handle.ToInt32(); } }

        public Client(Socket socket)
        {
            CurrentBufferPosition = 0;
            Buffer = new byte[Constants.BufferSize];
            Socket = socket;
            Account = new Account();
            Commands = new Queue<string>();
            OutputBuffer = new StringBuilder();
        }

        public void Disconnect()
        {
            OutputBuffer.AppendLine("Farewell");
            Connected = false;
        }

        public void Connect()
        {
            OutputBuffer.AppendLine("Welcome, friend");
            Connected = true;
        }
    }
}
