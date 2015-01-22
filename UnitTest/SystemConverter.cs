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

            // 準備
            var g_x = new Vector3D(1, 0, 0);
            var g_z = new Vector3D(0, 0, 1);
            var l_x = xaxis - origin;
            var l_z = Vector3D.CrossProduct(l_x, xyplain - origin);
            l_x.Normalize();
            l_z.Normalize();

            // X軸を合わせる回転
            var x_dot = Vector3D.DotProduct(g_x, l_x);
            var x_angle = Math.Acos(x_dot) * 180 / Math.PI;
            var x_ax = (x_dot == -1.0)?g_z:Vector3D.CrossProduct(g_x, l_x);
            var rot_x = new RotateTransform3D(new AxisAngleRotation3D(x_ax, x_angle));
            
            // 局所Z軸を回転する
            var v_z = rot_x.Transform(l_z);

            // Z軸を合わせる回転
            var z_dot = Vector3D.DotProduct(g_z, v_z);
            var z_ax = (z_dot==-1.0)?g_x:Vector3D.CrossProduct(g_z, v_z);
            var z_angle = Math.Acos(z_dot) * 180 / Math.PI;
            var rot_z = new RotateTransform3D(new AxisAngleRotation3D(z_ax, z_angle));

            // 登録
            transform.Children.Add(rot_x);
            transform.Children.Add(rot_z);
            transform.Children.Add(new TranslateTransform3D(origin.X, origin.Y, origin.Z));
        }

    }
}
