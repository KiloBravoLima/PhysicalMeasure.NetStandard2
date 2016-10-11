using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

using System.Reflection;
using PhysicalMeasure;

using TokenParser;
using CommandParser;

using PhysicalCalculator.Identifiers;
using PhysicalCalculator.CommandBlock;
using PhysicalCalculator.Function;
using PhysicalCalculator.Expression;

namespace PhysicalCalculator
{
    class PhysCalculator : Commandhandler
    {
        CommandReader CommandLineReader = null;
        ResultWriter ResultLineWriter = null;

        const string AccumulatorName = "Accumulator";
        Quantity Accumulator = null;
        CalculatorEnvironment GlobalContext;
        public CalculatorEnvironment CurrentContext;

        public PhysCalculator()
        {
            InitGlobalContext();

            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = new CommandReader("Calculator Prompt", this.ResultLineWriter);
        }

        public PhysCalculator(CommandReader someCommandLineReader)
        {
            InitGlobalContext();

            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = someCommandLineReader;
        }

        public PhysCalculator(String[] PhysCalculatorConfig_args)
        {
            InitGlobalContext();

            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = new CommandReader("Calculator Prompt", PhysCalculatorConfig_args, this.ResultLineWriter);
        }

        public PhysCalculator(CommandReader someCommandLineReader, String[] PhysCalculatorConfig_args)
        {
            InitGlobalContext();

            this.ResultLineWriter = new ResultWriter();
            this.CommandLineReader = someCommandLineReader;
        }

        public PhysCalculator(CommandReader someCommandLineReader, ResultWriter someResultLineWriter)
        {
            InitGlobalContext();

            this.ResultLineWriter = someResultLineWriter;
            this.CommandLineReader = someCommandLineReader;
        }

        private void FillPredefinedSystemContext(CalculatorEnvironment somePredefinedSystem)
        {
            // (Physical quantity) constants
            somePredefinedSystem.NamedItems.AddItem("False", new NamedConstant(PhysicalCalculator.Expression.PhysicalExpression.PQ_False));
            somePredefinedSystem.NamedItems.AddItem("True", new NamedConstant(PhysicalCalculator.Expression.PhysicalExpression.PQ_True));

            // Physical quantity functions
            somePredefinedSystem.NamedItems.AddItem("Pow", new PhysicalQuantityFunction_PQ_SB(somePredefinedSystem, (pq, exp) => pq.Pow(exp)));
            somePredefinedSystem.NamedItems.AddItem("Rot", new PhysicalQuantityFunction_PQ_SB(somePredefinedSystem, (pq, exp) => pq.Rot(exp)));
        }

        private void UsingUniversalPhysicalConstants(CalculatorEnvironment someEnvironment)
        {
            // PhysicalMeasure.Constants  Table of universal constants
            someEnvironment.NamedItems.AddItem("c", new NamedConstant(PhysicalMeasure.Constants.c));
            someEnvironment.NamedItems.AddItem("G", new NamedConstant(PhysicalMeasure.Constants.G));
            someEnvironment.NamedItems.AddItem("h", new NamedConstant(PhysicalMeasure.Constants.h));
            someEnvironment.NamedItems.AddItem("h_bar", new NamedConstant(PhysicalMeasure.Constants.h_bar));
        }

        private void UsingElectromagneticPhysicalConstants(CalculatorEnvironment someEnvironment)
        {
            // PhysicalMeasure.Constants  Table of electromagnetic constants
            someEnvironment.NamedItems.AddItem("my0", new NamedConstant(PhysicalMeasure.Constants.my0));
            someEnvironment.NamedItems.AddItem("epsilon0", new NamedConstant(PhysicalMeasure.Constants.epsilon0));
            someEnvironment.NamedItems.AddItem("Z0", new NamedConstant(PhysicalMeasure.Constants.Z0));
            someEnvironment.NamedItems.AddItem("ke", new NamedConstant(PhysicalMeasure.Constants.ke));

            someEnvironment.NamedItems.AddItem("e", new NamedConstant(PhysicalMeasure.Constants.e));
            someEnvironment.NamedItems.AddItem("myB", new NamedConstant(PhysicalMeasure.Constants.myB));
            someEnvironment.NamedItems.AddItem("G0", new NamedConstant(PhysicalMeasure.Constants.G0));
            someEnvironment.NamedItems.AddItem("K_J", new NamedConstant(PhysicalMeasure.Constants.KJ));
            someEnvironment.NamedItems.AddItem("phi0", new NamedConstant(PhysicalMeasure.Constants.phi0));
            someEnvironment.NamedItems.AddItem("myN", new NamedConstant(PhysicalMeasure.Constants.myN));
            someEnvironment.NamedItems.AddItem("RK", new NamedConstant(PhysicalMeasure.Constants.RK));
        }
        
        private void UsingAtomicAndNuclearPhysicalConstants(CalculatorEnvironment someEnvironment)
        {
        }

        private CalculatorEnvironment InitPredefinedSystemContext()
        {
            CalculatorEnvironment PredefinedSystem = new CalculatorEnvironment("Predefined Identifiers", EnvironmentKind.NamespaceEnv);
            PredefinedSystem.OutputTracelevel = TraceLevels.None;
            //
            PredefinedSystem.FormatProviderSource = FormatProviderKind.DefaultFormatProvider;
            //PredefinedSystem.FormatProviderSource = FormatProviderKind.InvariantFormatProvider;

            FillPredefinedSystemContext(PredefinedSystem);

            return PredefinedSystem;
        }

        private void InitGlobalContext()
        {
            GlobalContext = new CalculatorEnvironment(InitPredefinedSystemContext(), "Global", EnvironmentKind.NamespaceEnv);
            //
            GlobalContext.FormatProviderSource = FormatProviderKind.DefaultFormatProvider;
            //GlobalContext.FormatProviderSource = FormatProviderKind.InvariantFormatProvider;

            CurrentContext = GlobalContext;
        }

        public IEnvironment GetDeclarationEnvironment()
        {
            IEnvironment NewItemDeclarationNamespace;

            if (CurrentContext.DefaultDeclarationEnvironment == VariableDeclarationEnvironment.Global)
            {
                NewItemDeclarationNamespace = GlobalContext;
            }
            else
            {
                NewItemDeclarationNamespace = CurrentContext;
            }

            return NewItemDeclarationNamespace;
        }

        public void Setup()
        {
            // Setup Lookup callback delegate static globals
            PhysicalExpression.IdentifierItemLookupCallback = IdentifierItemLookup;
            PhysicalExpression.QualifiedIdentifierItemLookupCallback = QualifiedIdentifierItemLookup;
            PhysicalExpression.IdentifierContextLookupCallback = IdentifierContextLookup;
            PhysicalExpression.QualifiedIdentifierContextLookupCallback = QualifiedIdentifierCalculatorContextLookup;

            PhysicalExpression.VariableValueGetCallback = VariableGet;
            PhysicalExpression.UnitGetCallback = UnitGet;

            PhysicalExpression.FunctionLookupCallback = FunctionLookup;
            PhysicalExpression.FunctionEvaluateCallback = FunctionEvaluate;
            PhysicalExpression.FunctionEvaluateFileReadCallback = FunctionEvaluateFileRead;

            PhysicalCommandBlock.ExecuteCommandsCallback = ExecuteCommandsCallback;

            PhysicalFunction.ExecuteCommandsCallback = ExecuteFunctionCommandsCallback; // ExecuteCommandsCallback;

            CurrentContext.ParseState = CommandParserState.ExecuteCommandLine;
        }

        public void Run()
        {
            Setup();
            ExecuteCommands(CurrentContext, CommandLineReader, ResultLineWriter);
        }

