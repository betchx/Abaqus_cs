using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;


namespace Abaqus
{
    /// <summary>
    /// 節点
    /// </summary>
    public class Node : IComparable<Node>
    {
        private Point3D point;

        public Point3D pos
        {
            get { return point; }
            set { point = value; }
        }
        public double X
        {
            get { return point.X; }
            set { point.X = value; }
        }
        public double Y
        {
            get { return point.Y; }
            set { point.Y = value; }
        }
        public double Z
        {
            get { return point.Z; }
            set { point.Z = value; }
        }

        public uint id { get; private set; }

        /// <summary>
        ///  各要素を明示して作成する．
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Node(uint id, double x, double y, double z)
        {
            this.id = id;
            point = new Point3D(x, y, z);
        }

        /// <summary>
        ///  各要素と変換を明示して作成する．
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="system">変換</param>
        public Node(uint id, double x, double y, double z, Transform3DGroup system)
        {
            this.id = id;
            point = system.Transform(new Point3D(x, y, z));
        }
        /// <summary>
        ///  Inpの節点定義文字列から作成する．
        ///  メインのコンストラクタ．
        /// </summary>
        /// <param name="line"></param>
        public Node(string line)
        {
            var list = line.Trim().TrimEnd(',').Split(',');
            id = uint.Parse(list.First());
            var xyz = list.Skip(1).Select(s => double.Parse(s)).ToArray();
            var n = xyz.Length;
            if(n > 0) point.X = xyz[0];
            if(n > 1) point.Y = xyz[1];
            if(n > 2) point.Z = xyz[2];
        }

        public Node(string line, Transform3DGroup system):this(line)
        {
            point = system.Transform(point);
        }


        public Node translate_by(TranslateTransform3D  translator)
        {
            point = translator.Transform(point);
            return this;
        }

        public Node rotate_by(RotateTransform3D rot)
        {
            this.point = rot.Transform(point);
            return this;
        }

        public Nodes parent { get; set; }

        public Model model { get; set; }

        public int CompareTo(Node other)
        {
            return id.CompareTo(other.id);
        }

        internal void Transform(Transform3DGroup system)
        {
            pos = system.Transform(pos);
        }
        public override string ToString()
        {
            return base.ToString() + ":(" + id.ToString() + ")[" + pos.ToString() + "]";
        }
    }
}
