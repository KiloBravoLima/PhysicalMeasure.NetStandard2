﻿using System;
using System.Linq;

namespace TokenParser
{
    public static class TokenString
    {
        #region Static parse methods

        public static Boolean ParseChar(Char ch, ref String commandLine, ref string resultLine)
        {
            Boolean IsExpectedChar = (!String.IsNullOrEmpty(commandLine)) && commandLine[0] == ch;

            if (IsExpectedChar)
            {
                commandLine = commandLine.Substring(1);
            }
            else
            {
                resultLine = resultLine + " Char '" + ch + "' expected";
            }
            return IsExpectedChar;
        }

        public static Boolean TryParseChar(Char ch, ref String commandLine)
        {
            Boolean IsExpectedChar = (!String.IsNullOrEmpty(commandLine)) && commandLine[0] == ch;
            if (IsExpectedChar)
            {
                commandLine = commandLine.Substring(1);
            }
            return IsExpectedChar;
        }

        public static Boolean ParseToken(String token, ref String commandLine, ref string resultLine)
        {
            Boolean IsExpectedToken = commandLine.StartsWithKeyword(token);
            if (IsExpectedToken)
            {
                commandLine = commandLine.SkipToken(token);
            }
            else
            {
                resultLine = resultLine + " Token '" + token + "' expected";
            }
            return IsExpectedToken;
        }

        public static Boolean TryParseToken(String token, ref String commandLine)
        {
            Boolean IsExpectedToken = commandLine.StartsWithKeyword(token);

            if (IsExpectedToken)
            {
                commandLine = commandLine.SkipToken(token);
            }
            return IsExpectedToken;
        }

        public static Boolean TryParseTokenPrefix(String token, ref String commandLine)
        {
            int Tokenlen = commandLine.StartsWithKeywordPrefix(token);

            Boolean IsExpectedToken = Tokenlen > 0;

            if (IsExpectedToken)
            {
                commandLine = commandLine.SkipToken(token.Substring(0, Tokenlen));
            }
            return IsExpectedToken;
        }


        #endregion Static parse methodes

        #region Class extension parse methods

        //public static Boolean IsHexDigit(this Char c) => (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') || (c >= '0' && c <= '9');
        public static Boolean IsHexDigitLetter(this Char c) => (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');


        public static int PeekChar(this String commandLine, out Char ch)
        {
            int i = commandLine.Length;
            if (i > 0)
            {
                ch = commandLine[0];
                i = 1;
            }
            else
            {
                ch = '\0';
            }
            return i;
        }


        public static Boolean IsKeyword(this String commandLine, String keyword) => commandLine.Equals(keyword, StringComparison.OrdinalIgnoreCase);

        public static Boolean StartsWithKeyword(this String commandLine, String keyword) => commandLine.StartsWith(keyword, StringComparison.OrdinalIgnoreCase);

        public static int StartsWithKeywordPrefix(this String commandLine, String keyword)
        {
            int i = commandLine.TakeWhile(c => Char.IsLetterOrDigit(c) || Char.Equals(c, '_')).Count();
            int compareLen = Math.Min(Math.Min(commandLine.Length, i), keyword.Length);
            if (commandLine.StartsWith(keyword.Substring(0, compareLen), StringComparison.OrdinalIgnoreCase))
            {
                return compareLen;
            }
            else 
            {
                return 0;
            }
        }

        public static String SkipToken(this String commandLine, String token) => commandLine.Substring(token.Length).TrimStart();

        public static String ReadToken(this String commandLine, out String token)
        {
            int i = PeekToken(commandLine, out token);
            return commandLine.Substring(i).TrimStart();
        }

        public static int PeekToken(this String commandLine, out String token)
        {
            int i = commandLine.IndexOf(" ");
            if (i < 0)
            {
                i = commandLine.Length;
            }
            token = commandLine.Substring(0, i);
            return i;
        }

        public static int PeekIdentifier(this String commandLine, out String token)
        {
            int i = commandLine.TakeWhile(c => Char.IsLetterOrDigit(c) || Char.Equals(c, '_')).Count();
            token = commandLine.Substring(0, i);
            return i;
        }

        public static Boolean IsIdentifier(this String commandLine) => (!String.IsNullOrEmpty(commandLine)) && (Char.IsLetter(commandLine[0]) || Char.Equals(commandLine[0], '_'));

        public static String ReadIdentifier(this String commandLine, out String identifier)
        {
            if (IsIdentifier(commandLine))
            {
                int i = commandLine.TakeWhile(c => Char.IsLetterOrDigit(c) || Char.Equals(c, '_')).Count();
                identifier = commandLine.Substring(0, i);

                return commandLine.Substring(i).TrimStart();
            }
            else
            {
                identifier = null;
                return commandLine;
            }
        }

        #endregion Class extension parse methodes

    }
}
