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
        static int sledeciId = 1;
        static List<Igrac> igraci = new List<Igrac>();
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

                    bool igraAnagram = false;
                    bool igraPO = false;
                    bool igraAS = false;


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

                            if (igra == "an")
                            {
                                igraAnagram = true;
                            }
                            if (igra == "po")
                            {
                                igraPO = true;
                            }
                            if (igra == "as")
                            {
                                igraAS = true;
                            }

                        }
                    }

                    if (!ispravnaPrijava)
                    {
                        Console.WriteLine("Prijava NIJE ispravna – nepoznata igra.");
                    }
                    else
                    {

                        // BROJ IGARA (ime je delovi[0], igre su od 1 nadalje)
                        int brojIgara = delovi.Length - 1;

                        // KREIRAMO NOVOG IGRACA
                        Igrac noviIgrac = new Igrac(sledeciId, ime, brojIgara);

                        // DODAJEMO U LISTU
                        igraci.Add(noviIgrac);

                        Console.WriteLine("Kreiran igrac sa ID = " + sledeciId);

                        // POVECAVAMO ID ZA SLEDECEG
                        sledeciId++;


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

                            if (igraAnagram)
                            {
                                Console.WriteLine("Pokrece se igra ANAGRAMI.");

                                // --- ANAGRAMI ---
                                Anagrami anagrami = new Anagrami();
                                anagrami.UcitajRec("CelziOsvaja");

                                string porukaAnagram = "ANAGRAM_REČ: " + anagrami.OriginalnaRec;
                                byte[] anagramBuf = Encoding.UTF8.GetBytes(porukaAnagram);
                                tcpClientSocket.Send(anagramBuf);

                                Console.WriteLine("Poslata rec za anagram: " + anagrami.OriginalnaRec);

                                // PRIJEM ANAGRAMA OD KLIJENTA
                                byte[] odgovorBuf = new byte[1024];
                                int brOdgovor = tcpClientSocket.Receive(odgovorBuf);
                                string anagramPoruka = Encoding.UTF8.GetString(odgovorBuf, 0, brOdgovor);

                                string anagram = anagramPoruka.Substring("ANAGRAM: ".Length);

                                bool tacno = anagrami.ProveriAnagram(anagram);

                                if (tacno)
                                {
                                    int poeni = anagrami.IzracunajPoene();
                                    Console.WriteLine("Anagram TACAN! Poeni: " + poeni);
                                }
                                else
                                {
                                    Console.WriteLine("Anagram NIJE tacan.");
                                }
                            }
                            if (igraPO)
                            {
                                Console.WriteLine("Pokrece se igra PITANJA I ODGOVORI.");

                                PitanjaOdgovori po = new PitanjaOdgovori();
                                po.UcitajPitanje();

                                // Saljemo pitanje i odgovore
                                string porukaPO = "PO_PITANJE: " + po.Pitanje + "|" +
                                                  string.Join("|", po.Odgovori);
                                byte[] poBuf = Encoding.UTF8.GetBytes(porukaPO);
                                tcpClientSocket.Send(poBuf);

                                Console.WriteLine("Poslato pitanje za PO.");

                                // Prijem izbora (npr. "PO_ODGOVOR: 0")
                                byte[] izborBuf = new byte[256];
                                int brIzbor = tcpClientSocket.Receive(izborBuf);
                                string izborPoruka = Encoding.UTF8.GetString(izborBuf, 0, brIzbor);

                                int izbor = int.Parse(izborPoruka.Substring("PO_ODGOVOR: ".Length));

                                if (po.Proveri(izbor))
                                {
                                    int poeni = po.Poeni();
                                    Console.WriteLine("PO tacno! Poeni: " + poeni);
                                }
                                else
                                {
                                    Console.WriteLine("PO netacno.");
                                }
                            }
                            if (igraAS) 
                            {
                                Console.WriteLine("Pokrece se igra ASOCIJACIJE.");
                                Asocijacije asoc = new Asocijacije();
                                asoc.UcitaAsocijaciju("asocijacije.txt");

                                // 2) Slanje pocetnog stanja
                                string stanje = FormatirajAsocijaciju(asoc);
                                byte[] stanjeBuf = Encoding.UTF8.GetBytes(stanje);
                                tcpClientSocket.Send(stanjeBuf);

                                int greske = 0;

                                // 3) Petlja igre (do 5 gresaka ili resenja)
                                while (greske < 5)
                                {
                                    byte[] unosBuf = new byte[256];
                                    int brUnos = tcpClientSocket.Receive(unosBuf);
                                    string unos = Encoding.UTF8.GetString(unosBuf, 0, brUnos).Trim();

                                    Console.WriteLine("Primljen unos: " + unos);

                                    // A1, B3, itd – otvaranje polja
                                    if (unos.Length == 2 && char.IsLetter(unos[0]) && char.IsDigit(unos[1]))
                                    {
                                        int kol = char.ToUpper(unos[0]) - 'A';
                                        int red = unos[1] - '1';

                                        if (kol >= 0 && kol < 4 && red >= 0 && red < 4)
                                        {
                                            asoc.OtvoriPolje(kol, red);
                                        }
                                    }
                                    // A:ODGOVOR – pokusaj kolone
                                    else if (unos.Length > 2 && unos[1] == ':')
                                    {
                                        int kol = char.ToUpper(unos[0]) - 'A';
                                        string odgovorKolone = unos.Substring(2);

                                        if (!asoc.Kolone[kol][4]
                                            .Equals(odgovorKolone, StringComparison.OrdinalIgnoreCase))
                                        {
                                            greske++;
                                        }
                                    }
                                    // K:ODGOVOR – konacno resenje
                                    else if (unos.StartsWith("K:"))
                                    {
                                        string konacniOdgovor = unos.Substring(2);

                                        if (asoc.KonacnoResenje
                                            .Equals(konacniOdgovor, StringComparison.OrdinalIgnoreCase))
                                        {
                                            Console.WriteLine("Asocijacija RESENA.");
                                            break;
                                        }
                                        else
                                        {
                                            greske++;
                                        }
                                    }

                                    // 4) Slanje novog stanja nakon svakog poteza
                                    string novoStanje = FormatirajAsocijaciju(asoc);
                                    byte[] novoBuf = Encoding.UTF8.GetBytes(novoStanje);
                                    tcpClientSocket.Send(novoBuf);
                                }

                                Console.WriteLine("Kraj igre ASOCIJACIJE.");
                            }

                            else
                            {
                                // Console.WriteLine("Igra ANAGRAMI nije izabrana.");
                            }
                        }
                        

                    }
                }
                else
                {
                    Console.WriteLine("Neispravan format poruke.");
                }



            }
        }

        static string FormatirajAsocijaciju(Asocijacije a)
        {
            StringBuilder sb = new StringBuilder();

            for (int k = 0; k < 4; k++)
            {
                char kol = (char)('A' + k);

                for (int r = 0; r < 4; r++)
                {
                    sb.AppendLine(
                        $"{kol}{r + 1}: " +
                        (a.Otvoreno[k][r] ? a.Kolone[k][r] : "???")
                    );
                }

                sb.AppendLine($"{kol}: ???");
            }

            return sb.ToString();
        }


    }
}
