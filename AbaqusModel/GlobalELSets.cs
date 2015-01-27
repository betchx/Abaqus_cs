using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class GlobalELSets : SortedDictionary<string, ELSet>
    {
        public Model model { get; private set; }
        public Part parent { get; private set; }
        public GlobalELSets(Model model)
        {
            this.model = model;
            this.parent = model;
        }
        public void Add(ELSet set)
        {
            // 注意 setのparentやmodelは設定しない．
            base.Add(set.name, set);
        }

    }
}
