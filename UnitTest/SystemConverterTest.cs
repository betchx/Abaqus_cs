using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Mocks;
using System.Windows.Media.Media3D;
using Abaqus;


namespace UnitTest
{



    [TestFixture]
    class SystemConverterTest
    {

        [TestCase(10, 0, 0, 0, 0, 0, 10, 0, 0, 0.001)]
        [TestCase(10, 0, 0, 1, 1, 1, 11, 1, 1, 0.001)]
        [TestCase(10, 20, 30, 1, 1, 1, 11, 21, 31, 0.001)]
        public void Translate1Test(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double c1, double c2, double c3,
            double delta)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D source = new Point3D(b1, b2, b3);
            Point3D expected = new Point3D(c1, c2, c3);

            var conv = new SystemConverter(origin);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, delta, "X");
            Assert.AreEqual(expected.Y, res.Y, delta, "Y");
            Assert.AreEqual(expected.Z, res.Z, delta, "Z");
        }

        // 並進
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  11, 0, 0,  /*source*/ 2, 0, 0, /*expected*/ 12, 0, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  11, 0, 0,  /*source*/ 2, 3, 4, /*expected*/ 12, 3, 4, /*delta*/ 0.001)]
        public void Translate2Test(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double c1, double c2, double c3,
            double d1, double d2, double d3,
            double delta = 0.001)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D xaxis = new Point3D(b1, b2, b3);
            Point3D source = new Point3D(c1, c2, c3);
            Point3D expected = new Point3D(d1, d2, d3);

            var conv = new SystemConverter(origin, xaxis);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, delta, "X");
            Assert.AreEqual(expected.Y, res.Y, delta, "Y");
            Assert.AreEqual(expected.Z, res.Z, delta, "Z");
        }
        
        //原点で90°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 1, 0,  /*source*/ 2, 0, 0, /*expected*/ 0, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 1, 0,  /*source*/ 2, 3, 0, /*expected*/ -3, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 5, 0,  /*source*/ 2, 3, 0, /*expected*/ -3, 2, 0, /*delta*/ 0.001)]
        // 反転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -1, 0, 0,  /*source*/ 2, 0, 0, /*expected*/ -2, 0, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -1, 0, 0,  /*source*/ 2, 3, 0, /*expected*/ -2, -3, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -6, 0, 0,  /*source*/ 2, 3, 0, /*expected*/ -2, -3, 0, /*delta*/ 0.001)]
        //原点で-90°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, -1, 0,  /*source*/ 2, 0, 0, /*expected*/ 0, -2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, -1, 0,  /*source*/ 2, 3, 0, /*expected*/ 3, -2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, -5, 0,  /*source*/ 2, 3, 0, /*expected*/ 3, -2, 0, /*delta*/ 0.001)]
        // 時計まわりに45°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 1, 0,  /*source*/ 2, 0, 0, /*expected*/ 1.414, 1.414, 0, /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 1, 0,  /*source*/ 1, 1, 0, /*expected*/ 0, 1.414, 0, /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, 5, 0,  /*source*/ 1, 1, 0, /*expected*/ 0, 1.414, 0, /*delta*/ 0.01)]
        // 反時計まわりに45°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, -1, 0,  /*source*/ 2, 0, 0, /*expected*/ 1.414, -1.414, 0, /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, -1, 0,  /*source*/ 1, 1, 0, /*expected*/ 1.414, 0, 0, /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, -5, 0,  /*source*/ 1, 1, 0, /*expected*/ 1.414, 0, 0, /*delta*/ 0.01)]
        // [10, 0, 0] で90°回転
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 1, 0,  /*source*/ 2, 0, 0, /*expected*/ 10, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 1, 0,  /*source*/ 2, 3, 0, /*expected*/ 10-3, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 5, 0,  /*source*/ 2, 3, 0, /*expected*/ 10-3, 2, 0, /*delta*/ 0.001)]
        public void ZRotate2Test(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double c1, double c2, double c3,
            double d1, double d2, double d3,
            double delta = 0.001)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D xaxis = new Point3D(b1, b2, b3);
            Point3D source = new Point3D(c1, c2, c3);
            Point3D expected = new Point3D(d1, d2, d3);

            var conv = new SystemConverter(origin, xaxis);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, delta, "X");
            Assert.AreEqual(expected.Y, res.Y, delta, "Y");
            Assert.AreEqual(expected.Z, res.Z, delta, "Z");
        }

        // [10, 20, 30] で90°回転
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  10, 21, 30,  /*source*/ 2, 0, 0, /*expected*/ 10, 22, 30, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  10, 21, 30,  /*source*/ 2, 3, 0, /*expected*/ 10 - 3, 22, 30, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  10, 25, 30,  /*source*/ 2, 3, 0, /*expected*/ 10 - 3, 22, 30, /*delta*/ 0.001)]
        public void TransAndZRotate2Test(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double c1, double c2, double c3,
            double d1, double d2, double d3,
            double delta = 0.001)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D xaxis = new Point3D(b1, b2, b3);
            Point3D source = new Point3D(c1, c2, c3);
            Point3D expected = new Point3D(d1, d2, d3);

            var conv = new SystemConverter(origin, xaxis);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, delta, "X");
            Assert.AreEqual(expected.Y, res.Y, delta, "Y");
            Assert.AreEqual(expected.Z, res.Z, delta, "Z");
        }


        // 並進
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  11, 0, 0, /*plane*/ 10, 1, 0,  /*source*/ 2, 3, 4, /*expected*/ 12, 3, 4, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  11, 20, 30, /*plane*/ 10, 21, 30,  /*source*/ 2, 3, 4, /*expected*/ 12, 23, 34, /*delta*/ 0.001)]
        public void Translate3Test(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double p1, double p2, double p3,
            double c1, double c2, double c3,
            double d1, double d2, double d3,
            double delta = 0.001)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D xaxis = new Point3D(b1, b2, b3);
            Point3D plane = new Point3D(p1, p2, p3);
            Point3D source = new Point3D(c1, c2, c3);
            Point3D expected = new Point3D(d1, d2, d3);

            var conv = new SystemConverter(origin, xaxis, plane);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, delta, "X");
            Assert.AreEqual(expected.Y, res.Y, delta, "Y");
            Assert.AreEqual(expected.Z, res.Z, delta, "Z");
        }

        //Z 軸回転
        //原点で90°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 1, 0, /*plane*/ -1, 0, 0, /*source*/ 2, 0, 0, /*expected*/ 0, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 1, 0, /*plane*/ -1, 0, 0, /*source*/ 2, 3, 0, /*expected*/ -3, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 5, 0, /*plane*/ -1, 0, 0, /*source*/ 2, 3, 0, /*expected*/ -3, 2, 0, /*delta*/ 0.001)]
        // 反転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -1, 0, 0, /*plane*/ 0, -1, 0, /*source*/ 2, 0, 0, /*expected*/ -2, 0, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -1, 0, 0, /*plane*/ 0, -1, 0, /*source*/ 2, 3, 0, /*expected*/ -2, -3, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -6, 0, 0, /*plane*/ 0, -1, 0, /*source*/ 2, 3, 0, /*expected*/ -2, -3, 0, /*delta*/ 0.001)]
        //原点で-90°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, -1, 0, /*plane*/ 1, 0, 0, /*source*/ 2, 0, 0, /*expected*/ 0, -2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, -1, 0, /*plane*/ 1, 0, 0, /*source*/ 2, 3, 0, /*expected*/ 3, -2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, -5, 0, /*plane*/ 1, 0, 0, /*source*/ 2, 3, 0, /*expected*/ 3, -2, 0, /*delta*/ 0.001)]
        // 時計まわりに45°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 1, 0, /*plane*/ 0, 1, 0, /*source*/ 2, 0, 0, /*expected*/ 1.414, 1.414, 0, /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 1, 0, /*plane*/ 0, 1, 0, /*source*/ 1, 1, 0, /*expected*/ 0, 1.414, 0, /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, 5, 0, /*plane*/ 0, 1, 0, /*source*/ 1, 1, 0, /*expected*/ 0, 1.414, 0, /*delta*/ 0.01)]
        // 反時計まわりに45°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, -1, 0, /*plane*/ 1, 0, 0,  /*source*/ 2, 0, 0, /*expected*/ 1.414, -1.414, 0, /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, -1, 0, /*plane*/ 1, 0, 0,  /*source*/ 1, 1, 0, /*expected*/ 1.414, 0, 0, /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, -5, 0, /*plane*/ 1, 0, 0,  /*source*/ 1, 1, 0, /*expected*/ 1.414, 0, 0, /*delta*/ 0.01)]
        // [10, 0, 0] で90°回転
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 1, 0, /*plane*/ 0, 0, 0, /*source*/ 2, 0, 0, /*expected*/ 10, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 1, 0, /*plane*/ 0, 0, 0, /*source*/ 2, 3, 0, /*expected*/ 10 - 3, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 5, 0, /*plane*/ 0, 0, 0, /*source*/ 2, 3, 0, /*expected*/ 10 - 3, 2, 0, /*delta*/ 0.001)]
        // [10, 20, 30] で90°回転
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  10, 21, 30, /*plane*/ 0, 20, 30, /*source*/ 2, 0, 0, /*expected*/ 10, 22, 30, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  10, 21, 30, /*plane*/ 0, 20, 30, /*source*/ 2, 3, 0, /*expected*/ 10 - 3, 22, 30, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  10, 25, 30, /*plane*/ 0, 20, 30, /*source*/ 2, 3, 0, /*expected*/ 10 - 3, 22, 30, /*delta*/ 0.001)]
        public void ZRotate3Test(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double p1, double p2, double p3,
            double c1, double c2, double c3,
            double d1, double d2, double d3,
            double delta = 0.001)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D xaxis = new Point3D(b1, b2, b3);
            Point3D plane = new Point3D(p1, p2, p3);
            Point3D source = new Point3D(c1, c2, c3);
            Point3D expected = new Point3D(d1, d2, d3);

            var conv = new SystemConverter(origin, xaxis, plane);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, delta, "X");
            Assert.AreEqual(expected.Y, res.Y, delta, "Y");
            Assert.AreEqual(expected.Z, res.Z, delta, "Z");
        }



