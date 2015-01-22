using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Instances : SortedDictionary<string, Instance>
    {
        public Model model { get; set; }

        private new void Add(string s, Instance i)
        {
            base.Add(s, i);
        }

        public void Add(Instance i)
        {
            i.parent = this;
            i.model = model;
            base.Add(i.name, i);
        }


    }
}
