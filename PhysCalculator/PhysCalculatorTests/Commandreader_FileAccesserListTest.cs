using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CommandParser;
using PhysicalCalculator;

namespace PhysCalculatorTests
{
    
    
    /// <summary>
    ///This is a test class for Commandreader_FileAccesserListTest and is intended
    ///to contain all Commandreader_FileAccesserListTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Commandreader_FileAccesserListTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for CommandAccessorStack Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PhysCalc.exe")]
        public void Commandreader_CommandAccessorStackConstructorTest()
        {
            CommandAccessorStack target = new CommandAccessorStack();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for GetCommandLine
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PhysCalc.exe")]
        public void GetCommandLineTest()
        {
            CommandAccessorStack target = new CommandAccessorStack(); 
            string ResultLine = "test"; 
            string ResultLineExpected = "test";
            string expected = null;
            string actual;
            actual = target.GetCommandLine(ref ResultLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }
    }
}
