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
        [TestCase(10, 0, 0, 0, 0, 0, 10, 0, 0)]
        [TestCase(10, 0, 0, 1, 1, 1, 11, 1, 1)]
        [TestCase(10, 20, 30, 1, 1, 1, 11, 21, 31)]
        public void TranslateTest(
            double a1, double a2, double a3,
            double b1, double b2, double b3,
            double c1, double c2, double c3)
        {
            Point3D origin = new Point3D(a1, a2, a3);
            Point3D source = new Point3D(b1, b2, b3);
            Point3D expected = new Point3D(c1, c2, c3);

            var conv = new SystemConverter(origin);
            var res = conv.transform.Transform(source);

            Assert.AreEqual(expected.X, res.X, 0.0001);
            Assert.AreEqual(expected.Y, res.Y, 0.0001);
            Assert.AreEqual(expected.Z, res.Z, 0.0001);
        }

        //原点から90°回転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 1, 0,  /*source*/ 2, 0, 0, /*expected*/ 0, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 1, 0,  /*source*/ 2, 3, 0, /*expected*/ -3, 2, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  0, 5, 0,  /*source*/ 2, 3, 0, /*expected*/ -3, 2, 0, /*delta*/ 0.001)]
        // 反転
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -1, 0, 0,  /*source*/ 2, 0, 0, /*expected*/ -2, 0, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -1, 0, 0,  /*source*/ 2, 3, 0, /*expected*/ -2, -3, 0, /*delta*/ 0.001)]
        [TestCase(/*origin*/0, 0, 0, /*xaxis*/  -6, 0, 0,  /*source*/ 2, 3, 0, /*expected*/ -2, -3, 0, /*delta*/ 0.001)]
        public void XYRotateTest(
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

            Assert.AreEqual(expected.X, res.X, delta);
            Assert.AreEqual(expected.Y, res.Y, delta);
            Assert.AreEqual(expected.Z, res.Z, delta);
        }

    }
}
