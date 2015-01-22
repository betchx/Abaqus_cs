using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class ELSet : SortedSet<Element>
    {
        public string name { get; private set; }
        public ELSet(string name)
        {
            this.name = name;
        }
        public ELSets parent { get; set; }
        public Model model { get; set; }
    }
}
