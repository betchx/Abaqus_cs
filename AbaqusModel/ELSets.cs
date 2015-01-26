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

        public new void Add(string key, ELSet value)
        {
            // redirect
            this.Add(value);
        }

        public void Add(ELSet set)
        {
            set.parent = this;
            set.model = model;
            base.Add(set.name, set);

            // 親がモデルのときはallに登録
            if (parent.isMmodel) { model.all_elsets.Add(set.name, set); }
        }

        // Syntax suger
        public new ELSet this[string key] { get { ELSet set; return base.TryGetValue(key, out set) ? set : null; } }

        public override string ToString()
        {
            return "ELSets" + base.ToString();
        }
    }
}
