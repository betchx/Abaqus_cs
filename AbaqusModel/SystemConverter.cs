using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace Abaqus
{
    public class SystemConverter
    {
        public Transform3DGroup transform { get; private set; }

        /// <summary>
        ///  修正なし
        /// </summary>
        public SystemConverter()
        {
            transform = new Transform3DGroup();
        }

        /// <summary>
        ///  並進のみ
        /// </summary>
        /// <param name="origin">ローカル座標の原点</param>
        public SystemConverter(Point3D origin)
        {
            transform = new Transform3DGroup();
            transform.Children.Add(new TranslateTransform3D(origin.X, origin.Y, origin.Z));
        }


        /// <summary>
        ///  グローバルXY平面内での回転（Z軸まわり回転）と並進
        /// </summary>
        /// <param name="origin">ローカル座標の原点</param>
        /// <param name="xaxis">ローカルX軸上の点</param>
        public SystemConverter(Point3D origin, Point3D xaxis)
        {
            transform = new Transform3DGroup();

            // 準備
            var g_x = new Vector3D(1, 0, 0);
            var g_z = new Vector3D(0, 0, 1);
            var l_x = xaxis - origin;
            l_x.Normalize();

            // 回転角度 0～180°
            var dot = Vector3D.DotProduct(g_x, l_x);
            var angle = Math.Acos( dot ) * 180.0 / Math.PI;
            // 回転軸 ＝＞ 回転方向．  Acosで算出している関係で回転方向がなくなるので，外積で算出する．
            var ax = (dot == -1.0)?g_z:Vector3D.CrossProduct(g_x, l_x);
 
            // グローバルZ軸まわりの回転を追加
            var rot = new AxisAngleRotation3D(ax, angle);
            transform.Children.Add(new RotateTransform3D(rot));
            // 並進を追加
            transform.Children.Add(new TranslateTransform3D(origin.X, origin.Y, origin.Z));
        }

        /// <summary>
        /// 任意の局所座標からの変換
        /// </summary>
        /// <param name="origin">ローカル座標の原点</param>
        /// <param name="xaxis">ローカルX軸上の点</param>
        /// <param name="xyplain">ローカルXY平面上でY軸が正の方向にある点</param>
        public SystemConverter(Point3D origin, Point3D xaxis, Point3D xyplain)
        {
            transform = new Transform3DGroup();

            var u = xaxis - origin;
            u.Normalize();
            var w = Vector3D.CrossProduct(u, xyplain - origin);
            w.Normalize();
            var v = Vector3D.CrossProduct(w, u);
            v.Normalize();

            // 変換マトリックスの作成
            var mat = new Matrix3D(
                        u.X, u.Y, u.Z, 0.0,
                        v.X, v.Y, v.Z, 0.0,
                        w.X, w.Y, w.Z, 0.0,
                        origin.X, origin.Y, origin.Z, 1.0);

            // 登録
            transform.Children.Add(new MatrixTransform3D(mat));
        }

    }
}
