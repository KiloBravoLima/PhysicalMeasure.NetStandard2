using System;
using System.Collections.Generic;
using System.Globalization;

using System.Diagnostics;
using System.IO;

using PhysicalMeasure;

using TokenParser;

using PhysicalCalculator.Identifiers;


namespace PhysicalCalculator.Expression
{

    public interface IEnvironment
    {
        TraceLevels OutputTracelevel { get; set; }
        FormatProviderKind FormatProviderSource { get; set; }

        CultureInfo CurrentCultureInfo { get; }

        Boolean SetLocalIdentifier(String identifierName, INametableItem item);
        Boolean RemoveLocalIdentifier(String identifierName);

        Boolean FindLocalIdentifier(String identifierName, out INametableItem item);
        Boolean FindIdentifier(String identifierName, out IEnvironment foundInContext, out INametableItem item);

        Boolean SystemSet(String systemName, out INametableItem systemItem);

        Boolean UnitGet(String unitName, out Unit unitValue, ref String resultLine);
        Boolean UnitSet(IUnitSystem unitSystem, String unitName, Quantity unitValue, out INametableItem unitItem);

        Boolean VariableGet(String variableName, out Quantity variableValue, ref String resultLine);
        Boolean VariableSet(String variableName, Quantity variableValue);


        Boolean FunctionFind(String functionName, out IFunctionEvaluator functionEvaluator);
    }

    static class PhysicalExpression 
    {
        #region Physical Expression parser methods
        /**
            CE = E | E [ SYS ] .
            E = E "+" T | E "-" T | T .
            T = T "*" F | T "/" F | F .
            F = PE | PE ^ SN .         
            PE = PQ | UE | VAR | FUNC "(" EXPLIST ")" | "(" E ")" .         
            PQ = num SU . 
            SU = sU | U .
            SYS = sys | sys "." SU | SU .
            UE = + F | - F .
            SN = + num | - num | num .
            EXPLIST = E | E "," EXPLIST .
          
         
            CE = E CEopt .
            Eopt = [ SYS ] | e .
            E = T Eopt .
            Eopt = "+" T Eopt | "-" T Eopt | e .
          
     *      T = F Topt .
     *      Topt = "*" F Topt | "/" F Topt | e .

     **     T1 = T2 T1opt .
     **     T1opt = "*" T2 T1opt | e .
     **     T2 = F T2opt .
     **     T2opt = "/" F T2opt | e .
         
            F = PE Fopt .
            PE = PQ | UE | VAR | FUNC "(" EXPLIST ")" | "(" E ")" .
            Fopt = ^ SN | e .
            PQ = num SU . 
            SU = sU | U | e .
            SYS = SYST | SU .
            SYST = system SYSTopt .
            SN = Sopt num .
            Sopt = + | - | e .
            SYSTopt = "." SU | e .
            UE =  + F | - F .
            EXPLIST = E EXPLISTopt . 
            EXPLISTopt = "," EXPLIST | e .
          
         **/

        // Delegate types
        // VariableLookup callback
        public delegate Boolean IdentifierItemLookupFunc(String identifierName, out IEnvironment foundInContext, out INametableItem item);
        public delegate Boolean QualifiedIdentifierItemLookupFunc(IEnvironment lookInContext, String identifierName, out INametableItem item);
        public delegate Boolean IdentifierContextLookupFunc(String identifierName, out IEnvironment foundInContext, out IdentifierKind identifierKind);
        public delegate Boolean QualifiedIdentifierContextLookupFunc(IEnvironment lookInContext, String identifierName, out IEnvironment foundInContext, out IdentifierKind identifierKind);

        public delegate Boolean VariableValueLookupFunc(IEnvironment lookInContext, String variableName, out Quantity variableValue, ref String resultLine);
        public delegate Boolean UnitLookupFunc(IEnvironment lookInContext, String variableName, out Unit unitValue, ref String resultLine);
        public delegate Boolean FunctionLookupFunc(IEnvironment lookInContext, String functionName, out IFunctionEvaluator functionEvaluator);
        public delegate Boolean FunctionEvaluateFunc(String functionName, IFunctionEvaluator functionevaluator, List<Quantity> parameterlist, out Quantity functionResult, ref String resultLine);
        public delegate Boolean FunctionEvaluateFileReadFunc(String functionName, out Quantity functionResult, ref String resultLine);

        // Delegate static globals
        public static IdentifierItemLookupFunc IdentifierItemLookupCallback;
        public static QualifiedIdentifierItemLookupFunc QualifiedIdentifierItemLookupCallback;

        public static IdentifierContextLookupFunc IdentifierContextLookupCallback;
        public static QualifiedIdentifierContextLookupFunc QualifiedIdentifierContextLookupCallback;

        public static VariableValueLookupFunc VariableValueGetCallback;
        public static UnitLookupFunc UnitGetCallback;
        public static FunctionLookupFunc FunctionLookupCallback;
        public static FunctionEvaluateFunc FunctionEvaluateCallback;
        public static FunctionEvaluateFileReadFunc FunctionEvaluateFileReadCallback;

        // static access functions

        public static Boolean IdentifierItemLookup(String identifierName, out IEnvironment foundInContext, out INametableItem item, ref String resultLine)
        {
            item = null;
            foundInContext = null;
            if (IdentifierItemLookupCallback != null)
            {
                return IdentifierItemLookupCallback(identifierName, out foundInContext, out item);
            }
            return false;
        }

        public static Boolean QualifiedIdentifierItemLookup(IEnvironment lookInContext, String identifierName, out INametableItem item, ref String resultLine)
        {
            item = null;
            if (QualifiedIdentifierItemLookupCallback != null)
            {
                return QualifiedIdentifierItemLookupCallback(lookInContext, identifierName, out item);
            }
            return false;
        }

        public static Boolean IdentifierContextLookup(String variableName, out IEnvironment foundInContext, out IdentifierKind identifierKind, ref String resultLine)
        {
            identifierKind = IdentifierKind.Unknown;
            foundInContext = null;
            if (IdentifierContextLookupCallback != null)
            {
                return IdentifierContextLookupCallback(variableName, out foundInContext, out identifierKind);
            }
            return false;
        }

        public static Boolean QualifiedIdentifierContextLookup(IEnvironment lookInContext, String variableName, out IEnvironment foundInContext, out IdentifierKind identifierKind, ref String resultLine)
        {
            identifierKind = IdentifierKind.Unknown;
            foundInContext = null;
            if (QualifiedIdentifierContextLookupCallback != null)
            {
                return QualifiedIdentifierContextLookupCallback(lookInContext, variableName, out foundInContext, out identifierKind);
            }
            return false;
        }

