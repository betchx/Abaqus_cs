using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RailLoad
{
    /// <summary>
    ///  速度を表す．
    ///  単位変換を容易にするためのもの
    /// </summary>
    class Speed
    {
        /// <summary>
        ///  m/sであらわした速度
        /// </summary>
        private double speed;

        /// <summary>
        ///  km/hであらわした速度
        /// </summary>
        public double kmph
        {
            get { return to_kmph(speed); }
            set { speed = to_mps(value); }
        }

        /// <summary>
        /// m/sであらわした速度
        /// </summary>
        public double mps
        {
            get { return speed; }
            set { speed = value; }
        }

        /// <summary>
        /// m/sからkm/hに変換
        /// </summary>
        /// <param name="mps">m/sの速度</param>
        /// <returns>km/hの速度</returns>
        private static double to_kmph(double mps) { return mps * 270.0 / 75.0; }

        /// <summary>
        /// km/hからm/sに変換
        /// </summary>
        /// <param name="kmph">km/hの速度</param>
        /// <returns>m/sの速度</returns>
        private static double to_mps(double kmph) { return kmph * 75.0 / 270.0; }

        /// <summary>
        /// コンストラクタ．
        /// 速度はゼロで構築される．
        /// 初期値を設定して構築したい場合は
        ///    Speed.FromKmph()
        ///    Speed.FromMps()
        /// を利用する．
        /// </summary>
        public Speed()
        {
            speed = 0.0;
        }

        /// <summary>
        ///  時速(km/h)から構築する
        /// </summary>
        /// <param name="kmph">km/hであらわした速度</param>
        /// <returns>構築したSpeed</returns>
        public static Speed FromKmph(double kmph)
        {
            var s = new Speed();
            s.kmph = kmph;
            return s;
        }

        /// <summary>
        ///  m/sの速度を与えて構築する
        /// </summary>
        /// <param name="mps">m/sであらわした速度</param>
        /// <returns>構築したSpeed</returns>
        public static Speed FromMps(double mps)
        {
            var s = new Speed();
            s.mps = mps;
            return s;
        }
    }
}
