using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Instances : SortedDictionary<string, Instance>
    {
        public Instances(Model model)
        {
            this.model = model;
        }
        public Model model { get; set; }

        public new void Add(string s, Instance i)
        {
            // redirect
            this.Add(i);
        }

        public void Add(Instance i)
        {
            i.parent = this;
            i.model = model;
            base.Add(i.name, i);

        }

        public Model parent { get { return model; } }

        // Syntax suger
        public new Instance this[string key] { get { Instance ins; return base.TryGetValue(key, out ins) ? ins : null; } }


    }
}
