using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ChatClient.Net.IO;

namespace ChatClient.Net
{
    class Server
    {
        TcpClient _client;
        PacketBuilder _packetBuilder;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action msgrecievedEvent;
        public event Action userdisconnectevent;

        public Server()
            {

            _client = new TcpClient();
            }

        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {

                //This no work nevermind it work now
                _client.Connect("127.0.0.1", 8000);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var ConnectPacket = new PacketBuilder();
                    ConnectPacket.WriteOpCode(0);
                    ConnectPacket.WriteMessage(username);
                    _client.Client.Send(ConnectPacket.GetPacketBytes());
                }
                ReadPackets();

            }
        }

        private void ReadPackets() 
        {

            Task.Run(() => 
            { 
            
            while (true )
                {
                    var opcode = PacketReader.ReadByte();

                    switch (opcode) 
                    {
                        case 1:
                            connectedEvent?.Invoke();

                            break;

                        case 5:
                            msgrecievedEvent?.Invoke();

                            break;

                        case 10:
                            userdisconnectevent?.Invoke();

                            break;


                        default:

                            Console.WriteLine("AHHHHHHH YEEEEEEEEZ");

                            break;

                    }
                }
            
            
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            _client.Client.Send(messagePacket.GetPacketBytes());
            

        }
    }
}
