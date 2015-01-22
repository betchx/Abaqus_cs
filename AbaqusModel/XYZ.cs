using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Abaqus
{
    class XYZ
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }


        public XYZ(IEnumerable<double> e)
        {
            var it = e.GetEnumerator();
            x = it.Current;
            it.MoveNext();
            y = it.Current;
            it.MoveNext();
            z = it.Current;
        }
        public XYZ()
        {
            x = 0.0;
            y = 0.0;
            z = 0.0;
        }

        public static implicit operator Point3D(XYZ xyz)
        {
            return new Point3D(xyz.x, xyz.y, xyz.z);

        }
    }
}
