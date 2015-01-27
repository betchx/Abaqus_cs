using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class GlobalNSets : SortedDictionary<string, NSet>
    {
        public Model model { get; private set; }
        public Part parent { get; private set; }
        public GlobalNSets(Model model)
        {
            this.model = model;
            this.parent = model;
        }
        public void Add(NSet set)
        {
            // 注意 setのparentやmodelは設定しない．
            base.Add(set.name, set);
        }
    }
}