        public void ExecuteCommands(CalculatorEnvironment localContext, CommandReader commandLineReader, ResultWriter resultLineWriter)
        {
            Boolean CommandLineFromAccessor = false;

            Boolean CommandLineEmpty;
            Boolean ResultLineEmpty; 
            Boolean LoopExit;

            do
            {
                String FullCommandLine;
                String CommandLine;
                String ResultLine;

                CommandLineEmpty = true;
                ResultLineEmpty = true;
                LoopExit = false;
                try
                {
                    ResultLine = "";

                    CommandLineFromAccessor = commandLineReader.HasAccessor();
                    commandLineReader.ReadCommand(ref ResultLine, out FullCommandLine);
                    CommandLineEmpty = String.IsNullOrWhiteSpace(FullCommandLine);
                    ResultLineEmpty = String.IsNullOrWhiteSpace(ResultLine);
                    if (!ResultLineEmpty)
                    {
                        resultLineWriter.WriteLine(ResultLine);
                        ResultLine = "";
                        LoopExit = false;   // Show error 
                    }

                    if (!CommandLineEmpty)
                    {
                        CommandLine = FullCommandLine.Trim();
                        do
                        {
                            CommandLine = CommandLine.Trim();

                            if (localContext.FunctionToParseInfo != null)
                            {
                                LoopExit = !FunctionDeclaration(ref CommandLine, out ResultLine);
                            }
                            else
                            {
                                // LoopExit = CommandLine.Equals("Exit", StringComparison.OrdinalIgnoreCase);
                                LoopExit = TryParseToken("Exit", ref CommandLine);
                                if (!LoopExit)
                                {
                                    LoopExit = !Command(ref CommandLine, out ResultLine);
                                }
                            }

                            if (!LoopExit)
                            {
                                if (!string.IsNullOrEmpty(CommandLine))
                                {
                                    if (!TryParseToken(";", ref CommandLine))
                                    {
                                        if (!String.IsNullOrEmpty(ResultLine))
                                        {
                                            ResultLine += ". ";
                                        }

                                        ResultLine += "Do not understand \"" + CommandLine + "\".";
                                        CommandLine = "";
                                    }
                                }

                                if (!String.IsNullOrWhiteSpace(ResultLine))
                                {
                                    resultLineWriter.ForegroundColor = ConsoleColor.White;
                                    resultLineWriter.WriteLine(ResultLine);
                                    resultLineWriter.ResetColor();
                                }
                                else
                                {
                                    Boolean ShowEmptyResultLine = (localContext.FunctionToParseInfo == null)
                                                                && !CommandLineFromAccessor;

                                    if (ShowEmptyResultLine)
                                    {
                                        resultLineWriter.ForegroundColor = ConsoleColor.Yellow;
                                        resultLineWriter.WriteLine("?");
                                        resultLineWriter.ResetColor();
                                    }
                                }
                            }
                        } while (!LoopExit && !String.IsNullOrWhiteSpace(CommandLine));
                    }
                }
                catch (Exception e)
                {
                    /**
                        Possible exceptions:
                            InvalidCastException("The 'obj' argument is not a IUnit object.");
                            ArgumentException("object'unitStr physical unit " + temp.Unit.ToString() + " is not convertible to a " + ConvToUnitName);
                            ArgumentException("object is not a IQuantity");
                            ArgumentException("Physical quantity is not a pure unit; but has a value = " + physicalQuantity.Value.ToString());
                            InvalidCastException("The 'obj' argument is not a IQuantity object."); 
                            ArgumentException("object'unitStr physical unit " + pq2.Unit.ToString()+ " is not convertible to a " + pq1.Unit.ToString());
                      
                    **/
                    String Message = String.Format("{0} Exception Source: {1} - {2}", e.GetType().ToString(), e.Source, e.ToString());
                    resultLineWriter.WriteErrorLine(Message);
                    LoopExit = false;
                }
            } while ((CommandLineFromAccessor || !CommandLineEmpty || !ResultLineEmpty) && !LoopExit);
        }

        public Boolean ExecuteFunctionCommandsCallback(CalculatorEnvironment localContext, List<String> funcBodyCommands, ref String funcBodyResult, out Quantity functionResult)
        {
            // Dummy: Never used    funcBodyResult
            CommandReader functionCommandLineReader = new CommandReader(localContext.Name, funcBodyCommands.ToArray(), CommandLineReader.ResultLineWriter);
            functionCommandLineReader.ReadFromConsoleWhenEmpty = false; // Return from ExecuteCommands() function when funcBodyCommands are done

            if (localContext.OutputTracelevel.HasFlag(TraceLevels.FunctionEnterLeave))
            {
                ResultLineWriter.WriteLine("Enter " + localContext.Name);
            }
            ExecuteCommands(localContext, functionCommandLineReader, ResultLineWriter);
            functionResult = Accumulator;
            if (localContext.OutputTracelevel.HasFlag(TraceLevels.FunctionEnterLeave))
            {
                ResultLineWriter.WriteLine("Leave " + localContext.Name);
            }
            return true;
        }

        public Boolean ExecuteCommandsCallback(CalculatorEnvironment localContext, List<String> commands, ref String resultLine, out Quantity commandBlockResult)
        {
            // Dummy: Never used    resultLine
            CommandReader CommandBlockLineReader = new CommandReader(localContext.Name, commands.ToArray(), CommandLineReader.ResultLineWriter);
            CommandBlockLineReader.ReadFromConsoleWhenEmpty = false; // Return from ExecuteCommands() function when funcBodyCommands are done

            if (localContext.OutputTracelevel.HasFlag(TraceLevels.BlockEnterLeave))
            {
                ResultLineWriter.WriteLine("Enter " + localContext.Name);
            }
            ExecuteCommands(localContext, CommandBlockLineReader, ResultLineWriter);
            commandBlockResult = Accumulator;
            if (localContext.OutputTracelevel.HasFlag(TraceLevels.BlockEnterLeave))
            {
                ResultLineWriter.WriteLine("Leave " + localContext.Name);
            }
            return true;
        }

