using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Nodes : SortedDictionary<uint, Node>
    {
        public Model model { get; set; }
        private new void Add(uint i, Node n)
        {
            base.Add(i, n);
        }
        public void Add(Node n)
        {
            n.model = model;
            n.parent = this;
            base.Add(n.id, n);
            model.all_nodes.Add(new Address(parent.name, n.id), n);
        }

        public Part parent { get; set; }
    }
}
