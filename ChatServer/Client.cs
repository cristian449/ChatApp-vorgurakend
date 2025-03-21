using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatServer.Net.IO;

namespace ChatServer
{
     class Client
    {

        public string Username { get; set; }
        public Guid UID { get; set; }
        
        public TcpClient ClientSocket { get; set; }


        _packetReader = _packetReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            var OpCode = new byte[1];



            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username:  {Username}" );
        }
    }
}
