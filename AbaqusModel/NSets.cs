using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class NSets : SortedDictionary<string, NSet>
    {
        public Model model { get; set; }
        public Part parent { get; set; }

        public new void Add(string s, NSet nset)
        {
            this.Add(nset);
        }

        internal void Add(NSet nset)
        {
            nset.parent = this;
            nset.model = model;
            base.Add(nset.name, nset);
            // 親がモデルのときはallに登録
            if (parent.isModel) { model.all_nsets.Add(nset.name, nset); }
        }


        // Syntax suger
        public new NSet this[string key] { get { NSet set; return this.TryGetValue(key, out set) ? set : null; } }


        public override string ToString()
        {
            return "NSets:" + base.ToString();
        }
    }
}
