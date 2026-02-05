using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Klijent
{
    internal class Klijent
    {
        static void Main(string[] args)
        {

            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50000);

            Console.Write("Unesite ime ili nadimak: ");
            string ime = Console.ReadLine();

            Console.Write("Unesite igre koje zelite da igrate (an, po, as), odvojene zarezima: ");
            string igre = Console.ReadLine();


            // Formiranje PRIJAVA poruke
            string prijava = "PRIJAVA: " + ime + ", " + igre;

            byte[] prijavaBuffer = Encoding.UTF8.GetBytes(prijava);
            udpSocket.SendTo(prijavaBuffer, serverEP);


            Console.WriteLine("Poslata prijava serveru:");
            Console.WriteLine(prijava);



            byte[] buffer = new byte[1024];
            EndPoint odgovorEP = new IPEndPoint(IPAddress.Any, 0);

            int receivedBytes = udpSocket.ReceiveFrom(buffer, ref odgovorEP);
            string odgovor = Encoding.UTF8.GetString(buffer, 0, receivedBytes);

            Console.WriteLine("Primljen odgovor od servera:");
            Console.WriteLine(odgovor);

            // Ocekivani format: TCP: ip, port
            string[] delovi = odgovor.Substring(4).Split(',');

            string tcpIP = delovi[0].Trim();
            int tcpPort = int.Parse(delovi[1].Trim());

           
            // 3. TCP KONEKCIJA
           

            Socket tcpSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);

            IPEndPoint tcpEP = new IPEndPoint(IPAddress.Parse(tcpIP),tcpPort);

            tcpSocket.Connect(tcpEP);
            Console.WriteLine("TCP konekcija uspostavljena sa serverom.");

            // 1) PRIMI POZDRAV OD SERVERA
            byte[] pozdravBuf = new byte[1024];
            int br = tcpSocket.Receive(pozdravBuf);
            string pozdrav = Encoding.UTF8.GetString(pozdravBuf, 0, br);

            Console.WriteLine("Primljena poruka od servera:");
            Console.WriteLine(pozdrav);

            // 2) POSALJI START
            string start = "START";
            byte[] startBuf = Encoding.UTF8.GetBytes(start);
            tcpSocket.Send(startBuf);

            Console.WriteLine("Poslat START.");

            // PRIJEM RECI ZA ANAGRAM
            byte[] recBuf = new byte[1024];
            int brRec = tcpSocket.Receive(recBuf);
            string porukaRec = Encoding.UTF8.GetString(recBuf, 0, brRec);

            Console.WriteLine(porukaRec);

            // Izdvajamo rec
            string originalnaRec = porukaRec.Substring("ANAGRAM_REČ: ".Length);

            Console.Write("Unesite anagram za datu rec: ");
            string mojAnagram = Console.ReadLine();

            // Saljemo anagram serveru
            string anagramPoruka = "ANAGRAM: " + mojAnagram;
            byte[] mojAnagramBuf = Encoding.UTF8.GetBytes(anagramPoruka);
            tcpSocket.Send(mojAnagramBuf);

            Console.WriteLine("Anagram poslat serveru.");


            Console.ReadLine();

        }
    }
}
