using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    // Node Set
    public class NSet : SortedSet<Address>
    {
        public string name { get; private set; }
        public NSet(string name)
        {
            this.name = name;
        }
        public void Add(string name, uint id) { base.Add(new Address(name, id)); }
        public NSets parent { get; set; }

        public Model model { get; set; }
        public override string ToString()
        {
            return base.ToString() + "(" + name + ")[" +Count.ToString() + "]";
        }
    }
}
