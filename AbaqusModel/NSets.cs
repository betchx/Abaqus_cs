using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class NSets : SortedDictionary<string, NSet>
    {
        public Model model { get; set; }

        private new void Add(string s, NSet n)
        {
            base.Add(s, n);
        }

        internal void Add(NSet nset)
        {
            nset.parent = this;
            nset.model = model;
            base.Add(nset.name, nset);
        }
    }
}
