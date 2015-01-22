using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    // Node Set
    public class NSet : SortedSet<Node>
    {
        public string name { get; private set; }
        public NSet(string name)
        {
            this.name = name;
        }
        public NSets parent { get; set; }

        public Model model { get; set; }
    }
}
