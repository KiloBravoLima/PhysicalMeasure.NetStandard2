using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CommandParser;
using PhysicalCalculator;

namespace PhysCalculatorTests
{
    /// <summary>
    ///This is a test class for Commandreader_FileAccesserTest and is intended
    ///to contain all Commandreader_FileAccesserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Commandreader_FileAccesserTest
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
        ///A test for CommandFileAccessor Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PhysCalc.exe")]
        public void Commandreader_CommandFileAccessorConstructorTest()
        {
            string FileNameStr = "Filename.ext";
            CommandFileAccessor target = new CommandFileAccessor(FileNameStr);
            Assert.IsNotNull(target);
            Assert.AreEqual(target.FileNameStr, FileNameStr);
        }

        /// <summary>
        ///A test for CommandFileAccessor Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PhysCalc.exe")]
        public void Commandreader_CommandFileAccessorConstructorTest1()
        {
            CommandFileAccessor target = new CommandFileAccessor();
            Assert.IsNotNull(target);
            //Assert.IsNull(target.FileNameStr);
            string expected = string.Empty;
            Assert.AreEqual(expected, target.FileNameStr);
        }

        /// <summary>
        ///A test for ReadFromFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PhysCalc.exe")]
        public void ReadFromFileTest_FileNotExists()
        {
            string FileNameStr = "Filename.ext";
            CommandFileAccessor target = new CommandFileAccessor(FileNameStr);
            string ResultLine = string.Empty; 
            string ResultLineStartExpected = "File '";
            string ResultLineEndExpected = "Filename.ext' not found";
            string expected = string.Empty;
            string actual;
            actual = target.GetCommandLine(ref ResultLine);
            //Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.IsTrue(ResultLine.StartsWith(ResultLineStartExpected));
            Assert.IsTrue(ResultLine.EndsWith(ResultLineEndExpected));
            Assert.AreEqual(expected, actual);
            //Assert.IsTrue(actual.StartsWith(expectedStart));
            //Assert.IsTrue(actual.EndsWith(expectedEnd));
        }

        /*****************
        /// <summary>
        ///A test for ReadFromFile
        ///</summary>
        [TestMethod()]
        [DeploymentItem("PhysCalc.exe")]
        public void ReadFromFileTest_FileExists()
        {

            string FileNameStr = "test.cal";
            Commandreader_Accessor.FileAccesser target = new Commandreader_Accessor.FileAccesser(FileNameStr);
            string ResultLine = string.Empty;
            string ResultLineExpected = string.Empty;
            string expected = "// Test.cal";
            string actual;
            actual = target.ReadFromFile(ref ResultLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }
        *****************/
    }
}
