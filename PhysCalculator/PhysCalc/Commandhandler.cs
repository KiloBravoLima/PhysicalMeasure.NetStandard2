﻿using System;

using TokenParser;

namespace CommandParser
{

    public class Commandhandler
    {
        public Commandhandler()
        {
        }

        public Commandhandler(String[] args)
        {
        }

        public virtual Boolean Command(ref String CommandLine, out String ResultLine)
        {
            Boolean CommandHandled = false;

            ResultLine = "Unknown command";

            return CommandHandled;
        }

        public delegate Boolean CommandDelegate(ref string CommandLine, ref string ResultLine);

        // static 
        public Boolean CheckForCommand(String CommandKeyword, CommandDelegate CmdHandler, ref String CommandLine, ref String ResultLine, ref Boolean CommandHandled)
        {
            Boolean IsThisCommand = TryParseToken(CommandKeyword, ref CommandLine);

            if (IsThisCommand)
            {
                ResultLine = "";
                CommandHandled = CmdHandler(ref CommandLine, ref ResultLine);
            }

            return IsThisCommand;
        }

        // static 
        public Boolean StartsWithKeyword(String Keyword, String CommandLine) => CommandLine.StartsWithKeyword(Keyword);

        // static 
        public String SkipToken(String Token, String CommandLine) => CommandLine.SkipToken(Token);

        // static 
        public Boolean ParseChar(Char ch, ref String CommandLine, ref string ResultLine) => TokenString.ParseChar(ch, ref CommandLine, ref ResultLine);

        // static 
        public Boolean TryParseChar(Char ch, ref String CommandLine) => TokenString.TryParseChar(ch, ref CommandLine);

        // static 
        public Boolean ParseToken(String Token, ref String CommandLine, ref string ResultLine) => TokenString.ParseToken(Token, ref CommandLine, ref ResultLine);

        // static 
        public Boolean TryParseToken(String Token, ref String CommandLine) => TokenString.TryParseToken(Token, ref CommandLine);

        public Boolean TryParseTokenPrefix(String Token, ref String CommandLine) => TokenString.TryParseTokenPrefix(Token, ref CommandLine);


        // static 
        public String ReadToken(String CommandLine, out String Token) => CommandLine.ReadToken(out Token);
    }
}
