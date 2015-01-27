using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Part
    {
        public Part(string name, Model model)
        {
            this.name= name;
            this.model = model;

            nodes = new Nodes();
            elements = new Elements();
            nsets = new NSets();
            elsets = new ELSets();

            nodes.parent = this;
            elements.parent = this;
            nsets.parent = this;
            elsets.parent = this;

            nodes.model = model;
            elements.model = model;
            nsets.model = model;
            elsets.model = model;
        }

        public string name { get;  set; }
        public Nodes nodes { get; set; }
        public Elements elements { get; set; }
        public NSets nsets { get; set; }
        public ELSets elsets { get; set; }

        public Parts parent { get; set; }
        public Model model { get; set; }
        public virtual bool isModel{ get { return false; }}
    }
}
