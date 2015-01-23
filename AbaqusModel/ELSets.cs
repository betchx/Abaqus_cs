using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class ELSets: SortedDictionary<string, ELSet>
    {
        public Model model { get; set; }
        public Part parent { get; set; }

        public void Add(ELSet set)
        {
            set.parent = this;
            set.model = model;
            base.Add(set.name, set);

            // 親がモデルのときはallに登録
            if (parent.isMmodel) { model.all_elsets.Add(set.name, set); }
        }
    }
}
