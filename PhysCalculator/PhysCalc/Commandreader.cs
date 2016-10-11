using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace PhysicalCalculator
{
    [Flags]
    public enum TraceLevels : Byte
    {
        None = 0x00,
        Results = 0x01,
        FileEnterLeave = 0x02,
        FunctionEnterLeave = 0x04,
        BlockEnterLeave = 0x08,
        Debug = 0x10,
        Low = Results,
        Normal = Results | FileEnterLeave,
        High = Results | FileEnterLeave | FunctionEnterLeave,
        All = 0xFF
    }

    public enum FormatProviderKind
    {
        InvariantFormatProvider = 0,
        DefaultFormatProvider = 1,
        InheritedFormatProvider = 2
    }

    interface ICommandAccessor
    {
        String Name { get; }
        TraceLevels OutputTracelevel { get; set; }
        FormatProviderKind FormatProviderSource { get; set; }
        Boolean IsEmpty { get; }
        String GetCommandLine(ref String ResultLine);
    }

    class CommandAccessorStack : List<ICommandAccessor>
    {
        public TraceLevels OutputTracelevel
        {
            get
            {
                ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
                if (CA != null)
                {
                    return CA.OutputTracelevel;
                }
                // Error; just trace All
                return TraceLevels.All;
            }

            set
            {
                ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
                if (CA != null)
                {
                    CA.OutputTracelevel = value;
                }
            }
        }

        public FormatProviderKind FormatProviderSource
        {
            get
            {
                ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
                if (CA != null)
                {
                    return CA.FormatProviderSource;
                }
                // Error; just use Invariant
                return FormatProviderKind.InvariantFormatProvider;
            }

            set
            {
                ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
                if (CA != null)
                {
                    CA.FormatProviderSource = value;
                }
            }
        }

        public String GetName()
        {
            ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
            String Name = null;
            if (CA != null)
            {
                Name = CA.Name;
            }
            return Name;
        }

        public String GetCommandLine(ref String ResultLine)
        {
            ICommandAccessor CA = Count > 0 ? this[Count - 1] : null;
            String CommandLine = null;
            if (CA != null)
            {
                CommandLine = CA.GetCommandLine(ref ResultLine);
                if (CA.IsEmpty)
                {
                    this.RemoveAt(this.Count - 1);
                }
            }

            return CommandLine;
        }
    }

    class CommandList : List<String>
    {
        public CommandList(String[] ListOfCommands)
        {
            foreach (String Command in ListOfCommands)
            {
                this.Add(Command);
            }
        }

        public CommandList(String Commands)
            : this(Commands.Split(new char[] { '\n', '\r' }))
        {
        }

        public String GetFirst()
        {
            String CommandLine = null;
            if (Count > 0)
            {
                CommandLine = this[0];
                this.RemoveAt(0);
            }
            return CommandLine;
        }
    }

    abstract class CommandAccessor : ICommandAccessor
    {
        private String _name;
        private TraceLevels OutTracelevel = TraceLevels.Normal; // TraceLevels.All; //TraceLevels.Normal;
        private FormatProviderKind FormatProviderSrc = FormatProviderKind.InheritedFormatProvider; 
        public int LinesRead = 0;

        public String Name { get { return (_name != null ? _name : ""); } set { _name = value;  } }
        public TraceLevels OutputTracelevel { get { return OutTracelevel; } set { OutTracelevel = value; } }
        public FormatProviderKind FormatProviderSource { get { return FormatProviderSrc; } set { FormatProviderSrc = value; } }

        //virtual public Boolean IsEmpty { get { return true; } } 
        abstract public Boolean IsEmpty { get; } 
        abstract public String GetCommandLine(ref String ResultLine);
    }

    class CommandBlockAccessor : CommandAccessor
    {
        public CommandList CommandBlock;

        public CommandBlockAccessor(CommandList CommandBlock)
            : this(null, CommandBlock)
        {
        }

        public CommandBlockAccessor(String CommandBlockName, CommandList CommandBlock)
        {
            this.Name = CommandBlockName;
            this.CommandBlock = CommandBlock;
        }

        public String CommandBlockName { get { return Name; } set { Name = value; } }

        override public Boolean IsEmpty => CommandBlock == null;

        override public String GetCommandLine(ref String ResultLine)
        {
            String S = null;
            if (CommandBlock == null)
            {
                S = "";
                if (!String.IsNullOrWhiteSpace(Name))
                {
                    ResultLine = "CommandBlock '" + Name + "' is empty";
                }
                else 
                {
                    ResultLine = "CommandBlock is empty";
                }
                CommandBlockName = null;
            }
            else if (LinesRead == 0)
            {
                if (OutputTracelevel.HasFlag(TraceLevels.FunctionEnterLeave))
                {
                    ResultLine = "Reading from '" + Name + "'";
                }
            }

            if (CommandBlock != null)
            {
                S = CommandBlock.GetFirst();
                if (S == null)
                {
                    S = "";
                    if (OutputTracelevel.HasFlag(TraceLevels.FunctionEnterLeave))
                    {
                        if (!String.IsNullOrEmpty(ResultLine))
                        {
                            ResultLine += "\n";
                        }
                        //resultLine += "End of CommandBlock '" + name + "'";
                        ResultLine += "End of '" + Name + "'";
                    }

                    CommandBlock = null;
                    CommandBlockName = null;
                }
                else
                {
                    LinesRead++;
                }
            }

            return S;
        }
    }

    class CommandFileAccessor : CommandAccessor
    {
        //public String FileNameStr;
        public StreamReader FileReader;

        public CommandFileAccessor()
            : this(null)
        {
        } 

        public CommandFileAccessor(String FileNameStr)
        {
            this.FileNameStr = FileNameStr;
            this.FileReader = null;
        }

        // IDisposable
        // public virtual void Dispose()
        public void Dispose()
        {
            FileReader.Dispose();
        }

        public String FileNameStr { 
            get { return Name; } 
            set { Name = value; } 
        }

        // Will not work because FileReader is null also before the file is read: return FileReader == null;
        override public Boolean IsEmpty => String.IsNullOrWhiteSpace(FileNameStr);

        override public String GetCommandLine(ref String ResultLine)
        {
            const String DefaultCalculatorScriptsSubdir = "Calculator Scripts";
            String S = null;
            if (FileReader == null && !String.IsNullOrWhiteSpace(FileNameStr))
            {
                if (!File.Exists(FileNameStr)
                    && !Path.HasExtension(FileNameStr))
                {
                    FileNameStr += ".cal";
                }
                if (!File.Exists(FileNameStr)
                    && String.Compare(Path.GetPathRoot(FileNameStr), DefaultCalculatorScriptsSubdir, true) != 0
                    && Directory.Exists(DefaultCalculatorScriptsSubdir))
                {
                    FileNameStr = DefaultCalculatorScriptsSubdir + FileNameStr;
                }
                try
                {
                    //FileReader = File.OpenText(FileNameStr);
                    //Encoding FileEncoding = Encoding.UTF8;
                    //Encoding FileEncoding = Encoding.ASCII;
                    //Encoding FileEncoding = Encoding.UTF7;
                    FileReader = new StreamReader(FileNameStr, Encoding.UTF7);
                    if (OutputTracelevel.HasFlag(TraceLevels.FileEnterLeave))
                    {
                        ResultLine = "Reading from file " + FileNameStr;
                    }
                }
                catch (DirectoryNotFoundException e)
                {
                    S = "";
                    ResultLine = "Directory not found. " + e.Message;
                    FileNameStr = null;
                }
                catch (FileNotFoundException e)
                {
                    S = "";
                    ResultLine = "File '" + e.FileName + "' not found";
                    FileNameStr = null;
                }
            }

            if (FileReader != null)
            {
                S = FileReader.ReadLine();
                if (S == null)
                {
                    S = "";
                    if (OutputTracelevel.HasFlag(TraceLevels.FileEnterLeave))
                    {
                        ResultLine = "End of File '" + FileNameStr + "'";
                    }

                    FileReader.Close();
                    FileReader = null;
                    FileNameStr = null;
                }
                else
                {
                    LinesRead++;
                }
            }

            return S;
        }
    }

    public class ResultWriter
    {
        public String FileNameStr = null;
        public StreamWriter FileStreamWriter = null;

        public List<String> ResultLines = null;

        public Int32 LinesWritten = 0;

        private ConsoleColor backgroundColor = Console.BackgroundColor;
        private ConsoleColor foregroundColor = Console.ForegroundColor;

        public Boolean UseColors = true;


        public ResultWriter()
            : this((String)null)
        {
        }

        public ResultWriter(String FileNameStr)
        {
            this.FileNameStr = FileNameStr;
            this.FileStreamWriter = null;
            this.ResultLines = null;
        }
        
        public ResultWriter(List<String> ResultLines)
            : this((String)null)
        {
            this.ResultLines = ResultLines;
        }

        public ConsoleColor BackgroundColor { 
            get { return backgroundColor; }
            set { if (backgroundColor != value) { SetBackgroundColor(value); } } 
        }

        public ConsoleColor ForegroundColor { 
            get { return foregroundColor; }
            set { if (foregroundColor != value) { SetForegroundColor(value); } } 
        }

        private void SetBackgroundColor(ConsoleColor color)
        {
            if (!UseColors) return;

            // this.CheckFile(ref ResultLine);

            this.backgroundColor = color; 

            if (FileStreamWriter != null)
            {
                String SetColorCommand = "\0background color=" + backgroundColor.ToString();
                FileStreamWriter.WriteLine(SetColorCommand);
            }
            else if (ResultLines != null)
            {
                String SetColorCommand = "\0background color=" + backgroundColor.ToString();
                ResultLines.Add(SetColorCommand);
            }
            else
            {
                Console.ForegroundColor = backgroundColor;
            }
        }

        private void SetForegroundColor(ConsoleColor color)
        {
            if (!UseColors) return;

            // this.CheckFile(ref ResultLine);

            this.foregroundColor = color; 

            if (FileStreamWriter != null)
            {
                String SetColorCommand = "\0foreground color=" + foregroundColor.ToString();
                FileStreamWriter.WriteLine(SetColorCommand);
            }
            else if (ResultLines != null)
            {
                String SetColorCommand = "\0foreground color=" + foregroundColor.ToString();
                ResultLines.Add(SetColorCommand);
            }
            else
            {
                Console.ForegroundColor = foregroundColor;
            }
        }

        public void ResetColor()
        {
            if (!UseColors) return;

            // this.CheckFile(ref ResultLine);

            if (FileStreamWriter != null)
            {
                String SetColorCommand = "\0ResetColor";
                FileStreamWriter.WriteLine(SetColorCommand);
            }
            else if (ResultLines != null)
            {
                String SetColorCommand = "\0ResetColor";
                ResultLines.Add(SetColorCommand);
            }
            else
            {
                Console.ResetColor();
            }

            backgroundColor = Console.BackgroundColor;
            foregroundColor = Console.ForegroundColor;
        }

        public enum TextMode 
        {
            Normal = 0,
            Highlight = 1,
            Warning = 2,
            Error = 3
        }

        public void SetTextMode(TextMode tm)
        {
            if (tm == TextMode.Error)
            {
                if (UseColors && ForegroundColor != ConsoleColor.Red)
                { 
                    SetForegroundColor(ConsoleColor.Red);
                }
            }
            else
            {
                //if (UseColors && ForegroundColor != ConsoleColor.Gray)
                if (UseColors && ForegroundColor == ConsoleColor.Red)
                {
                    ResetColor();
                }
            }
        }

        public void CheckFile(ref String ResultText)
        {
            if (FileStreamWriter == null)
            {
                if (!String.IsNullOrWhiteSpace(FileNameStr))
                {
                    if (!Path.HasExtension(FileNameStr))
                    {
                        FileNameStr += ".cres";
                    }
                    try
                    {
                        FileStreamWriter = File.CreateText(FileNameStr);
                    }
                    catch (FileNotFoundException e)
                    {
                        ResultText = "File '" + e.FileName + "' not found";
                        FileNameStr = null;
                    }
                }
            }
        }

        public void WriteLine(String ResultLine)
        {
            this.CheckFile(ref ResultLine);

            if (FileStreamWriter != null)
            {
                FileStreamWriter.WriteLine(ResultLine);
            }
            else if (ResultLines != null)
            {
                ResultLines.Add(ResultLine);
            }
            else
            {
                Console.WriteLine(ResultLine);
            }
            LinesWritten++;
        }

        public void Write(String ResultText)
        {
            this.CheckFile(ref ResultText);

            if (FileStreamWriter != null)
            {
                FileStreamWriter.Write(ResultText);
            }
            else if (ResultLines != null)
            {
                if (ResultLines.Count > 0)
                {
                    ResultLines[ResultLines.Count - 1] += ResultText;
                }
                else
                {
                    ResultLines.Add(ResultText);
                }
            }
            else
            {
                Console.Write(ResultText);
            }
        }


        public void WriteErrorLine(String ResultLine)
        {
            SetTextMode(TextMode.Error);
            WriteLine(ResultLine);
            SetTextMode(TextMode.Normal);
        }

        public void Close()
        {
            if (FileStreamWriter != null) {
                FileStreamWriter.Flush();
                FileStreamWriter.Close();
                FileNameStr = null;
            }
        }
    }

    public class CommandReader
    {
        private CommandAccessorStack CommandAccessors = null;
        public ResultWriter ResultLineWriter = null;
        public Boolean WriteCommandToResultWriter = true;
        public Boolean ReadFromConsoleWhenEmpty = false;
        private TraceLevels GlobalOutputTracelevel = TraceLevels.Normal; //TraceLevels.All; // TraceLevels.Normal;
        private FormatProviderKind GlobalFormatProviderSource = FormatProviderKind.DefaultFormatProvider;

        public List<String> CommandHistory = new List<String>();

        public CommandReader(ResultWriter ResultLineWriter = null)
        {
            this.ResultLineWriter = ResultLineWriter;
        }

        public CommandReader(String[] args, ResultWriter ResultLineWriter = null)
            : this(ResultLineWriter)
        {
            // args = CheckForOptions(args);

            if (args.Length == 1 && File.Exists(args[0]))
            {
                AddFile(args[0]);
            }
            else if (args.Length >= 1)
            {
                AddCommandList("Command list", args);
            }
        }

        public CommandReader(String CommandLine, ResultWriter ResultLineWriter = null)
            : this(ResultLineWriter)
        {
            if (File.Exists(CommandLine))
            {
                AddFile(CommandLine);
            }
            else
            {
                AddCommandBlock("Command block", CommandLine);
            }
        }

        public CommandReader(String CommandName, String[] args, ResultWriter ResultLineWriter = null)
            : this(ResultLineWriter)
        {
            if (args.Length == 1 && File.Exists(args[0]))
            {
                AddFile(args[0]);
            }
            else if (args.Length >= 1)
            {
                AddCommandList(CommandName, args);
            }
        }

        public CommandReader(String CommandName, String CommandLine, ResultWriter ResultLineWriter = null)
            : this(ResultLineWriter)
        {
            if (File.Exists(CommandLine))
            {
                AddFile(CommandLine);
            }
            else
            {
                AddCommandBlock(CommandName, CommandLine);
            }
        }

        public TraceLevels OutputTracelevel { 
            get 
            {
                if (CommandAccessors != null)
                {
                    return CommandAccessors.OutputTracelevel;
                }
                else 
                {
                    return GlobalOutputTracelevel;
                }
            } 

            set 
            {
                if (CommandAccessors != null)
                {
                    CommandAccessors.OutputTracelevel = value;
                }
                else
                {
                    GlobalOutputTracelevel = value;
                }
            } 
        }

        public FormatProviderKind FormatProviderSource
        {
            get
            {
                if (CommandAccessors != null)
                {
                    return CommandAccessors.FormatProviderSource;
                }
                else
                {
                    return GlobalFormatProviderSource;
                }
            }

            set
            {
                if (CommandAccessors != null)
                {
                    CommandAccessors.FormatProviderSource = value;
                }
                else
                {
                    GlobalFormatProviderSource = value;
                }
            }
        }

        public void Write(String Line)
        {   // Echo Line to output
            if (ResultLineWriter != null) 
            {   // Echo Line to file output
                ResultLineWriter.Write(Line);
            }
            else
            {   // Echo Line to console output
                Console.Write(Line);
            }
        }

        public void WriteLine(String Line)
        {   // Echo Line to output
            if (ResultLineWriter != null)
            {   // Echo Line to file output
                ResultLineWriter.WriteLine(Line);
            }
            else
            {   // Echo Line to console output
                Console.WriteLine(Line);
            }
        }

        public void AddCommandList(String CommandListName, String[] ListOfCommands)
        {
            CommandList CL = new CommandList(ListOfCommands);

            if (CommandAccessors == null)
            {
                CommandAccessors = new CommandAccessorStack();
            }

            CommandAccessors.Add(new CommandBlockAccessor(CommandListName, CL));
        }

        public void AddCommandBlock(String CommandBlockName, String CommandBlock)
        {
            if (!String.IsNullOrWhiteSpace(CommandBlock))
            {
                if (CommandAccessors == null)
                {
                    CommandAccessors = new CommandAccessorStack();
                }

                CommandList CL = new CommandList(CommandBlock);

                CommandAccessors.Add(new CommandBlockAccessor(CommandBlockName, CL));
            }
        }
        
        public void AddFile(String filename)
        {
            if (CommandAccessors == null)
            {
                CommandAccessors = new CommandAccessorStack();
            }
            CommandAccessors.Add(new CommandFileAccessor(filename));
        }

        public Boolean HasAccessor() => (CommandAccessors != null);

        public String Accessor()
        {
            String AccessorName = null;
            if (CommandAccessors != null)
            {
                AccessorName = CommandAccessors.GetName();
            }
            return AccessorName;
        }

        public virtual Boolean ReadCommand(ref String ResultLine, out String CommandLine)
        {
            if (CommandAccessors != null)
            {
                CommandLine = CommandAccessors.GetCommandLine(ref ResultLine);
                if (CommandAccessors.Count <= 0)
                {
                    CommandAccessors = null;
                }

                if (WriteCommandToResultWriter && !String.IsNullOrWhiteSpace(CommandLine))
                {   // Echo Command to output
                    if (!String.IsNullOrWhiteSpace(ResultLine))
                    {
                        WriteLine(ResultLine);
                        ResultLine = "";
                    }
                    Write("| ");
                    WriteLine(CommandLine);
                }
            }
            else if (ReadFromConsoleWhenEmpty)
            {
                // Show that we are ready to next Command from user
                Write("|>");

                CommandLine = Console.ReadLine();
                CommandHistory.Add(CommandLine);
            }
            else
            {
                CommandLine = null;
            }

            Boolean CommandRead = CommandLine != null;
            return CommandRead;
        }
    }

}