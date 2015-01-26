using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Element: IComparable<Element>, IEnumerable<uint>
    {
        public uint id { get; private set; }
        private uint[] node_ids;
        public uint this[uint n]{
            get { return node_ids[n]; }
            set { node_ids[n] = value; }
        }
        public string type { get; private set; }

        private Element(string type)
        {
            this.type = type.ToUpper();
        }

        public Element(string type, string line):this(type)
        {
            var arr = line.Split(',').Select(s => uint.Parse(s));
            id = arr.First();
            node_ids = arr.Skip(1).ToArray();
        }
        public Element(Element template):this(template.type)
        {
            this.id = template.id;
            this.node_ids = template.node_ids; // shallow copy
        }


        internal Elements parent { get; set; }

        public Model model { get; set; }

        public int CompareTo(Element other)
        {
            return id.CompareTo(other.id);
        }
        public override string ToString()
        {
            return base.ToString() + "(" + id.ToString() + ")[" + type + "]";
        }

        public System.Collections.Generic.IEnumerator<uint> GetEnumerator()
        {
            foreach (var item in node_ids) {
                yield return item;
            }
        }

        public int Count { get { return node_ids.Length; } }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var item in node_ids) {
                yield return item;
            }
        }
    }
}
