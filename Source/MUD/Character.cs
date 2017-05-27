using System.Net.Sockets;

namespace MUD
{
    public class Character
    {
        public Socket Socket { get { return Client.Socket; } }
        public string Name { get; set; }
        public Client Client { get; set; }
    }
}