        public static Boolean VariableGet(IEnvironment lookInContext, String variableName, out Quantity variableValue, ref String resultLine)
        {
            variableValue = null;
            if (VariableValueGetCallback != null)
            {
                return VariableValueGetCallback(lookInContext, variableName, out variableValue, ref resultLine);
            }
            return false;
        }

        public static Boolean UnitGet(IEnvironment lookInContext, String unitName, out Unit unitValue, ref String resultLine)
        {
            unitValue = null;
            if (UnitGetCallback != null)
            {
                return UnitGetCallback(lookInContext, unitName, out unitValue, ref resultLine);
            }
            return false;
        }

        public static Boolean FunctionGet(IEnvironment lookInContext, String functionName, List<Quantity> parameterlist, out Quantity functionResult, ref String resultLine)
        {
            functionResult = null;
            IFunctionEvaluator functionevaluator;
            if (FunctionLookupCallback(lookInContext, functionName, out functionevaluator))
            {
                if (FunctionEvaluateCallback != null)
                {
                    return FunctionEvaluateCallback(functionName, functionevaluator, parameterlist, out functionResult, ref resultLine);
                }
                else
                {
                    resultLine = "Internal error: No FunctionEvaluateCallback handler specified";
                }
            }
            else
            {
                resultLine = "Internal error: FunctionLookupCallback failed";
            }

            return false;
        }

        public static Boolean FileFunctionGet(String functionName, out Quantity functionResult, ref String resultLine)
        {
            functionResult = null;
            if (FunctionEvaluateFileReadCallback != null)
            {
                return FunctionEvaluateFileReadCallback(functionName, out functionResult, ref resultLine);
            }
            else
            {
                resultLine = "Internal error: No FunctionEvaluateFileReadCallback handler specified";
            }
            return false;
        }

        public static List<Quantity> ParseExpressionList(ref String commandLine, ref String resultLine, List<String> ExpectedFollow, Boolean AllowEmptyList = false )
        {
            List<Quantity> pqList = new List<Quantity>();
            Boolean MoreToParse = false;
            Boolean OK = true;
            List<String> TempExpectedFollow = ExpectedFollow;
            if (!TempExpectedFollow.Contains(","))
            {
                TempExpectedFollow = new List<string>(ExpectedFollow);
                TempExpectedFollow.Add(","); 
            }
            do
            {
                Quantity pq = null;
                pq = ParseConvertedExpression(ref commandLine, ref resultLine, TempExpectedFollow);
                OK = pq != null;
                if (OK)
                {
                    pqList.Add(pq);
                    MoreToParse = TokenString.TryParseToken(",", ref commandLine);
                }
                else
                {
                    if (!AllowEmptyList)
                    {
                        return null;
                    }
                }
            } while (OK && MoreToParse);
            return pqList;
        }


        public static Nullable<Boolean> ParseBooleanExpression(ref String commandLine, ref String resultLine, List<String> ExpectedFollow)
        {
            IQuantity pq = ParseExpression(ref commandLine, ref resultLine, ExpectedFollow);
            if (pq != null)
            {
                return !pq.Equals(PQ_False);
            }
            return null;
        }

        public static Quantity ParseConvertedExpression(ref String commandLine, ref String resultLine, List<String> ExpectedFollow)
        {
            Quantity pq;

            List<String> TempExpectedFollow = ExpectedFollow;
            if (!TempExpectedFollow.Contains("["))
            {
                TempExpectedFollow = new List<string>(ExpectedFollow);
                TempExpectedFollow.Add("[");
            }
            pq = ParseExpression(ref commandLine, ref resultLine, TempExpectedFollow);
            if (pq != null)
            {
                pq = ParseOptionalConvertedExpression(pq, ref commandLine, ref resultLine);
            }

            return pq;
        }

        private static readonly NamedDerivedUnit ConvertToBaseUnits = new NamedDerivedUnit(null, "BaseUnitDimensions", "Dims", new SByte[] { -127, -127, -127, -127, -127, -127, -127 });

        public static Quantity ParseOptionalConvertedExpression(Quantity pq, ref String commandLine, ref String resultLine)
        {
            Unit pu = null;
            if (!String.IsNullOrEmpty(commandLine))
            {
                pu = ParseOptionalConvertToUnit(ref commandLine, ref resultLine);
                if (Object.ReferenceEquals(pu, ConvertToBaseUnits))
                {
                    pu = new CombinedUnit(pq.Unit.AsPrefixedUnitExponentList());
                }
            }

            Quantity pqRes;
            if (pu != null)
            {
                pqRes = pq.ConvertTo(pu);
                if (pqRes == null)
                {
                    resultLine = "The unit " + pq.Unit.ToPrintString() + " can't be converted to " + pu.ToPrintString() + "\n";
                    CombinedUnit newRelativeUnit = new CombinedUnit(pu).CombineMultiply(pq.Unit.Divide(pu));
                    pqRes = pq.ConvertTo(newRelativeUnit);
                }
            }
            else
            {
                // No unit specified to convert to; Check if pq can shown as a NamedDerivedUnit
                pqRes = CheckForNamedDerivedUnit(pq);
            }
            return pqRes;
        }

        public static Quantity CheckForNamedDerivedUnit(Quantity pq)
        {
            Quantity pqRes = pq;
            if (pqRes != null)
            {
                Unit namedDerivedUnit = pqRes.Unit?.AsNamedUnit;
                if (namedDerivedUnit != null)
                {
                    pqRes = new Quantity(pqRes.Value, namedDerivedUnit);
                }
            }

            return pqRes;
        }

        public static Unit ParseOptionalConvertToUnit(ref String commandLine, ref String resultLine)
        {
            Unit pu = null;
            if (TokenString.TryParseToken("[", ref commandLine))
            { // "Convert to unit" square parentheses

                int UnitStringLen = commandLine.IndexOf(']');
                if (UnitStringLen == 0)
                {
                    resultLine = "Missing unit to convert to";
                }
                else
                {
                    String UnitString;
                    if (UnitStringLen == -1)
                    {   // Not terminated by ']', but handle that later
                        // Try to parse rest of line as an unit 
                        UnitString = commandLine;
                        UnitStringLen = commandLine.Length;
                    }
                    else
                    {   // Parse only the valid unit formatted string
                        UnitString = commandLine.Substring(0, UnitStringLen);
                    }

                    String DimToken;
                    UnitString.Trim().ReadToken(out DimToken);
                    if (   DimToken.Equals("base", StringComparison.InvariantCultureIgnoreCase) 
                        || DimToken.Equals("dim", StringComparison.InvariantCultureIgnoreCase))
                    {
                        pu = ConvertToBaseUnits;
                    }
                    else
                    {
                        pu = ParsePhysicalUnit(ref UnitString, ref resultLine);
                    }
                }
                
                commandLine = commandLine.Substring(UnitStringLen);
                commandLine = commandLine.TrimStart();

                TokenString.ParseToken("]", ref commandLine, ref resultLine);
            }

            return pu;
        }

