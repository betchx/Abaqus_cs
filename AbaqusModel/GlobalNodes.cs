using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class GlobalNodes : SortedDictionary<Address,Node>
    {
        public Model model { get; private set; }
        public Part parent { get; private set; }
        public GlobalNodes(Model model)
        {
            this.model = model;
            this.parent = model;
        }
    }
}
