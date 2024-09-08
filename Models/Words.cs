using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dictianary.Models
{
    public class Word
    {
        public string WordName { get; set; }
        public string Translation { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Word otherWord)
            {
                return WordName == otherWord.WordName;
            }
            return false;
        }

    }
}
