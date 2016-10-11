using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

using TokenParser;

namespace PhysCalculatorTests
{
    
    
    /// <summary>
    ///This is a test class for TokenStringTest and is intended
    ///to contain all TokenStringTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TokenStringTest
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
        ///A test for ReadIdentifier
        ///</summary>
        [TestMethod()]
        public void ReadIdentifierTest()
        {
            string CommandLine = "Some_name in this line";
            string Identifier = string.Empty;
            string IdentifierExpected = "Some_name";
            string expected = "in this line";  
            string actual;
            actual = TokenString.ReadIdentifier(CommandLine, out Identifier);
            Assert.AreEqual(IdentifierExpected, Identifier);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ReadToken
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorTests_ReadTokenTest()
        {
            string CommandLine = "Some_token in this line";
            string Token = string.Empty;
            string TokenExpected = "Some_token";
            string expected = "in this line";
            string actual;
            actual = TokenString.ReadToken(CommandLine, out Token);
            Assert.AreEqual(TokenExpected, Token);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SkipToken
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorTests_SkipTokenTest()
        {
            string CommandLine = "Some_token in this line";
            string Token = "Some_token";
            string expected = "in this line";
            string actual;
            actual = TokenString.SkipToken(CommandLine, Token);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for StartsWithKeyword
        ///</summary>
        [TestMethod()]
        public void StartsWithKeywordTest_Found()
        {
            string CommandLine = "Some_keyword in this line";
            string Keyword = "Some_keyword";
            bool expected = true; 
            bool actual;
            actual = TokenString.StartsWithKeyword(CommandLine, Keyword);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for StartsWithKeyword
        ///</summary>
        [TestMethod()]
        public void StartsWithKeywordTest_NotFound()
        {
            string CommandLine = "Some_keyword in this line";
            string Keyword = "Somekeyword";
            bool expected = false;
            bool actual;
            actual = TokenString.StartsWithKeyword(CommandLine, Keyword);
            Assert.AreEqual(expected, actual);
        }

    }
}
