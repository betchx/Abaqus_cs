using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public struct Address 
    {
        string name;
        uint id;
        public Address(uint id)
        {
            name = "";
            this.id = id;
        }
    }
}
