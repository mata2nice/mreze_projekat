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


            }
        }
    }
}
