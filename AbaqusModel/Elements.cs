using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Elements : SortedDictionary<uint, Element>
    {
        public Model model { get; set; }

        private new void Add(uint i, Element e)
        {
            base.Add(i, e);
        }
        public void Add(Element e)
        {
            e.parent = this;
            e.model = model;
            base.Add(e.id, e);
            model.all_elements.Add(new Address(parent.name, e.id), e);
        }

        public Part parent { get; set; }
    }
}
