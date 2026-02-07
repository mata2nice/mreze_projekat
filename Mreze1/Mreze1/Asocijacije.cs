using System.IO;

namespace Mreze1
{
    internal class Asocijacije
    {
        // 4 kolone, svaka ima 4 polja + resenje kolone
        public string[][] Kolone { get; private set; }

        // da li su polja otvorena (samo 4 po koloni)
        public bool[][] Otvoreno { get; private set; }

        // konacno resenje asocijacije
        public string KonacnoResenje { get; private set; }

        public Asocijacije()
        {
            Kolone = new string[4][];
            Otvoreno = new bool[4][];

            for (int i = 0; i < 4; i++)
            {
                Kolone[i] = new string[5];   // 4 polja + resenje kolone
                Otvoreno[i] = new bool[4];   // samo polja
            }
        }

        // UCITAVANJE IZ FAJLA
        public void UcitaAsocijaciju(string putanja)
        {
            string[] linije = File.ReadAllLines(putanja);

            foreach (string linija in linije)
            {
                string[] delovi = linija.Split(';');

                if (delovi[0] == "K")
                {
                    KonacnoResenje = delovi[1];
                }
                else
                {
                    int kolona = delovi[0][0] - 'A';

                    for (int i = 0; i < 5; i++)
                        Kolone[kolona][i] = delovi[i + 1];

                    for (int i = 0; i < 4; i++)
                        Otvoreno[kolona][i] = false;
                }
            }
        }

        public string OtvoriPolje(int kolona, int red)
        {
            Otvoreno[kolona][red] = true;
            return Kolone[kolona][red];
        }

        
    }
}
