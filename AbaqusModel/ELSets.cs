using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class ELSets: SortedDictionary<string, ELSet>
    {
        public Model model { get; set; }

        public void Add(ELSet set)
        {
            set.parent = this;
            set.model = model;
            Add(set.name, set);
        }
    }
}
