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
        public SystemConverter(Point3D origin): this()
        {
            // 並進はわざわざマトリックスを作成するまでもないので，直接作成する．
            transform.Children.Add(new TranslateTransform3D(origin.X, origin.Y, origin.Z));
        }


        /// <summary>
        ///  グローバルXY平面内での回転（Z軸まわり回転）と並進
        /// </summary>
        /// <param name="origin">ローカル座標の原点</param>
        /// <param name="xaxis">ローカルX軸上の点</param>
        public SystemConverter(Point3D origin, Point3D xaxis):this()
        {
            // XY平面の指定がない場合，局所Z軸はグローバルと同じになる．
            var w = new Vector3D(0, 0, 1);
            // 局所座標系の軸ベクトルを作成
            var u = xaxis - origin;
            var v = Vector3D.CrossProduct(w, u);

            // 登録
            RegisterTransform(origin, u, v, w);
        }

        /// <summary>
        /// 任意の局所座標からの変換
        /// </summary>
        /// <param name="origin">ローカル座標の原点</param>
        /// <param name="xaxis">ローカルX軸上の点</param>
        /// <param name="xyplain">ローカルXY平面上でY軸が正の方向にある点</param>
        public SystemConverter(Point3D origin, Point3D xaxis, Point3D xyplain) : this()
        {
            // 局所座標系の軸ベクトルを作成
            var u = xaxis - origin;
            var w = Vector3D.CrossProduct(u, xyplain - origin);
            var v = Vector3D.CrossProduct(w, u);

            // 登録
            RegisterTransform(origin, u, v, w);
        }


        /// <summary>
        ///   局所座標系の原点とベクトルから変換を作成して登録
        /// </summary>
        /// <param name="origin">局所座標系の原点</param>
        /// <param name="u">局所X軸ベクトル</param>
        /// <param name="v">局所Y軸ベクトル</param>
        /// <param name="w">局所Z軸ベクトル</param>
        private void RegisterTransform(Point3D origin, Vector3D u, Vector3D v, Vector3D w)
        {
            // ベクトルを正規化
            u.Normalize();
            w.Normalize();
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
