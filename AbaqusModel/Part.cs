using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Part
    {

        public Part(string name)
        {
            this.name= name;
        }

        public string name { get;  set; }

        public Nodes nodes { get; set; }
        public Elements elements { get; set; }
        public NSets nsets { get; set; }
        public ELSets elsets { get; set; }
        public Parts parent { get; set; }
        public Model model { get; set; }
    }
}
