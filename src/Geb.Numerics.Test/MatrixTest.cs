using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Geb.Numerics.Test
{
    [TestClass]
    public class MatrixTest
    {
        [TestMethod]
        public void TestInverse()
        {
            double[] data = new double[] { 1, 3, 2, 5};
            Matrix m = new Matrix(data,2,2);
            Assert.AreEqual(m[0, 0], 1);
            Matrix inv = m.Inverse();
            Assert.AreEqual(inv[0, 0], -5);
        }
    }
}
