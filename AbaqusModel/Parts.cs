using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Parts: SortedDictionary<string, Part>
    {
        public Model model { get; set; }
        public void Add(Part part)
        {
            part.parent = this;
            part.model = model;
            this.Add(part.name, part);
        }
    }
}
