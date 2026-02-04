using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mreze1
{
    internal class Program
    {
        static void Main(string[] args)
        {

            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Any, 50000);
            udpSocket.Bind(serverEP);

            Console.WriteLine("UDP server za prijavu igraca je pokrenut...");
            Console.WriteLine("Cekam prijave igraca...");

            while (true)
            { 
                byte[] buffer = new byte[1024];
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);

                int ReceivedBytes = udpSocket.ReceiveFrom(buffer, ref clientEP);
                string poruka = Encoding.UTF8.GetString(buffer, 0, ReceivedBytes);
                Console.WriteLine("Primljena poruka od" + clientEP.ToString());
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
                        // TEK OVDE kasnije:
                        // - saljes TCP port
                        // - cuvas igraca
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
