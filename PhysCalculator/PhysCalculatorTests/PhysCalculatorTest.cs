using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

using PhysicalMeasure;

using PhysicalCalculator;
using PhysicalCalculator.Identifiers;
using System.Linq;

namespace PhysCalculatorTests
{
    
    
    /// <summary>
    ///This is a test class for PhysCalculatorTest and is intended
    ///to contain all PhysCalculatorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PhysCalculatorTest
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
        ///A test for PhysCalculator Constructor
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorConstructorTest()
        {
            string[] PhysCalculatorConfig_args = {"read test", "print 1 m + 2 Km"}; 
            PhysCalculator target = new PhysCalculator(PhysCalculatorConfig_args);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for PhysCalculator Constructor
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorConstructorTest1()
        {
            CommandReader CommandLineReader = new CommandReader(); 
            string[] PhysCalculatorConfig_args = { "read test", "print 1 m + 2 Km" }; 
            PhysCalculator target = new PhysCalculator(CommandLineReader, PhysCalculatorConfig_args);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for PhysCalculator Constructor
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorConstructorTest2()
        {
            PhysCalculator target = new PhysCalculator();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for PhysCalculator Constructor
        ///</summary>
        [TestMethod()]
        public void PhysCalculatorConstructorTest3()
        {
            CommandReader CommandLineReader = null; 
            PhysCalculator target = new PhysCalculator(CommandLineReader);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Command
        ///</summary>
        [TestMethod()]
        public void CommandTest()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "Print 13 g + 99.987 Kg - 10 mg";
            string ResultLine = string.Empty;
            //string ResultLineExpected = "99.99999 Kg"; 
            string ResultLineExpected = (99999.99).ToString()+" g"; 
            bool expected = true; 
            bool actual;
            actual = target.Command(ref CommandLine, out ResultLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandClear
        ///</summary>
        [TestMethod()]
        public void CommandClearTest()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = string.Empty; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty; 
            string ResultLineExpected = string.Empty; 
            bool expected = true; 
            bool actual;
            actual = target.CommandClear(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Command About
        ///</summary>
        [TestMethod()]
        public void CommandAboutTest()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "About";
            string ResultLine = string.Empty;
            string CommandLineExpected = string.Empty; 
            List<String> ResultLineExpected = new List<String>();
            ResultLineExpected.Add("PhysCalculator");
            ResultLineExpected.Add("PhysCalc");
            ResultLineExpected.Add("PhysicalMeasure");
            ResultLineExpected.Add("codeplex");

            bool expected = true;
            bool actual;
            actual = target.Command(ref CommandLine, out ResultLine);

            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(expected, actual);

            for (int i = 0; i < ResultLineExpected.Count; i++)
            {
                Assert.IsTrue(ResultLine.Contains(ResultLineExpected[i]), "ResultLine (" + (i+1).ToString() + ") expected to contain " + ResultLineExpected[i] + ", but contains \"" + ResultLine + "\"");
            }

        }


        /// <summary>
        ///A test for CommandComment
        ///</summary>
        [TestMethod()]
        public void CommandCommentTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            string CommandLine = "// test of comment"; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty;
            //string ResultLineExpected = "// test of comment"; 
            string ResultLineExpected = string.Empty;
            bool expected = true; 
            bool actual;
            actual = target.CommandComment(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandHelp
        ///</summary>
        [TestMethod()]
        public void CommandHelpTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            string CommandLine = string.Empty; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty;
            /***
            string ResultLineExpected = 
                     "Commands:\n"
                    +"    Read <filename>\n"
                    +"    Save <filename>\n"
                    +"    Set <varname> = <expression>\n"
                    +"    Print <expression> [, <expression> ]* \n"
                    +"    Store <varname>\n"
                    +"    Remove <varname>\n"
                    +"    Clear\n"
                    +"    List\n"
                    +"    Read <filename>"; 
            ***/

            /***
            List<String> ResultLineExpected = new List<String>();
            ResultLineExpected.Add("Include <filename>");
            ResultLineExpected.Add("Save <filename>");
            ResultLineExpected.Add("Set <varname> [ = ] <expression> [, <varname> [ = ] <expression> ]*");
            ResultLineExpected.Add("[ Print ] <expression> [, <expression> ]*");
            ResultLineExpected.Add("List");
            ResultLineExpected.Add("Store <varname>");
            ResultLineExpected.Add("Remove <varname>");
            ResultLineExpected.Add("Clear");
            ***/

            List<String> ResultLineExpected = new List<String>();
            ResultLineExpected.Add("Include <filename>");
            ResultLineExpected.Add("Save [ items | commands ] <filename>");
            ResultLineExpected.Add("Files [ -sort=create | write | access ] [ [-path=] <folderpath> ]");
            ResultLineExpected.Add("Using [ universal | electromagnetic | atomic ]");
            ResultLineExpected.Add("Var [ <contextname> . ] <varname> [ = <expression> ] [, <var> ]*");
            ResultLineExpected.Add("Set <varname> [ = ] <expression> [, <varname> [ = ] <expression> ]*");
            ResultLineExpected.Add("System <systemname>");
            ResultLineExpected.Add("Unit [ <systemname> . ] <unitname> [ [ = ] <expression> ]");
            ResultLineExpected.Add("[ Print ] <expression> [, <expression> ]*");
            ResultLineExpected.Add("List [ items ] [ settings ] [ commands ] ");
            ResultLineExpected.Add("Store <varname>");
            ResultLineExpected.Add("Remove <varname> [, <varname> ]*");
            ResultLineExpected.Add("Clear [ items | commands ]");
            ResultLineExpected.Add("Func <functionname> ( <paramlist> )  { <commands> }");
            ResultLineExpected.Add("Help [ expression | parameter | commands | setting | all ]");

            bool expected = true; 
            bool actual;
            actual = target.CommandHelp(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(expected, actual);

            for (int i = 0; i < ResultLineExpected.Count; i++)
            {
                Assert.IsTrue(ResultLine.Contains(ResultLineExpected[i]), "ResultLine expected to contain \"" + ResultLineExpected[i] + "\", but contains \"" + ResultLine + "\"");
            }

        }

        /// <summary>
        ///A test for CommandList
        ///</summary>
        [TestMethod()]
        public void CommandListTest()
        {
            PhysCalculator target = new PhysCalculator(); 

            string CommandLine = "varname = 3 N * 4 m";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            //string ResultLineExpected = "12 m2·Kg·s-2";

            bool expected = true;
            bool actual;

            actual = target.CommandSet(ref CommandLine, ref ResultLine);


            CommandLine = string.Empty; 
            CommandLineExpected = string.Empty; 
            ResultLine = string.Empty;
            //ResultLineExpected = " Global:\r\nvar varname = 12 m2·Kg·s-2";
            List<String> ResultLinesExpected = new List<String>();
            ResultLinesExpected.Add("Global:");
            //ResultLinesExpected.Add("var varname = 12 m2·Kg·s-2");
            ResultLinesExpected.Add("var varname = 12 J");

            expected = true;
            actual = target.CommandList(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            //Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);

            for (int i = 0; i < ResultLinesExpected.Count; i++)
            {
                Assert.IsTrue(ResultLine.Contains(ResultLinesExpected[i]), "ResultLine expected to contain \"" + ResultLinesExpected[i] + "\"");
            }

        }

        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrintTest()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "123 J·S·Ω/m + 0.456 m·Kg·s-2"; 
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = (123.456).ToString()+" N"; 
            bool expected = true; 
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine, "for commandLine" );
            Assert.AreEqual(ResultLineExpected, ResultLine, "for ResultLine");
            Assert.AreEqual(expected, actual, "for result");
        }


        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint2Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "1 m/s * 60 s/min * 60 min/h";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = (3600).ToString() + " m/h";
            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine, "for commandLine");
            Assert.AreEqual(ResultLineExpected, ResultLine, "for ResultLine");
            Assert.AreEqual(expected, actual, "for result");
        }


        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_GWh_pr_TWh_Test()
        {
            PhysCalculator target = new PhysCalculator();
            //string CommandLine = "set Var4 = 3451776 GW·h / 20200 TWh";
            string CommandLine = "3451776 GW·h / 20200 TWh";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "0,17088";
            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine, "for commandLine");
            Assert.AreEqual(ResultLineExpected, ResultLine, "for ResultLine");
            Assert.AreEqual(expected, actual, "for result");
        }


        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_HPlusMinPlusS_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "123.25 h + 38.920123133333 min + 364.789012 s";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            //string ResultLineExpected = "123:59:59,9964000000091 h:min:s";
            string ResultLineExpected = (123.999999).ToString() + " h";
            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_HPlusMinPlusS_ConvertTo_HMinS_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "123.25 h + 38.920123133333 min + 364.789012 s [h:min:s]";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "123:59:59,9964000000091 h:min:s";
            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_HPlusMinPlusS_ConvertTo_DHMinS_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "123.25 h + 38.920123133333 min + 364.789012 s [d:h:min:s]";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "5:03:59:59,9963999999579 d:h:min:s";

            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_DPlusHPlusMinPlusS_ConvertTo_DHMinS_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "5 d  + 23.25 h + 42.5 min + 149.123 s [d:h:min:s]";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "5:23:59:59,1230000000314 d:h:min:s";

            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_Nm_ConvertTo_kWh_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "12 N * 34 m [kWh]";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = (12d*34/(1000*3600)).ToString() +" KW·h";

            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_m3_ConvertTo_hl_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "1.2 m * 3.4 m * 0.56 m [hl]";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = (1.2 * 3.4 * 0.56 * 1000/100).ToString() + " Hl";

            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_GWd_ConvertTo_GWh_Test()
        {

            /*
// Saved 2014-05-19 13:35:00 to ShortenFractions.cal
// 


set Var1 = 1010 GW * 0,4 * 356 d * 24 h/d
// 3451776 GW·h

// set Var2 = 1010 GW * 0,4 * 356 d * 24 h/d [TWh]
// // 3451,776 TW·h
// set Var3 = 1010 GW * 0,4 * 356 d * 24 h/d / 20200 TWh
// // 170,88 GW/TW
              
            */
            PhysCalculator target = new PhysCalculator();
            //string CommandLine = "1010 GW * 0,4 * 356 d * 24 h/d";
            string CommandLine = "2 GW * 3 d * 4 h/d";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            //string ResultLineExpected = (1010 * 0.4 * 356 * 24).ToString() + " GW·h";
            string ResultLineExpected = (2 * 3 * 4).ToString() + " GW·h";

            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_GWd_ConvertTo_GWh_full_Test()
        {

            /*
// Saved 2014-05-19 13:35:00 to ShortenFractions.cal
// 


set Var1 = 1010 GW * 0,4 * 356 d * 24 h/d
// 3451776 GW·h

// set Var2 = 1010 GW * 0,4 * 356 d * 24 h/d [TWh]
// // 3451,776 TW·h
// set Var3 = 1010 GW * 0,4 * 356 d * 24 h/d / 20200 TWh
// // 170,88 GW/TW
              
            */
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "1010 GW * 0,4 * 356 d * 24 h/d";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = (1010 * 0.4 * 356 * 24).ToString() + " GW·h";
            
            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_number_MultTo_MWh_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "982 * 1000 MWh";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = (982 * 1000).ToString() + " MW·h";
            
            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_MWh_MultTo_h_par_d_MultTo_d_per_x_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "1000 MW * 24 h/d * 356 d/x";
            string CommandLineExpected = "x"; 

            string ResultLine = string.Empty;
            string ResultLineExpected = "The string argument is not in a valid physical expression format. Invalid or missing operand at 'x' at position 25\n"
                                         + (1000 * 24 * 356).ToString() + " MW·h";

            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_MWh_MultTo_h_par_x_MultTo_d_per_z_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "1000 MW * 24 h/xx * 356 d/z";
            string CommandLineExpected = "xx * 356 d/z";
            string ResultLine = string.Empty;

            string ResultLineExpected = "The string argument is not in a valid physical expression format. Invalid or missing operand at 'xx * 356 d/z' at position 15\n"
                                        + (1000 * 24 ).ToString() + " MW·h";

            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_MWh_MultTo_h_par_d_MultTo_d_per_year_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "1000 MW * 24 h/d * 356 d/year";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = (1000 * 24 * 356).ToString() + " MW·h/year";

            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for CommandPrint
        ///</summary>
        [TestMethod()]
        public void CommandPrint_number_MultTo_MWh_MultTo_ms_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "982 * 1000 MWh * 123 ms";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = (982 * 1000 * 123).ToString() + " MW·h·ms";

            bool expected = true;
            bool actual;
            actual = target.CommandPrint(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for CommandReadFromFile
        ///</summary>
        [TestMethod()]
        public void CommandReadFromFileNameTest()
        {
            PhysCalculator target = new PhysCalculator();
            //string commandLine = string.Empty; 
            string CommandLine = "epotfunc.cal";
            string CommandLineExpected = string.Empty;
            string ResultLine = "";
            string ResultLineExpected = "Reading from 'epotfunc.cal' ";
            bool expected = true;
            bool actual;
            actual = target.CommandReadFromFile(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine, "for commandLine");
            Assert.AreEqual(ResultLineExpected, ResultLine, "for ResultLine");
            Assert.AreEqual(expected, actual, "for actual");
        }


        /// <summary>
        ///A test for CommandReadFromFile
        ///</summary>
        [TestMethod()]
        public void CommandReadFromFileCommandTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            //string commandLine = string.Empty; 
            string CommandLine = "epotfunc.cal";
            //string commandLine = "testfunc.cal"; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = "";
            string ResultLineExpected = "Reading from 'epotfunc.cal' ";
            // string ResultLineExpected = "Reading from 'testfunc.cal' "; 
            bool expected = true; 
            bool actual;
            actual = target.CommandReadFromFile(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine, "for commandLine");
            Assert.AreEqual(ResultLineExpected, ResultLine, "for ResultLine");
            Assert.AreEqual(expected, actual, "for actual");

            target.Run();

            //Assert.AreEqual(expectedResultLines, target.resultLines , "for resultLines");
        }


        /// <summary>
        ///A test for CommandRemove
        ///</summary>
        [TestMethod()]
        public void CommandRemoveTest_VarNotFound()
        {
            PhysCalculator target = new PhysCalculator(); 
            string CommandLine = "varname"; 
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "'varname' not known";
            bool expected = false; 
            bool actual;
            actual = target.CommandRemove(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandRemove
        ///</summary>
        [TestMethod()]
        public void CommandRemoveTest_VarFound()
        {
            PhysCalculator target = new PhysCalculator();

            string CommandLine = "varname = 3 N * 4 m";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "12 m2Kgs-2";
            bool expected = true;
            bool actual;

            actual = target.CommandSet(ref CommandLine, ref ResultLine);

            CommandLine = "varname";
            CommandLineExpected = string.Empty;
            ResultLine = string.Empty;
            ResultLineExpected = string.Empty;
            expected = true;
            actual = target.CommandRemove(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for CommandSaveToFile
        ///</summary>
        [TestMethod()]
        public void CommandSaveToFileTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            string CommandLine = "savetest.cal"; 
            string CommandLineExpected = string.Empty; 
            string ResultLine = string.Empty;
            string ResultLineExpected = "Saved "; 
            bool expected = true; 
            bool actual;
            actual = target.CommandSaveToFile(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine, "for CommandLine");
            Assert.AreEqual(ResultLineExpected, ResultLine.Substring(0, ResultLineExpected.Length), "for ResultLine");
            Assert.AreEqual(expected, actual, "for actual");
        }

        /// <summary>
        ///A test for CommandSet
        ///</summary>
        [TestMethod()]
        public void CommandSetTest()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "varname = 3 N * 4 m";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "varname = 12 J";
            bool expected = true; 
            bool actual;
            actual = target.CommandSet(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine, "commandLine not as expected");
            Assert.AreEqual(ResultLineExpected, ResultLine, "ResultLine not as expected");
            Assert.AreEqual(expected, actual, "CommandSet() return value not as expected");
        }

        /// <summary>
        ///A test for CommandSet
        ///</summary>
        [TestMethod()]
        public void CommandSet_PeriodInDays_Test()
        {
            PhysCalculator target = new PhysCalculator();
            string CommandLine = "PeriodInDays = 1220 day";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "PeriodInDays = " + (1220).ToString() + " day";
            bool expected = true;
            bool actual;
            actual = target.Command(ref CommandLine, out ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine, "commandLine not as expected");
            Assert.AreEqual(ResultLineExpected, ResultLine, "ResultLine not as expected");
            Assert.AreEqual(expected, actual, "CommandSet() return value not as expected");
        }


        /// <summary>
        ///A test for CommandStore
        ///</summary>
        [TestMethod()]
        public void CommandStoreTest()
        {
            PhysCalculator target = new PhysCalculator();

            // Set some value in accumulator
            string CommandLine = "3 N * 4 m";
            string ResultLine = string.Empty;
            target.CommandPrint(ref CommandLine, ref ResultLine);

            // Store accumulator value to var
            CommandLine = "Varname";
            string CommandLineExpected = string.Empty; 
            ResultLine = string.Empty;
            string ResultLineExpected = "12 J";
            bool expected = true; 
            bool actual;
            actual = target.CommandStore(ref CommandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, CommandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for CommandIf
        ///</summary>
        [TestMethod()]
        public void CommandIfTest()
        {
            PhysCalculator target = new PhysCalculator();
            target.Setup();
            string CommandLine = "if (123 J·S·Ω/m + 0.456 m·Kg·s-2 == 123.456 N) { Print 111 } else { if (1 == 2-1 ) { print 222/0 } else { print 333/0} }";
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = string.Empty; // = null;
            Quantity AccumulatorExpected = new Quantity(111);
            bool expected = true;
            bool actual;
            Quantity AccumulatorActual;
            actual = target.Command(ref CommandLine, out ResultLine);
            string AccumulatorAccessResultLineExpected = "";

            Assert.AreEqual(true, target.VariableGet(null, "Accumulator", out AccumulatorActual, ref AccumulatorAccessResultLineExpected), "for accumulator access");
            Assert.AreEqual(AccumulatorExpected, AccumulatorActual, "for accumulator");

            Assert.AreEqual(CommandLineExpected, CommandLine, "for commandLine");
            Assert.AreEqual(ResultLineExpected, ResultLine, "for ResultLine");
            Assert.AreEqual(expected, actual, "for result");
        }


        /// <summary>
        ///A test for Command "unit USD; 1500 USD /0.23 KWh"
        ///</summary>
        [TestMethod()]
        public void CommandPrint_Combined_user_defined_unit_and_physical_unit_Test()
        {
            PhysCalculator target = new PhysCalculator();
            target.Setup();
            string[] CommandLines = { "unit USD", "1500 USD /0.23 KWh" };
            List<String> ResultLines = new List<string>();
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "\0ResetColor";

            //bool expected = true;
            //bool actual;

            CalculatorEnvironment localContext = new CalculatorEnvironment() ;
            ResultWriter resultLineWriter = new ResultWriter(ResultLines);
            CommandReader commandLineReader = new CommandReader(CommandLines, resultLineWriter) ;
            target.ExecuteCommands(localContext, commandLineReader, resultLineWriter);

            Quantity AccumulatorActual;
            // actual = target.Command(ref CommandLine, out ResultLine);
            string AccumulatorAccessResultLineExpected = "";

            PhysicalCalculator.Expression.IEnvironment context;
            INametableItem USDItem;

            Assert.AreEqual(true, target.VariableGet(null, "Accumulator", out AccumulatorActual, ref AccumulatorAccessResultLineExpected), "for accumulator access");
            Assert.AreEqual(true, target.IdentifierItemLookup("USD", out context, out USDItem), "for accumulator access");
            CombinedUnit ExpectedUnit = new CombinedUnit();
            Assert.AreEqual(USDItem.Identifierkind, IdentifierKind.Unit, "For USD unit item");
            ExpectedUnit = ExpectedUnit.CombineMultiply(((NamedUnit)USDItem).pu);

            CombinedUnit kWh_Unit = SI.W.CombineMultiply(Prefix.k).CombineMultiply(SI.h);
            ExpectedUnit = ExpectedUnit.CombineDivide(kWh_Unit);

            // IPhysicalUnit ExpectedUnit;
            Quantity AccumulatorExpected = new Quantity(6521.73913043478, ExpectedUnit);  // {6521,73913043478 USD/KW·h}
            Assert.AreEqual(AccumulatorExpected, AccumulatorActual, "For accumulator");

            ResultLine = ResultLines[ResultLines.Count - 1]; 
            Assert.AreEqual(ResultLineExpected, ResultLine, "For ResultLine");


            // Assert.AreEqual(CommandLineExpected, CommandLine, "for commandLine");
            //Assert.AreEqual(ResultLineExpected, ResultLine, "for ResultLine");
            // Assert.AreEqual(expected, actual, "for result");

            // Clean up global info for default unit system
            Physics.CurrentUnitSystems.Reset();
        }

        /// <summary>
        ///A test for Command "unit USD; 1500 USD /0.23 KWh"
        ///</summary>
        [TestMethod()]
        public void CommandPrint_Combined_user_defined_unit_and_physical_unit_Test_2()
        {
            PhysCalculator target = new PhysCalculator();
            target.Setup();
            string[] CommandLines = 
              { "unit DKR",
                "unit Øre = 0.01 DKR",
                "set EnergyUnitPrice = 241.75 Øre /1.0 KWh",
                "set EnergyConsumed = 1234.56 kWh",
                "set PriceEnergyConsumed = EnergyConsumed * EnergyUnitPrice",
                "print PriceEnergyConsumed [DKR]",
                "set PriceDKREnergyConsumed=PriceEnergyConsumed [DKR]"
              };
            List<String> ResultLines = new List<string>();
            string CommandLineExpected = string.Empty;
            string ResultLine = string.Empty;
            string ResultLineExpected = "\0ResetColor";

            CalculatorEnvironment localContext = new CalculatorEnvironment();
            ResultWriter resultLineWriter = new ResultWriter(ResultLines);
            CommandReader commandLineReader = new CommandReader(CommandLines, resultLineWriter);
            target.ExecuteCommands(localContext, commandLineReader, resultLineWriter);

            Quantity AccumulatorActual;
            // actual = target.Command(ref CommandLine, out ResultLine);
            string AccumulatorAccessResultLineExpected = "";

            PhysicalCalculator.Expression.IEnvironment context;
            INametableItem DKRItem;
            INametableItem OereItem;
            INametableItem EnergyUnitPriceItem;
            INametableItem EnergyConsumedItem;
            INametableItem PriceEnergyConsumedItem;
            INametableItem PriceDKREnergyConsumedItem;

            Assert.AreEqual(true, target.VariableGet(null, "Accumulator", out AccumulatorActual, ref AccumulatorAccessResultLineExpected), "for accumulator access");
            Assert.AreEqual(true, target.IdentifierItemLookup("DKR", out context, out DKRItem), "for DKR access");
            Assert.AreEqual(true, target.IdentifierItemLookup("Øre", out context, out OereItem), "for Øre access");
            Assert.AreEqual(true, target.IdentifierItemLookup("EnergyUnitPrice", out context, out EnergyUnitPriceItem), "for EnergyUnitPrice access");
            Assert.AreEqual(true, target.IdentifierItemLookup("EnergyConsumed", out context, out EnergyConsumedItem), "for EnergyConsumed access");
            Assert.AreEqual(true, target.IdentifierItemLookup("PriceEnergyConsumed", out context, out PriceEnergyConsumedItem), "for PriceEnergyConsumed access");
            Assert.AreEqual(true, target.IdentifierItemLookup("PriceDKREnergyConsumed", out context, out PriceDKREnergyConsumedItem), "for PriceDKREnergyConsumedItem access");

            Assert.AreEqual(IdentifierKind.Unit, DKRItem.Identifierkind, " for DKR unit item");
            Assert.AreEqual(IdentifierKind.Unit, OereItem.Identifierkind, " for Øre unit item");
            Assert.AreEqual(IdentifierKind.Variable, EnergyUnitPriceItem.Identifierkind, " for EnergyUnitPrice variable item");
            Assert.AreEqual(IdentifierKind.Variable, EnergyConsumedItem.Identifierkind, " for EnergyConsumed variable item");
            Assert.AreEqual(IdentifierKind.Variable, PriceEnergyConsumedItem.Identifierkind, " for PriceEnergyConsumed variable item");
            Assert.AreEqual(IdentifierKind.Variable, PriceDKREnergyConsumedItem.Identifierkind, " for PriceDKREnergyConsumed variable item");

            IQuantity EnergyUnitPriceExpected = new Quantity(241.75, ((NamedUnit)OereItem).pu.Divide( SI.W * Prefix.k * SI.h));
            IQuantity EnergyConsumedExpected = new Quantity( 1234.56, SI.W * Prefix.k * SI.h);
            IQuantity PriceEnergyConsumedExpected = new Quantity(298454.88, ((NamedUnit)OereItem).pu);
            IQuantity PriceDKREnergyConsumedExpected = new Quantity(2984.5488, ((NamedUnit)DKRItem).pu);
            IQuantity AccumulatorExpected = new Quantity(2.4175 * 1234.56, ((NamedUnit)DKRItem).pu);

            IQuantity pq = PriceEnergyConsumedItem as IQuantity;
            Unit pu = pq.Unit;
            UnitKind uk = pu.Kind;

            ICombinedUnit cu = pu as ICombinedUnit;
            int noOfNumerators = cu.Numerators.Count;
            int noOfDenominators = cu.Denominators.Count;
            IUnitSystem SSus = cu.SomeSimpleSystem;

            IUnitSystem Eus = cu.ExponentsSystem;

            SByte[] esponents = cu.Exponents;

            Assert.AreEqual(EnergyUnitPriceExpected, EnergyUnitPriceItem as Quantity, "for EnergyUnitPrice");
            Assert.AreEqual(EnergyConsumedExpected, EnergyConsumedItem as Quantity, "for EnergyConsumed");
            Assert.AreEqual(PriceEnergyConsumedExpected, PriceEnergyConsumedItem as Quantity, "for PriceEnergyConsumed");
            Assert.AreEqual(PriceDKREnergyConsumedExpected, PriceDKREnergyConsumedItem as Quantity, "for PriceDKREnergyConsumed");
            Assert.AreEqual(AccumulatorExpected, AccumulatorActual, "for accumulator");

            ResultLine = ResultLines[ResultLines.Count - 1];
            Assert.AreEqual(ResultLineExpected, ResultLine, "for ResultLine");

            // Clean up global info for default unit system
            Physics.CurrentUnitSystems.Reset();
        }


        public static String PhysicalExpressionEval(String txtExpression, ResultWriter ResultLineWriter = null)
        {
            String Result = "Internal error";
            try
            {
                if (ResultLineWriter == null)
                {
                    ResultLineWriter = new ResultWriter();
                }
                CommandReader CommandLineReader = new CommandReader(txtExpression, ResultLineWriter);
                if (CommandLineReader == null)
                {
                    ResultLineWriter.WriteErrorLine(String.Format("PhysCalculator CommandReader failed to load with {0} arguments: \"{1}\" ", 1, txtExpression));
                }
                else
                {
                    CommandLineReader.ReadFromConsoleWhenEmpty = true;
                    PhysCalculator Calculator = new PhysCalculator(CommandLineReader, ResultLineWriter);
                    if (Calculator == null)
                    {
                        ResultLineWriter.WriteErrorLine(String.Format("PhysCalculator failed to load with {0} arguments: \"{1}\" ", 1, txtExpression));
                    }
                    else
                    {
                        CommandLineReader.ReadFromConsoleWhenEmpty = false;
                        CommandLineReader.WriteCommandToResultWriter = false;
                        ResultLineWriter.ResultLines = new List<String>();
                        // ResultLineWriter.WriteLine("PhysCalculator ready ");
                        Calculator.Run();
                        //ResultLineWriter.WriteLine("PhysCalculator finished");

                        if (ResultLineWriter.ResultLines.Count > 0)
                        {   // Use 
                            /*
                            Result = "";
                            ResultLineWriter.ResultLines.ForEach(ResultLine  =>
                            {
                                // Console.WriteLine(name);
                                if (!String.IsNullOrEmpty(Result))
                                {
                                    Result += "\n";
                                }
                                Result += ResultLine;
                            }
                            );
                            */

                            Result = ResultLineWriter.ResultLines.Aggregate("", (result, resultLine) =>
                            {
                                // Console.WriteLine(name);
                                if (!String.IsNullOrEmpty(result))
                                {
                                    result += "\n";
                                }
                                result += resultLine;
                                return result;
                            }
                            );
                        }
                    }
                }
            }
            /* 
            catch (EvalException ex)
            {
                // Report expression error and move caret to error position
                Result = "Evaluation error : " + ex.Message;
            }
            */
            catch (Exception ex)
            {
                // Unknown error
                Result = "Unexpected error : " + ex.Message;
            }

            return Result;
        }

        static void ShowPhysicalMeasureEval(ResultWriter ResultLineWriter, string str)
        {
            ResultWriter singleLineWriter = new ResultWriter();
            singleLineWriter.UseColors = false;
            string res = PhysicalExpressionEval(str, singleLineWriter);
            ResultLineWriter.WriteLine(str + " = " + res);
        }


        static void ShowStartLines(ResultWriter ResultLineWriter)
        {
            ResultLineWriter.WriteLine("c#:");

            ResultLineWriter.WriteLine("3 / 4 * 5 / 6 = " + (3D / 4 * 5 / 6).ToString());
            ResultLineWriter.WriteLine("(3 / 4) * (5 / 6) = " + ((3D / 4) * (5D / 6)).ToString());
            ResultLineWriter.WriteLine("3 / 4 / 5 * 6 / 7 / 8 = " + (3D / 4 / 5 * 6D / 7 / 8).ToString());
            ResultLineWriter.WriteLine("((3 / 4) / 5) * ((6 / 7) / 8) = " + (((3D / 4) / 5) * ((6D / 7) / 8)).ToString());

            ResultLineWriter.WriteLine("3 + -2 = " + (3 + -2).ToString());
            ResultLineWriter.WriteLine("3 - -2 = " + (3 - -2).ToString());
            ResultLineWriter.WriteLine("3 - - -2 = " + (3 - - -2).ToString());
            ResultLineWriter.WriteLine("3 - - - -2 = " + (3 - - - -2).ToString());
            ResultLineWriter.WriteLine("3 - - - + -2 = " + (3 - - -+-2).ToString());
            ResultLineWriter.WriteLine("3 - - - + + -2 = " + (3 - - -+ +-2).ToString());

            ResultLineWriter.WriteLine("PhysicalMeasure:");

            ShowPhysicalMeasureEval(ResultLineWriter, "3 / 4 * 5 / 6");
            ShowPhysicalMeasureEval(ResultLineWriter, "(3 / 4) * (5 / 6)");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 / 4 / 5 * 6 / 7 / 8");
            ShowPhysicalMeasureEval(ResultLineWriter, "((3 / 4) / 5) * ((6 / 7) / 8)");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 + -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - - -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - - - -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - - - + -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - - - + + -2");
        }


        /// <summary>
        ///A test for testing how C3 and PhysicalCalculator.Program.PhysicalExpressionEval evaluates expressions"
        ///</summary>
        [TestMethod()]
        public void TestShowStartLines()
        {
            ResultWriter ResultLineWriter = new ResultWriter();
            ResultLineWriter.ResultLines = new List<string>(); 

            ResultLineWriter.UseColors = false;
            ShowStartLines(ResultLineWriter);

            if (ResultLineWriter.ResultLines.Count > 2)
            {   
                // Output all lines
                ResultLineWriter.ResultLines.ForEach(ResultLine => Console.WriteLine(ResultLine));
                
                // Check result lines
                int NoOfExpressions = ResultLineWriter.ResultLines.Count / 2 - 1;
                for (int ExpIndex = 1; ExpIndex < NoOfExpressions; ExpIndex++)
                {
                    Assert.AreEqual(ResultLineWriter.ResultLines[ExpIndex], ResultLineWriter.ResultLines[ExpIndex + NoOfExpressions + 1]);
                }
            }
        }

        /*****************
        /// <summary>
        ///A test for GetQuantity
        ///</summary>
        [TestMethod()]
        public void GetQuantityTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string commandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IQuantity expected = null; // TODO: Initialize to an appropriate value
            IQuantity actual;
            actual = target.GetQuantity(ref commandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, commandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseConvertedExpression
        ///</summary>
        [TestMethod()]
        public void ParseConvertedExpressionTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string commandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IQuantity expected = null; // TODO: Initialize to an appropriate value
            IQuantity actual;
            actual = target.ParseConvertedExpression(ref commandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, commandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseExpression
        ///</summary>
        [TestMethod()]
        public void ParseExpressionTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string commandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IQuantity expected = null; // TODO: Initialize to an appropriate value
            IQuantity actual;
            actual = target.ParseExpression(ref commandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, commandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        *****************/

            /**
            /// <summary>
            ///A test for ParseExpressionList
            ///</summary>
            [TestMethod()]
            public void ParseExpressionListTest_1()
            {
                PhysCalculator target = new PhysCalculator();
                string commandLine = " 1 , 2 m, 3 KgN, 4.5 J/K , 6 h,7 g, 8 Km , 9 mm , 10 mKgs-2 ";
                string CommandLineExpected = string.Empty; 
                string ResultLine = string.Empty; 
                string ResultLineExpected = string.Empty; 
                List<IQuantity> expected = new List<IQuantity>();
                expected.Add(PhysicalMeasure.Quantity.Parse(" 1 "));
                expected.Add(PhysicalMeasure.Quantity.Parse(" 2 m "));
                expected.Add(PhysicalMeasure.Quantity.Parse("3 KgN "));
                expected.Add(PhysicalMeasure.Quantity.Parse("4.5 J/K "));
                expected.Add(PhysicalMeasure.Quantity.Parse("6 h "));
                expected.Add(PhysicalMeasure.Quantity.Parse("7 g "));
                expected.Add(PhysicalMeasure.Quantity.Parse("8 Km"));
                expected.Add(PhysicalMeasure.Quantity.Parse("9 mm"));
                expected.Add(PhysicalMeasure.Quantity.Parse("10 mKgs-2"));

                List<IQuantity> actual;
                actual = target.ParseExpressionList(ref commandLine, ref ResultLine);
                Assert.AreEqual(CommandLineExpected, commandLine);
                Assert.AreEqual(ResultLineExpected, ResultLine);

                for (int i = 0; i < expected.Count; i++ )
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
            **/

            /// <summary>
            ///A test for ParseExpressionList
            ///</summary>
        [TestMethod()]
        public void RunArgCommandsTest()
        {

            string[] PhysCalculatorConfig_args =
            {
                "set SoundSpeed = 340 m/s",
                "set SoundSpeedInKmPrHour = SoundSpeed [ Km/h ]",
                "list;",
                "save unittest_1"
            }; 
            PhysCalculator target = new PhysCalculator(PhysCalculatorConfig_args);
            Assert.IsNotNull(target);

            target.Run();

            Quantity VarValue;
            Quantity VarValueExpected = new Quantity(340, SI.m/SI.s);
            String ResultLine = null;
            bool GetVarRes = target.VariableGet(target.CurrentContext, "SoundSpeed", out VarValue, ref ResultLine);
            Assert.IsTrue(GetVarRes);
            Assert.AreEqual<IQuantity>( VarValueExpected, VarValue);
      
            VarValueExpected = new Quantity(1224, new CombinedUnit() *  new PrefixedUnitExponent(Prefix.K, SI.m, 1) /SI.h);
            GetVarRes = target.VariableGet(target.CurrentContext, "SoundSpeedInKmPrHour", out VarValue, ref ResultLine);
            Assert.IsTrue(GetVarRes);
            Assert.AreEqual<IQuantity>( VarValueExpected, VarValue);
        }

        /*****************
        /// <summary>
        ///A test for ParseFactor
        ///</summary>
        [TestMethod()]
        public void ParseFactorTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string commandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IQuantity expected = null; // TODO: Initialize to an appropriate value
            IQuantity actual;
            actual = target.ParseFactor(ref commandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, commandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseOptionalConvertedExpression
        ///</summary>
        [TestMethod()]
        public void ParseOptionalConvertedExpressionTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            IQuantity pq = null; // TODO: Initialize to an appropriate value
            string commandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IQuantity expected = null; // TODO: Initialize to an appropriate value
            IQuantity actual;
            actual = target.ParseOptionalConvertedExpression(pq, ref commandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, commandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseOptionalExpression
        ///</summary>
        [TestMethod()]
        public void ParseOptionalExpressionTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            IQuantity pq = null; // TODO: Initialize to an appropriate value
            string commandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IQuantity expected = null; // TODO: Initialize to an appropriate value
            IQuantity actual;
            actual = target.ParseOptionalExpression(pq, ref commandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, commandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ParseOptionalTerm
        ///</summary>
        [TestMethod()]
        public void ParseOptionalTermTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            IQuantity pq = null; // TODO: Initialize to an appropriate value
            string commandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IQuantity expected = null; // TODO: Initialize to an appropriate value
            IQuantity actual;
            actual = target.ParseOptionalTerm(pq, ref commandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, commandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        

        /// <summary>
        ///A test for ParseTerm
        ///</summary>
        [TestMethod()]
        public void ParseTermTest()
        {
            PhysCalculator target = new PhysCalculator(); // TODO: Initialize to an appropriate value
            string commandLine = string.Empty; // TODO: Initialize to an appropriate value
            string CommandLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLine = string.Empty; // TODO: Initialize to an appropriate value
            string ResultLineExpected = string.Empty; // TODO: Initialize to an appropriate value
            IQuantity expected = null; // TODO: Initialize to an appropriate value
            IQuantity actual;
            actual = target.ParseTerm(ref commandLine, ref ResultLine);
            Assert.AreEqual(CommandLineExpected, commandLine);
            Assert.AreEqual(ResultLineExpected, ResultLine);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
        *****************/

        /// <summary>
        ///A test for VariableGet
        ///</summary>
        [TestMethod()]
        public void VariableGetTest()
        {
            PhysCalculator target = new PhysCalculator();
            string VariableName = "Testvar";
            Quantity VariableValue = 2 * SI.N * SI.m;
            bool expected = true;
            bool actual;
            actual = target.VariableSet(VariableName, VariableValue);
            Assert.AreEqual(expected, actual);

            Quantity VariableValueExpected = VariableValue;
            String ResultLine = null;
            actual = target.VariableGet(target.CurrentContext, VariableName, out VariableValue, ref ResultLine);
            Assert.AreEqual(VariableValueExpected, VariableValue);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for VariableRemove
        ///</summary>
        [TestMethod()]
        public void VariableRemoveTest()
        {
            PhysCalculator target = new PhysCalculator();
            string VariableName = "Testvar";
            Quantity VariableValue = 2 * SI.N * SI.m;
            bool expected = true;
            bool actual;
            actual = target.VariableSet(VariableName, VariableValue);
            Assert.AreEqual(expected, actual);

            //actual = target..VariableRemove(VariableName);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for VariableSet
        ///</summary>
        [TestMethod()]
        public void VariableSetTest()
        {
            PhysCalculator target = new PhysCalculator();
            string VariableName = "Testvar";
            Quantity VariableValue = 2 * SI.N * SI.m; 
            bool expected = true; 
            bool actual;
            actual = target.VariableSet(VariableName, VariableValue);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for VariablesClear
        ///</summary>
        [TestMethod()]
        public void VariablesClearTest()
        {
            PhysCalculator target = new PhysCalculator(); 
            bool expected = true; 
            bool actual;
            // 
            actual = target.IdentifiersClear();
            //actual = target.IdentifiersClear();
            Assert.AreEqual(expected, actual);
        }
    }
}
