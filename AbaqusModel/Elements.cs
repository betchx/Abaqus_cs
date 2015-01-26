using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Elements : SortedDictionary<uint, Element>
    {
        public Model model { get; set; }

        /// <summary>
        ///   Add(e)のスタブ．
        /// </summary>
        /// <param name="i">無視される．（e.idが利用される為）</param>
        /// <param name="e">追加する要素</param>
        public new void Add(uint i /*ignored*/, Element e)
        {
            // redirect
            this.Add(e);
        }

        /// <summary>
        ///   要素を追加する．
        /// </summary>
        /// <param name="e"></param>
        public void Add(Element e)
        {
            e.parent = this;
            e.model = model;
            base.Add(e.id, e);
            model.all_elements.Add(new Address(parent.name, e.id), e);
        }

        public Part parent { get; set; }

        // Syntax suger
        public new Element this[uint index] { get { Element e; return base.TryGetValue(index, out e) ? e : null; } }

        public override string ToString()
        {
            return "Elements:" + base.ToString();
        }
    }
}
