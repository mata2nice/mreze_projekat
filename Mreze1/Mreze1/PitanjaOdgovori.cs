using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mreze1
{
    internal class PitanjaOdgovori
    {
        public string Pitanje { get; private set; }
        public string[] Odgovori { get; private set; }
        public int TacanIndeks { get; private set; }

        public void UcitajPitanje()
        {
            Pitanje = "Ko je osvojio Ligu sampiona 2021?";
            Odgovori = new string[]
            {
                "Chelsea",
                "Manchester City",
                "PSG",
                "Real Madrid"
            };
            TacanIndeks = 0; // Chelsea
        }

        public bool Proveri(int izbor)
        {
            return izbor == TacanIndeks;
        }

        public int Poeni()
        {
            return 10;
        }
    }
}
