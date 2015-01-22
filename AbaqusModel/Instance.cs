using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Abaqus
{
    public class Instance
    {
        public string name { get; set; }
        public string part { get; set; }
        public Transform3DGroup system { get; set; }

        public Instance(string name, string part, string trans = "", string rot = "")
        {
            this.name = name;
            this.part = part;
            if( trans != ""){
                var arr = trans.Split(',').Select(s => double.Parse(s)).ToArray();
                var tr = new TranslateTransform3D(arr[0], arr[1], arr[2]);
                system.Children.Add(tr);
            }
            if( rot != ""){
                var arr = rot.Split(',').Select(s => double.Parse(s)).ToArray();
                var a = new Point3D(arr[0], arr[1], arr[2]);
                var b = new Point3D(arr[3], arr[4], arr[5]);
                var angle = arr[6];
                var rotator = new AxisAngleRotation3D(b-a, angle);
                system.Children.Add(new RotateTransform3D(rotator, a));
            }
        }


        public Model model { get; set; }

        public Instances parent { get; set; }
    }
}
