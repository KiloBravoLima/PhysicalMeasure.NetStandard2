using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using PhysicalMeasure;

using TokenParser;

using PhysicalCalculator.Identifiers;

namespace PhysicalCalculator.CommandBlock
{

    abstract class PhysicalQuantityCommandBlock : IEvaluator
    {
        virtual public String ToListString(String name)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            ListStringBuilder.AppendFormat("Commands {0}", name);
            return ListStringBuilder.ToString();
        }

        abstract public Boolean Evaluate(CalculatorEnvironment localContext, out Quantity functionResult, ref String resultLine);

        public void WriteToTextFile(String name, System.IO.StreamWriter file)
        {
            file.WriteLine(ToListString(name));
        }
    }

    class PhysicalQuantityCommandsBlock : PhysicalQuantityCommandBlock, ICommandsEvaluator
    {
        private List<String> _commands;
        public List<String> Commands { get { return _commands; } set { _commands = value; } }

        override public String ToListString(String name)
        {
            StringBuilder ListStringBuilder = new StringBuilder();

            //ListStringBuilder.AppendLine("//");
            if (Commands.Count <= 1)
            {
                // Single line func
                ListStringBuilder.AppendFormat("Commands {0} {{ {1} }}", name, Commands.Count > 0 ? Commands[0] : "");
            }
            else
            {
                // Multi line func
                ListStringBuilder.AppendFormat("Commands {0}", name);
                ListStringBuilder.AppendLine();
                ListStringBuilder.AppendLine("{");
                foreach (String CommandLine in Commands)
                {
                    ListStringBuilder.AppendFormat("\t{0}", CommandLine);
                    ListStringBuilder.AppendLine();
                }
                ListStringBuilder.Append("}");
                //ListStringBuilder.AppendLine();
            }
            return ListStringBuilder.ToString();
        }

        override public Boolean Evaluate(CalculatorEnvironment localContext, out Quantity commandBlockResult, ref String resultLine)
        {
            if (PhysicalCommandBlock.ExecuteCommandsCallback != null)
            {
                // Run commands
                String TempCommandBlockResult = ""; // Dummy: Never used
                Boolean result = PhysicalCommandBlock.ExecuteCommandsCallback(localContext, Commands, ref TempCommandBlockResult, out commandBlockResult);

                return result;
            }
            else
            {
                if (Commands != null)
                {
                    resultLine = "Function call: PhysicalCommandBlock.ExecuteCommandsCallback is null. Don't know how to evaluate command block.";
                }
                commandBlockResult = null;
                return false;
            }
        }
    }

    static class PhysicalCommandBlock
    {
        #region Physical Function parser methods
        /**
            COMMANDBLOCK = "{" COMMANDS "}" .         
            COMMANDS = COMMAND | COMMAND "\n" COMMANDS.
          
            COMMANDBLOCK = "{" COMMANDS "}" .         
            COMMANDS = COMMAND COMMANDSopt . 
            COMMANDSopt = "\n" COMMANDS | e .
          
         **/

        public delegate Boolean ExecuteCommandsFunc(CalculatorEnvironment localContext, List<String> FuncBodyCommands, ref String funcBodyResult, out Quantity functionResult);

        public static ExecuteCommandsFunc ExecuteCommandsCallback;

        public static ICommandsEvaluator ParseCommandBlockDeclaration(CalculatorEnvironment localContext, ref String commandLine, ref String resultLine)
        {
            // COMMANDBLOCK = "{" COMMANDS "}" .
            // COMMANDBLOCK = #4 "{" COMMANDS "}" .

            Boolean OK = true;
                
            if (!String.IsNullOrEmpty(commandLine))
            {
                if (localContext.ParseState == CommandParserState.ReadCommandBlock)
                {
                    if (commandLine.StartsWith("//"))
                    {   // #4
                        commandLine = null;
                        return null;
                    }

                    OK = TokenString.ParseToken("{", ref commandLine, ref resultLine);
                    if (OK)
                    {
                        localContext.ParseState = CommandParserState.ReadCommands;
                        localContext.CommandBlockToParseInfo.InnerBlockCount = 0;
                    }
                }
                if (localContext.ParseState == CommandParserState.ReadCommands)
                {
                    if (!String.IsNullOrEmpty(commandLine))
                    {
                        String tempCommandLine = commandLine;
                        int indexStartComment = commandLine.IndexOf("//");
                        if (indexStartComment >= 0)
                        {   // Command line are terminated by "//"
                            tempCommandLine = commandLine.Substring(0, indexStartComment);
                        }

                        int indexCommandEnd = commandLine.Length;
                        {
                            int indexStartInnerBlock = tempCommandLine.IndexOf("{");
                            int tempIndexCommandEnd = tempCommandLine.IndexOf('}');
                            while (tempIndexCommandEnd > 0)
                            {
                                // some command block is terminated by '}' (before comment)
                                // Look for inner command blocks
                                //indexStartInnerBlock = tempCommandLine.IndexOf("{", indexStartInnerBlock+1);
                                while ((indexStartInnerBlock >= 0) && (tempIndexCommandEnd > indexStartInnerBlock))
                                {   // Commands has inner block start
                                    localContext.CommandBlockToParseInfo.InnerBlockCount++;
                                    indexStartInnerBlock = tempCommandLine.IndexOf('{', indexStartInnerBlock + 1);
                                }
                                // InnerBlockCount now holds the no of inner block starts
                                if (localContext.CommandBlockToParseInfo.InnerBlockCount > 0)
                                {   // The found '}' terminates the most inner of the inner blocks
                                    localContext.CommandBlockToParseInfo.InnerBlockCount--;
                                    // Look for next '}'
                                    tempIndexCommandEnd = tempCommandLine.IndexOf('}', tempIndexCommandEnd + 1);
                                }
                                else
                                {   // The found '}' terminates this (most outer) block
                                    indexCommandEnd = tempIndexCommandEnd;
                                    tempIndexCommandEnd = -2;
                                }
                            }

                            if (localContext.CommandBlockToParseInfo.InnerBlockCount > 0)
                            {    // this (most outer) block are not terminated in this line
                                // Line are not terminated by '}'
                                // Whole commandLine is part of Command Block 
                                // indexCommandEnd = commandLine.Length;
                                Debug.Assert(tempIndexCommandEnd == -1);
                                Debug.Assert(indexCommandEnd == commandLine.Length);
                            }
                            else
                            {
                                Debug.Assert(tempIndexCommandEnd == -2);
                            }
                        }

                        if (indexCommandEnd > 0)
                        {
                            if (localContext.CommandBlockToParseInfo.CommandBlock.Commands == null)
                            {
                                localContext.CommandBlockToParseInfo.CommandBlock.Commands = new List<String>();
                            }
                            localContext.CommandBlockToParseInfo.CommandBlock.Commands.Add(commandLine.Substring(0, indexCommandEnd));
                            commandLine = commandLine.Substring(indexCommandEnd);
                        }

                        if (!String.IsNullOrEmpty(commandLine))
                        {
                            OK = TokenString.ParseToken("}", ref commandLine, ref resultLine);
                            if (OK)
                            {
                                if (localContext.CommandBlockToParseInfo.InnerBlockCount > 0)
                                {
                                    localContext.CommandBlockToParseInfo.InnerBlockCount--;
                                }
                                else
                                {
                                    // Completed function declaration parsing 
                                    localContext.ParseState = CommandParserState.ExecuteCommandLine;
                                    return localContext.CommandBlockToParseInfo.CommandBlock;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
          
        #endregion Physical Expression parser methods
    }
}

