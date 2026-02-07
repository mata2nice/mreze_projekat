using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Klijent
{
    internal class Klijent
    {
        static void Main(string[] args)
        {
            // --- UDP PRIJAVA ---
            Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50000);

            Console.Write("Unesite ime ili nadimak: ");
            string ime = Console.ReadLine();

            Console.Write("Unesite igre koje zelite da igrate (an, po, as), odvojene zarezima: ");
            string igre = Console.ReadLine();

            string prijava = "PRIJAVA: " + ime + ", " + igre;
            byte[] prijavaBuf = Encoding.UTF8.GetBytes(prijava);
            udpSocket.SendTo(prijavaBuf, serverEP);

            Console.WriteLine("Poslata prijava serveru:");
            Console.WriteLine(prijava);

            // --- PRIJEM TCP INFO ---
            byte[] buffer = new byte[1024];
            EndPoint odgovorEP = new IPEndPoint(IPAddress.Any, 0);
            int br = udpSocket.ReceiveFrom(buffer, ref odgovorEP);
            string odgovor = Encoding.UTF8.GetString(buffer, 0, br);

            Console.WriteLine("Primljen odgovor od servera:");
            Console.WriteLine(odgovor);

            string[] delovi = odgovor.Substring(4).Split(',');
            string tcpIP = delovi[0].Trim();
            int tcpPort = int.Parse(delovi[1].Trim());

            // --- TCP KONEKCIJA ---
            Socket tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint tcpEP = new IPEndPoint(IPAddress.Parse(tcpIP), tcpPort);
            tcpSocket.Connect(tcpEP);

            Console.WriteLine("TCP konekcija uspostavljena sa serverom.");

            // --- POZDRAV ---
            byte[] pozdravBuf = new byte[1024];
            br = tcpSocket.Receive(pozdravBuf);
            string pozdrav = Encoding.UTF8.GetString(pozdravBuf, 0, br);

            Console.WriteLine("Primljena poruka od servera:");
            Console.WriteLine(pozdrav);

            // --- START ---
            byte[] startBuf = Encoding.UTF8.GetBytes("START");
            tcpSocket.Send(startBuf);
            Console.WriteLine("Poslat START.");

            // --- PRIJEM IGRE (AN / PO) ---
            byte[] gameBuf = new byte[1024];
            br = tcpSocket.Receive(gameBuf);
            string gamePoruka = Encoding.UTF8.GetString(gameBuf, 0, br);

            Console.WriteLine(gamePoruka);

            // ===== ANAGRAMI =====
            if (gamePoruka.StartsWith("ANAGRAM_REČ: "))
            {
                string originalnaRec = gamePoruka.Substring("ANAGRAM_REČ: ".Length);

                Console.Write("Unesite anagram za datu rec: ");
                string mojAnagram = Console.ReadLine();

                string anagramPoruka = "ANAGRAM: " + mojAnagram;
                byte[] anagramBuf = Encoding.UTF8.GetBytes(anagramPoruka);
                tcpSocket.Send(anagramBuf);

                Console.WriteLine("Anagram poslat serveru.");
            }
            // ===== PITANJA I ODGOVORI =====
            else if (gamePoruka.StartsWith("PO_PITANJE: "))
            {
                string s = gamePoruka.Substring("PO_PITANJE: ".Length);
                string[] deloviPO = s.Split('|');

                Console.WriteLine(deloviPO[0]); // pitanje

                char slovo = 'A';

                for (int i = 1; i < deloviPO.Length; i++)
                {
                    Console.WriteLine($"{slovo}) {deloviPO[i]}");
                    slovo++;
                }


                Console.Write("Izaberite odgovor (A/B/C/D): ");
                string unos = Console.ReadLine().Trim().ToUpper();

                int izbor = -1;

                if (unos == "A") izbor = 0;
                else if (unos == "B") izbor = 1;
                else if (unos == "C") izbor = 2;
                else if (unos == "D") izbor = 3;
                else
                {
                    Console.WriteLine("Neispravan izbor.");
                    return;
                }

                string odgovorPO = "PO_ODGOVOR: " + izbor;
                byte[] odgBuf = Encoding.UTF8.GetBytes(odgovorPO);
                tcpSocket.Send(odgBuf);

                Console.WriteLine("PO odgovor poslat serveru.");



            }
            else
            {
                Console.WriteLine("Pokrenuta igra ASOCIJACIJE.");

                string stanje = gamePoruka;

                while (true)
                {
                    Console.WriteLine(stanje);

                    Console.Write("Unesite potez (A1, B:ODGOVOR ili K:ODGOVOR): ");
                    string unos = Console.ReadLine();

                    byte[] unosBuf = Encoding.UTF8.GetBytes(unos);
                    tcpSocket.Send(unosBuf);

                    // primi novo stanje od servera
                    byte[] novoBuf = new byte[2048];
                    int brNovo = tcpSocket.Receive(novoBuf);
                    stanje = Encoding.UTF8.GetString(novoBuf, 0, brNovo);

                    // izlaz ako je gotovo
                    if (stanje.Contains("RESENA") || stanje.Contains("Kraj"))
                    {
                        Console.WriteLine(stanje);
                        break;
                    }
                }
            }

            Console.ReadLine();
        }
    }
}
