using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Nodes : SortedDictionary<uint, Node>
    {
        public Model model { get; set; }

        public new void Add(uint i, Node n)
        {
            // redirect
            this.Add(n);
        }
        public void Add(Node n)
        {
            n.model = model;
            n.parent = this;
            base.Add(n.id, n);
            model.global_nodes.Add(new Address(parent.name, n.id), n);
        }

        public Part parent { get; set; }

        // Syntax suger
        public new Node this[uint index] { get { Node n; return TryGetValue(index, out n) ? n : null; } }

        public override string ToString()
        {
            return "Nodes:" + base.ToString();
        }
    }
}
