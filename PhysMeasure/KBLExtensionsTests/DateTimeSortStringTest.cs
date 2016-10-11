using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace ExtensionsTests
{


    /// <summary>
    ///This is a test class for DateTimeSortStringTest and is intended
    ///to contain all DateTimeSortStringTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DateTimeSortStringTest
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
        ///A test for ToSortShortDateString
        ///</summary>
        [TestMethod()]
        public void ToSortShortDateStringTest()
        {
            DateTime Me = new DateTime(1963, 1, 17, 12, 13, 14);
            string expected = "1963-01-17";
            string actual;
            // actual = KBL.Extensions.DateTimeSortString.ToSortShortDateString(Me);
            actual = Me.ToSortShortDateString();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ToSortString
        ///</summary>
        [TestMethod()]
        public void ToSortStringTest()
        {
            DateTime Me = new DateTime(1963, 1, 17, 12, 13, 14);
            string expected = "1963-01-17 12:13:14";
            string actual;
            //actual = KBL.Extensions.DateTimeSortString.ToSortString(Me);
            //actual = Me.ToSortShortDateString();
            actual = Me.ToSortString();
            Assert.AreEqual(expected, actual);
        }
    }
}
