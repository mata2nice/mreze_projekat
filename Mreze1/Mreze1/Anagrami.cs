using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mreze1
{
    internal class Anagrami
    {
        public string OriginalnaRec { get; set; }
        public string PredlozeniAnagram { get; set; }

        public void UcitajRec()
        {
            OriginalnaRec = "CelziOsvaja";
        }

        public bool ProveriAnagram(string anagram) 
        {
            PredlozeniAnagram = anagram;

            if (anagram.Length != OriginalnaRec.Length)
            {
                return false;
            }

            string sortiranaOriginalna = new string(OriginalnaRec.OrderBy(c => c).ToArray());

            string sortiraniAnagram = new string(anagram.OrderBy(c => c).ToArray());

            return sortiranaOriginalna == sortiraniAnagram;
        }
    }
}
