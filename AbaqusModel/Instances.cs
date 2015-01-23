using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Instances : SortedDictionary<string, Instance>
    {
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

    }
}