        // Token kinds
        public enum TokenKind
        {
            None = 0,
            Operand = 1,
            Operator = 2 /*,
            UnaryOperator = 3 */
        }

        class token
        {
            public readonly TokenKind TokenKind;

            public readonly Quantity Operand;
            public readonly OperatorKind Operator;

            public token(Quantity Operand)
            {
                this.TokenKind = TokenKind.Operand;
                this.Operand = Operand;
            }

            public token(OperatorKind Operator)
            {
                this.TokenKind = TokenKind.Operator;
                this.Operator = Operator;
            }

            public override string ToString() => TokenKind.ToString() + (TokenKind == TokenKind.None ? "" : " " + (TokenKind == TokenKind.Operand ? Operand.ToString() : Operator.ToString()));
        }

        class expressiontokenizer
        {
            public String InputString;
            public String ResultString;

            public int Pos = 0;

            public Unit dimensionless = Physics.dimensionless;
            public List<String> ExpectedFollow = new List<String>(); // The list of words and symbols which will terminate parsing the InputString without signaling an error.

            public Boolean ThrowExceptionOnInvalidInput = false;

            private Boolean inputRecognized = true;
            private Boolean errorReported = false;
            private Boolean InvalidPhysicalExpressionFormatErrorReported = false;


            private Stack<OperatorKind> Operators = new Stack<OperatorKind>();
            private List<token> Tokens = new List<token>();

            TokenKind LastReadToken = TokenKind.None;
            int ParenCount = 0;

            public expressiontokenizer(String InputString)
            {
                this.InputString = InputString;
            }

            public expressiontokenizer(Unit dimensionless, String InputString)
            {
                this.dimensionless = dimensionless;
                this.InputString = InputString;
            }

            public string RemainingInput => InputString.Substring(Pos);

            public Boolean InputRecognized => inputRecognized;

            private Boolean PushNewOperator(OperatorKind newOperator)
            {
                Boolean NewOperatorValid = (LastReadToken != TokenKind.Operator);

                if (!NewOperatorValid)
                {
                    if (newOperator == OperatorKind.add)
                    {
                        newOperator = OperatorKind.unaryplus;
                        NewOperatorValid = true;
                    }
                    else
                        if (newOperator == OperatorKind.sub)
                    {
                        newOperator = OperatorKind.unaryminus;
                        NewOperatorValid = true;
                    }
                }

                if (NewOperatorValid)
                {
                    if (Operators.Count > 0)
                    {
                        // Pop operators with precedence higher than new operator
                        OperatorKind NewOperatorPrecedence = newOperator.Precedence();
                        Boolean KeepPoping = true;
                        while ((Operators.Count > 0) && KeepPoping)
                        {
                            OperatorKind NextOperatorsPrecedence = Operators.Peek().Precedence();
                            KeepPoping = ((NextOperatorsPrecedence > NewOperatorPrecedence)
                                          || ((NextOperatorsPrecedence == NewOperatorPrecedence)
                                              && (NewOperatorPrecedence != OperatorKind.unaryplus)));
                            if (KeepPoping)
                            {
                                Tokens.Add(new token(Operators.Pop()));
                            }
                        }
                    }
                    Operators.Push(newOperator);
                    LastReadToken = TokenKind.Operator;

                    return true;
                }
                else
                {
                    ReportInvalidPhysicalExpressionFormatError("Invalid or missing operand at pos " + Pos.ToString());

                    return false;
                }
            }

            private Boolean PushNewParenbegin()
            {
                if (LastReadToken == TokenKind.Operand)
                {
                    // Cannot follow operand
                    ReportInvalidPhysicalExpressionFormatError("Invalid or missing operator at pos " + Pos.ToString());
                    return false;
                }
                else
                {
                    // Push opening parenthesis onto stack
                    Operators.Push(OperatorKind.parenbegin);
                    //LastReadToken = TokenKind.Operator;
                    // Track number of parentheses
                    ParenCount++;

                    return true;
                }
            }

            private Boolean PopUntilParenbegin()
            {
                if (ParenCount == 0)
                {
                    if (!ExpectedFollow.Contains(")"))
                    {
                        // Must have matching opening parenthesis
                        ReportInvalidPhysicalExpressionFormatError("Unmatched closing parenthesis at pos " + Pos.ToString());
                    }

                    return false;
                }
                else if (LastReadToken != TokenKind.Operand)
                {
                    // Must follow operand
                    ReportInvalidPhysicalExpressionFormatError("Invalid or missing operand at pos " + Pos.ToString());

                    return false;
                }
                else
                {
                    // Pop all operators until matching opening parenthesis found
                    OperatorKind temp = Operators.Pop();
                    while (temp != OperatorKind.parenbegin)
                    {
                        Tokens.Add(new token(temp));
                        temp = Operators.Pop();
                    }

                    // Track number of opening parenthesis
                    ParenCount--;

                    return true;
                }
            }


            private token RemoveFirstToken()
            {   // return first operator from post fix operators
                token Token = Tokens[0];
                Tokens.RemoveAt(0);

                return Token;
            }

            public void ReportError(String errorMessage)
            {
                // End of recognized input; Stop reading and return operator tokens from stack.
                inputRecognized = false;
                if (!errorReported)
                {
                    if (!String.IsNullOrEmpty(ResultString))
                    {
                        ResultString += ". ";
                    }
                    ResultString += errorMessage;
                    errorReported = true;
                }
                if (ThrowExceptionOnInvalidInput)
                {
                    throw new PhysicalUnitFormatException(ResultString);
                }
            }

            public void ReportInvalidPhysicalExpressionFormatError(String errorMessage)
            {
                
                if (!InvalidPhysicalExpressionFormatErrorReported)
                {
                    errorMessage = "The string argument is not in a valid physical expression format. " + errorMessage;
                }
                ReportError(errorMessage);
                InvalidPhysicalExpressionFormatErrorReported = true;
            }

