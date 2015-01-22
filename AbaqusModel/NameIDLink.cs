using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    /// <summary>
    /// 名前とIDの対応関係を保持する．
    /// </summary>
    class NameIDLink
    {
        private SortedDictionary<uint, string> id2name;
        private SortedDictionary<string, uint> name2id;

        public NameIDLink()
        {
            id2name = new SortedDictionary<uint, string>();
            name2id = new SortedDictionary<string, uint>();
        }

        public NameIDLink add(uint id, string name)
        {
            id2name.Add(id, name);
            name2id.Add(name, id);
            return this;
        }
        public NameIDLink add(string name, uint id)
        {
            return add(id, name);
        }

        public uint this[string name]
        {
            get { return name2id[name]; }
        }

        public string this[uint id]
        {
            get { return id2name[id]; }
        }

    }
}
