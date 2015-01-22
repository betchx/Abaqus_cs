using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Abaqus
{
    
    public class NamedStore<K,T> :  SortedDictionary<string, SortedDictionary<K, T>>
    {
        public SortedDictionary<K, T> global { get; protected set; }

        public NamedStore()
        {
            global = new SortedDictionary<K,T>();
            this.Add("Assembly", global);
            this.Add("", global);
            this.Add("global", global);
        }

        public void Add(string instance, K id, T value)
        {
            if (this[instance] == null) this.Add(instance, new SortedDictionary<K, T>());
            this[instance].Add(id, value);
        }

        public void Add(K it, T value)
        {
            this.Add("Assembly", it, value);
        }
        public T get(K id) { return global[id]; }
        public T get(string name, K id)
        {
            return this[name][id];
        }

    }

    public class NamedIDStore<T> : NamedStore<uint,T>
    {
        public T find(string name)
        {

            if (name.Contains('.'))
            {
                var arr = name.Split('.');
                return get(arr[0], uint.Parse(arr[1]));
            }
            else
            {
                return global[uint.Parse(name)];
            }
        }
    }

    public class NamedSetStore<T> : NamedStore<string, T>
    {
        public T find(string name)
        {
            if (name.Contains('.'))
            {
                var arr = name.Split('.');
                return get(arr[0], arr[1]);
            }
            else
            {
                return global[name];
            }

        }
    }


}