        public override Boolean Command(ref String commandLine, out String resultLine)
        {
            Boolean CommandHandled = false;
            resultLine = "Unknown Command";
            Boolean CommandFound =     CheckForCommand("//", CommandComment, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Read", CommandReadFromFile, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Include", CommandReadFromFile, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Load", CommandReadFromFile, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Save", CommandSaveToFile, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Files", CommandListFiles, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Using", CommandUsingConstants, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Var", CommandVar, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Set", CommandSet, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("System", CommandSystem, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Unit", CommandUnit, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Print", CommandPrint, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("List", CommandList, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Store", CommandStore, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Remove", CommandRemove, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Clear", CommandClear, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Func", CommandFunc, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("If", CommandIf, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Help", CommandHelp, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("Version", CommandVersion, ref commandLine, ref resultLine, ref CommandHandled)
                                    || CheckForCommand("About", CommandAbout, ref commandLine, ref resultLine, ref CommandHandled)
                                    || (CommandHandled = IdentifierAssumed(ref commandLine, ref resultLine)) // Assume a print or set Command
                                    || (CommandHandled = CommandPrint(ref commandLine, ref resultLine)) // Assume a print Command
                                    || (CommandHandled = base.Command(ref commandLine, out resultLine));

            return CommandHandled;
        }

        public Boolean FunctionDeclaration(ref String commandLine, out String resultLine)
        {
            resultLine = "";
            Boolean DeclarationDone = ParseFunctionDeclaration(ref commandLine, ref resultLine);

            return true; // CommandHandled; Don't exit
        }

        #region Command methods

        enum CommandHelpParts
        {
            None = 0,
            Expression = 1,
            Parameter = 2,
            Command = 3,
            Setting = 4,
            all = 0xF
        }

        public Boolean CommandHelp(ref String commandLine, ref String resultLine)
        {
            CommandHelpParts HelpPart = CommandHelpParts.Command;
            if (commandLine.StartsWithKeywordPrefix("Expression") > 0)
                HelpPart = CommandHelpParts.Expression;
            else if (commandLine.StartsWithKeywordPrefix("Parameter") > 0)
                HelpPart = CommandHelpParts.Parameter;
            else if (commandLine.StartsWithKeywordPrefix("Commands") > 0)
                HelpPart = CommandHelpParts.Command;
            else if (commandLine.StartsWithKeywordPrefix("Setting") > 0)
                HelpPart = CommandHelpParts.Setting;
            else if (commandLine.StartsWithKeywordPrefix("All") > 0)
                HelpPart = CommandHelpParts.all;

            resultLine = "";
            if (HelpPart == CommandHelpParts.Command || HelpPart == CommandHelpParts.all)
            {   //            "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"
                //            "         1         2         3         4         5         6         7         8         9        10        11        12        13        14"
                resultLine += "Commands:\n"
                            + "    || Read | Include | Load||  <filename>                                   Reads commands from file and execute them\n"
                            + "    Save [ items | commands ] <filename>                                     Save to file the current variables and functions\n"
                            + "                                                                                 declarations, or the command history\n"
                            + "                                                                                 when 'commands' is specified\n"
                            + "    Files [ -sort=create | write | access ] [ [-path=] <folderpath> ]        List files in folder\n"
                            + "    Using [ universal | electromagnetic | atomic ]                           Declare constants from specified predefined constant group\n"

                            + "    Var [ <contextname> . ] <varname> [ = <expression> ] [, <var> ]*         Declare new Variable (local or in specified context)\n"
                            + "    Set <varname> [ = ] <expression> [, <varname> [ = ] <expression> ]*      Assign Variable or declare it locally if not already declared\n"
                            + "    System <systemname>                                                      Define new unit system\n"
                            + "    Unit [ <systemname> . ] <unitname> [ [ = ] <expression> ]                Define new unit. Without an Expression it becomes\n"
                            + "                                                                                 a base unit, else a converted (scaled) unit\n"
                            + "    [ Print ] <expression> [, <expression> ]*                                Evaluate expressions and show values\n"
                            + "    List [ items ] [ settings ] [ commands ]                                 Show All Variable values and functions declarations, \n"
                            + "                                                                                 setting and commands as specified\n"
                            + "    Store <varname>                                                          Save last calculation's result to Variable\n"
                            + "    Remove <varname> [, <varname> ]*                                         Remove Variable\n"
                            + "    Clear [ items | commands ]                                               Remove All variables or\n"
                            + "                                                                                 the command history when 'commands' is specified \n"
                            + "    Func <functionname> ( <paramlist> )  { <commands> }                      Declare a function\n"
                            + "    Help [ expression | parameter | commands | setting | all ]               Help on topic\n"
                            + "    Version                                                                  Shows application version info\n"
                            + "    About                                                                    Shows application info";
            }
            if (HelpPart == CommandHelpParts.Expression || HelpPart == CommandHelpParts.all)
            {
                if (HelpPart == CommandHelpParts.all)
                {
                    resultLine += "\n\n";
                }
                resultLine += "Expression:\n"
                         // + "    <Expression> = <CE> .                                                    Expression\n"
                            + "    <CE> = <E> [ '[' <SYS> ']' ]                                             Converted Expression\n"
                            + "    <E> = <T> [ ('+' | '-') <T> ]                                            Expression (simple/unconverted)\n"
                            + "    <T> = <F> [ ('*' | '/') <F> ]                                            Term\n"
                            + "    <F> = <PE> [ '^' number ]                                                Factor\n"
                            + "    <UE> = [ ('+' | '-') ] <E>                                               Unary operator Expression\n"
                            + "    <PE> = <PQ> | <UE> | <varname>                                           Primary Expression\n"
                            + "         | <functionname> '(' <explist> ')' | '(' <E> ')'  \n"
                            + "    <PQ> = number <SYSUNIT>                                                  Physical Quantity\n"
                            + "    <SYSUNIT> = [ sys '.' ] <SCALEDUNIT>                                     System unit\n"
                            + "    <SCALEDUNIT> = [ scaleprefix ] unit                                      Scaled Unit\n"
                            + "    <SYS> = sys | <SYSUNIT>                                                  System or unit\n"
                            + "    <explist> = <CE> [ ',' <CE> ]*                                           Expression List";
            }
            
            if (HelpPart == CommandHelpParts.Parameter || HelpPart == CommandHelpParts.all)
            {
                if (HelpPart == CommandHelpParts.all)
                {
                    resultLine += "\n\n";
                }
                resultLine += "Parameter list:\n"
                            + "    <paramlist> = <parameter> [ ',' <parameter> ]*                           Parameter list\n"
                            + "    <parameter> = <paramname> [ '[' <SYS> ']' ]                              Parameter";
            }

            if (HelpPart == CommandHelpParts.Setting || HelpPart == CommandHelpParts.all)
            {
                if (HelpPart == CommandHelpParts.all)
                {
                    resultLine += "\n\n";
                }
                resultLine += "Settings:\n"
                            + "    set [ <contextname> . ] Tracelevel = [normal|on|off|debug]               Set Tracelevel for current or specified context\n"
                            + "    set [ <contextname> . ] FormatProvider = [invariant|default|Inherited]   Set FormatProvider for current or specified context";
            }

            commandLine = "";
            return true;
        }

        public Boolean CommandVersion(ref String commandLine, ref String resultLine)
        {
            //PhysCalc
            System.Reflection.Assembly PhysCaclAsm = System.Reflection.Assembly.GetExecutingAssembly();

            //PhysicalMeasure
            System.Reflection.Assembly PhysicalMeasureAsm = typeof(Quantity).Assembly;

            resultLine = PhysCaclAsm.AssemblyInfo() + "\n" + PhysicalMeasureAsm.AssemblyInfo();

            commandLine = "";
            return true;
        }

        public Boolean CommandAbout(ref String commandLine, ref String resultLine)
        {
            //PhysCalc
            System.Reflection.Assembly PhysCaclAsm = System.Reflection.Assembly.GetExecutingAssembly();

            //PhysicalMeasure
            System.Reflection.Assembly PhysicalMeasureAsm = typeof(Quantity).Assembly;

            resultLine = "PhysCalculator" + "\n";
            resultLine += PhysCaclAsm.AssemblyInfo() + "\n" + PhysicalMeasureAsm.AssemblyInfo() + "\n";
            resultLine += "http://physicalmeasure.codeplex.com";

            commandLine = "";
            return true;
        }

        public Boolean CommandComment(ref String commandLine, ref String resultLine)
        {
            resultLine = ""; // = commandLine;
            commandLine = "";
            return true;
        }
        
        public Boolean CommandReadFromFile(ref String commandLine, ref String resultLine)
        {
            String FilePathStr;
            resultLine = "";
            int statementSeparatorIndex = commandLine.IndexOf(';');
            if (statementSeparatorIndex >= 0)
            {   // To end of statement
                FilePathStr = commandLine.Substring(0, statementSeparatorIndex);
                commandLine = commandLine.Substring(statementSeparatorIndex);
            }
            else
            {   // To end of line
                FilePathStr = commandLine;
                commandLine = "";
            }
            

            if ((CommandLineReader != null) && (!string.IsNullOrWhiteSpace(FilePathStr)))
            {
                CommandLineReader.AddFile(FilePathStr);
                resultLine = "Reading from '" + CommandLineReader.Accessor() + "' ";
            }
            return true;
        }

        public Boolean CommandSaveToFile(ref String commandLine, ref String resultLine)
        {
            bool SaveContext = true;

            if (TryParseTokenPrefix("Context", ref commandLine))
            {
                SaveContext = true;
            }
            else if (TryParseTokenPrefix("Commands", ref commandLine))
            {
                SaveContext = false;
            }

            String FileNameStr;
            FileNameStr = commandLine;
            commandLine = "";
            resultLine = "";

            if (!Path.HasExtension(FileNameStr))
            {
                FileNameStr += ".cal";
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileNameStr))
            {
                string ContentType = "";
                if (SaveContext)
                {
                    ContentType = "context";
                }
                else
                {
                    ContentType = "command history";
                }
                string headerLine = String.Format("Saved {0} {1} to {2}", DateTime.Now.ToSortString(), ContentType, FileNameStr);
                file.WriteLine("// {0}", headerLine);
                if (SaveContext)
                {
                    SaveContextToFile(CurrentContext, file);

                    if (Accumulator != null)
                    {
                        file.WriteLine("set {0} = {1}", AccumulatorName, Accumulator.ToString());
                    }
                }
                else
                {
                    SaveCommandHistoryToFile(CommandLineReader, file);
                }

                resultLine = headerLine;
            }
            return true;
        }

        public void SaveContextToFile(CalculatorEnvironment context, System.IO.StreamWriter file)
        {
            if (context.OuterContext != null)
            {
                SaveContextToFile(context.OuterContext, file);
            }

            file.WriteLine("// {0}", context.Name);

            foreach (KeyValuePair<String, INametableItem> Item in context.NamedItems)
            {
                String ItemTextLine = Item.Value.ToListString(Item.Key);
                file.WriteLine(ItemTextLine);
            }
        }


        public void SaveCommandHistoryToFile(CommandReader CommandLineReader, System.IO.StreamWriter file)
        {
            //file.WriteLine("// {0}", CommandLineReader);

            foreach (String CommandLine in CommandLineReader.CommandHistory)
            {
                file.WriteLine(CommandLine);
            }
        }


        public Boolean CommandListFiles(ref String commandLine, ref String resultLine)
        {
            String FilePathStr = "."; // Look in current (local) dir
            String SortStr = "name"; // Sort by file name
            Func<FileInfo, String> KeySelector;
            resultLine = "";

            if (!string.IsNullOrWhiteSpace(commandLine))
            {
                while (TryParseChar('-', ref commandLine))
                {
                    if (TryParseToken("sort", ref commandLine))
                    {
                        ParseChar('=', ref commandLine, ref resultLine);
                        commandLine = commandLine.ReadToken(out SortStr);
                    }
                    else if (TryParseToken("path", ref commandLine))
                    {
                        ParseChar('=', ref commandLine, ref resultLine);
                        //commandLine.ReadToken(out FilePathStr);
                        FilePathStr = commandLine;
                        commandLine = "";
                    }
                }

                // Last token is FilePathStr
                if (!string.IsNullOrWhiteSpace(commandLine))
                {
                    FilePathStr = commandLine;
                    commandLine = "";
                }
            }

            if (SortStr.StartsWithKeyword("create"))
            {
                KeySelector = (e => e.CreationTime.ToSortString());
            }
            else if (SortStr.StartsWithKeyword("write"))
            {
                KeySelector = (e => e.LastWriteTime.ToSortString());
            }
            else if (SortStr.StartsWithKeyword("access"))
            {
                KeySelector = (e => e.LastAccessTime.ToSortString());
            }
            else
            // if (SortStr.StartsWithKeyword("name"))
            {
                KeySelector = (e => e.Name);
            }

            StringBuilder ListStringBuilder = new StringBuilder();

            System.IO.DirectoryInfo dir;
            IEnumerable<FileInfo> CalFiles;
            try
            {
                dir = new System.IO.DirectoryInfo(FilePathStr);

                if (dir.Exists)
                {
                    // Get All of the .cal files from the directory and order them 
                    CalFiles = dir.GetFiles("*.cal").OrderBy(KeySelector);
                    int fileCount = 0;
                    foreach (FileInfo fi in CalFiles)
                    {
                        if (fileCount == 0)
                        {
                            ListStringBuilder.AppendFormat("{0,16}   {1,8}   {2,16}   {3,16}   {4,16}\n\r", "File name", "size/bytes", "last access", "last write", "created");
                        }
                        fileCount++;

                        ListStringBuilder.AppendFormat("{0,16}   {1,8}   {2,16}   {3,16}   {4,16}\n\r", fi.Name, fi.Length, fi.LastAccessTime, fi.LastWriteTime, fi.CreationTime);
                    }

                    ListStringBuilder.AppendFormat("Found {0} .cal files in '{1}'", fileCount, dir.FullName);
                }
                else
                {
                    ListStringBuilder.AppendFormat("Folder not found '{0}'", dir.FullName);
                }
            }
            catch (Exception e)
            {
                String Message = String.Format("{0} Exception Source: {1} - {2}", e.GetType().ToString(), e.Source, e.ToString());

                ListStringBuilder.AppendFormat("Folder not found '{0}'. {1}", FilePathStr, Message);
            }

            resultLine = ListStringBuilder.ToString();
            return true;
        }
        
        public Boolean CommandUsingConstants(ref String commandLine, ref String resultLine)
        {
            String ConstantGroupName;
            commandLine = commandLine.ReadIdentifier(out ConstantGroupName);
            if (ConstantGroupName == null)
            {
                resultLine = "Constant group name expected";
            }
            else
            {
                Debug.Assert(ConstantGroupName != null);

                if (ConstantGroupName.StartsWithKeywordPrefix("Universal") > 0)
                {   // PhysicalMeasure.Constants  Table of universal constants
                    UsingUniversalPhysicalConstants(CurrentContext);
                    resultLine = "Using universal constants";
                }
                else if (ConstantGroupName.StartsWithKeywordPrefix("Electromagnetic") > 0)
                {   // PhysicalMeasure.Constants  Table of electromagnetic constants
                    UsingElectromagneticPhysicalConstants(CurrentContext);
                    resultLine = "Using electromagnetic constants";
                }
                /*
                else if (ConstantGroupName.StartsWithKeywordPrefix("Atomic")  > 0)
                {   // PhysicalMeasure.Constants  Table of atomic and nuclear constants
                    UsingAtomicAndNuclearPhysicalConstants(CurrentContext);
                    resultLine = "Using atomic and nuclear constants";
                }
                 */
                else 
                {
                    resultLine = "Unknown Constant group name";
                }
            }

            return true;
        }

        public Boolean CommandVar(ref String commandLine, ref String resultLine)
        {
            String VariableName;
            resultLine = "";
            Boolean OK;
            do
            {
                OK = false;

                commandLine = commandLine.ReadIdentifier(out VariableName);
                if (VariableName == null)
                {
                    resultLine = "Variable name expected";
                }
                else
                {
                    Debug.Assert(VariableName != null);

                    IEnvironment VariableContext;
                    INametableItem Item;

                    Boolean IdentifierFound = CurrentContext.FindIdentifier(VariableName, out VariableContext, out Item);

                    IEnvironment NewVariableDeclarationNamespace = GetDeclarationEnvironment();

                    Boolean ALocalIdentifier = IdentifierFound && VariableContext == NewVariableDeclarationNamespace;
                    OK = !ALocalIdentifier || Item.Identifierkind == IdentifierKind.Variable;

                    if (OK)
                    {
                        TryParseToken("=", ref commandLine);

                        List<string> ExpectedFollow = new List<string>();
                        ExpectedFollow.Add(";");

                        Quantity pq = GetPhysicalQuantity(ref commandLine, ref resultLine, ExpectedFollow);

                        if (!ALocalIdentifier || pq != null)
                        {
                            // Declare new local var or set new value for local var
                            OK = VariableSet(NewVariableDeclarationNamespace, VariableName, pq);
                            if (pq != null)
                            {
                                resultLine = VariableName + " = " + pq.ToString();
                            }
                            else
                            {
                                resultLine = "Variable '" + VariableName + "' declared";
                            }
                        }
                        else
                        {
                            resultLine = "Local Variable '" + VariableName + "' is already declared";
                        }
                    }
                    else 
                    {
                        Debug.Assert(ALocalIdentifier && Item.Identifierkind != IdentifierKind.Variable);
                        resultLine = "Local identifier '" + VariableName + "' is already declared as a " + Item.Identifierkind.ToString();
                    }
                }
            } while (OK && TryParseToken(",", ref commandLine));
            return true;
        }

        public Boolean CommandSet(ref String commandLine, ref String resultLine)
        {
            IEnvironment IdentifierContext;
            String QualifiedIdentifierName;
            String VariableName;
            INametableItem Item;

            resultLine = "";
            Boolean OK;
            do
            {
                OK = false;
                Boolean IdentifierFound = ParseQualifiedIdentifier(ref commandLine, ref resultLine, out IdentifierContext, out QualifiedIdentifierName, out VariableName, out Item);

                if (VariableName == null)
                {
                    resultLine = "Variable name expected";
                }
                else
                {
                    IEnvironment NewVariableDeclarationNamespace = GetDeclarationEnvironment();

                    Boolean ALocalIdentifier = IdentifierFound && IdentifierContext == NewVariableDeclarationNamespace;
                    OK = !ALocalIdentifier || Item.Identifierkind == IdentifierKind.Variable;

                    if (OK)
                    {
                        TryParseToken("=", ref commandLine);

                        Boolean IsCalculatorSetting = CheckForCalculatorSetting(IdentifierContext, VariableName, ref commandLine, ref resultLine);
                        if (!IsCalculatorSetting)
                        {
                            List<string> ExpectedFollow = new List<string>();
                            ExpectedFollow.Add(";");

                            Quantity pq = GetPhysicalQuantity(ref commandLine, ref resultLine, ExpectedFollow);

                            if (pq != null)
                            {
                                OK = VariableSet(IdentifierContext, VariableName, pq);

                                resultLine = VariableName + " = " + pq.ToString();
                                Accumulator = pq;
                            }
                        }
                    }
                    else
                    {
                        Debug.Assert(ALocalIdentifier && Item.Identifierkind != IdentifierKind.Variable);
                        resultLine = "Local identifier '" + VariableName + "' is already declared as a " + Item.Identifierkind.ToString();
                    }
                }
            } while (OK && TryParseToken(",", ref commandLine));

            return true;
        }

        public Boolean CommandSystem(ref String commandLine, ref String resultLine)
        {
            String SystemName;
            resultLine = "";
            Boolean OK;
            do
            {
                OK = false;

                commandLine = commandLine.ReadIdentifier(out SystemName);
                if (SystemName == null)
                {
                    resultLine = "System name expected";
                }
                else
                {
                    Debug.Assert(SystemName != null);

                    IEnvironment SystemContext;
                    INametableItem Item;

                    Boolean IdentifierFound = CurrentContext.FindIdentifier(SystemName, out SystemContext, out Item);

                    IEnvironment NewSystemDeclarationNamespace = GetDeclarationEnvironment();

                    Boolean LocalIdentifier = IdentifierFound && SystemContext == NewSystemDeclarationNamespace;
                    OK = !LocalIdentifier || Item.Identifierkind == IdentifierKind.Unit;

                    if (OK)
                    {
                        OK = SystemSet(NewSystemDeclarationNamespace, SystemName, null, out Item);
                        if (OK)
                        {
                            // Defined new local base unit 
                            resultLine = "System '" + SystemName + "' declared.";
                        }
                        else
                        {
                            resultLine = "System '" + SystemName + "' can't be declared.\r\n" + resultLine;
                        }
                    }
                    else
                    {
                        resultLine = "Identifier '" + SystemName + "' is already declared as a " + Item.Identifierkind.ToString();
                    }
                }
            } while (OK && TryParseToken(",", ref commandLine));
            return true;
        }

        public Boolean CommandUnit(ref String commandLine, ref String resultLine)
        {
            IEnvironment IdentifierContext;
            String QualifiedIdentifierName;
            String UnitName;
            INametableItem Item;

            resultLine = "";
            Boolean OK;
            do
            {
                OK = false;

                Boolean IdentifierFound = ParseQualifiedIdentifier(ref commandLine, ref resultLine, out IdentifierContext, out QualifiedIdentifierName, out UnitName, out Item);

                if (UnitName == null)
                {
                    resultLine = "Unit name expected";
                }
                else
                {
                    Debug.Assert(UnitName != null);

                    IEnvironment NewUnitDeclarationNamespace = GetDeclarationEnvironment();

                    Boolean LocalIdentifier = IdentifierFound && IdentifierContext == NewUnitDeclarationNamespace;
                    OK = !LocalIdentifier || Item.Identifierkind == IdentifierKind.Unit;

                    if (OK)
                    {
                        TryParseToken("=", ref commandLine);

                        List<string> ExpectedFollow = new List<string>{",", ";"};

                        Quantity pq = GetPhysicalQuantity(ref commandLine, ref resultLine, ExpectedFollow);

                        if ((pq != null) && pq.IsDimensionless)
                        {
                            // Defined new local base unit 
                            resultLine = "Unit '" + UnitName + "' can't be declared.\r\n" + "Scaled unit must not be dimension less";
                        }
                        else
                        {

                            IUnitSystem UnitSys = null;
                            INametableItem SystemItem = null;
                            IEnvironment SystemContext;
                            Boolean SystemIdentifierFound = CurrentContext.FindIdentifier(QualifiedIdentifierName, out SystemContext, out SystemItem);
                            if (SystemItem != null && SystemItem.Identifierkind == IdentifierKind.UnitSystem)
                            {
                                UnitSys = ((NamedSystem)SystemItem).UnitSystem;
                            }
                            else
                            {
                                IUnitSystem Default_UnitSystem = Physics.CurrentUnitSystems.Default;
                                if (Default_UnitSystem.IsIsolatedUnitSystem && !Default_UnitSystem.IsCombinedUnitSystem)
                                {
                                    UnitSys = Default_UnitSystem;
                                }
                            }
                            OK = UnitSet(NewUnitDeclarationNamespace, UnitSys, UnitName, pq, out Item);
                            if (OK)
                            {
                                /*
                                string SystemName = "";
                                if (UnitSys != null)
                                {
                                    // SystemName = ((NamedSystem)UnitSys).UnitSystem.Name + ".";
                                    SystemName = UnitSys.Name + ".";
                                }
                                */
                                if (pq != null)
                                {
                                    // Defined new local unit as scaled unit
                                    resultLine = Item.ToListString(UnitName);
                                }
                                else
                                {
                                    // Defined new local base unit 
                                    resultLine = "Unit '" + UnitName + "' declared.";
                                }
                            }
                            else
                            {
                                resultLine = "Unit '" + UnitName + "' can't be declared.\r\n" + resultLine;
                            }
                        }
                    }
                    else
                    {
                        resultLine = "Identifier '" + UnitName + "' is already declared as a " + Item.Identifierkind.ToString();
                    }
                }
            } while (OK && TryParseToken(",", ref commandLine));
            return true;
        }

        public Boolean CommandStore(ref String commandLine, ref String resultLine)
        {   // Store accumulator value to var
            Boolean OK = false;
            String VariableName;
            resultLine = "";
            commandLine = commandLine.ReadIdentifier(out VariableName);

            if (VariableName == null)
            {
                resultLine = "Variable name expected";
            }
            else
            {
                Quantity pq = Accumulator;

                if (pq != null)
                {
                    OK = VariableSet(CurrentContext, VariableName, pq);

                    resultLine = pq.ToString();
                }
            }

            return OK;
        }

        public Boolean CommandRemove(ref String commandLine, ref String resultLine)
        {
            Boolean OK = false;
            String ItemName;
            resultLine = "";
            do
            {
                commandLine = commandLine.ReadIdentifier(out ItemName);
                if (ItemName == null)
                {
                    resultLine = "Variable name or unit name or function name expected";
                }
                else
                {
                    IEnvironment IdentifierContext;
                    INametableItem Item;

                    Boolean IdentifierFound = CurrentContext.FindIdentifier(ItemName, out IdentifierContext, out Item);

                    if (IdentifierFound)
                    {
                        OK = IdentifierContext.RemoveLocalIdentifier(ItemName);
                    }
                    else
                    {
                        resultLine = "'" + ItemName + "' not known";
                    }
                }
            } while (OK && TryParseToken(",", ref commandLine));
            return OK;
        }

        public Boolean CommandClear(ref String commandLine, ref String resultLine)
        {
            Boolean clearNamedItems = TryParseTokenPrefix("Items", ref commandLine);
            Boolean clearCommands = TryParseTokenPrefix("Commands", ref commandLine);

            if (!clearNamedItems && !clearCommands)
            {
                // Default to both
                clearNamedItems = clearCommands = true;
            }

            Boolean result = true;
            resultLine = "";

            if (clearNamedItems)
            {
                Accumulator = null;
                result = IdentifiersClear();
            }
            if (clearCommands)
            {
                CommandLineReader.CommandHistory.Clear();
            }

            return result;
        }

        public Boolean CommandList(ref String commandLine, ref String resultLine)
        {
            Boolean listNamedItems = TryParseTokenPrefix("Items", ref commandLine);
            Boolean listSettings = TryParseTokenPrefix("Settings", ref commandLine);
            Boolean listCommands = TryParseTokenPrefix("Commands", ref commandLine);
            
            if (!listNamedItems && !listSettings && !listCommands)
            {
                listNamedItems = true;
            }

            StringBuilder ListStringBuilder = new StringBuilder();

            if (listNamedItems || listSettings)
            {
                ListStringBuilder.AppendLine("Default unit system: " + Physics.CurrentUnitSystems.Default.Name);
                ListStringBuilder.Append(CurrentContext.ListIdentifiers(false, listNamedItems, listSettings));
            }
            else
            {
                foreach (String CommandLine in CommandLineReader.CommandHistory)
                {
                    ListStringBuilder.AppendLine(CommandLine);
                }
            }
            resultLine = ListStringBuilder.ToString();

            return true;
        }

        public Boolean CommandPrint(ref String commandLine, ref String resultLine)
        {
            resultLine = "";

            List<string> ExpectedFollow = new List<string>();
            ExpectedFollow.Add(";");
            ExpectedFollow.Add(",");

            do
            {
                Quantity pq = GetPhysicalQuantity(ref commandLine, ref resultLine, ExpectedFollow);

                if (pq != null)
                {
                    if (!String.IsNullOrWhiteSpace(resultLine))
                    {
                        if (resultLine[resultLine.Length-1] != '\n')
                        {
                            resultLine += ", ";
                        }
                    }
                    resultLine += pq.ToString(null, CurrentContext.CurrentCultureInfo);

                    Accumulator = pq;
                }
            } while (!String.IsNullOrWhiteSpace(commandLine) && TryParseToken(",", ref commandLine));

            return true;
        }

        public Boolean IdentifierAssumed(ref String commandLine, ref String resultLine)
        {
            string identifier;
            int len = commandLine.PeekIdentifier(out identifier);

            if (len > 0)
            {
                Char token2;
                int len2 = commandLine.Substring(len).TrimStart().PeekChar(out token2);

                if (token2 == '=')
                {
                    // Assume a set Command
                    return CommandSet(ref commandLine, ref resultLine);
                }
                else
                {
                    // Assume a print Command
                    return CommandPrint(ref commandLine, ref resultLine);
                }
            }

            return false;
        }

        public Boolean CommandFunc(ref String commandLine, ref String resultLine)
        {
            String FunctionName;
            resultLine = "";
            commandLine = commandLine.ReadIdentifier(out FunctionName);
            if (FunctionName == null)
            {
                resultLine = "Function name expected";
            }
            else
            {
                IEnvironment IdentifierContext;
                INametableItem Item;

                Boolean IdentifierFound = CurrentContext.FindIdentifier(FunctionName, out IdentifierContext, out Item);

                IEnvironment NewFunctionDeclarationNamespace = GetDeclarationEnvironment();

                Boolean IsALocalIdentifier = IdentifierFound && IdentifierContext == NewFunctionDeclarationNamespace;
                Boolean OK = !IsALocalIdentifier || Item.Identifierkind == IdentifierKind.Function;
                if (OK)
                {
                    CurrentContext.BeginParsingFunction(FunctionName);

                    if (IsALocalIdentifier)
                    {
                        resultLine = "Function '" + FunctionName + "' is already declared";
                        CurrentContext.FunctionToParseInfo.RedefineItem = Item;
                    }

                    ParseFunctionDeclaration(ref commandLine, ref resultLine);
                }
                else
                {
                    resultLine = "'" + FunctionName + "' is already defined as a " + Item.Identifierkind.ToString();
                }
            }
            return true;
        }

        public Boolean CommandIf(ref String commandLine, ref String resultLine)
        {
            resultLine = "";
            List<string> ExpectedFollow = new List<string>();
            // ExpectedFollow.Add("//");
            ExpectedFollow.Add("{");

            Boolean testResult = GetBoolean(ref commandLine, ref resultLine, ExpectedFollow);
            CurrentContext.BeginParsingCommandBlock();
            ICommandsEvaluator thenBlock = ParseCommandBlockDeclaration(ref commandLine, ref resultLine);
            if (thenBlock != null && testResult)
            {
                //thenBlock.Evaluate(CurrentContext, out Accumulator, ref resultLine);
                CommandsBlockEvaluate("", thenBlock, out Accumulator, ref resultLine);
            }
            if (TryParseToken("else", ref commandLine))
            {
                CurrentContext.BeginParsingCommandBlock();
                ICommandsEvaluator elseBlock = ParseCommandBlockDeclaration(ref commandLine, ref resultLine);
                if (elseBlock != null && !testResult)
                {
                    //elseBlock.Evaluate(CurrentContext, out Accumulator, ref resultLine);
                    CommandsBlockEvaluate("", elseBlock, out Accumulator, ref resultLine);
                }
            }

            return true;
        }


        #endregion Command methods

        #region Command helpers

        public Quantity GetPhysicalQuantity(ref String commandLine, ref String resultLine, List<String> ExpectedFollow)
        {
            Quantity pq = PhysicalCalculator.Expression.PhysicalExpression.ParseConvertedExpression(ref commandLine, ref resultLine, ExpectedFollow);

            if (pq == null)
            {
                if (!String.IsNullOrEmpty(resultLine) && !resultLine.EndsWith(".") && !resultLine.EndsWith("\n")) 
                {
                    resultLine += ". ";
                }
                resultLine += "Physical quantity expected";
            }

            return pq;
        }

        public Boolean GetBoolean(ref String commandLine, ref String resultLine, List<String> ExpectedFollow)
        {
            Nullable<Boolean> boolRes = PhysicalCalculator.Expression.PhysicalExpression.ParseBooleanExpression(ref commandLine, ref resultLine, ExpectedFollow);

            if (boolRes == null)
            {
                if (!String.IsNullOrEmpty(resultLine) && !resultLine.EndsWith(".") && !resultLine.EndsWith("\n")) 
                {
                    resultLine += ". ";
                }
                resultLine += "Boolean expression expected";
                boolRes = false;
            }

            return boolRes.Value;
        }


        public Boolean ParseQualifiedIdentifier(ref String commandLine, ref String resultLine, 
                        out IEnvironment qualifiedIdentifierContext, out String qualifiedIdentifierName, out String identifierName, out INametableItem item)
        {
            IEnvironment PrimaryContext;
            IdentifierKind identifierkind = IdentifierKind.Unknown;

            commandLine = commandLine.ReadIdentifier(out identifierName);
            Debug.Assert(identifierName != null);

            Boolean IdentifierFound = IdentifierItemLookup(identifierName, out PrimaryContext, out item);
            if (IdentifierFound)
            {
                identifierkind = item.Identifierkind;
            }


            qualifiedIdentifierContext = PrimaryContext;
            qualifiedIdentifierName = identifierName;

            while (IdentifierFound
                && (identifierkind == IdentifierKind.Environment || identifierkind == IdentifierKind.UnitSystem) 
                && !String.IsNullOrEmpty(commandLine) 
                && commandLine[0] == '.')
            {
                TokenString.ParseChar('.', ref commandLine, ref resultLine);
                commandLine = commandLine.TrimStart();

                commandLine = commandLine.ReadIdentifier(out identifierName);
                Debug.Assert(identifierName != null);
                commandLine = commandLine.TrimStart();

                if (identifierkind == IdentifierKind.Environment)
                {
                    IEnvironment FoundInContext;

                    IdentifierFound = qualifiedIdentifierContext.FindIdentifier(identifierName, out FoundInContext, out item);
                    if (IdentifierFound)
                    {
                        qualifiedIdentifierContext = FoundInContext;
                    }
                }
                else
                {
                    NamedSystem UserNamedSystem = (NamedSystem)item;
                    IUnitSystem UserSystem = UserNamedSystem.UnitSystem;
                    INamedSymbolUnit UserSymbol = null;
                    if (UserSystem != null)
                    {
                        UserSymbol = UserSystem.UnitFromSymbol(identifierName);
                    }
                    IdentifierFound = UserSymbol != null;
                }
                if (IdentifierFound)
                {
                    identifierkind = item.Identifierkind;

                    qualifiedIdentifierName += "." + identifierName;
                }
            }

            return IdentifierFound;
        }

        public Boolean ParseFunctionDeclaration(ref String commandLine, ref String resultLine)
        {
            IFunctionEvaluator FuncEval = null;
            FuncEval = PhysicalCalculator.Function.PhysicalFunction.ParseFunctionDeclaration(CurrentContext, ref commandLine, ref resultLine);
            if (FuncEval != null)
            {
                if (CurrentContext.FunctionToParseInfo.RedefineItem != null) 
                {
                    CurrentContext.NamedItems.SetItem(CurrentContext.FunctionToParseInfo.FunctionName, FuncEval);
                    resultLine = "Function '" + CurrentContext.FunctionToParseInfo.FunctionName + "' re-defined";
                }
                else 
                {
                    CurrentContext.NamedItems.AddItem(CurrentContext.FunctionToParseInfo.FunctionName, FuncEval);
                    resultLine = "Function '" + CurrentContext.FunctionToParseInfo.FunctionName + "' declared";
                }

                /*
                CurrentContext.FunctionToParse = null;
                CurrentContext.FunctionToParseName = null;
                */
                CurrentContext.FunctionToParseInfo = null;

                CurrentContext.ParseState = CommandParserState.ExecuteCommandLine;
            }

            return FuncEval != null;
        }

        public ICommandsEvaluator ParseCommandBlockDeclaration(ref String commandLine, ref String resultLine)
        {
            ICommandsEvaluator CommandsEval = null;
            CommandsEval = PhysicalCalculator.CommandBlock.PhysicalCommandBlock.ParseCommandBlockDeclaration(CurrentContext, ref commandLine, ref resultLine);
            if (CommandsEval != null)
            {
                CurrentContext.CommandBlockToParseInfo = null;
                CurrentContext.ParseState = CommandParserState.ExecuteCommandLine;
            }

            return CommandsEval;
        }


        public Boolean CheckForCalculatorSetting(IEnvironment identifierContext, String variableName, ref String commandLine, ref String resultLine)
        {
            Boolean SettingFound = false;
            if (variableName.IsKeyword("Tracelevel"))
            {
                SettingFound = true;
                String tracelevelvaluestr;
                TraceLevels tl; // = TraceLevels.All ;
                commandLine = commandLine.ReadToken(out tracelevelvaluestr);
                if (tracelevelvaluestr.IsKeyword("Normal"))
                {
                    tl = TraceLevels.Normal;
                }
                else if (tracelevelvaluestr.IsKeyword("On"))
                {
                    tl = TraceLevels.High;
                }
                else if (tracelevelvaluestr.IsKeyword("Off"))
                {
                    tl = TraceLevels.Low;
                }
                //else if (tracelevelvaluestr.IsKeyword("Debug"))
                else 
                {
                    tracelevelvaluestr = "Debug";
                    tl = TraceLevels.All;
                }

                if (identifierContext != null)
                {
                    identifierContext.OutputTracelevel = tl;
                    resultLine = "Tracelevel set to " + tracelevelvaluestr;
                }
                else
                {
                    CommandLineReader.OutputTracelevel = tl;
                    CurrentContext.OutputTracelevel = tl;
                    resultLine = "Current tracelevel set to " + tracelevelvaluestr;
                }
            }
            else if (variableName.IsKeyword("FormatProvider"))
            {
                SettingFound = true;
                String formatProviderValuestr;
                FormatProviderKind fp; 
                commandLine = commandLine.ReadToken(out formatProviderValuestr);
                if (formatProviderValuestr.IsKeyword("Inherited"))
                {
                    fp = FormatProviderKind.InheritedFormatProvider;
                }
                else if (formatProviderValuestr.IsKeyword("Default"))
                {
                    fp = FormatProviderKind.DefaultFormatProvider;
                }
                else // if (formatProviderValuestr.IsKeyword("Invariant"))
                {
                    fp = FormatProviderKind.InvariantFormatProvider;
                    formatProviderValuestr = "Invariant";
                }

                if (identifierContext != null)
                {
                    identifierContext.FormatProviderSource = fp;
                    resultLine = "FormatProvider set to " + formatProviderValuestr;
                }
                else
                {
                    CommandLineReader.FormatProviderSource = fp;
                    CurrentContext.FormatProviderSource = fp;
                    resultLine = "Current FormatProvider set to " + formatProviderValuestr;
                }
            }
            else if (variableName.IsKeyword("System") || variableName.IsKeyword("Default_System"))
            {
                SettingFound = true;
                String systemvaluestr;
                commandLine = commandLine.ReadToken(out systemvaluestr);

                IUnitSystem us = Physics.UnitSystems.UnitSystemFromName(systemvaluestr);
                if (us == null)
                {   // Not a unit system from PhysicalMeasure; Try look for a user defined unit system.
                    // TODO: Look for a user defined unit system with specified name.
                }
                if (us != null)
                {
                    if (Physics.CurrentUnitSystems.Use(us))
                    {
                        resultLine = "System set to " + us.Name;
                    }
                    else
                    {
                        resultLine = "System already set to " + us.Name;
                    }
                }
                else
                {
                    resultLine = "System " + systemvaluestr + " was not found; Current system is " + Physics.CurrentUnitSystems.Default;
                }
            }
            return SettingFound;
        }

        #endregion Command helpers

        #region Variables access

        public Boolean VariableSet(IEnvironment context, String variableName, Quantity variableValue)
        {
            if (variableName == AccumulatorName)
            {
                Accumulator = variableValue;
                //return true;
                return false;
            }
            else 
            {
                if (context == null)
                {
                    context = CurrentContext;
                }
                return context.VariableSet(variableName, variableValue);
            }
        }

        public Boolean VariableSetLocal(String variableName, Quantity variableValue) => VariableSet(CurrentContext, variableName, variableValue);

        public Boolean VariableSetGlobal(String variableName, Quantity variableValue) => VariableSet(GlobalContext, variableName, variableValue);

        public Boolean VariableSet(String variableName, Quantity variableValue)
        {
            if (variableName == AccumulatorName)
            {
                Accumulator = variableValue;
                return true;
            }
            else 
            {
                return CurrentContext.VariableSet(variableName, variableValue);
            }
        }

        public Boolean VariableGet(IEnvironment context, String variableName, out Quantity variableValue, ref String resultLine)
        {
            if (variableName == AccumulatorName)
            {
                variableValue = Accumulator;
                return true;
            }
            else
            {
                if (context == null)
                {
                    context = CurrentContext;
                }
                Debug.Assert(context != null);
                return context.VariableGet(variableName, out variableValue, ref resultLine);
            }
        }

        public Boolean UnitGet(IEnvironment context, String variableName, out Unit unitValue, ref String resultLine)
        {
            if (context == null)
            {
                context = CurrentContext;
            }
            Debug.Assert(context != null);
            return context.UnitGet(variableName, out unitValue, ref resultLine);
        }

        #endregion  Variables access

        #region  Custom Unit access

        //return context.SystemSet(systemName, unitValue, out systemItem);
        public Boolean SystemSet(IEnvironment context, String systemName, IQuantity unitValue, out INametableItem systemItem) => context.SystemSet(systemName, out systemItem);

        public Boolean UnitSet(IEnvironment context, IUnitSystem unitSystem, String unitName, Quantity unitValue, out INametableItem unitItem) => context.UnitSet(unitSystem, unitName, unitValue, out unitItem);

        #endregion  Custom Unit  access

        #region  Function access

        public Boolean FunctionLookup(IEnvironment context, String functionName, out IFunctionEvaluator functionevaluator) => context.FunctionFind(functionName, out functionevaluator);

        public Boolean CommandsBlockEvaluate(String CommandBlockName, ICommandsEvaluator commandsEvaluator, out Quantity commandsResult, ref String resultLine)
        {
            Boolean OK = false;
            commandsResult = null;

            if (commandsEvaluator != null)
            {
                CalculatorEnvironment LocalContext = new CalculatorEnvironment(CurrentContext, "Commands " + CommandBlockName, EnvironmentKind.FunctionEnv);
                //LocalContext.FormatProviderSource = FormatProviderKind.DefaultFormatProvider;
                LocalContext.FormatProviderSource = FormatProviderKind.InheritedFormatProvider;
                CalculatorEnvironment OldCurrentContext = CurrentContext;

                CurrentContext = LocalContext;

                OK = commandsEvaluator.Evaluate(LocalContext, out commandsResult, ref resultLine);
                CurrentContext = OldCurrentContext;
                LocalContext.OuterContext = null;
            }

            return OK;
        }


        public Boolean FunctionEvaluate(String FunctionName, IFunctionEvaluator functionEvaluator, List<Quantity> parameterlist, out Quantity functionResult, ref String resultLine)
        {
            Boolean OK = false;
            functionResult = null;

            if (functionEvaluator != null)
            {
                CalculatorEnvironment FunctionStaticOuterContext = functionEvaluator.StaticOuterContext;
                Debug.Assert(FunctionStaticOuterContext != null);
                CalculatorEnvironment LocalContext = new CalculatorEnvironment(FunctionStaticOuterContext, "Function " + FunctionName, EnvironmentKind.FunctionEnv);
                //LocalContext.FormatProviderSource = FormatProviderKind.DefaultFormatProvider;
                LocalContext.FormatProviderSource = FormatProviderKind.InheritedFormatProvider;
                CalculatorEnvironment OldCurrentContext = CurrentContext;

                CurrentContext = LocalContext;

                OK = functionEvaluator.Evaluate(LocalContext, parameterlist, out functionResult, ref resultLine);
                CurrentContext = OldCurrentContext;
                LocalContext.OuterContext = null;
            }

            return OK;
        }

        public Boolean FunctionEvaluateFileRead(String functionName, out Quantity functionResult, ref String resultLine)
        {
            Boolean OK = false;
            functionResult = null;

            if (CommandLineReader != null)
            {
                CalculatorEnvironment LocalContext = new CalculatorEnvironment(GlobalContext, "File Function " + functionName, EnvironmentKind.FunctionEnv);
                //LocalContext.FormatProviderSource = FormatProviderKind.DefaultFormatProvider;
                LocalContext.FormatProviderSource = FormatProviderKind.InheritedFormatProvider;
                CalculatorEnvironment OldCurrentContext = CurrentContext;

                CurrentContext = LocalContext;

                CommandReader functionCommandLineReader = new CommandReader(functionName + ".cal", CommandLineReader.ResultLineWriter);

                functionCommandLineReader.ReadFromConsoleWhenEmpty = false; // Return from ExecuteCommands() function when file commands are done

                if (LocalContext.OuterContext.OutputTracelevel.HasFlag(TraceLevels.FunctionEnterLeave))
                {
                    functionCommandLineReader.ResultLineWriter.WriteLine("Enter " + LocalContext.Name);
                }
                ExecuteCommands(LocalContext, functionCommandLineReader, ResultLineWriter);
                functionResult = Accumulator;
                if (LocalContext.OuterContext.OutputTracelevel.HasFlag(TraceLevels.FunctionEnterLeave))
                {
                    functionCommandLineReader.ResultLineWriter.WriteLine("Leave " + LocalContext.Name);
                }
                OK = true;

                CurrentContext = OldCurrentContext;
                LocalContext.OuterContext = null;
            }

           return OK;
        }

        #endregion  Function access

        #region  Identifier access

        public Boolean IdentifierItemLookup(String identifierName, out IEnvironment context, out INametableItem item)
        {
            Boolean IdentifierFound = CurrentContext.FindIdentifier(identifierName, out context, out item);
            if (!IdentifierFound)
            {   // Look for Global system settings and predefined symbols
                CalculatorEnvironment PrimaryContext;
                IdentifierFound = PredefinedContextIdentifierLookup(identifierName, out PrimaryContext);
                context = PrimaryContext;
                item = PrimaryContext;
            }

            return IdentifierFound;
        }

        public Boolean PredefinedContextIdentifierLookup(String IdentifierName, out CalculatorEnvironment PrimaryContext)
        {
            // Look for Global system settings and predefined symbols
            Boolean IdentifierFound = IdentifierName.Equals("Global", StringComparison.OrdinalIgnoreCase);
            if (IdentifierFound)
            {
                PrimaryContext = GlobalContext;
            }
            else
            {
                IdentifierFound = IdentifierName.Equals("Outer", StringComparison.OrdinalIgnoreCase);
                if (IdentifierFound)
                {
                    PrimaryContext = CurrentContext.OuterContext;
                }
                else
                {
                    IdentifierFound = IdentifierName.Equals("Local", StringComparison.OrdinalIgnoreCase);
                    if (IdentifierFound)
                    {
                        PrimaryContext = CurrentContext;
                    }
                    else
                    {
                        PrimaryContext = null;
                    }
                }
            }
            return IdentifierFound;
        }


        public Boolean QualifiedIdentifierItemLookup(IEnvironment LookInContext, String IdentifierName, out INametableItem Item)
        {
            return LookInContext.FindLocalIdentifier(IdentifierName, out Item);
        }

        public Boolean IdentifierContextLookup(String IdentifierName, out IEnvironment FoundInContext, out IdentifierKind identifierkind)
        {
            INametableItem item;
            Boolean identifierFound = IdentifierItemLookup(IdentifierName, out FoundInContext, out item);
            if (identifierFound)
            {
                identifierkind = item.Identifierkind;
            }
            else
            {
                identifierkind = IdentifierKind.Unknown;
            }

            return identifierFound;
        }

        public Boolean QualifiedIdentifierCalculatorContextLookup(IEnvironment LookInContext, String IdentifierName, out IEnvironment FoundInContext, out IdentifierKind identifierkind)
        {
            INametableItem Item;

            Boolean Found = LookInContext.FindLocalIdentifier(IdentifierName, out Item);

            if (Found)
            {
                identifierkind = Item.Identifierkind;
                FoundInContext = LookInContext;
            }
            else
            {
                identifierkind = IdentifierKind.Unknown;
                FoundInContext = null;
            }
            return Found;
        }

        public Boolean IdentifiersClear() => CurrentContext.ClearLocalIdentifiers();

        #endregion  Identifier access
    }




}


