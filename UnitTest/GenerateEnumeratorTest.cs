using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest
{
    [TestFixture]
    class GenerateEnumeratorTest
    {

        [TestCase(10u, 1u, 4u)]
        [TestCase(3u, 1u, 1u)]
        [TestCase(1000u, 10u, 10u)]
        public void InvalidArgumentTest(uint start, uint last, uint step)
        {
            //Assert.Fail();
            Assert.Throws<ArgumentException>(() => new GenerateEnumerator(start, last, step));
        }


        [TestCase(1u, 10u, 1u, Result=1u)]
        [TestCase(4u, 20u, 1u, Result=4u)]
        [TestCase(8u, 12u, 2u, Result=8u)]
        public uint FirstTest(uint start, uint last, uint step)
        {
            //Assert.Fail();
            var e = new GenerateEnumerator(start, last, step);
            Assert.True(e.MoveNext());
            return e.Current;
        }

        [TestCase(1u, 9u, 1u, Result=2u)]
        [TestCase(3u, 5u, 2u, Result=5u)]
        [TestCase(2u, 8u, 4u, Result=6u)]
        public uint NextTest(uint start, uint last, uint step)
        {
            //Assert.Fail();
            var e = new GenerateEnumerator(start, last, step);
            Assert.IsTrue(e.MoveNext());
            Assert.IsTrue(e.MoveNext());
            return e.Current;
        }

        [TestCase(1u, 1u, 1u, Result = false)]
        [TestCase(11u, 11u, 3u, Result = false)]
        [TestCase(11u, 21u, 3u, Result = true)]
        public bool MoveNextTest(uint start, uint last, uint step)
        {
            //Assert.Fail();
            var e = new GenerateEnumerator(start, last, step);
            Assert.IsTrue(e.MoveNext());
            return e.MoveNext();
        }


        [TestCase(1u, 2u, 1u, Result = 2u)]
        [TestCase(1u, 1u, 1u, Result = 1u)]
        [TestCase(1u, 10u, 1u, Result = 10u)]
        [TestCase(1u, 3u, 5u, Result = 1u)]
        [TestCase(4u, 6u, 1u, Result = 6u)]
        [TestCase(4u, 6u, 10u, Result = 4u)]
        public uint LastValueTest(uint start, uint last, uint step)
        {
            //Assert.Fail();
            var e = new GenerateEnumerator(start, last, step);
            uint res = 0u;
            while (e.MoveNext()) { res = e.Current; }
            return res;
        }

        [Test]
        public void InvalidOperationTest()
        {
            //Assert.Fail();
            var e = new GenerateEnumerator(1u, 1u, 1u);
            while (e.MoveNext()) ;
            object x;
            Assert.Throws<InvalidOperationException>(() => x = e.Current);
        }

        [Test]
        public void NoMoveNextTest()
        {
            //Assert.Fail();
            var e = new GenerateEnumerator(1u, 2u, 3u);
            object x;
            Assert.Throws<InvalidOperationException>(() => x = e.Current );
        }

        // 一般のEnumeratorの動作確認用
        [Test]
        public void EnumeratorTest()
        {
            //Assert.Fail();
            var arr = new int[] { 1, 2, 3 };
            var e = arr.GetEnumerator();
            Assert.True(e.MoveNext());
            Assert.AreEqual(1, e.Current);
            Assert.True(e.MoveNext());
            Assert.AreEqual(2, e.Current);
            Assert.True(e.MoveNext());
            Assert.AreEqual(3, e.Current);
            Assert.False(e.MoveNext());
            object x;
            Assert.Throws<InvalidOperationException>( () => x = e.Current );
        }


    }
}
