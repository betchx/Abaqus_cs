using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class SetStore<T> : SortedDictionary<string, SortedSet<T>>
    {
        public SortedSet<T> global  { get; private set; }
        public SetStore()
        {
            global = new SortedSet<T>();
            this.Add("", global);
            this.Add("global", global);
            this.Add("Assembly", global);
        }

        public void Add(T value) { global.Add(value); }
        public void Add(string name, T value) { this[name].Add(value); }

    }
}
