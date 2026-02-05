using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mreze1
{
    internal class Igrac
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public int[] PoeniPoIgrama { get; set; } 
        public bool Kvisko { get; set; }


        public Igrac(int id, string ime, int brojIgara)
        {
            Id = id;
            Ime = ime;
            PoeniPoIgrama = new int[brojIgara];
            Kvisko = false;
        }
    }
}
