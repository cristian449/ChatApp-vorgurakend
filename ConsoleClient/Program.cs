using System;
using System.Net.Sockets;
using System.Threading;
using ChatClient.Net.IO;

namespace ConsoleClient
{
    class Program
    {
        static PacketReader _packetReader;
        static string username;

        static void Main(string[] args)
        {
            Console.Title = "Console Chat Client";

            Console.Write("Sisesta oma kasutajanimi: ");
            username = Console.ReadLine();

            var client = new TcpClient();
            client.Connect("127.0.0.1", 8000);

            var stream = client.GetStream();
            _packetReader = new PacketReader(stream);

            // Send initial connection packet
            var connectPacket = new PacketBuilder();
            connectPacket.WriteOpCode(0); // või mis iganes opcode server ootab alguses
            connectPacket.WriteMessage(username);
            stream.Write(connectPacket.GetPacketBytes());

            Console.Clear();
            Console.WriteLine($"[{DateTime.Now}] Ühendatud serveriga kui {username}.");
            Console.WriteLine("Kirjuta sõnum ja vajuta ENTER.");

            // Start listener thread
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        var opcode = _packetReader.ReadByte();
                        switch (opcode)
                        {
                            case 1: // new user connected
                                string newUser = _packetReader.ReadMessage();
                                string uid = _packetReader.ReadMessage();
                                Console.WriteLine($"[{DateTime.Now}] {newUser} liitus vestlusega.");
                                break;
                            case 5: // message received
                                string msg = _packetReader.ReadMessage();
                                Console.WriteLine(msg);
                                break;
                            case 10: // user disconnected
                                string discUid = _packetReader.ReadMessage();
                                Console.WriteLine($"[{DateTime.Now}] Kasutaja lahkus ({discUid})");
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Serveriga ühendus katkes.");
                        break;
                    }
                }
            }).Start();

            // Main input loop
            while (true)
            {
                string message = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    var msgPacket = new PacketBuilder();
                    msgPacket.WriteOpCode(5);
                    msgPacket.WriteMessage(message);
                    stream.Write(msgPacket.GetPacketBytes());
                }
            }
        }
    }
}
