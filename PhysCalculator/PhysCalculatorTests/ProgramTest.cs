using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using PhysicalCalculator;

namespace PhysCalculatorTests
{
    
    
    /// <summary>
    ///This is a test class for ProgramTest and is intended
    ///to contain all ProgramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProgramTest
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
        ///A test for Program Constructor
        ///</summary>
        [TestMethod()]
        public void ProgramConstructorTest()
        {
            Program target = new Program();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Main
        /// Code coverage enlarger
        ///</summary>
        [TestMethod()]
        // [DeploymentItem("*.cal")]
        [DeploymentItem("AllTestFiles.cal")]
        [DeploymentItem("Expression.cal")]
        [DeploymentItem("Valuta.cal")]
        [DeploymentItem("SecInYear.cal")]
        [DeploymentItem("TestFunc.cal")]
        [DeploymentItem("EPotFunc.cal")]
        [DeploymentItem("PhysCalc.exe")]
        public void MainTest()
        {
            string[] args = {"read AllTestFiles ; Exit"};
            try
            {
                Program.Main(args);

                // var pt_program = new PrivateType(typeof(Program));
                // pt_program.InvokeStatic( "Main", args);
                //Assert.Inconclusive("A method that does not return a value cannot be verified.");
            }
            catch 
            {
                // Must not throw any (unhandled) exceptions
                Assert.Fail();
            }
        }
    }
}
