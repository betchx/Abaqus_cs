using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class GlobalElements : SortedDictionary<Address,Element>
    {
        public Model model { get; private set; }
        public Part parent { get; private set; }

        public GlobalElements(Model model)
        {
            this.model = model;
            this.parent = model;
        }
    }
}