            public token GetToken()
            {
                Debug.Assert(InputString != null);

                if (Tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }

                while (InputString.Length > Pos && InputRecognized)
                {
                    Char c = InputString[Pos];

                    if (Char.IsWhiteSpace(c))
                    {
                        // Ignore spaces, tabs, etc.
                        Pos++; // Shift to next char
                    }
                    else if (c == '(') 
                    {
                        // Push opening parenthesis onto operator stack
                        if (PushNewParenbegin())
                        {
                            Pos++; // Shift to next char
                        }
                        else
                        {
                            // End of recognized input; Stop reading and return operator tokens from stack.
                            inputRecognized = false;
                        }
                    }
                    else if (c == ')')
                    {
                        // Pop all operators until matching opening parenthesis found
                        if (PopUntilParenbegin())
                        {
                            Pos++; // Shift to next char
                        }
                        else
                        {
                            // End of recognized input; Stop reading and return operator tokens from stack.
                            inputRecognized = false;
                        }
                    }
                    else
                    {
                        OperatorKind NewOperator = OperatorKind.none;

                        if (Pos + 1 < InputString.Length && InputString[Pos + 1] == '=')
                        {
                            NewOperator = OperatorKindExtensions.OperatorKindFromChar1Equal(c);
                            if (NewOperator != OperatorKind.none)
                            {
                                Pos++; // Shift to next char
                            }
                        }

                        if (NewOperator == OperatorKind.none)
                        {
                            NewOperator = OperatorKindExtensions.OperatorKindFromChar(c);
                        }

                        if (NewOperator != OperatorKind.none)
                        {
                            if (PushNewOperator(NewOperator))
                            {
                                Pos++; // Shift to next char
                            }
                            else
                            {
                                // End of recognized input; Stop reading and return operator tokens from stack.
                                // Error signaling already done in PushNewOperator
                            }
                        }
                        else if (Char.IsDigit(c))
                        {
                            if (LastReadToken == TokenKind.Operand)
                            {
                                // End of recognized input; Stop reading and return operator tokens from stack.
                                ReportInvalidPhysicalExpressionFormatError("An operator must follow a operand. Invalid operand at '" + c + "' at pos " + Pos.ToString());
                            }
                            else
                            {
                                Double D;

                                String CommandLine = RemainingInput;
                                int OldLen = CommandLine.Length;
                                String ResultLine = "";
                                Boolean OK = ParseDouble(ref CommandLine, ref ResultLine, out D);
                                Pos += OldLen - CommandLine.Length;
                                if (OK)
                                {
                                    Unit pu = null;
                                    if (!String.IsNullOrWhiteSpace(CommandLine))
                                    {   // Parse optional unit
                                        OldLen = CommandLine.Length;
                                        CommandLine = CommandLine.TrimStart();
                                        if (!String.IsNullOrEmpty(CommandLine) && (Char.IsLetter(CommandLine[0])))
                                        {
                                            ResultLine = "";
                                            pu = ParsePhysicalUnit(ref CommandLine, ref ResultLine);
                                            Pos += OldLen - CommandLine.Length;
                                        }
                                    }
                                    if (pu == null)
                                    {
                                        pu = dimensionless;
                                    }

                                    Quantity pq = new Quantity(D, pu);

                                    LastReadToken = TokenKind.Operand;
                                    return new token(pq);
                                }

                                // End of recognized input; Stop reading and return operator tokens from stack.
                                ReportInvalidPhysicalExpressionFormatError("Invalid or missing operand after '" + c + "' at position " + Pos.ToString());
                            }
                        }
                        else if (Char.IsLetter(c) || Char.Equals(c, '_'))
                        {
                            String IdentifierName;

                            String CommandLine = RemainingInput;
                            int OldLen = CommandLine.Length;
                            String ResultLine = "";

                            Quantity pq;

                            Boolean PrimaryIdentifierFound = ParseQualifiedIdentifier(ref CommandLine, ref ResultLine, out IdentifierName, out pq);
                            // 2014-09-09 Moved to only be done when PrimaryIdentifierFound or call of .cal file as function : Pos += OldLen - CommandLine.Length;
                            int newPos = Pos + OldLen - CommandLine.Length;

                            if (!String.IsNullOrEmpty(ResultLine))
                            {
                                if (!String.IsNullOrEmpty(ResultString))
                                {
                                    ResultString += ". ";
                                }
                                ResultString += ResultLine;
                            }

                            if (PrimaryIdentifierFound)
                            {
                                // Increment read pos; mark IdentifierName as read
                                Pos = newPos;

                                // Check if any inner identifier was found
                                Boolean InnerIdentifierFound = (pq != null);
                                if (!InnerIdentifierFound)
                                {
                                    LastReadToken = TokenKind.Operand;
                                    return new token(null);
                                }
                            }
                            else
                            {
                                if (!String.IsNullOrEmpty(IdentifierName) && !String.IsNullOrEmpty(CommandLine) && CommandLine[0] == '(')
                                {
                                    OldLen = CommandLine.Length;

                                    string line2 = CommandLine.Substring(1).TrimStart();
                                    if (!String.IsNullOrEmpty(line2) && line2[0] == ')')
                                    {   // Undefined function without parameters? Maybe it is a .cal file name? 
                                        PrimaryIdentifierFound = File.Exists(IdentifierName + ".cal");
                                        if (PrimaryIdentifierFound)
                                        {
                                            TokenString.ParseChar('(', ref CommandLine, ref ResultLine);
                                            CommandLine = CommandLine.TrimStart();
                                            TokenString.ParseChar(')', ref CommandLine, ref ResultLine);

                                            FileFunctionGet(IdentifierName, out pq, ref ResultLine);
                                            Pos = newPos + OldLen - CommandLine.Length;
                                        }
                                    }
                                }

                                if (!PrimaryIdentifierFound)
                                {
                                    Unit pu = null;
                                    CommandLine = RemainingInput;
                                    if (!String.IsNullOrWhiteSpace(CommandLine))
                                    {   // Try parse an unit
                                        OldLen = CommandLine.Length;
                                        CommandLine = CommandLine.TrimStart();
                                        if (!String.IsNullOrEmpty(CommandLine) && (Char.IsLetter(CommandLine[0])))
                                        {
                                            ResultLine = "";
                                            pu = ParsePhysicalUnit(ref CommandLine, ref ResultLine);
                                            if (pu != null)
                                            {
                                                Pos += OldLen - CommandLine.Length;
                                                pq = new Quantity(1, pu);
                                            }
                                        }
                                    }
                                }

                                /**
                                if (!PrimaryIdentifierFound)
                                {
                                    resultLine = "Unknown identifier: '" + IdentifierName + "'";
                                }
                                **/

                            }

                            if (pq != null)
                            {
                                LastReadToken = TokenKind.Operand;
                                return new token(pq);
                            }

                            // End of recognized input; Stop reading and return operator tokens from stack.
                            ReportInvalidPhysicalExpressionFormatError("Invalid or missing operand at '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
                        }
                        else
                        {
                            // End of recognized input; Stop reading and return operator tokens from stack.
                            inputRecognized = false;

                            if (!ExpectedFollow.Contains(new String(c,1)))
                            {
                                ReportInvalidPhysicalExpressionFormatError("Invalid input '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
                            }
                        }
                    }

                    if (Tokens.Count > 0)
                    {   // return first operator from post fix operators
                        return RemoveFirstToken();
                    }
                };

                // Expression cannot end with operator
                if (LastReadToken == TokenKind.Operator)
                {
                    // End of recognized input; Stop reading and return operator tokens from stack.
                    ReportInvalidPhysicalExpressionFormatError("Operand expected '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
                }
                // Check for balanced parentheses
                if (ParenCount > 0)
                {
                    // End of recognized input; Stop reading and return operator tokens from stack.
                    ReportInvalidPhysicalExpressionFormatError("Closing parenthesis expected '" + InputString.Substring(Pos) + "' at position " + Pos.ToString());
                }
                // Retrieve remaining operators from stack
                while (Operators.Count > 0) 
                {
                    Tokens.Add(new token(Operators.Pop()));
                }

                if (Tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }

                return null;
            }
        }

        public static readonly Quantity PQ_False = new Quantity(0);
        public static readonly Quantity PQ_True = new Quantity(1);

        public static Quantity ParseExpression(ref String commandLine, ref String resultLine, List<String> ExpectedFollow ) // = null)
        {
            //public static readonly 
            Unit dimensionless = new CombinedUnit();

            expressiontokenizer Tokenizer = new expressiontokenizer(dimensionless, commandLine);

            Tokenizer.ExpectedFollow = ExpectedFollow;
            Tokenizer.ThrowExceptionOnInvalidInput = false;
            Stack<Quantity> Operands = new Stack<Quantity>();

            token Token = Tokenizer.GetToken();
            while (Token != null)
            {
                // OperatorKind.parenbegin indicates error in Tokenizer.GetToken();
                Debug.Assert(Token.TokenKind != TokenKind.Operand || Token.Operator != OperatorKind.parenbegin);

                if (Token.TokenKind == TokenKind.Operand)
                {
                    // Stack Quantity operand
                    Debug.Assert(Token.Operand != null);
                    Operands.Push(Token.Operand);
                }
                else if (Token.TokenKind == TokenKind.Operator)
                {

                    if (Token.Operator == OperatorKind.unaryplus)
                    {
                        // Nothing to do
                    }
                    else if (Token.Operator == OperatorKind.unaryminus)
                    {
                        Debug.Assert(Operands.Count >= 1);

                        Quantity pqTop = Operands.Pop();
                        // Invert sign of pq
                        Quantity pqResult = pqTop.Multiply(-1);
                        Debug.Assert(pqResult != null);
                        Operands.Push(pqResult);
                    }
                    else if (Token.Operator == OperatorKind.not)
                    {
                        Debug.Assert(Operands.Count >= 1);

                        Quantity pqTop = Operands.Pop();
                        // Invert pqTop as boolean
                        if (!pqTop.Equals(PQ_False))
                        {
                            pqTop = PQ_False;
                        }
                        else
                        {
                            pqTop = PQ_True;
                        }
                        Operands.Push(pqTop);
                    }
                    else if (Operands.Count >= 2)
                    {
                        Debug.Assert(Operands.Count >= 2);

                        Quantity pqSecond = Operands.Pop();
                        Quantity pqFirst = Operands.Pop();

                        Debug.Assert(pqSecond != null);
                        Debug.Assert(pqFirst != null);

                        if (Token.Operator == OperatorKind.add)
                        {
                            // Combine pq1 and pq2 to the new Quantity pq1*pq2   
                            Quantity pqResult = pqFirst.Add(pqSecond);
                            Debug.Assert(pqResult != null);
                            Operands.Push(pqResult);
                        }
                        else if (Token.Operator == OperatorKind.sub)
                        {
                            // Combine pq1 and pq2 to the new Quantity pq1/pq2
                            Quantity pqResult = pqFirst.Subtract(pqSecond);
                            Debug.Assert(pqResult != null);
                            Operands.Push(pqResult);
                        }
                        else if (Token.Operator == OperatorKind.mult)
                        {
                            // Combine pq1 and pq2 to the new Quantity pq1*pq2   
                            Quantity pqResult = pqFirst.Multiply(pqSecond);
                            Debug.Assert(pqResult != null);
                            Operands.Push(pqResult);
                        }
                        else if (Token.Operator == OperatorKind.div)
                        {
                            // Combine pq1 and pq2 to the new Quantity pq1/pq2
                            Quantity pqResult = pqFirst.Divide(pqSecond);
                            Debug.Assert(pqResult != null);
                            Operands.Push(pqResult);
                        }
                        else if (   (Token.Operator == OperatorKind.pow)
                                 || (Token.Operator == OperatorKind.root))
                        {
                            SByte Exponent;
                            if (pqSecond.Value >= 1)
                            {   // Use operator and Exponent
                                Exponent = (SByte)pqSecond.Value;
                            }
                            else
                            {   // Invert operator and Exponent
                                Exponent = (SByte)(1 / pqSecond.Value);

                                if (Token.Operator == OperatorKind.pow)
                                {
                                    Token = new token(OperatorKind.root);
                                }
                                else
                                {
                                    Token = new token(OperatorKind.pow);
                                }
                            }

                            if (Token.Operator == OperatorKind.pow)
                            {
                                // Combine pq and exponent to the new Quantity pq^expo
                                Quantity pqResult = pqFirst.Pow(Exponent);
                                Debug.Assert(pqResult != null);
                                Operands.Push(pqResult);
                            }
                            else
                            {
                                // Combine pq and exponent to the new Quantity pq^(1/expo)
                                Quantity pqResult = pqFirst.Rot(Exponent);
                                Debug.Assert(pqResult != null);
                                Operands.Push(pqResult);
                            }
                        }
                        else if (Token.Operator == OperatorKind.equals)
                        {
                            // Save pqFirst == pqSecond
                            Operands.Push(pqFirst.Equals(pqSecond) ?  PQ_True : PQ_False);
                        }
                        else if (Token.Operator == OperatorKind.differs)
                        {
                            // Save pqFirst != pqSecond
                            Operands.Push(pqFirst.Equals(pqSecond) ? PQ_False : PQ_True);
                        }
                        else if (   Token.Operator == OperatorKind.lessthan
                                 || Token.Operator == OperatorKind.lessorequals
                                 || Token.Operator == OperatorKind.largerthan
                                 || Token.Operator == OperatorKind.largerorequals
                                )
                        {
                            int res = pqFirst.CompareTo(pqSecond);

                            if (   ((Token.Operator == OperatorKind.lessthan) && (res < 0))
                                || ((Token.Operator == OperatorKind.lessorequals) && (res <= 0))
                                || ((Token.Operator == OperatorKind.largerthan) && (res > 0))
                                || ((Token.Operator == OperatorKind.largerorequals) && (res >= 0))
                                )
                            {
                                Operands.Push(PQ_True);
                            }
                            else
                            {
                                Operands.Push(PQ_False);
                            }
                        }
                        else
                        {
                            Debug.Assert(false);
                        }
                    }
                    else
                    if (Tokenizer.InputRecognized)
                    {   // Error: Unexpected token or missing operands (Operands.Count < 2).
                        Debug.Assert(Token.TokenKind == TokenKind.Operand);
                        // OperatorKind.parenbegin indicates error in Tokenizer.GetToken();
                        Debug.Assert(Token.Operator  != OperatorKind.parenbegin);

                        Debug.Assert(Operands.Count >= 2);
                        Debug.Assert(false);
                    }
                }

                Token = Tokenizer.GetToken();
            }

            commandLine = Tokenizer.RemainingInput; // Remaining of input string
            if (!String.IsNullOrEmpty(Tokenizer.ResultString))
            {
                resultLine += Tokenizer.ResultString;
                resultLine += '\n';
            }

            /*
            if (!Tokenizer.InputRecognized)
            {
                if (!String.IsNullOrEmpty(resultLine))
                {
                    resultLine += ". "; 
                }
                resultLine += Tokenizer.ErrorMessage + '\n'; // 
            }
            */ 
            /*
            if (!String.IsNullOrEmpty(commandLine)) 
            {
                // resultLine += "Physical quantity expected"  + '\n'; // ;
            }
            */

            Debug.Assert(Operands.Count <= 1);  // 0 or 1

            return (Operands.Count > 0) ? Operands.Pop() : null;
        }

        public static Boolean ParseQualifiedIdentifier(ref String commandLine, ref String resultLine, out String identifierName, out Quantity identifierValue)
        {
            identifierValue = null;

            IEnvironment PrimaryContext;

            commandLine = commandLine.ReadIdentifier(out identifierName);
            Debug.Assert(identifierName != null);

            INametableItem PrimaryItem;
            Boolean PrimaryIdentifierFound = IdentifierItemLookup(identifierName, out PrimaryContext, out PrimaryItem, ref resultLine);

            if (PrimaryIdentifierFound)
            {
                Boolean IdentifierFound = PrimaryIdentifierFound;
                String QualifiedIdentifierName = identifierName;
                IEnvironment QualifiedIdentifierContext = PrimaryContext;
                INametableItem IdentifierItem = PrimaryItem;

                while (IdentifierFound && !String.IsNullOrEmpty(commandLine) && commandLine[0] == '.')
                {
                    TokenString.ParseChar('.', ref commandLine, ref resultLine);
                    commandLine = commandLine.TrimStart();

                    commandLine = commandLine.ReadIdentifier(out identifierName);
                    Debug.Assert(identifierName != null);
                    commandLine = commandLine.TrimStart();

                    IEnvironment QualifiedIdentifierInnerContext = IdentifierItem as IEnvironment; // IdentifierItem.InnerContext;
                    if (QualifiedIdentifierInnerContext != null)
                    {
                        QualifiedIdentifierContext = QualifiedIdentifierInnerContext;
                    }

                    IdentifierFound = QualifiedIdentifierItemLookup(QualifiedIdentifierContext, identifierName, out IdentifierItem, ref resultLine);
                    if (IdentifierFound)
                    {
                        QualifiedIdentifierName += "." + identifierName;
                    }
                    else
                    {
                        resultLine = QualifiedIdentifierName + " don't have a field named '" + identifierName + "'";
                    }
                }

                if (IdentifierFound)
                {
                    IdentifierKind identifierkind = IdentifierItem.Identifierkind;
                    switch (identifierkind)
                    {
                        case IdentifierKind.Constant:
                        case IdentifierKind.Variable:
                            VariableGet(QualifiedIdentifierContext, identifierName, out identifierValue, ref resultLine);
                            break;
                        case IdentifierKind.Function:
                            TokenString.ParseChar('(', ref commandLine, ref resultLine);
                            commandLine = commandLine.TrimStart();

                            List<string> ExpectedFollow = new List<string>();
                            ExpectedFollow.Add(")");
                            List<Quantity> parameterlist = ParseExpressionList(ref commandLine, ref resultLine, ExpectedFollow, true);
                            Boolean OK = parameterlist != null;
                            if (OK)
                            {
                                TokenString.ParseChar(')', ref commandLine, ref resultLine);

                                FunctionGet(QualifiedIdentifierContext, identifierName, parameterlist, out identifierValue, ref resultLine);

                                commandLine = commandLine.TrimStart();
                            }
                            else
                            {
                                // Error in result line
                                Debug.Assert(!String.IsNullOrEmpty(resultLine));
                            }
                            break;
                        case IdentifierKind.Unit:
                            Unit foundUnit;
                            UnitGet(QualifiedIdentifierContext, identifierName, out foundUnit, ref resultLine);
                            identifierValue = new Quantity(foundUnit);
                            // ref resultLine
                            break;
                        case IdentifierKind.UnitSystem:
                        case IdentifierKind.Unknown:
                        case IdentifierKind.Environment:
                        default:
                            // Unexpeted identifier kind
                            Debug.Assert((identifierkind == IdentifierKind.Variable) || (identifierkind == IdentifierKind.Constant) || (identifierkind == IdentifierKind.Function) || (identifierkind == IdentifierKind.Unit));
                            break;
                    }
                }

            }
            return PrimaryIdentifierFound;  // Indicate if PrimaryIdentifier was found; even if inner identifier was not found
        }

        public static Boolean isValidDigit(Char ch, int numberBase)
        {
            return    Char.IsDigit(ch)
                   || (numberBase == 0x10) && TokenString.IsHexDigitLetter(ch);
        }

        public static Boolean ParseDouble(ref String commandLine, ref String resultLine, out Double D)
        {
            Boolean OK = false;
            D = 0.0;
            commandLine = commandLine.TrimStart();

            if (String.IsNullOrEmpty(commandLine))
            {
                // resultLine = "Double not found";
            }
            else
            {
                //NumberStyles styles = NumberStyles.Float;
                //IFormatProvider provider = NumberFormatInfo.InvariantInfo;
                // 0x010203.040506 + 0x102030.405060

                // Scan number
                int numLen = 0;
                int maxLen = commandLine.Length; // Max length of sign and digits to look for
                int numberBase = 10; // Decimal number expected
                int exponentNumberBase = 10; // Decimal exponent number expected

                int numberSignPos = -1; // No number sign found
                int hexNumberPos = -1; // No hex number prefix found
                int DecimalCharPos = -1; // No decimal char found
                int exponentCharPos = -1;  // No exponent char found
                int exponentNumberSignPos = -1; // No exponent number sign found
                int exponentHexNumberPos = -1; // No exponent hex number prefix found
                Boolean canParseMore = true;

                if ((commandLine[numLen] == '-') || (commandLine[numLen] == '+'))
                {
                    numberSignPos = numLen;
                    numLen++;
                }

                while (numLen < maxLen && Char.IsDigit(commandLine[numLen]))
                {
                    numLen++;
                }

                if (   (numLen < maxLen)
                    && (commandLine[numLen] == 'x')
                    && (numLen > 0)
                    && (commandLine[numLen-1] == '0')
                    && (   (numLen < 2)
                        || (!Char.IsDigit(commandLine[numLen-2]))))
                {
                    numLen++;
                    hexNumberPos = numLen;
                    numberBase = 0x10; // Hexadecimal number expected
                }

                while ((numLen < maxLen) && isValidDigit(commandLine[numLen], numberBase))
                {
                    numLen++;
                }

                if (   (numLen < maxLen)
                    && (   (commandLine[numLen] == '.')
                        || (commandLine[numLen] == ','))
                    )
                {
                    canParseMore = (numLen+1 < maxLen) && isValidDigit(commandLine[numLen + 1], numberBase);
                    if (canParseMore)
                    {
                        DecimalCharPos = numLen;
                        numLen++;
                    }
                }
                while (canParseMore && (numLen < maxLen) && isValidDigit(commandLine[numLen], numberBase))
                {
                    numLen++;
                }

                if (   canParseMore 
                    && (numLen < maxLen)
                    && (   (commandLine[numLen] == 'E')
                        || (commandLine[numLen] == 'e')
                        || (commandLine[numLen] == 'H')
                        || (commandLine[numLen] == 'h')))
                {
                    exponentCharPos = numLen;

                    numLen++;
                    if ((numLen < maxLen)
                        && ((commandLine[numLen] == '-')
                            || (commandLine[numLen] == '+')))
                    {
                        exponentNumberSignPos = numLen;
                        numLen++;
                    }

                    while (numLen < maxLen && Char.IsDigit(commandLine[numLen]))
                    {
                        numLen++;
                    }

                    if ((numLen < maxLen)
                        && (commandLine[numLen] == 'x')
                        && (numLen > 0)
                        && (commandLine[numLen - 1] == '0')
                        && ((numLen < 2)
                            || (!Char.IsDigit(commandLine[numLen - 2]))))
                    {
                        numLen++;
                        exponentHexNumberPos = numLen;
                        exponentNumberBase = 0x10; // Hexadecimal number expected
                    }

                    while ((numLen < maxLen) && isValidDigit(commandLine[numLen], numberBase))
                    {
                        numLen++;
                    }

                }

                if (numLen > 0)
                {
                    //System.Globalization.NumberStyles numberStyle = System.Globalization.NumberStyles.Float;
                     
                    if (numberBase == 0x10 || exponentNumberBase == 0x10)
                    {   // Hex number
                        //Double baseNumberD = 0;
                        int baseNumberLen = numLen;
                        if (exponentCharPos > 0)
                        {
                            baseNumberLen = exponentCharPos -1;
                        }
                        //OK = Double.TryParse(commandLine.Substring(0, numLen), numberStyle, NumberFormatInfo.InvariantInfo, out D); 

                        if (numberBase == 10)
                        {
                            System.Globalization.NumberStyles numberStyle = System.Globalization.NumberStyles.Float;
                            OK = Double.TryParse(commandLine.Substring(0, baseNumberLen), numberStyle, null, out D);
                            if (!OK)
                            {
                                OK = Double.TryParse(commandLine.Substring(0, baseNumberLen), numberStyle, NumberFormatInfo.InvariantInfo, out D);
                            }
                        }
                        else
                        {
                            long baseNumberL = 0;
                            int baseIntegralNumberLen = baseNumberLen - hexNumberPos;
                            if (DecimalCharPos > 0)
                            {
                                baseIntegralNumberLen = DecimalCharPos - hexNumberPos;
                            }
                            
                            System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.AllowHexSpecifier; // HexNumber
                            OK = long.TryParse(commandLine.Substring(hexNumberPos, baseIntegralNumberLen), numberstyle, NumberFormatInfo.InvariantInfo, out baseNumberL);
                            D = baseNumberL;

                            if (DecimalCharPos > 0)
                            {
                                int NoOfChars = baseNumberLen - (DecimalCharPos + 1);
                                OK = long.TryParse(commandLine.Substring(DecimalCharPos + 1, NoOfChars), numberstyle, NumberFormatInfo.InvariantInfo, out baseNumberL);
                                D = D + (baseNumberL / Math.Pow(16, NoOfChars)) ;
                                
                            }
                            
                            if (numberSignPos > 0 && commandLine[numberSignPos] == '-')
                            {
                                D = -D;
                            }
                        }
                        if (OK && exponentCharPos > 0)
                        {
                            Double exponentNumberD = 0;
                            if (numberBase == 10)
                            {
                                System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.Float;
                                OK = Double.TryParse(commandLine.Substring(baseNumberLen + 1, numLen - (baseNumberLen + 1)), numberstyle, null, out exponentNumberD);
                                if (!OK)
                                {
                                    OK = Double.TryParse(commandLine.Substring(baseNumberLen + 1, numLen - (baseNumberLen + 1)), numberstyle, NumberFormatInfo.InvariantInfo, out exponentNumberD);
                                }
                            }
                            else
                            {
                                long exponentNumber = 0;
                                System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.AllowHexSpecifier; // HexNumber
                                OK = long.TryParse(commandLine.Substring(exponentHexNumberPos, numLen - (exponentHexNumberPos-1)), numberstyle, NumberFormatInfo.InvariantInfo, out exponentNumber);
                                exponentNumberD = exponentNumber;

                                if (exponentNumberSignPos > 0 && commandLine[exponentNumberSignPos] == '-')
                                {
                                    exponentNumberD = -exponentNumberD;
                                }
                            }

                            if (OK)
                            {
                                Double Exponent;
                                if ((commandLine[exponentCharPos] == 'H') || (commandLine[exponentCharPos] == 'h'))
                                {
                                    Exponent = 0x10;
                                }
                                else
                                {
                                    Exponent = 10;
                                }

                                D = D * Math.Pow(Exponent, exponentNumberD);
                            }
                        }
                    }
                    else
                    {
                        System.Globalization.NumberStyles numberstyle = System.Globalization.NumberStyles.Float;
                        OK = Double.TryParse(commandLine.Substring(0, numLen), numberstyle, null, out D); // styles, provider
                        if (!OK)
                        {
                            OK = Double.TryParse(commandLine.Substring(0, numLen), numberstyle, NumberFormatInfo.InvariantInfo, out D); // styles, provider
                        }
                    }
                    if (OK)
                    {
                        commandLine = commandLine.Substring(numLen);
                    }
                    else
                    {
                        resultLine = commandLine.Substring(0, numLen) + " is not a valid number";
                    }
                }
            }
            return OK;
        }

        public static Unit ParsePhysicalUnit(ref String commandLine, ref String resultLine)
        {
            Unit pu = null;
            Boolean UnitIdentifierFound = false;
            String IdentifierName;
            IEnvironment Context;

            String CommandLineRest = commandLine.ReadIdentifier(out IdentifierName);

            if (IdentifierName != null)
            {
                // Check for custom defined unit
                INametableItem Item;
                UnitIdentifierFound = IdentifierItemLookup(IdentifierName, out Context, out Item, ref resultLine);
                if (UnitIdentifierFound)
                {
                    if (Item.Identifierkind == IdentifierKind.Unit)
                    {
                        commandLine = CommandLineRest;
                        pu = ((NamedUnit)Item).pu;
                    }
                    else
                    {
                        resultLine = IdentifierName + " is a " + Item.Identifierkind.ToString() + ". Expected an unit";
                    }
                }
            }

            if (pu == null)
            {   // Standard physical unit expressions

                // Parse unit
                commandLine = commandLine.TrimStart();
                if (!String.IsNullOrEmpty(commandLine) && (Char.IsLetter(commandLine[0])))
                {
                    int UnitStringLen = commandLine.IndexOfAny(new Char[] { ' ' });  // ' '
                    if (UnitStringLen < 0 )
                    {
                        UnitStringLen = commandLine.Length;
                    }
                    String UnitStr = commandLine.Substring(0, UnitStringLen);
                    String tempResultLine = null;
                    pu = Unit.ParseUnit(ref UnitStr, ref tempResultLine, false);
                    if (pu != null || String.IsNullOrEmpty(resultLine))
                    {
                        resultLine = tempResultLine;
                    }

                    int Pos = UnitStringLen - UnitStr.Length;
                    commandLine = commandLine.Substring(Pos);
                }

            }
            return pu;
        }

        #endregion Physical Expression parser methods
    }


    // Operator kinds
    // Precedence for a group of operators is same as first (lowest) enum in the group
    public enum OperatorKind
    {
        // Precedence == 0
        none = 0,

        //Precedence == 1
        parenbegin = 1,
        parenend = 2,

        //Precedence == 3
        equals = 3,
        differs = 4,

        //Precedence == 5
        lessthan = 5,
        largerthan = 6,
        lessorequals = 7,
        largerorequals = 8,

        //Precedence == 9
        not = 9,

        //Precedence == 11
        add = 11,
        sub = 12,

        //Precedence == 13
        mult = 13,
        div = 14,

        //Precedence == 15
        pow = 15,
        root = 16,

        //Precedence == 17
        unaryplus = 17,
        unaryminus = 18
    }

    public static class OperatorKindExtensions
    {

        public static OperatorKind OperatorPrecedence(OperatorKind operatoren)
        {
            switch (operatoren)
            {
                case OperatorKind.parenbegin: // "("
                case OperatorKind.parenend: // ")":
                    return OperatorKind.parenbegin; //   1;
                case OperatorKind.equals: // "=="
                case OperatorKind.differs: // "!=":
                    return OperatorKind.equals; // 3;
                case OperatorKind.lessthan: // "<"
                case OperatorKind.largerthan: // ">":
                case OperatorKind.lessorequals: // "<="
                case OperatorKind.largerorequals: // ">=":
                    return OperatorKind.lessthan; //   5;
                case OperatorKind.not: // "!"
                    return OperatorKind.not; //   9;
                case OperatorKind.add: // "+"
                case OperatorKind.sub: // "-":
                     return OperatorKind.add; //   11;
                case OperatorKind.mult: // "*":
                case OperatorKind.div: // "/":
                     return OperatorKind.mult; //   13;
                case OperatorKind.pow: // "^":
                case OperatorKind.root: // "!":
                     return OperatorKind.pow; //   15;
                case OperatorKind.unaryplus: // UnaryPlus:
                case OperatorKind.unaryminus: // UnaryMinus:
                     return OperatorKind.unaryplus; //   17;
            }

            return OperatorKind.none;
        }

        public static OperatorKind OperatorKindFromChar(Char c)
        {
            switch (c)
            {
                case '(':
                    return OperatorKind.parenbegin; //   1;
                case ')':
                    return OperatorKind.parenend; //   2;


                case '!':
                    return OperatorKind.not; // 9;
                case '<':
                    return OperatorKind.lessthan; // 5;
                case '>':
                    return OperatorKind.largerthan; // 1;

                case '+':
                    return OperatorKind.add; // 3;
                case '-':
                    return OperatorKind.sub; // 4;
                case '*':
                case '·':  // center dot  '\0x0B7' (Char)183 U+00B7
                    return OperatorKind.mult; // 5;
                case '/':
                    return OperatorKind.div; // 6;
                case '^':
                    return OperatorKind.pow; // 7;
                // case '!':
                //      return OperatorKind.Root; // 8;
                /*
                case '+': // UnaryPlus:
                     return OperatorKind.unaryplus; // 9;
                case '-': // UnaryMinus:
                     return OperatorKind.unaryminus; // 10;
                 */
            }

            return OperatorKind.none;
        }

        public static OperatorKind OperatorKindFromChar1Equal(Char c1)
        {
            switch (c1)
            {
                case '=':   // ==
                    return OperatorKind.equals;
                case '!':   // !=
                    return OperatorKind.differs;
                case '<':   // <=
                    return OperatorKind.lessorequals;
                case '>':   // >=
                    return OperatorKind.largerorequals;
            }

            return OperatorKind.none;
        }

        public static OperatorKind Precedence(this OperatorKind operatoren) => OperatorPrecedence(operatoren);
    }
}
