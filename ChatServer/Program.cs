using System.Net;
using System.Net.Sockets;
using ChatServer.Net.IO;

namespace ChatServer
{
    class Program
    {
        static List<Client> _users;
        static TcpListener _listner;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listner = new TcpListener(IPAddress.Parse("127.0.0.1"), 8000);
            _listner.Start();


            while (true)
            {

                var client = new Client(_listner.AcceptTcpClient());
                _users.Add(client);

                //Broadcase the connection to everyone on the server

                BroadCastConnection();
            }


            //Console.WriteLine("Client Has Connected YAY");
        }


        static void BroadCastConnection()
        {
            foreach (var user in _users)
            {
                foreach (var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }

        }
    }

}