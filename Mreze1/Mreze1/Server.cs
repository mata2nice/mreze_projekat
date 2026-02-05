using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mreze1
{
    internal class Server
    {
        static void Main(string[] args)
        {

            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50000);
            udpSocket.Bind(serverEP);

            Console.WriteLine("UDP server za prijavu igraca je pokrenut...");
            Console.WriteLine("Cekam prijave igraca...");

            Socket tcpListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint tcpEP = new IPEndPoint(IPAddress.Any, 51000);
            tcpListenSocket.Bind(tcpEP);
            tcpListenSocket.Listen(10);

            Console.WriteLine("TCP server (listen) spreman na portu 51000...");


            while (true)
            { 
                byte[] buffer = new byte[1024];
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);

                int ReceivedBytes = udpSocket.ReceiveFrom(buffer, ref clientEP);
                string poruka = Encoding.UTF8.GetString(buffer, 0, ReceivedBytes);
                Console.WriteLine("Primljena poruka od " + clientEP.ToString());
                Console.WriteLine(poruka);
                Console.WriteLine("--------------------");

                if (poruka.StartsWith("PRIJAVA:"))
                {
                    string sadrzaj = poruka.Substring(8).Trim();
                    string[] delovi = sadrzaj.Split(',');

                    string ime = delovi[0].Trim();

                    bool ispravnaPrijava = true;

                    Console.WriteLine("Ime igraca: " + ime);
                    Console.WriteLine("Izabrane igre:");

                    for (int i = 1; i < delovi.Length; i++)
                    {
                        string igra = delovi[i].Trim();

                        if (igra != "an" && igra != "po" && igra != "as")
                        {
                            ispravnaPrijava = false;
                        }
                        else
                        {
                            Console.WriteLine("- " + igra);
                        }
                    }

                    if (!ispravnaPrijava)
                    {
                        Console.WriteLine("Prijava NIJE ispravna – nepoznata igra.");
                    }
                    else
                    {
                        Console.WriteLine("Prijava je ispravna.");

                        // 1) Saljemo TCP info klijentu (UDP odgovor)
                        string tcpIP = "127.0.0.1";
                        int tcpPort = 51000;

                        string odgovor = "TCP: " + tcpIP + ", " + tcpPort;
                        byte[] odgovorBuffer = Encoding.UTF8.GetBytes(odgovor);
                        udpSocket.SendTo(odgovorBuffer, clientEP);

                        Console.WriteLine("Poslat TCP info klijentu.");

                        // 2) Cekamo TCP konekciju
                        Console.WriteLine("Cekam TCP konekciju...");
                        Socket tcpClientSocket = tcpListenSocket.Accept();
                        Console.WriteLine("TCP konekcija uspostavljena!");

                        // 3) Saljemo pozdravnu poruku
                        string pozdrav =
                            "Dobrodosli u trening igru kviza Kviskoteka, danasnji takmicar je " + ime;

                        byte[] pozdravBuf = Encoding.UTF8.GetBytes(pozdrav);
                        tcpClientSocket.Send(pozdravBuf);

                        Console.WriteLine("Poslat pozdrav klijentu.");

                        // 4) Cekamo START
                        byte[] startBuf = new byte[256];
                        int br = tcpClientSocket.Receive(startBuf);
                        string startMsg = Encoding.UTF8.GetString(startBuf, 0, br);

                        if (startMsg.Trim().ToUpper() == "START")
                        {
                            Console.WriteLine("Primljen START. Kviz moze da pocne!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Neispravan format poruke.");
                }



            }
        }
    }
}
