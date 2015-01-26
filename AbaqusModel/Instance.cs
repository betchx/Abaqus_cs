using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Abaqus
{
    public class Instance : Part // TODO: Partと共通の基底クラスを作成し，そちらを使う様にする．
    {
        public string part { get; set; }
        public Transform3DGroup system { get; set; }

        public new Instances parent { get; set; }

        public Instance(string name, string part, Model model, string trans = "", string rot = ""):base(name, model)
        {
            this.part = part;
            system = new Transform3DGroup();

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

        //private Address address(uint id) { return new Address(name, id); }

        //public void Add(Node n)
        //{
        //    var pos = system.Transform(n.pos);
        //    nodes.Add(new Node(n.id, pos.X, pos.Y, pos.Z));
        //}
        //public void Add(Nodes ns) { ns.Values.ForEach(n => Add(n)); }
        //public void Add(Elements es) { es.Values.ForEach(e => Add(e)); }
        //public void Add(Element e) { elements.Add(e); }
        //public void Add(NSet ns) { nsets.Add(ns); }
        //public void Add(ELSet es) { elsets.Add(es); }
    }
}
