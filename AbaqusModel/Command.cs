using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Abaqus
{
    public class Command
    {
        public string keyword { get; set; }
        public Dictionary<string, string> parameters { get; set; }
        public List<string> datablock { get; set; }
    }
}
