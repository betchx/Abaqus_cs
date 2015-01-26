using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class ELSet : SortedSet<Address>
    {
        public string name { get; private set; }
        public ELSet(string name)
        {
            this.name = name;
        }
        public void Add(string name, uint id) { base.Add(new Address(name, id)); }
        public ELSets parent { get; set; }
        public Model model { get; set; }
        public override string ToString()
        {
            return base.ToString() + "(" + name + ")[" + Count.ToString() + "]";
        }
    }
}
