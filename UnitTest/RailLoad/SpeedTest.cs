using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RailLoad;
using NUnit.Framework;

namespace UnitTest.RailLoad
{
    class SpeedTest
    {
        private Speed speed;
        [SetUp]
        public void Setup()
        {
            speed = new Speed();
        }
      
        [Test]
        public void ConvertTest()
        {
            //Assert.Fail("ConvertTest");
            speed.mps = 75.0;
            Assert.That(speed.mps, Is.EqualTo(75.0).Within(0.001));
            Assert.That(speed.kmph, Is.EqualTo(270.0).Within(0.001));
        }

        static double[] kmph_arr = new double[] { 270.0, 180.0, 90.0 };
        static double[] mps_arr = new double[] { 75.0, 50.0, 25.0 };


        [Test, Sequential]
        public void MpsToKmphTest(
            [ValueSource("mps_arr")] double mps,
            [ValueSource("kmph_arr")] double kmph)
        {
            //Assert.Fail("MpsToKmphTest");
            speed.mps = mps;
            Assert.That(speed.kmph, Is.EqualTo(kmph).Within(0.001));
        }

        [Test, Sequential]
        public void KmphToMpsTest(
            [ValueSource("mps_arr")] double mps,
            [ValueSource("kmph_arr")] double kmph)
        {
            //Assert.Fail("KmphToMpsTest");
            speed.kmph = kmph;
            Assert.That(speed.mps, Is.EqualTo(mps).Within(0.001));

        }
    }
}