#if true
        //X 軸回転
        //原点で90°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, 0, 1, /*source*/ 2, 0, 0, /*expected*/ 2, 0, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, 0, 1, /*source*/ 2, 3, 0, /*expected*/ 2, 0, 3, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, 0, 0, /*plane*/ 0, 0, 1, /*source*/ 2, 3, 4, /*expected*/ 2, -4, 3, /*delta*/ 0.001)]
        // 反転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, -1, 0, /*source*/ 2, 0, 0, /*expected*/ 2,  0, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, -1, 0, /*source*/ 2, 3, 0, /*expected*/ 2, -3, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  6, 0, 0, /*plane*/ 0, -1, 0, /*source*/ 2, 3, 4, /*expected*/ 2, -3, -4, /*delta*/ 0.001)]
        //原点で-90°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, 0, -1, /*source*/ 2, 0, 0, /*expected*/ 2, 0, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, 0, -1, /*source*/ 2, 3, 0, /*expected*/ 2, 0, -3, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, 0, 0, /*plane*/ 0, 0, -1, /*source*/ 2, 3, 4, /*expected*/ 2, 4, -3, /*delta*/ 0.001)]
        // 時計まわりに45°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, 1, 1, /*source*/ 0, 2, 0, /*expected*/ 0, 1.414, 1.414,  /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, 1, 1, /*source*/ 0, 1, 1, /*expected*/ 0, 0, 1.414,  /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, 0, 0, /*plane*/ 0, 1, 1, /*source*/ 0, 1, 1, /*expected*/ 0, 0, 1.414,  /*delta*/ 0.01)]
        // 反時計まわりに45°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, 1, -1,  /*source*/ 0, 2, 0, /*expected*/ 0, 1.414, -1.414,  /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 0, /*plane*/ 0, 1, -1,  /*source*/ 0, 1, 1, /*expected*/ 0, 1.414, 0,  /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, 0, 0, /*plane*/ 0, 1, -1,  /*source*/ 0, 1, 1, /*expected*/ 0, 1.414, 0,  /*delta*/ 0.01)]
        // [10, 0, 0] で90°回転
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  11, 0, 0, /*plane*/ 10, 0, 1, /*source*/ 2, 0, 0, /*expected*/ 12, 0, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  11, 0, 0, /*plane*/ 10, 0, 1, /*source*/ 2, 3, 0, /*expected*/ 12, 0, 3, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  15, 0, 0, /*plane*/ 10, 0, 1, /*source*/ 2, 3, 0, /*expected*/ 12, 0, 3, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  15, 0, 0, /*plane*/ 10, 0, 1, /*source*/ 2, 3, 4, /*expected*/ 12, -4, 3, /*delta*/ 0.001)]
        // [10, 20, 30] で90°回転
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  11, 20, 30, /*plane*/ 10, 20, 40, /*source*/ 2, 0, 1, /*expected*/ 12, 19, 30, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  11, 20, 30, /*plane*/ 10, 20, 40, /*source*/ 2, 3, 0, /*expected*/ 12, 20, 33, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  15, 20, 30, /*plane*/ 10, 20, 40, /*source*/ 2, 3, 0, /*expected*/ 12, 20, 33, /*delta*/ 0.001)]
#endif
        public void XRotateTest(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double p1, double p2, double p3,
            double c1, double c2, double c3,
            double d1, double d2, double d3,
            double delta = 0.001)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D xaxis = new Point3D(b1, b2, b3);
            Point3D plane = new Point3D(p1, p2, p3);
            Point3D source = new Point3D(c1, c2, c3);
            Point3D expected = new Point3D(d1, d2, d3);

            var conv = new SystemConverter(origin, xaxis, plane);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, delta, "X");
            Assert.AreEqual(expected.Y, res.Y, delta, "Y");
            Assert.AreEqual(expected.Z, res.Z, delta, "Z");
        }



        //Y 軸回転
        //原点で90°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 0, 1, /*plane*/ 0, 1, 0, /*source*/ 2, 0, 0, /*expected*/  0, 0, 2, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 0, 1, /*plane*/ 0, 1, 0, /*source*/ 2, 3, 0, /*expected*/  0, 3, 2, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 0, 5, /*plane*/ 0, 1, 0, /*source*/ 2, 3, 4, /*expected*/ -4, 3, 2, /*delta*/ 0.001)]
        // 反転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -1, 0, 0, /*plane*/ 0, 1, 0, /*source*/ 2, 0, 0, /*expected*/ -2, 0, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -1, 0, 0, /*plane*/ 0, 1, 0, /*source*/ 2, 3, 0, /*expected*/ -2, 3, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -5, 0, 0, /*plane*/ 0, 1, 0, /*source*/ 2, 3, 4, /*expected*/ -2, 3, -4, /*delta*/ 0.001)]
        //原点で-90°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 0, -1, /*plane*/ 0, 1, 0, /*source*/ 2, 0, 0, /*expected*/ 0, 0, -2, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 0, -1, /*plane*/ 0, 1, 0, /*source*/ 2, 3, 0, /*expected*/ 0, 3, -2, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 0, -5, /*plane*/ 0, 1, 0, /*source*/ 2, 3, 4, /*expected*/ 4, 3, -2, /*delta*/ 0.001)]
        // 時計まわりに45°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 1, /*plane*/ 0, 1, 0, /*source*/ 2, 2, 0, /*expected*/ 1.414, 2, 1.414,  /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, 1, /*plane*/ 0, 1, 0, /*source*/ 1, 1, 1, /*expected*/ 0, 1, 1.414,  /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, 0, 5, /*plane*/ 0, 1, 0, /*source*/ 1, 1, 1, /*expected*/ 0, 1, 1.414,  /*delta*/ 0.01)]
        // 反時計まわりに45°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, -1, /*plane*/ 0, 1, 0,  /*source*/ 2, 2, 0, /*expected*/ 1.414, 2, -1.414,  /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  1, 0, -1, /*plane*/ 0, 1, 0,  /*source*/ 1, 1, 1, /*expected*/ 1.414, 1, 0,  /*delta*/ 0.01)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  5, 0, -5, /*plane*/ 0, 1, 0,  /*source*/ 1, 1, 1, /*expected*/ 1.414, 1, 0,  /*delta*/ 0.01)]
        // [10, 0, 0] で90°回転
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 0, 1, /*plane*/ 10, 1, 0, /*source*/ 2, 0, 0, /*expected*/ 10, 0, 2, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 0, 1, /*plane*/ 10, 1, 0, /*source*/ 2, 3, 0, /*expected*/ 10, 3, 2, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 0, 5, /*plane*/ 10, 5, 0, /*source*/ 2, 3, 0, /*expected*/ 10, 3, 2, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 0, 0, /*xaxis*/  10, 0, 5, /*plane*/ 10, 5, 0, /*source*/ 2, 3, 4, /*expected*/ 10-4, 3, 2, /*delta*/ 0.001)]
        // [10, 20, 30] で90°回転
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  10, 20, 31, /*plane*/ 10, 21, 30, /*source*/ 2, 2, 1, /*expected*/ 10-1, 22, 32, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  10, 20, 31, /*plane*/ 10, 21, 30, /*source*/ 2, 3, 4, /*expected*/ 10-4, 23, 32, /*delta*/ 0.001)]
        [TestCase(/*origin*/10, 20, 30, /*xaxis*/  10, 20, 35, /*plane*/ 10, 25, 30, /*source*/ 2, 3, 4, /*expected*/ 10-4, 23, 32, /*delta*/ 0.001)]
        public void YRotateTest(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double p1, double p2, double p3,
            double c1, double c2, double c3,
            double d1, double d2, double d3,
            double delta = 0.001)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D xaxis = new Point3D(b1, b2, b3);
            Point3D plane = new Point3D(p1, p2, p3);
            Point3D source = new Point3D(c1, c2, c3);
            Point3D expected = new Point3D(d1, d2, d3);

            var conv = new SystemConverter(origin, xaxis, plane);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, delta, "X");
            Assert.AreEqual(expected.Y, res.Y, delta, "Y");
            Assert.AreEqual(expected.Z, res.Z, delta, "Z");
        }


        [TestCase(
            /*origin*/      0,     0,     0, 
            /*X axis*/      1,     1,     1, 
            /*Y axis*/     -1,     1,     1, 
            /*target*/      1,     0,     0,
            /*expected*/0.577, 0.577, 0.577, 
            /*delta*/ 0.001  )]
        public void ArbitrayRotate3Test(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double p1, double p2, double p3,
            double c1, double c2, double c3,
            double d1, double d2, double d3,
            double delta = 0.001)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D xaxis = new Point3D(b1, b2, b3);
            Point3D plane = new Point3D(p1, p2, p3);
            Point3D source = new Point3D(c1, c2, c3);
            Point3D expected = new Point3D(d1, d2, d3);

            var conv = new SystemConverter(origin, xaxis, plane);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, delta, "X");
            Assert.AreEqual(expected.Y, res.Y, delta, "Y");
            Assert.AreEqual(expected.Z, res.Z, delta, "Z");
        }

        [Test]
        public void ARotTest()
        {
            var transform = new Transform3DGroup();

            var i = new Vector3D(1, 0, 0);
            var j = new Vector3D(0, 1, 0);
            var k = new Vector3D(0, 0, 1);

            var u = new Vector3D(1, 1, 1);
            u.Normalize();
            var t = new Vector3D(-1, 1, 1);
            t.Normalize();
            var w = Vector3D.CrossProduct(u, t);
            w.Normalize();
            var v = Vector3D.CrossProduct(w, u);

            var m = new MatrixTransform3D(
                new Matrix3D(
                    u.X, u.Y, u.Z, 0.0,
                    v.X, v.Y, v.Z, 0.0,
                    w.X, w.Y, w.Z, 0.0,
                    0.0, 0.0, 0.0, 1.0));

            var source = new Point3D(Math.Sqrt(3.0), 0, 0);
            var res = m.Transform(source);
            Assert.AreEqual(1.0, res.X, 0.01, "x");
            Assert.AreEqual(1.0, res.Y, 0.01, "y");
            Assert.AreEqual(1.0, res.Z, 0.01, "z");
        }



    }
}
