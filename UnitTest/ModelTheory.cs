using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Abaqus;

namespace UnitTest
{
    class ModelTheory
    {
        [Datapoint]
        static Model Empty = new Model();


        [Theory]
        public void モデルはモデルである(Model model)
        {
            Assert.That(model.isModel, Is.True);
        }

        [Theory]
        public void 親はいない(Model model)
        {
            Assert.That(model.parent, Is.Null);
        }

        [Theory]
        public void モデルのモデルは自分である(Model model)
        {
            Assert.That(model.model, Is.SameAs(model));
        }
        
    }
}
