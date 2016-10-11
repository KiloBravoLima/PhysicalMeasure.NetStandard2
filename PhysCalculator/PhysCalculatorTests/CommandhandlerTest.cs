using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using CommandParser;
using PhysicalCalculator;

namespace PhysCalculatorTests
{
    
    
    /// <summary>
    ///This is a test class for CommandhandlerTest and is intended
    ///to contain all CommandhandlerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CommandhandlerTest
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
        ///A test for Commandhandler Constructor
        ///</summary>
        [TestMethod()]
        public void CommandhandlerConstructorWithArgsTest()
        {
            string[] args = null; // TODO: Initialize to an appropriate value
            Commandhandler target = new Commandhandler(args);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Commandhandler Constructor
        ///</summary>
        [TestMethod()]
        public void CommandhandlerConstructorNoArgsTest()
        {
            Commandhandler target = new Commandhandler();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for ParseChar
        ///</summary>
        [TestMethod()]
        public void ParseCharTest_CharFound()
        {
            Commandhandler target = new Commandhandler(); 
            char ch = 'K'; 
            string CommandLine = "K is found"; 
            string CommandLineExpected = " is found";
            string ResultLine = string.Empty; 
            string ResultLineExpected = string.Empty; 
            bool expected = true;
            bool actual;
            actual = target.ParseChar(ch, ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ParseChar
        ///</summary>
        [TestMethod()]
        public void ParseCharTest_CharNotFound()
        {
            Commandhandler target = new Commandhandler();
            char ch = 'K';
            string CommandLine = "Char K is not first char and not found";
            string CommandLineExpected = CommandLine;
            string ResultLine = string.Empty; 
            string ResultLineExpected = " Char 'K' expected"; 
            bool expected = false;
            bool actual;
            actual = target.ParseChar(ch, ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ParseToken found
        ///</summary>
        [TestMethod()]
        public void ParseTokenTest_TokenFound()
        {
            Commandhandler target = new Commandhandler();
            string Token = "Keyword";
            string CommandLine = "Keyword. to find ";
            string CommandLineExpected = ". to find ";
            string ResultLine = string.Empty;
            string ResultLineExpected = string.Empty; 
            bool expected = true; 
            bool actual;
            actual = target.ParseToken(Token, ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ParseToken not found
        ///</summary>
        [TestMethod()]
        public void ParseTokenTest_TokenNotFound()
        {
            Commandhandler target = new Commandhandler();
            string Token = "Keyword";
            string CommandLine = " keywor? to find ";
            string CommandLineExpected = " keywor? to find ";
            string ResultLine = string.Empty;
            string ResultLineExpected = " Token 'Keyword' expected";
            bool expected = false;
            bool actual;
            actual = target.ParseToken(Token, ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ReadToken
        ///</summary>
        [TestMethod()]
        public void CommandhandlerTest_ReadTokenTest()
        {
            Commandhandler target = new Commandhandler(); 
            string CommandLine = "keyword to find ";
            string Token = string.Empty;
            string TokenExpected = "keyword";
            string expected = "to find "; 
            string actual;
            actual = target.ReadToken(CommandLine, out Token);
            Assert.AreEqual(TokenExpected, Token);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for SkipToken
        ///</summary>
        [TestMethod()]
        public void CommandhandlerTest_SkipTokenTest()
        {
            Commandhandler target = new Commandhandler(); 
            string Token = "Keyword";
            string CommandLine = "keyword. to find ";
            string expected = ". to find "; 
            string actual;
            actual = target.SkipToken(Token, CommandLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for StartsWithKeyword
        ///</summary>
        [TestMethod()]
        public void StartsWithKeywordTest()
        {
            Commandhandler target = new Commandhandler(); 
            string Keyword = "Keyword"; 
            string CommandLine = "Keyword in front of rest of line";
            bool expected = true;
            bool actual;
            actual = target.StartsWithKeyword(Keyword, CommandLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryParseChar
        ///</summary>
        [TestMethod()]
        public void TryParseCharTest_CharFound()
        {
            Commandhandler target = new Commandhandler(); 
            char ch = 'W'; 
            string CommandLine = "Word in front of rest of line";
            string CommandLineExpected = "ord in front of rest of line"; 
            bool expected = true; 
            bool actual;
            actual = target.TryParseChar(ch, ref CommandLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryParseChar
        ///</summary>
        [TestMethod()]
        public void TryParseCharTest_CharNotFound()
        {
            Commandhandler target = new Commandhandler(); 
            char ch = 'W'; 
            string CommandLine = "word in front of rest of line";
            string CommandLineExpected = "word in front of rest of line"; 
            bool expected = false; 
            bool actual;
            actual = target.TryParseChar(ch, ref CommandLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryParseToken
        ///</summary>
        [TestMethod()]
        public void TryParseTokenTest_TokenFound()
        {
            Commandhandler target = new Commandhandler(); 
            string Token = "Keyword"; 
            string CommandLine = "keyword in front of line"; 
            string CommandLineExpected = "in front of line"; 
            bool expected = true; 
            bool actual;
            actual = target.TryParseToken(Token, ref CommandLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for TryParseToken
        ///</summary>
        [TestMethod()]
        public void TryParseTokenTest_TokenNotFound()
        {
            Commandhandler target = new Commandhandler(); 
            string Token = "Keyword"; 
            string CommandLine = "token in front of line";
            string CommandLineExpected = "token in front of line"; 
            bool expected = false; 
            bool actual;
            actual = target.TryParseToken(Token, ref CommandLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for Command
        ///</summary>
        [TestMethod()]
        public void CommandTest_1_Empty()
        {
            Commandhandler target = new Commandhandler();
            string CommandLine = string.Empty; 
            string ResultLine = string.Empty;
            string ResultLineExpected = "Unknown command";
            bool expected = false; 
            bool actual;
            actual = target.Command(ref CommandLine, out ResultLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Command
        ///</summary>
        [TestMethod()]
        public void CommandTest_2_Space()
        {
            Commandhandler target = new Commandhandler();
            string CommandLine = " "; 
            string ResultLine = string.Empty;
            string ResultLineExpected = "Unknown command"; 
            bool expected = false; 
            bool actual;
            actual = target.Command(ref CommandLine, out ResultLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Command
        ///</summary>
        [TestMethod()]
        public void CommandTest_3_PrintExpresion_1Add2()
        {
            Commandhandler target = new Commandhandler();
            string CommandLine = "print 1 + 2";
            string ResultLine = string.Empty;
            string ResultLineExpected = "Unknown command";
            bool expected = false;
            bool actual;
            actual = target.Command(ref CommandLine, out ResultLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

    }
}
