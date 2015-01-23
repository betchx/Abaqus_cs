using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public struct Address : IComparable
    {
        public string name;
        public uint id;
        public Address(string name, uint id)
        {
            this.name = name;
            this.id = id;
        }
        public Address(uint id)
            : this("", id)
        {
        }

        public int CompareTo(Object obj)
        {
            if (obj is Address)
            {
                var other = (Address)obj;
                var res = name.CompareTo(other.name);
                if (res == 0)
                {
                    res = id.CompareTo(other.id);
                }
                return res;
            }
            if (obj is uint)
            {
                return CompareTo(new Address("",(uint)obj));
            }
            return -1;
        }

        public static implicit operator Address(uint id)
        {
            return new Address(id);
        }

    }
}
