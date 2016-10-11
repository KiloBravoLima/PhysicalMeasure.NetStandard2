using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhysicalMeasure;

namespace PhysicalMeasureTest
{
    /// <summary>
    /// Summary description for ExamplesTest
    /// </summary>
    [TestClass]
    public class ExamplesTest
    {
        public ExamplesTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CalculateEnergyIn1GramTest()
        {
            Quantity actual = new PhysicalMeasureExamples.PhysicalMeasureExamples().CalculateEnergyIn1Gram();

            Quantity expected = (0.001 * 299792458 * 299792458) * SI.J;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculatePriceInEuroForEnergiConsumedTest()
        {
            String actual = new PhysicalMeasureExamples.PhysicalMeasureExamples().CalculatePriceInEuroForEnergiConsumed();
            String expected = "391 € 97 ¢"; 
            Assert.AreEqual(expected, actual);
        }

    }
}
