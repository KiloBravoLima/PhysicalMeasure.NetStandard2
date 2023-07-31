/*   http://physicalmeasure.codeplex.com                          */
/*   http://en.wikipedia.org/wiki/International_System_of_Units   */
/*   http://en.wikipedia.org/wiki/Physical_quantity               */
/*   http://en.wikipedia.org/wiki/Physical_constant               */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;

using static PhysicalMeasure.DimensionExponentsExtension;

namespace PhysicalMeasure
{
    #region Physical Measure Classes

    #region Physical Measure Exceptions

    [Serializable]
    public class PhysicalUnitFormatException : FormatException
    {
        public PhysicalUnitFormatException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PhysicalUnitFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PhysicalUnitFormatException()
            : this("The string argument is not in a valid physical unit format.")
        {
        }

        public PhysicalUnitFormatException(String message)
            : base(message)
        {
        }
    }

    [Serializable]
    public class PhysicalUnitMathException : Exception
    {
        public PhysicalUnitMathException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PhysicalUnitMathException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public PhysicalUnitMathException()
            : this("The result of the math operation on the Unit argument can't be represented by this implementation of PhysicalMeasure.")
        {
        }

        public PhysicalUnitMathException(String message)
            : base(message)
        {
        }
    }

    #endregion Physical Measure Exceptions

    #region Dimension Exponents Classes

    public class DimensionExponents : IEquatable<DimensionExponents>
    {

        private SByte[] exponents;

        public DimensionExponents(SByte[] exponents)
        {
            this.exponents = exponents;
        }

        public override int GetHashCode()
        {
            if (exponents == null)
            {
                return base.GetHashCode();
            }
            return exponents.GetHashCode();
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
                return false;

            DimensionExponents DimensionExponentsObj = obj as DimensionExponents;
            if (DimensionExponentsObj == null)
                return false;
            else
                return Equals(DimensionExponentsObj);
        }

        public Boolean Equals(DimensionExponents other)
        {
            if (other == null)
                return false;

            return Equals(this.exponents, other.exponents);
        }

    }

    public static class DimensionExponentsExtension
    {
        public static Boolean DimensionEquals(this SByte[] exponents1, SByte[] exponents2)
        {
            Debug.Assert(exponents1 != null, "Parameter must be specified");
            Debug.Assert(exponents2 != null, "Parameter must be specified");

            if (ReferenceEquals(exponents1, exponents2))
            {
                return true;
            }

            SByte MinNoOfBaseUnits = (SByte)Math.Min(exponents1.Length, exponents2.Length);
            SByte MaxNoOfBaseUnits = (SByte)Math.Max(exponents1.Length, exponents2.Length);

            Debug.Assert(MaxNoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + MaxNoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            Boolean equal = true;
            SByte i = 0;

            do
            {   // Compare exponents where defined in both arrays
                equal = exponents1[i] == exponents2[i];
                i++;
            }
            while (equal && i < MinNoOfBaseUnits);

            // Check tail of longest array to contain only zeros
            while (equal && i < MaxNoOfBaseUnits)
            {
                if (exponents1.Length > exponents2.Length)
                {
                    equal = exponents1[i] == 0;
                }
                else
                {
                    equal = exponents2[i] == 0;
                }
                i++;
            }
            return equal;
        }

        public static Boolean IsDimensionless(this SByte[] exponents)
        {
            Debug.Assert(exponents != null, "Parameter needed");

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + NoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            Boolean isDimensionless = true;
            SByte i = 0;
            do
            {
                isDimensionless = exponents[i] == 0;
                i++;
            }
            while (i < NoOfBaseUnits && isDimensionless);

            return isDimensionless;
        }

        public static SByte NoOfDimensions(this SByte[] exponents)
        {
            Debug.Assert(exponents != null, "Parameter needed");

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + NoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            SByte noOfDimensions = 0;
            SByte i = 0;
            do
            {
                if (exponents[i] != 0)
                {
                    noOfDimensions++;
                }
                i++;
            }
            while (i < NoOfBaseUnits);

            return noOfDimensions;
        }

        public delegate SByte CombineExponentsFunc(SByte e1, SByte e2);

        public static SByte SByte_Mult(SByte e1, SByte e2) => (SByte)(e1 * e2);
        public static SByte SByte_Div(SByte e1, SByte e2) => (SByte)(e1 / e2);

        public static SByte SByte_Add(SByte e1, SByte e2) => (SByte)(e1 + e2);
        public static SByte SByte_Sub(SByte e1, SByte e2) => (SByte)(e1 - e2);


        public static SByte[] CombineExponentArrays(this SByte[] exponents1, SByte[] exponents2, CombineExponentsFunc cef)
        {
            Debug.Assert(exponents1 != null, "Parameter exponents1 needed");
            Debug.Assert(exponents2 != null, "Parameter exponents2 needed");

            SByte NoOfBaseUnits1 = (SByte)exponents1.Length;
            SByte NoOfBaseUnits2 = (SByte)exponents2.Length;
            SByte MaxNoOfBaseUnits = (SByte)Math.Max(NoOfBaseUnits1, NoOfBaseUnits2);
            SByte MinNoOfBaseUnits = (SByte)Math.Min(NoOfBaseUnits1, NoOfBaseUnits2);

            Debug.Assert(NoOfBaseUnits1 <= Physics.NoOfBaseQuanties + 1, "exponents1 has too many base units:" + NoOfBaseUnits1.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");
            Debug.Assert(NoOfBaseUnits2 <= Physics.NoOfBaseQuanties + 1, "exponents2 has too many base units:" + NoOfBaseUnits2.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            SByte[] NewExponents = new SByte[MaxNoOfBaseUnits];
            SByte i = 0;

            do
            {
                NewExponents[i] = cef(exponents1[i], exponents2[i]);
                i++;
            }
            while (i < MinNoOfBaseUnits);

            while (i < MaxNoOfBaseUnits)
            {
                if (NoOfBaseUnits1 > NoOfBaseUnits2)
                {
                    NewExponents[i] = exponents1[i];
                }
                else
                {
                    NewExponents[i] = cef(0, exponents2[i]);
                }

                i++;
            }

            return NewExponents;
        }
        public static SByte[] Multiply(this SByte[] exponents1, SByte[] exponents2)
        {
            SByte[] NewExponents = CombineExponentArrays(exponents1, exponents2, SByte_Add);
            return NewExponents;
        }

        public static SByte[] Divide(this SByte[] exponents1, SByte[] exponents2)
        {
            SByte[] NewExponents = CombineExponentArrays(exponents1, exponents2, SByte_Sub);
            return NewExponents;
        }

        public static SByte[] Power(this SByte[] exponents, SByte exponent)
        {
            Debug.Assert(exponents != null, "Parameter needed");
            Debug.Assert(exponent != 0, "Parameter needed");

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + NoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            SByte[] NewExponents = new SByte[NoOfBaseUnits];
            SByte i = 0;
            do
            {
                NewExponents[i] = (SByte)(exponents[i] * exponent);

                i++;
            }
            while (i < NoOfBaseUnits);

            return NewExponents;
        }

        public static SByte[] Root(this SByte[] exponents, SByte exponent)
        {
            Debug.Assert(exponents != null, "Parameter needed");
            Debug.Assert(exponent != 0, "Parameter needed");

            SByte NoOfBaseUnits = (SByte)exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties + 1, "Too many base units:" + NoOfBaseUnits.ToString() + ". No more than " + (Physics.NoOfBaseQuanties + 1) + " expected.");

            SByte[] NewExponents = new SByte[NoOfBaseUnits];
            SByte i = 0;
            Boolean OK = true;
            do
            {
                int Remainder;
                int NewExponent = MathUtils.DivRem(exponents[i], exponent, out Remainder);
                OK = Remainder == 0;
                NewExponents[i] = (SByte)NewExponent;

                i++;
            }
            while (i < NoOfBaseUnits && OK);

            if (!OK)
            {
                Debug.Assert(OK, "Verify to not happening");

                //if (ThrowExceptionOnUnitMathError) {
                throw new PhysicalUnitMathException("The result of the math operation on the Unit argument can't be represented by this implementation of PhysicalMeasure: (" + exponents.ToString() + ").Root(" + exponent.ToString() + ")");
                //}
                //NewExponents = null;
            }
            return NewExponents;
        }


        public static SByte[] AllExponents(this SByte[] Exponents, SByte length)
        {
            SByte[] resExponents;
            if (Exponents.Length < length)
            {
                resExponents = new SByte[length];
                foreach (int i in Enumerable.Range(0, length))
                {
                    resExponents[i] = Exponents[i];
                }
            }
            else
            {
                Debug.Assert(Exponents.Length == length);
                resExponents = Exponents;
            }

            return resExponents;
        }

        public static String ArrayToString(this SByte[] exponents)
        {
            String str = "[";

            foreach (int i in Enumerable.Range(0, exponents.Length))
            {
                if (i > 0)
                {
                    str = str + ", ";
                }
                str = str + exponents[i].ToString();
            }
            str = str + "]";

            return str;
        }
    }
    #endregion Dimension Exponents Classes

    public class NamedObject : INamed
    {
        private readonly String name;
        public String Name => name;

        public NamedObject(String someName)
        {
            this.name = someName;
        }

        public override String ToString() => Name;
    }

    #region Physical Unit prefix Classes

    public class UnitPrefixExponent : IUnitPrefixExponent
    {
        private SByte exponent;

        public SByte Exponent => exponent;
        public Double Value => Math.Pow(10, exponent);

        public UnitPrefixExponent(SByte somePrefixExponent)
        {
            this.exponent = somePrefixExponent;
        }

        #region IUnitPrefixExponentMath implementation
        public IUnitPrefixExponent Multiply(IUnitPrefixExponent prefix) => new UnitPrefixExponent((SByte)(this.exponent + prefix.Exponent));

        public IUnitPrefixExponent Divide(IUnitPrefixExponent prefix) => new UnitPrefixExponent((SByte)(this.exponent - prefix.Exponent));

        public IUnitPrefixExponent Power(SByte someExponent) => new UnitPrefixExponent((SByte)(this.exponent * someExponent));

        public IUnitPrefixExponent Root(SByte someExponent)
        {
            SByte result_exponent = (SByte)(this.exponent / someExponent);
            Debug.Assert(result_exponent * someExponent == this.exponent, " Root result exponent must be an integer");
            return new UnitPrefixExponent(result_exponent);
        }

        #endregion IUnitPrefixExponentMath implementation

        public override String ToString() => Exponent.ToString();
    }

    public class UnitPrefix : NamedObject, IUnitPrefix
    {
        private IUnitPrefixTable unitPrefixTable;
        private Char prefixChar;
        IUnitPrefixExponent prefixExponent;

        #region IUnitPrefix implementation

        public Char PrefixChar => prefixChar;

        public SByte Exponent => prefixExponent.Exponent;

        public Double Value => prefixExponent.Value;

        #endregion IUnitPrefix implementation

        public UnitPrefix(IUnitPrefixTable someUnitPrefixTable, String someName, Char somePrefixChar, IUnitPrefixExponent somePrefixExponent)
            : base(someName)
        {
            this.unitPrefixTable = someUnitPrefixTable;
            this.prefixChar = somePrefixChar;
            this.prefixExponent = somePrefixExponent;
        }

        public UnitPrefix(IUnitPrefixTable someUnitPrefixTable, String someName, Char somePrefixChar, SByte somePrefixExponent)
            : this(someUnitPrefixTable, someName, somePrefixChar, new UnitPrefixExponent(somePrefixExponent))
        {
        }

        public IUnitPrefix Multiply(IUnitPrefix prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Multiply(prefix);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                unitPrefix = new UnitPrefix(null, null, '\0', resultExponent);
            }
            return unitPrefix;
        }

        public IUnitPrefix Divide(IUnitPrefix prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Divide(prefix);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                unitPrefix = new UnitPrefix(null, null, '\0', resultExponent);
            }
            return unitPrefix;
        }


        #region IUnitPrefixExponentMath implementation
        public IUnitPrefixExponent Multiply(IUnitPrefixExponent prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Multiply(prefix);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IUnitPrefixExponent Divide(IUnitPrefixExponent prefix)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Divide(prefix);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IUnitPrefixExponent Power(SByte someExponent)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Power(someExponent);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IUnitPrefixExponent Root(SByte someExponent)
        {
            IUnitPrefix unitPrefix = null;
            IUnitPrefixExponent resultExponent = this.prefixExponent.Root(someExponent);
            if (!unitPrefixTable.GetUnitPrefixFromExponent(resultExponent, out unitPrefix))
            {
                return resultExponent;
            }
            return unitPrefix;
        }

        public IPrefixedUnit Multiply(INamedSymbolUnit symbolUnit) => new PrefixedUnit(this, symbolUnit);


        #endregion IUnitPrefixExponentMath implementation

        public override String ToString() => PrefixChar.ToString();
    }

    public class UnitPrefixTable : IUnitPrefixTable
    {
        private readonly UnitPrefix[] unitPrefixes;

        public UnitPrefix[] UnitPrefixes => unitPrefixes;

        public UnitPrefixTable(UnitPrefix[] someUnitPrefix)
        {
            this.unitPrefixes = someUnitPrefix;
        }

        public Boolean GetUnitPrefixFromExponent(IUnitPrefixExponent someExponent, out IUnitPrefix unitPrefix)
        {
            Debug.Assert(someExponent.Exponent != 0);

            IUnitPrefix TempUnitPrefix;
            SByte ScaleFactorExponent;

            GetFloorUnitPrefixAndScaleFactorFromExponent(someExponent.Exponent, out TempUnitPrefix, out ScaleFactorExponent);

            if (ScaleFactorExponent == 0)
            {
                unitPrefix = TempUnitPrefix;
                return true;
            }
            else
            {
                unitPrefix = null;
                return false;
            }
        }

        public void GetFloorUnitPrefixAndScaleFactorFromExponent(SByte someExponent, out IUnitPrefix unitPrefix, out SByte ScaleFactorExponent)
        {
            Debug.Assert(someExponent != 0);

            int UnitPrefix = 11; // 10^1
            while (UnitPrefix - 1 >= 0 && UnitPrefixes[UnitPrefix - 1].Exponent <= someExponent)
            {
                UnitPrefix--;
            }
            while (UnitPrefix + 1 < UnitPrefixes.Length && UnitPrefixes[UnitPrefix + 1].Exponent >= someExponent)
            {
                UnitPrefix++;
            }
            unitPrefix = UnitPrefixes[UnitPrefix];
            ScaleFactorExponent = (SByte)(someExponent - unitPrefix.Exponent);
        }

        public Boolean GetPrefixCharFromExponent(IUnitPrefixExponent someExponent, out Char prefixChar)
        {
            prefixChar = '\0';
            foreach (UnitPrefix us in UnitPrefixes)
            {
                if (us.Exponent == someExponent.Exponent)
                {
                    prefixChar = us.PrefixChar;
                    return true;
                }
            }
            return false;
        }

        public Boolean GetUnitPrefixFromPrefixChar(Char somePrefixChar, out IUnitPrefix unitPrefix)
        {
            switch (somePrefixChar)
            {
                case '\x03BC':

                    // 'μ' // '\0x03BC' (char)956
                    // 'µ' // '\0x00B5' (char)181
                    somePrefixChar = 'µ'; // 'µ' MICRO SIGN  '\0x00B5' (char)181
                    break;
                case 'k':
                    somePrefixChar = 'K'; // Kilo
                    break;
                case 'h':
                    somePrefixChar = 'H'; // Hecto
                    break;
            }

            foreach (UnitPrefix up in UnitPrefixes)
            {
                if (up.PrefixChar == somePrefixChar)
                {
                    unitPrefix = up;
                    return true;
                }
            }
            unitPrefix = null;
            return false;
        }

        public Boolean GetExponentFromPrefixChar(Char somePrefixChar, out IUnitPrefixExponent exponent)
        {
            switch (somePrefixChar)
            {
                case '\x03BC':

                    // 'μ' // '\0x03BC' (Char)956
                    // 'µ' // '\0x00B5' (Char)181
                    somePrefixChar = 'µ'; // 'µ' MICRO SIGN  '\0x00B5' (Char)181
                    break;
                case 'k':
                    somePrefixChar = 'K'; // Kilo
                    break;
                case 'h':
                    somePrefixChar = 'H'; // Hecto
                    break;
            }

            foreach (UnitPrefix up in UnitPrefixes)
            {
                if (up.PrefixChar == somePrefixChar)
                {
                    exponent = up;
                    return true;
                }
            }
            exponent = null;
            return false;
        }


        public IUnitPrefix UnitPrefixFromPrefixChar(char somePrefixChar)
        {
            IUnitPrefix unitPrefix = null;
            GetUnitPrefixFromPrefixChar(somePrefixChar, out unitPrefix);
            return unitPrefix;
        }

        public IUnitPrefixExponent ExponentFromPrefixChar(char somePrefixChar)
        {
            IUnitPrefixExponent exponent = null;
            GetExponentFromPrefixChar(somePrefixChar, out exponent);
            return exponent;
        }

        public IUnitPrefix this[char somePrefixChar] => UnitPrefixFromPrefixChar(somePrefixChar);
    }

    #endregion Physical Unit prefix Classes

    #region Value Conversion Classes

    public abstract class ValueConversion : IValueConversion
    {
        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
        public Double Convert(Double value, Boolean backwards = false)
        {
            if (backwards)
            {
                return ConvertToPrimaryUnit(value);
            }
            else
            {
                return ConvertFromPrimaryUnit(value);
            }
        }

        public abstract Double ConvertFromPrimaryUnit(Double value);
        public abstract Double ConvertToPrimaryUnit(Double value);

        // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
        public Double Convert(Boolean backwards = false)
        {
            if (backwards)
            {
                return ConvertToPrimaryUnit();
            }
            else
            {
                return ConvertFromPrimaryUnit();
            }
        }

        public abstract Double ConvertFromPrimaryUnit();
        public abstract Double ConvertToPrimaryUnit();

        public abstract Double LinearOffset { get; }
        public abstract Double LinearScale { get; }
    }

    public class LinearValueConversion : ValueConversion
    {
        private Double offset;
        private Double scale;

        public Double Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        public Double Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public override Double LinearOffset => offset;

        public override Double LinearScale => scale;

        public LinearValueConversion(Double someOffset, Double someScale)
        {
            this.Offset = someOffset;
            this.Scale = someScale;
        }

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
        public override Double ConvertFromPrimaryUnit(Double value) => (value * this.Scale) + this.Offset;

        public override Double ConvertToPrimaryUnit(Double value)
        {
           Double convertedValue = (value - this.Offset) / this.Scale;
            return convertedValue;
        }

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
        public override Double ConvertFromPrimaryUnit() => 1.0d * this.Scale;

        public override Double ConvertToPrimaryUnit() => 1.0d / this.Scale;
    }

    public class ScaledValueConversion : LinearValueConversion
    {
        public ScaledValueConversion(Double someScale)
            : base(0, someScale)
        {
            Debug.Assert(someScale != 0, "Parameter needed");
            Debug.Assert(!Double.IsInfinity(someScale), "Finite scale value needed");

            if (someScale == 0)
            {
                throw new ArgumentException("0 is not a valid scale", nameof(someScale));
            }
            if (Double.IsInfinity(someScale))
            {
                throw new ArgumentException("Infinity is not a valid scale", nameof(someScale));
            }
        }
    }

    public class IdentityValueConversion : ScaledValueConversion
    {
        public IdentityValueConversion()
            : base(1)
        {
        }
    }

    public class CombinedValueConversion : ValueConversion
    {
        private IValueConversion firstValueConversion;
        private IValueConversion secondValueConversion;

        private Boolean firstValueConversionDirectionInverted;
        private Boolean secondValueConversionDirectionInverted;

        public override Double LinearOffset => firstValueConversion.LinearOffset + secondValueConversion.LinearOffset;

        public override Double LinearScale => firstValueConversion.LinearScale * secondValueConversion.LinearScale;


        public CombinedValueConversion(IValueConversion firstValueConversion, Boolean firstValueConversionDirectionInverted, IValueConversion secondValueConversion, Boolean secondValueConversionDirectionInverted)
        {
            this.firstValueConversion = firstValueConversion;
            this.firstValueConversionDirectionInverted = firstValueConversionDirectionInverted;
            this.secondValueConversion = secondValueConversion;
            this.secondValueConversionDirectionInverted = secondValueConversionDirectionInverted;
        }

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.

        public override Double ConvertFromPrimaryUnit(Double value) => this.secondValueConversion.Convert(this.firstValueConversion.Convert(value, this.firstValueConversionDirectionInverted), this.secondValueConversionDirectionInverted);

        public override Double ConvertToPrimaryUnit(Double value) => this.firstValueConversion.Convert(this.secondValueConversion.Convert(value, !this.secondValueConversionDirectionInverted), !this.firstValueConversionDirectionInverted);

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.

        public override Double ConvertFromPrimaryUnit() => this.secondValueConversion.Convert(this.firstValueConversion.Convert(this.firstValueConversionDirectionInverted), this.secondValueConversionDirectionInverted);

        public override Double ConvertToPrimaryUnit() => this.firstValueConversion.Convert(this.secondValueConversion.Convert(!this.secondValueConversionDirectionInverted), !this.firstValueConversionDirectionInverted);
    }

    #endregion Value Conversion Classes

    #region Physical Unit Classes

    public class NamedSymbol : NamedObject, INamedSymbol
    {
        private String symbol;
        public String Symbol { get { return symbol; } set { symbol = value; } }

        public NamedSymbol(String someName, String someSymbol)
            : base(someName)
        {
            this.Symbol = someSymbol;
        }
    }

    public abstract class Unit : ISystemItem, IUnit /* <BaseUnit | DerivedUnit | ConvertibleUnit | CombinedUnit> */
    {
        protected Unit()
        {
        }

        public static IUnit MakePhysicalUnit(SByte[] exponents, Double ConversionFactor = 1, Double ConversionOffset = 0)
        {
            return MakePhysicalUnit(Physics.SI_Units, exponents, ConversionFactor, ConversionOffset);
        }

        public static Unit MakePhysicalUnit(IUnitSystem system, SByte[] exponents, Double ConversionFactor = 1, Double ConversionOffset = 0)
        {
            Unit res_unit = null;
            int nod = exponents.NoOfDimensions();
            if (nod == 0)
            {
                res_unit = Physics.dimensionless;
            }
            else
            {
                res_unit = system.UnitFromExponents(exponents);
            }
            if (ConversionFactor != 1 || ConversionOffset != 0)
            {
                if (ConversionOffset == 0)
                {
                    res_unit = new ConvertibleUnit(null, res_unit, new ScaledValueConversion(ConversionFactor));
                }
                else
                {
                    res_unit = new ConvertibleUnit(null, res_unit, new LinearValueConversion(ConversionOffset, ConversionFactor));
                }
            }

            Debug.Assert(res_unit != null, "res_unit must be found");
            return res_unit;
        }

        public PrefixedUnitExponentList AsPrefixedUnitExponentList()
        {
            IUnitSystem us = this.ExponentsSystem;
            PrefixedUnitExponentList res = new PrefixedUnitExponentList(this.Exponents.Select((exp, i) =>
            {
                if (exp != 0)
                {
                    return new PrefixedUnitExponent(us.BaseUnits[i], exp);
                }
                else
                {
                    return null;
                }
            }));
            return res;
        }

#if DEBUG
        // [Conditional("DEBUG")]
        public void TestPropertiesPrint()
        {
            Boolean test = true;
            if (test)
            {
                string KindStr = this.Kind.ToString();
                string SimpleSystemStr = SimpleSystem?.ToString();
                string ExponentsSystemStr = ExponentsSystem?.ToString();
                string ExponentsStr = Exponents?.ArrayToString();

                string DimensionlessStr = Dimensionless?.ToPrintString();
                string IsDimensionlessStr = IsDimensionless.ToString();

                string ThisPrintStr = this.ToPrintString();
                string ThisStr = this.ToString();
            }
        }
#endif // DEBUG

        public abstract IUnitSystem SimpleSystem { get; set; }
        public abstract IUnitSystem ExponentsSystem { get; }

        public abstract UnitKind Kind { get; }
        public abstract SByte[] Exponents { get; }


        public virtual Unit Dimensionless => Physics.dimensionless;
        public virtual Boolean IsDimensionless
        {
            get
            {
                SByte[] exponents = Exponents;
                if (Exponents == null)
                {
                    Debug.Assert(Exponents != null, "Exponents must be found");
                    return false; // Maybe combined unit with Assume
                }
                return Exponents.IsDimensionless();
            }
        }

        public override int GetHashCode() => this.ExponentsSystem.GetHashCode() + this.Exponents.GetHashCode();

        #region Unit Expression parser methods
        /**
            U = U "*" F | U F | U "/" F | F .
            F = SUX | "(" U ")" .
            SUX = U | S U | U X | S U X .

            U = F Uopt .
            //Uopt = "*" F Uopt | "/" F Uopt | UX | e .
            Uopt = "*" F Uopt | "/" F Uopt| U | e .
            F = SUX | "(" U ")" .
            SUX = SU Xopt .
            SU = Sopt u .
            Sopt = s | e .
            Xopt = x | e .

         *  s : scale prefix char
         *  u : unit symbol
         *  x : exponent number
         **/

        #region IPhysicalUnit unit expression parser methods

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// [prefix] [unitSymbol]
        /// </summary>
        public static Unit Parse(String unitString)
        {
            Unit pu = null;
            String resultLine = null;
            pu = ParseUnit(ref unitString, ref resultLine, throwExceptionOnInvalidInput: true);
            return pu;
        }

        public static Unit ParseUnit(ref String unitString, ref String resultLine, Boolean throwExceptionOnInvalidInput = true)
        {
            Unit pu = null;

            Char timeSeparator = ':';
            Char[] separators = { timeSeparator };

            Char fractionUnitSeparator = '\0';
            String fractionUnitSeparatorStr = null;

            int unitStrCount = 0;
            int unitStrStartCharIndex = 0;
            int nextUnitStrStartCharIndex = 0;
            Boolean validFractionalUnit = true;
            int lastUnitFieldRemainingLen = 0;

            Stack<Tuple<string, Unit>> FractionalUnits = new Stack<Tuple<string, Unit>>();

            while (validFractionalUnit && (unitStrStartCharIndex >= 0) && (unitStrStartCharIndex < unitString.Length))
            {
                int unitStrLen;

                int unitStrSeparatorCharIndex = unitString.IndexOfAny(separators, unitStrStartCharIndex);
                if (unitStrSeparatorCharIndex == -1)
                {
                    unitStrLen = unitString.Length - unitStrStartCharIndex;

                    nextUnitStrStartCharIndex = unitString.Length;
                }
                else
                {
                    unitStrLen = unitStrSeparatorCharIndex - unitStrStartCharIndex;

                    nextUnitStrStartCharIndex = unitStrSeparatorCharIndex + 1;
                }

                if (unitStrLen > 0)
                {
                    unitStrCount++;
                    string unitFieldString = unitString.Substring(unitStrStartCharIndex, unitStrLen).Trim();

                    Unit tempPU = ParseUnit(null, ref unitFieldString);

                    if (tempPU == null)
                    {
                        validFractionalUnit = false;
                        resultLine = "'" + unitFieldString + "' is not a valid unit.";
                        if (throwExceptionOnInvalidInput)
                        {
                            throw new PhysicalUnitFormatException("The string argument unitString is not in a valid physical unit format. " + resultLine);
                        }
                    }
                    else
                    {
                        fractionUnitSeparatorStr = fractionUnitSeparator.ToString();
                        FractionalUnits.Push(new Tuple<string, Unit>(fractionUnitSeparatorStr, tempPU));

                        lastUnitFieldRemainingLen = unitFieldString.Length;
                        if (lastUnitFieldRemainingLen != 0)
                        {   // Unparsed chars in (last?) field
                            unitStrLen -= lastUnitFieldRemainingLen;
                        }
                    }
                }

                // Shift to next field
                if (unitStrSeparatorCharIndex >= 0)
                {
                    fractionUnitSeparator = unitString[unitStrSeparatorCharIndex];
                }
                unitStrStartCharIndex = nextUnitStrStartCharIndex;
            }

            unitString = unitString.Substring(nextUnitStrStartCharIndex - lastUnitFieldRemainingLen);

            foreach (Tuple<string, Unit> tempFU in FractionalUnits)
            {
                Unit tempPU = tempFU.Item2;
                String tempFractionUnitSeparator = tempFU.Item1;
                if (pu == null)
                {
                    pu = tempPU;
                    fractionUnitSeparatorStr = tempFractionUnitSeparator;
                }
                else
                {
                    if (new Quantity(tempPU).ConvertTo(pu) != null)
                    {
                        Debug.Assert(fractionUnitSeparatorStr != null, "Unit separator needed");
                        pu = new MixedUnit(tempPU, fractionUnitSeparatorStr, pu);

                        fractionUnitSeparatorStr = tempFractionUnitSeparator;
                    }
                    else
                    {
                        Debug.Assert(resultLine == null, "No resultLine expected");
                        resultLine = tempPU.ToPrintString() + " is not a valid fractional unit for " + pu.ToPrintString() + ".";

                        if (throwExceptionOnInvalidInput)
                        {
                            throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. " + resultLine);
                        }
                    }
                }
            }
            return pu;
        }

        // Token kind enum values
        public enum TokenKind
        {
            None = 0,
            Unit = 1,
            Exponent = 2,
            Operator = 3
        }

        // Operator kind enum values
        // Precedence for a group of operators is same as first (lowest) enum in the group
        public enum OperatorKind
        {
            None = 0,

            // Precedence == 2
            Mult = 2,
            Div = 3,

            //Precedence == 4
            Pow = 4,
            Root = 5
        }

        private static OperatorKind OperatorPrecedence(OperatorKind operatoren)
        {
            OperatorKind precedence = (OperatorKind)((int)operatoren & 0XE);
            return precedence;
        }

        private class Token
        {
            public readonly TokenKind TokenKind;

            public readonly Unit PhysicalUnit;
            public readonly SByte Exponent;
            public readonly OperatorKind Operator;

            public Token(Unit physicalUni)
            {
                this.TokenKind = TokenKind.Unit;
                this.PhysicalUnit = physicalUni;
            }

            public Token(SByte exponent)
            {
                this.TokenKind = TokenKind.Exponent;
                this.Exponent = exponent;
            }

            public Token(OperatorKind Operator)
            {
                this.TokenKind = TokenKind.Operator;
                this.Operator = Operator;
            }
        }

        private class ExpressionTokenizer
        {
            private String inputString;
            private int pos = 0;
            private int afterLastOperandPos = 0;
            private int lastValidPos = 0;
            private Boolean inputRecognized = true;
            private IUnit dimensionless = Physics.dimensionless;
            private Boolean throwExceptionOnInvalidInput = false;

            private Stack<OperatorKind> operators = new Stack<OperatorKind>();
            private List<Token> tokens = new List<Token>();

            private TokenKind lastReadToken = TokenKind.None;

            public ExpressionTokenizer(String inputStr)
            {
                this.inputString = inputStr;
            }

            public ExpressionTokenizer(IUnit someDimensionless, String someInputStr)
            {
                this.dimensionless = someDimensionless;
                this.inputString = someInputStr;
            }

            public ExpressionTokenizer(IUnit someDimensionless, Boolean someThrowExceptionOnInvalidInput, String someInputStr)
            {
                this.dimensionless = someDimensionless;
                this.throwExceptionOnInvalidInput = someThrowExceptionOnInvalidInput;
                this.inputString = someInputStr;
            }

            public string GetRemainingInput() => inputString.Substring(pos);

            public string GetRemainingInputForLastValidPos() => inputString.Substring(lastValidPos);

            public void SetValidPos()
            {
                if (operators.Count <= 1 && tokens.Count == 0)
                {
                    lastValidPos = afterLastOperandPos;
                }
            }

            private Boolean PushNewOperator(OperatorKind newOperator)
            {
                if (lastReadToken != TokenKind.Operator)
                {
                    if (operators.Count > 0)
                    {
                        // Pop operators with precedence higher than new operator
                        OperatorKind precedence = OperatorPrecedence(newOperator);
                        while ((operators.Count > 0) && (operators.Peek() >= precedence))
                        {
                            tokens.Add(new Token(operators.Pop()));
                        }
                    }
                    operators.Push(newOperator);
                    lastReadToken = TokenKind.Operator;

                    return true;
                }
                else
                {
                    if (throwExceptionOnInvalidInput)
                    {
                        throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid or missing unit at position " + pos.ToString());
                    }

                    return false;
                }
            }

            private void HandleNewOperator(OperatorKind newOperator)
            {   // Push newOperator and shift Pos or mark as failed
                if (PushNewOperator(newOperator))
                {
                    pos++;
                }
                else
                {
                    inputRecognized = false;
                }
            }

            private Token RemoveFirstToken()
            {   // return first operator from post fix operators
                Token token = tokens[0];
                tokens.RemoveAt(0);

                return token;
            }

            public Token GetToken()
            {
                Debug.Assert(inputString != null, "Source needed");

                if (tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }
                int OperatorsCountForRecognizedTokens = operators.Count;
                while ((inputString.Length > pos) && inputRecognized)
                {
                    Char c = inputString[pos];
                    if (Char.IsWhiteSpace(c))
                    {
                        // Ignore spaces, tabs, etc.
                        pos++;
                    }
                    else if (c == '*'
                             || c == '·') // center dot  '\0x0B7' (char)183 U+00B7
                    {
                        HandleNewOperator(OperatorKind.Mult);
                    }
                    else if (c == '/')
                    {
                        HandleNewOperator(OperatorKind.Div);
                    }
                    else if (c == '^')
                    {
                        HandleNewOperator(OperatorKind.Pow);
                    }
                    else if (c == '-'
                             || c == '+'
                             || Char.IsDigit(c))
                    {
                        // An exponent
                        if ((lastReadToken != TokenKind.Unit)                // Exponent can follow unit directly
                            && ((lastReadToken != TokenKind.Operator)          // or follow Pow operator
                                 || (operators.Peek() != OperatorKind.Pow)))
                        {
                            if (throwExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An exponent must follow a unit or Pow operator. Invalid exponent at '" + c + "' at position " + pos.ToString());
                            }
                            else
                            {
                                // return null;
                                inputRecognized = false;
                            }
                        }
                        else
                        {
                            //// Try to read an exponent from input

                            Int16 numLen = 1;

                            int maxLen = Math.Min(inputString.Length - pos, 1 + 3); // Max length of sign and digits to look for
                            while (numLen < maxLen && Char.IsDigit(inputString[pos + numLen]))
                            {
                                numLen++;
                            }

                            SByte exponent;
                            if (numLen > 0 && SByte.TryParse(inputString.Substring(pos, numLen), out exponent))
                            {
                                if ((lastReadToken == TokenKind.Operator)
                                    && (operators.Peek() == OperatorKind.Pow))
                                {
                                    // Exponent follow Pow operator;
                                    // Remove Pow operator from operator stack since it is handled as implicit in parser.
                                    operators.Pop();
                                }

                                pos += numLen;
                                afterLastOperandPos = pos;

                                lastReadToken = TokenKind.Exponent;

                                return new Token(exponent);
                            }
                            else
                            {
                                if (throwExceptionOnInvalidInput)
                                {
                                    throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid or missing exponent after '" + c + "' at position " + pos.ToString());
                                }
                                else
                                {
                                    // return null;
                                    inputRecognized = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((lastReadToken == TokenKind.Operator)    // Unit follow Pow operator;
                            && (operators.Peek() == OperatorKind.Pow))
                        {
                            if (throwExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. An unit must not follow an pow operator. Missing exponent at '" + c + "' at position " + pos.ToString());
                            }
                            else
                            {
                                inputRecognized = false;
                            }
                        }
                        else
                        {
                            // Try to read a unit from input
                            int maxLen = Math.Min(1 + 3, inputString.Length - pos); // Max length of scale and symbols to look for

                            String tempStr = inputString.Substring(pos, maxLen);
                            maxLen = tempStr.IndexOfAny(new Char[] { ' ', '*', '·', '/', '^', '+', '-', '(', ')' });  // '·'  center dot '\0x0B7' (Char)183 U+00B7
                            if (maxLen < 0)
                            {
                                maxLen = tempStr.Length;
                            }

                            for (int unitLen = maxLen; unitLen > 0; unitLen--)
                            {
                                String unitStr = tempStr.Substring(0, unitLen);
                                Unit su = Physics.UnitSystems.ScaledUnitFromSymbol(unitStr);
                                if (su != null)
                                {
                                    if (lastReadToken == TokenKind.Unit)
                                    {   // Assume implicit Mult operator
                                        PushNewOperator(OperatorKind.Mult);
                                    }

                                    pos += unitLen;
                                    afterLastOperandPos = pos;

                                    lastReadToken = TokenKind.Unit;
                                    return new Token(su);
                                }
                            }

                            if (throwExceptionOnInvalidInput)
                            {
                                throw new PhysicalUnitFormatException("The string argument is not in a valid physical unit format. Invalid unit '" + inputString.Substring(pos, maxLen) + "' at position " + pos.ToString());
                            }
                            else
                            {
                                inputRecognized = false;
                            }
                        }
                    }

                    if (tokens.Count > 0)
                    {   // return first operator from post fix operators
                        return RemoveFirstToken();
                    }
                };

                if (!inputRecognized)
                {
                    // Remove operators from stack which was pushed for not recognized input
                    while (operators.Count > OperatorsCountForRecognizedTokens)
                    {
                        operators.Pop();
                    }
                }
                //// Retrieve remaining operators from stack
                while (operators.Count > 0)
                {
                    tokens.Add(new Token(operators.Pop()));
                }

                if (tokens.Count > 0)
                {   // return first operator from post fix operators
                    return RemoveFirstToken();
                }

                return null;
            }
        }

        public static Unit ParseUnit(IUnit dimensionless, ref String s)
        {
            if (dimensionless == null)
            {
                dimensionless = Physics.dimensionless;
            }

            ExpressionTokenizer tokenizer = new ExpressionTokenizer(dimensionless, /* throwExceptionOnInvalidInput = */ false, s);

            Stack<Unit> operands = new Stack<Unit>();

            Boolean inputTokenInvalid = false;
            tokenizer.SetValidPos();
            Token token = tokenizer.GetToken();

            while (token != null && !inputTokenInvalid)
            {
                if (token.TokenKind == TokenKind.Unit)
                {
                    // Stack unit operand
                    operands.Push(token.PhysicalUnit);
                }
                else if (token.TokenKind == TokenKind.Exponent)
                {
                    Unit pu = operands.Pop();

                    // Combine pu and exponent to the new unit pu^exponent
                    operands.Push(pu.CombinePow(token.Exponent));
                }
                else if (token.TokenKind == TokenKind.Operator)
                {
                    /****
                     * Pow operator is handled implicit
                     *
                    if (token.Operator == OperatorKind.Pow)
                    {
                        Debug.Assert(operands.Count >= 1, "The operands.Count must be 1 or more");
                        SByte exponentSecond = operands.Pop();
                        IUnit puFirst = operands.Pop();
                        // Combine pu and exponent to the new unit pu^exponent
                        operands.Push(puFirst.CombinePow(exponentSecond));
                    }
                    else
                    ****/
                    if (operands.Count >= 2)
                    {
                        Debug.Assert(operands.Count >= 2, "Two operands needed");

                        Unit puSecond = operands.Pop();
                        Unit puFirst = operands.Pop();

                        if (token.Operator == OperatorKind.Mult)
                        {
                            // Combine pu1 and pu2 to the new unit pu1*pu2
                            operands.Push(puFirst.CombineMultiply(puSecond));
                        }
                        else if (token.Operator == OperatorKind.Div)
                        {
                            // Combine pu1 and pu2 to the new unit pu1/pu2
                            operands.Push(puFirst.CombineDivide(puSecond));
                        }
                    }
                    else
                    {   // Missing operand(s). Operator not valid part of (this) unit
                        inputTokenInvalid = true;
                    }
                }
                if (!inputTokenInvalid)
                {
                    if (operands.Count == 1)
                    {
                        tokenizer.SetValidPos();
                    }
                    token = tokenizer.GetToken();
                }
            }

            s = tokenizer.GetRemainingInputForLastValidPos(); // Remaining of input string

            Debug.Assert(operands.Count <= 1, "Only one operand is allowed");  // 0 or 1

            return (operands.Count > 0) ? operands.Last() : null;
        }

        #endregion IPhysicalUnit unit expression parser methods

        #endregion Unit Expression parser methods

        #region Unit print string methods
        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public abstract String PureUnitString();

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// and prefixed with '(FactorValue)' if FactorValue is not 1
        /// </summary>
        public String UnitString()
        {
            String unitStr = PureUnitString();
            if (FactorValue != 1)
            {
                unitStr = "(" + FactorValue + ") " + unitStr;
            }

            return unitStr;
        }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public virtual String UnitPrintString() => this.UnitString();

        public virtual String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            Debug.Assert(invertExponents == false, "The invertExponents must be false");
            return this.UnitString();
        }

        /// <summary>
        /// String formatted by use of named derived unit symbols when possible(without system name prefixed).
        /// without debug asserts.2
        /// </summary>
        public virtual String ReducedUnitString() => this.UnitString();

        /// <summary>
        /// IFormattable.ToString implementation.
        /// Eventually with system name prefixed.
        /// </summary>
        public override String ToString()
        {
            String unitName = this.UnitString();
            IUnitSystem system = this.ExponentsSystem; // this.SimpleSystem;
            if ((!String.IsNullOrEmpty(unitName))
                && (system != null)
                && (system != Physics.CurrentUnitSystems.Default)
                && (!system.IsIsolatedUnitSystem)
                 /*
                 && (!(    Physics.SI_Units == Physics.Default_UnitSystem
                        && system.IsCombinedUnitSystem
                        && ((ICombinedUnitSystem)system).ContainsSubUnitSystem(Physics.Default_UnitSystem) ))
                  */
                 )
            {
                unitName = system.Name + "." + unitName;
            }

            return unitName;
        }

        /// <summary>
        /// IUnit.ToPrintString implementation.
        /// With system name prefixed if system specified.
        /// </summary>
        public virtual String ToPrintString()
        {
            String unitName = this.UnitPrintString();
            if (String.IsNullOrEmpty(unitName))
            {
                unitName = "dimensionless";
            }
            else
            {
                IUnitSystem system = this.ExponentsSystem; // this.SimpleSystem;
                if ((system != null)
                    && (system != Physics.CurrentUnitSystems.Default))
                {
                    unitName = system.Name + "." + unitName;
                }
            }
            return unitName;
        }

        public virtual string ValueString(double value) => value.ToString();

        public virtual string ValueString(double value, String format, IFormatProvider formatProvider)
        {
            String valStr = null;
            try
            {
                valStr = value.ToString(format, formatProvider);
            }
            catch
            {
                valStr = value.ToString() + " ?" + format + "?";
            }
            return valStr;
        }

        public virtual string ValueString(double value, String format)
        {
            String valStr = null;
            try
            {
                valStr = value.ToString(format);
            }
            catch
            {
                valStr = value.ToString() + " ?" + format + "?";
            }
            return valStr;
        }


        public virtual string ValueAndUnitString(String valStr)
        {
            String unitStr = this.ToString();
            if (String.IsNullOrEmpty(unitStr))
            {
                return valStr;
            }
            else
            {
                return valStr + " " + unitStr;
            }
        }

        public virtual string ValueAndUnitString(double value)
        {
            String valStr = ValueString(value);
            return ValueAndUnitString(valStr);
        }

        public virtual string ValueAndUnitString(double value, String format)
        {
            String valStr = ValueString(value, format);
            return ValueAndUnitString(valStr);
        }

        public virtual string ValueAndUnitString(double value, String format, IFormatProvider formatProvider)
        {
            String valStr = ValueString(value, format, formatProvider);
            return ValueAndUnitString(valStr);
        }

        public virtual Double FactorValue => 1;

        public virtual Unit PureUnit => this;

        public virtual Unit AsNamedUnit { get { return GetAsNamedUnit(); } }
        public virtual Unit GetAsNamedUnit() => null;

        #endregion Unit print string methods

        #region Unit conversion methods

        public abstract Boolean IsLinearConvertible();


        //public static implicit operator Unit(INamedSymbolUnit namedSymbolUnit) => ((Unit)namedSymbolUnit);


        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)

        public Quantity this[Unit convertToUnit] => this.ConvertTo(convertToUnit);

        public Quantity this[Quantity convertToUnit] => this.ConvertTo(convertToUnit.Unit).Multiply(convertToUnit.Value);


        public virtual Quantity ConvertTo(Unit convertToUnit)
        {
            Debug.Assert(convertToUnit != null, "The convertToUnit must be specified");

            // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
            Quantity pq = null;
            Quantity pq_systemUnit = this.ConvertToSystemUnit();
            if (pq_systemUnit != null)
            {
                Quantity pq_toUnit = pq_systemUnit.Unit.SimpleSystem.ConvertTo(pq_systemUnit.Unit, convertToUnit);
                if (pq_toUnit != null)
                {
                    pq = pq_toUnit.Multiply(pq_systemUnit.Value);
                }
            }
            return pq;
        }

        public virtual Quantity ConvertTo(IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(convertToUnitSystem != null, "The convertToUnitSystem must be specified");

            // No Conversion value is specified. Must assume relative conversion e.g. temperature interval.
            Quantity pq = null;
            Quantity pq_systemUnit = this.ConvertToSystemUnit();
            if (pq_systemUnit != null)
            {
                pq = pq_systemUnit.Unit.SimpleSystem.ConvertTo(pq_systemUnit.Unit, convertToUnitSystem);
                if (pq != null && pq_systemUnit.Value != 1)
                {
                    pq = pq.Multiply(pq_systemUnit.Value);
                }
            }
            return pq;
        }

        public abstract Quantity ConvertToSystemUnit();


        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        public virtual Quantity ConvertTo(ref Double value, Unit convertToUnit)
        {
            Debug.Assert(convertToUnit != null, "The convertToUnit must be specified");

            // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
            Quantity pq = null;
            Quantity pq_systemUnit = this.ConvertToSystemUnit(ref value);
            if (pq_systemUnit != null)
            {
                pq = pq_systemUnit.Unit.SimpleSystem.ConvertTo(pq_systemUnit, convertToUnit);
            }
            //// Mark quantity as used now
            value = 1;
            return pq;
        }

        public virtual Quantity ConvertTo(ref Double value, IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(convertToUnitSystem != null, "The convertToUnitSystem must be specified");

            // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
            Quantity pq = null;
            Quantity pq_systemUnit = this.ConvertToSystemUnit(ref value);
            if (pq_systemUnit != null)
            {
                pq = pq_systemUnit.Unit.SimpleSystem.ConvertTo(pq_systemUnit, convertToUnitSystem);
            }
            //// Mark quantity as used now
            value = 1;
            return pq;
        }

        // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
        public abstract Quantity ConvertToSystemUnit(ref Double value);

        public abstract Quantity ConvertToBaseUnit();

        public abstract Quantity ConvertToBaseUnit(Double value);

        public virtual Quantity ConvertToBaseUnit(Quantity physicalQuantity)
        {
            Quantity pq = physicalQuantity.ConvertTo(this);
            Debug.Assert(pq != null, "The 'physicalQuantity' must be valid and convertible to this unit");
            return this.ConvertToBaseUnit(pq.Value);
        }



        /// <summary>
        ///
        /// </summary>
        public Quantity ConvertToBaseUnit(IUnitSystem convertToUnitSystem)
        {
            Quantity pq = this.ConvertTo(convertToUnitSystem);
            if (pq != null)
            {
                pq = pq.ConvertToBaseUnit();
            }
            return pq;
        }

        /// <summary>
        ///
        public Quantity ConvertToBaseUnit(double quantity, IUnitSystem convertToUnitSystem)
        {
            Quantity pq = new Quantity(quantity, this).ConvertTo(convertToUnitSystem);
            if (pq != null)
            {
                pq = pq.ConvertToBaseUnit();
            }
            return pq;
        }


        /// <summary>
        ///
        /// </summary>
        public Quantity ConvertToBaseUnit(Quantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            Quantity pq = physicalQuantity.ConvertTo(this);
            if (pq != null)
            {
                pq = pq.ConvertTo(convertToUnitSystem);

                if (pq != null)
                {
                    pq = pq.ConvertToBaseUnit();
                }
            }
            return pq;
        }

        public abstract Quantity ConvertToDerivedUnit();

        public Boolean Equivalent(Unit other, out Double quotient)
        {
            /*
             * This will not all ways be true
            Debug.Assert(other != null, "The 'other' parameter must be specified");
            */

            if ((Object)other == null)
            {
                quotient = 0;
                return false;
            }

            Quantity pq1;
            Quantity pq2;

            if (this.ExponentsSystem != other.ExponentsSystem)
            {   // Must be same unit system
                if (this.ExponentsSystem == null || other.ExponentsSystem == null)
                {
                    if (this.IsDimensionless && other.IsDimensionless)
                    {
                        // Any dimensionless can be converted to any systems dimensionless
                        quotient = other.FactorValue / this.FactorValue;
                        return true;
                    }
                }

                if (this.ExponentsSystem.IsCombinedUnitSystem || other.ExponentsSystem.IsCombinedUnitSystem)
                {
                    if (this.ExponentsSystem.IsCombinedUnitSystem && other.ExponentsSystem.IsCombinedUnitSystem)
                    {
                        // Check for same sub systems
                        CombinedUnitSystem cus_this = (CombinedUnitSystem)this.ExponentsSystem;
                        CombinedUnitSystem cus_other = (CombinedUnitSystem)other.ExponentsSystem;
                        if (cus_this.Equals(cus_other))
                        {
                            Debug.Assert(false); // Missing check for scale factors and converted units
                            if (!this.Exponents.DimensionEquals(other.Exponents))
                            {
                                quotient = 0;
                                return false;
                            }
                        }
                    }
                }

                pq2 = other.ConvertTo(this.ExponentsSystem);
                if (Object.ReferenceEquals(null, pq2))
                {
                    quotient = 0;
                    return false;
                }

                pq1 = new Quantity(1, this);
                return pq1.Equivalent(pq2, out quotient);
            }


            if (this.Kind == UnitKind.MixedUnit)
            {
                IMixedUnit this_imu = (IMixedUnit)this;
                return this_imu.MainUnit.Equivalent(other, out quotient);
            }
            else if (other.Kind == UnitKind.MixedUnit)
            {
                IMixedUnit other_imu = (IMixedUnit)other;
                return this.Equivalent(other_imu.MainUnit, out quotient);
            }
            else if (this.Kind == UnitKind.ConvertibleUnit)
            {
                IConvertibleUnit this_icu = (IConvertibleUnit)this;
                if (!this_icu.IsLinearConvertible())
                {
                    quotient = 0;
                    return false;
                }
                pq1 = this_icu.ConvertToPrimaryUnit();
                pq2 = other.ConvertTo(this_icu.PrimaryUnit);
                if (pq2 == null)
                {
                    quotient = 0;
                    return false;
                }
                Boolean equals = pq1.Equivalent(pq2, out quotient);
                return equals;
            }
            else if (other.Kind == UnitKind.ConvertibleUnit)
            {
                IConvertibleUnit other_icu = (IConvertibleUnit)other;

                if (!other_icu.IsLinearConvertible())
                {
                    quotient = 0;
                    return false;
                }
                pq2 = other_icu.ConvertToPrimaryUnit();
                pq1 = this.ConvertTo(pq2.Unit);
                if (pq1 == null)
                {
                    quotient = 0;
                    return false;
                }
                Boolean equals = pq1.Equivalent(pq2, out quotient);
                return equals;
            }
            else if (this.Kind == UnitKind.PrefixedUnit)
            {
                IPrefixedUnit this_pu = (IPrefixedUnit)this;
                Double tempQuotient;
                Boolean equivalent = this_pu.Unit.Equivalent(other, out tempQuotient);
                if (equivalent)
                {
                    quotient = this_pu.Prefix.Value * tempQuotient;
                }
                else
                {
                    quotient = 0;
                }
                return equivalent;
            }
            else if (other.Kind == UnitKind.PrefixedUnit)
            {
                IPrefixedUnit other_pu = (IPrefixedUnit)other;
                Double tempQuotient;
                Boolean equivalent = this.Equivalent((Unit)other_pu.Unit, out tempQuotient);
                if (equivalent)
                {
                    quotient = tempQuotient / other_pu.Prefix.Value;
                }
                else
                {
                    quotient = 0;
                }
                return equivalent;
            }
            else if (this.Kind == UnitKind.PrefixedUnitExponent)
            {
                IPrefixedUnitExponent this_pue = (IPrefixedUnitExponent)this;
                Double tempQuotient;
                Boolean equivalent = this_pue.Unit.Equivalent(other, out tempQuotient);
                if (equivalent)
                {
                    quotient = tempQuotient / this_pue.Prefix.Value;
                }
                else
                {
                    quotient = 0;
                }
                return equivalent;
            }
            else if (other.Kind == UnitKind.PrefixedUnitExponent)
            {
                PrefixedUnitExponent other_pue = (PrefixedUnitExponent)other;
                Double tempQuotient;
                Boolean equivalent = this.Equivalent((Unit)other_pue.Unit, out tempQuotient);
                if (equivalent)
                {
                    quotient = tempQuotient / other_pue.Prefix.Value;
                }
                else
                {
                    quotient = 0;
                }
                return equivalent;
            }
            else if (this.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit this_icu = (ICombinedUnit)this;
                if (!this_icu.IsLinearConvertible())
                {
                    quotient = 0;
                    return false;
                }
                pq2 = other.ConvertToDerivedUnit();
                IQuantity pq_this = this_icu.ConvertToDerivedUnit();
                pq1 = pq_this.ConvertTo(pq2.Unit);
                if (pq1 == null)
                {
                    quotient = 0;
                    return false;
                }
                Boolean equals = pq1.Equivalent(pq2, out quotient);
                return equals;
            }
            else if (other.Kind == UnitKind.CombinedUnit)
            {
                ICombinedUnit other_icu = (ICombinedUnit)other;
                if (!other_icu.IsLinearConvertible())
                {
                    quotient = 0;
                    return false;
                }
                pq1 = this.ConvertToDerivedUnit();
                IQuantity pq_other = other_icu.ConvertToDerivedUnit();
                pq2 = pq_other.ConvertTo(pq1.Unit);
                if (pq2 == null)
                {
                    quotient = 0;
                    return false;
                }
                Boolean equals = pq1.Equivalent(pq2, out quotient);
                return equals;
            }

            Debug.Assert(this.Kind == UnitKind.BaseUnit || this.Kind == UnitKind.DerivedUnit);

            Boolean equals2 = this.Exponents.DimensionEquals(other.Exponents);
            if (!equals2)
            {
                quotient = 0;
                return false;
            }

            quotient = other.FactorValue / this.FactorValue;
            return true;
        }

        public Boolean Equivalent(Unit other)
        {
            Double quotient;
            return Equivalent(other, out quotient);
        }

        public virtual Boolean Equals(Unit other)
        {
            /*
             * This will not all ways be true
            Debug.Assert(other != null, "The 'other' parameter must be specified");
            */

            if ((Object)other == null)
            {
                return false;
            }

            Double quotient;
            Boolean equals = this.Equivalent(other, out quotient);
            return equals && quotient == 1;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
            {
                return base.Equals(obj);
            }

            Unit pu = obj as Unit;
            if (pu != null)
            {
                return this.Equals(pu);
            }

            Debug.Assert(obj is Unit, "The 'obj' argument is not a IUnit object");

            return false;
        }


        public static Boolean operator ==(Unit unit1, Unit unit2)
        {
            if ((Object)unit1 == null)
            {
                return (Object)unit2 == null;
            }

            Debug.Assert((Object)unit1 != null, "The 'unit1' parameter must be specified");

            return unit1.Equals(unit2);
        }

        public static Boolean operator !=(Unit unit1, Unit unit2)
        {
            if ((Object)unit1 == null)
            {
                return (Object)unit2 != null;
            }

            Debug.Assert((Object)unit1 != null, "The 'unit1' parameter must be specified");

            return !unit1.Equals(unit2);
        }

        #endregion Unit conversion methods

        #region Unit static operator methods

        protected delegate Double CombineQuantitiesFunc(Double q1, Double q2);


        protected static Quantity CombineUnits(Unit u1, Unit u2, CombineExponentsFunc cef, CombineQuantitiesFunc cqf)
        {
            IUnitSystem us = u1.ExponentsSystem;
            IQuantity u1_pq = u1.ConvertToBaseUnit(us);
            IQuantity u2_pq = u2.ConvertToBaseUnit(us);

            if (u1_pq == null || u2_pq == null)
            {
                // Found no conversion from u1_pq.Unit.System to u2_pq.Unit.System
                return null;
            }

            SByte[] u1Exponents = u1_pq.Unit.Exponents;
            SByte[] u2Exponents = u2_pq.Unit.Exponents;
            SByte u1ExponentsLen = (SByte)u1_pq.Unit.Exponents.Length;
            SByte u2ExponentsLen = (SByte)u2_pq.Unit.Exponents.Length;
            int NoOfBaseUnits = Math.Max(u1ExponentsLen, u2ExponentsLen);
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties, "The 'NoOfBaseUnits' must be <= Physics.NoOfBaseQuanties");

            SByte[] combinedExponents = new SByte[NoOfBaseUnits];

            for (int i = 0; i < NoOfBaseUnits; i++)
            {
                SByte u1Exponent = 0;
                SByte u2Exponent = 0;
                if (i < u1ExponentsLen)
                {
                    u1Exponent = u1Exponents[i];
                }
                if (i < u2ExponentsLen)
                {
                    u2Exponent = u2Exponents[i];
                }
                combinedExponents[i] = cef(u1Exponent, u2Exponent);
            }
            Debug.Assert(u1.ExponentsSystem != null, "The 'u1.ExponentsSystem' must be specified");
            Unit pu = new DerivedUnit(u1.ExponentsSystem, combinedExponents);
            return new Quantity(cqf(u1_pq.Value, u2_pq.Value), pu);
        }

        protected static Unit CombineUnitExponents(Unit u, SByte exponent, CombineExponentsFunc cef)
        {
            SByte[] exponents = u.Exponents;
            int NoOfBaseUnits = exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties, "The 'NoOfBaseUnits' must be <= Physics.NoOfBaseQuanties");

            SByte[] someExponents = new SByte[NoOfBaseUnits];

            for (int i = 0; i < NoOfBaseUnits; i++)
            {
                someExponents[i] = cef(u.Exponents[i], exponent);
            }

            // Not valid during SI system initialization: Debug.Assert(u.System != null);
            Unit pu = new DerivedUnit(u.ExponentsSystem, someExponents);
            return pu;
        }

        protected static Unit AsUnitExponents(Unit u)
        {
            SByte[] exponents = u.Exponents;
            int NoOfBaseUnits = exponents.Length;
            Debug.Assert(NoOfBaseUnits <= Physics.NoOfBaseQuanties, "The 'NoOfBaseUnits' must be <= Physics.NoOfBaseQuanties");

            // Not valid during SI system initialization: Debug.Assert(u.System != null);
            Unit pu = new DerivedUnit(u.ExponentsSystem, exponents);
            return pu;
        }


        public static Unit operator *(Unit u, IUnitPrefix up)
        {
            Debug.Assert(up != null, "The " + nameof(up) + " parameter must be specified");

            // return up.Multiply(u);
            return u.Multiply(up);
        }

        public static Unit operator *(IUnitPrefix up, Unit u)
        {
            Debug.Assert(up != null, "The " + nameof(up) + " parameter must be specified");

            return u.Multiply(up);
        }

        public static Quantity operator *(Unit u, Double d) => new Quantity(d, u);

        public static Quantity operator /(Unit u, Double d) => new Quantity(1 / d, u);

        public static Quantity operator *(Double d, Unit u) => new Quantity(d, u);

        public static Quantity operator /(Double d, Unit u) => new Quantity(d, 1 / u);


        public static Unit operator *(Unit u1, Unit u2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Multiply(u2);
        }

        public static Unit operator /(Unit u1, Unit u2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Divide(u2);
        }

        public static Unit operator *(Unit u1, IUnit u2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Multiply(u2);
        }

        public static Unit operator /(Unit u1, IUnit u2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Divide(u2);
        }

        public static Unit operator *(IUnit u1, Unit u2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Multiply(u2);
        }

        public static Unit operator /(IUnit u1, Unit u2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Divide(u2);
        }

        public static Unit operator *(Unit u1, PrefixedUnitExponent pue2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Multiply((IPrefixedUnitExponent)pue2);
        }

        public static Unit operator /(Unit u1, PrefixedUnitExponent pue2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Divide((IPrefixedUnitExponent)pue2);
        }

        public static Unit operator *(Unit u1, IPrefixedUnitExponent pue2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Multiply(pue2);
        }

        public static Unit operator /(Unit u1, IPrefixedUnitExponent pue2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u1), "The " + nameof(u1) + " parameter must be specified");

            return u1.Divide(pue2);
        }

        public static Unit operator ^(Unit u, SByte exponent)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u), "The " + nameof(u) + " parameter must be specified");

            return u.Pow(exponent);
        }

        public static Unit operator %(Unit u, SByte exponent)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u), "The "+ nameof(u) +" parameter must be specified");
            return u.Rot(exponent);
        }
        /*
        public static Quantity operator ^(Unit u, SByte exponent)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u), "The " + nameof(u) + " parameter must be specified");

            return new Quantity(u.Pow(exponent));
        }

        public static Quantity operator %(Unit u, SByte exponent)
        {
            Debug.Assert(!Object.ReferenceEquals(null, u), "The " + nameof(u) + " parameter must be specified");
            return new Quantity(u.Rot(exponent));
        }
        */
        #endregion Unit static operator methods

        public virtual Quantity AsQuantity() => AsPhysicalQuantity(1);

        public virtual Quantity AsPhysicalQuantity(double quantity) => new Quantity(quantity, this);

        public virtual Unit Power(SByte exponent) => CombineUnitExponents(this, exponent, SByte_Mult);

        public virtual Unit Root(SByte exponent) => CombineUnitExponents(this, exponent, SByte_Div);

        #region Unit math methods

        public virtual Unit Multiply(IUnitPrefixExponent prefix)
        {
            Debug.Assert(prefix != null, "The " + nameof(prefix) + " parameter must be specified");
            return this.CombineMultiply(prefix);
        }

        public virtual Unit Divide(IUnitPrefixExponent prefix)
        {
            Debug.Assert(prefix != null, "The " + nameof(prefix) + " parameter must be specified");
            return this.CombineDivide(prefix);
        }

        public virtual Unit Multiply(INamedSymbolUnit namedSymbolUnit)
        {
            Debug.Assert(namedSymbolUnit != null, "The " + nameof(namedSymbolUnit) + " parameter must be specified");
            return this.CombineMultiply(namedSymbolUnit);
        }

        public virtual Unit Divide(INamedSymbolUnit namedSymbolUnit)
        {
            Debug.Assert(namedSymbolUnit != null, "The " + nameof(namedSymbolUnit) + " parameter must be specified");
            return this.CombineDivide(namedSymbolUnit);
        }

        public virtual Unit Multiply(PrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null, "The " + nameof(prefixedUnit) + " parameter must be specified");
            return this.CombineMultiply(prefixedUnit);
        }

        public virtual Unit Divide(PrefixedUnit prefixedUnit)
        {
            Debug.Assert(prefixedUnit != null, "The " + nameof(prefixedUnit) + " parameter must be specified");
            return this.CombineDivide(prefixedUnit);
        }


        public virtual Unit Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null, "The " + nameof(prefixedUnitExponent) + " parameter must be specified");
            return this.CombineMultiply(prefixedUnitExponent);
        }

        public virtual Unit Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null, "The 'prefixedUnitExponent' parameter must be specified");
            return this.CombineDivide(prefixedUnitExponent);
        }


        public virtual Unit Multiply(Unit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");
            return this.CombineMultiply(physicalUnit);
        }

        public virtual Unit Divide(Unit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");
            return this.CombineDivide(physicalUnit);
        }

        public virtual Unit Multiply(IUnit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");
            return this.CombineMultiply(physicalUnit);
        }

        public virtual Unit Divide(IUnit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");
            return this.CombineDivide(physicalUnit);
        }


        public virtual Quantity Multiply(Quantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            Unit pu = this.Multiply(physicalQuantity.Unit);
            return pu.Multiply(physicalQuantity.Value);
        }

        public virtual Quantity Divide(Quantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            Unit pu = this.Divide(physicalQuantity.Unit);
            return pu.Divide(physicalQuantity.Value);
        }

        public virtual Quantity Multiply(Double value) => new Quantity(value, this);

        public virtual Quantity Divide(Double value) => new Quantity(1 / value, this);


        public virtual Quantity Multiply(Double value, Quantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);
            Unit pu = this.Multiply(physicalQuantity.Unit);
            return pu.Multiply(value * physicalQuantity.Value);
        }

        public virtual Quantity Divide(Double value, Quantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null, "The 'physicalQuantity' parameter must be specified");
            Unit pu = this.Divide(physicalQuantity.Unit);
            return pu.Multiply(value / physicalQuantity.Value);
        }

        public Unit Pow(SByte exponent) => this.Power(exponent);

        public Unit Rot(SByte exponent) => this.Root(exponent);

        #endregion Unit math methods

        #region Unit Combine math methods

        public virtual CombinedUnit CombineMultiply(Double value)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(value);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(Double value)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(value);
            return uRes;
        }

        public virtual CombinedUnit CombineMultiply(IUnitPrefixExponent prefixExponent)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(prefixExponent);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(IUnitPrefixExponent prefixExponent)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(prefixExponent);
            return uRes;
        }


        public virtual CombinedUnit CombineMultiply(BaseUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(BaseUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineMultiply(NamedDerivedUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(NamedDerivedUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineMultiply(ConvertibleUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(ConvertibleUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(physicalUnit);
            return uRes;
        }


        public virtual CombinedUnit CombineMultiply(INamedSymbolUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(INamedSymbolUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineMultiply(Unit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(Unit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineMultiply(IUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(physicalUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(IUnit physicalUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(physicalUnit);
            return uRes;
        }


        public virtual CombinedUnit CombinePow(SByte exponent)
        {
            CombinedUnit asCombinedUnit = new CombinedUnit(this);
            CombinedUnit uRes = asCombinedUnit.CombinePow(exponent);
            return uRes;
        }

        public virtual CombinedUnit CombineRot(SByte exponent)
        {
            CombinedUnit asCombinedUnit = new CombinedUnit(this);
            CombinedUnit uRes = asCombinedUnit.CombineRot(exponent);
            return uRes;
        }

        public virtual CombinedUnit CombineMultiply(PrefixedUnit prefixedUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(prefixedUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(PrefixedUnit prefixedUnit)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(prefixedUnit);
            return uRes;
        }

        public virtual CombinedUnit CombineMultiply(PrefixedUnitExponent prefixedUnitExponent)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineMultiply(prefixedUnitExponent);
            return uRes;
        }

        public virtual CombinedUnit CombineDivide(PrefixedUnitExponent prefixedUnitExponent)
        {
            CombinedUnit uRes = new CombinedUnit(this);
            uRes = uRes.CombineDivide(prefixedUnitExponent);
            return uRes;
        }

        #endregion Unit Combine math methods

        public static implicit operator Quantity(Unit physicalUnit) => new Quantity(physicalUnit);
    }

    public abstract class SystemUnit : Unit, ISystemUnit /* <BaseUnit | DerivedUnit | ConvertibleUnit> */
    {
        private IUnitSystem system;

        public override IUnitSystem SimpleSystem { get { return system; } set { system = value; } }
        public override IUnitSystem ExponentsSystem => system;

        protected SystemUnit(IUnitSystem someSystem = null)
        {
            this.system = someSystem;
        }

        public override Quantity ConvertToSystemUnit(ref Double value) => new Quantity(value, (Unit)this);

        public override Quantity ConvertToSystemUnit() => new Quantity(1, (Unit)this);
    }

    public class BaseUnit : SystemUnit, INamedSymbol, IBaseUnit //, IPrefixedUnit
    {
        private NamedSymbol namedSymbol;

        public String Name => this.namedSymbol.Name;
        public String Symbol => this.namedSymbol.Symbol;


        private SByte baseunitnumber;
        public SByte BaseUnitNumber => baseunitnumber;

        public override UnitKind Kind => UnitKind.BaseUnit;

        public override SByte[] Exponents
        {
            get
            {
                if (baseunitnumber < 0)
                {
                    Debug.Assert(baseunitnumber >= 0);
                }

                int NoOfBaseUnits = baseunitnumber + 1;
                if (SimpleSystem != null && SimpleSystem.BaseUnits != null)
                {
                    NoOfBaseUnits = SimpleSystem.BaseUnits.Length;
                }

                SByte[] tempExponents = new SByte[NoOfBaseUnits];
                tempExponents[baseunitnumber] = 1;
                return tempExponents;
            }
        }

        public BaseUnit(IUnitSystem someUnitSystem, SByte someBaseUnitNumber, NamedSymbol someNamedSymbol)
            : base(someUnitSystem)
        {
            if (someBaseUnitNumber < 0)
            {
                Debug.Assert(someBaseUnitNumber >= 0);
            }
            this.baseunitnumber = someBaseUnitNumber;
            this.namedSymbol = someNamedSymbol;
        }

        public BaseUnit(IUnitSystem someUnitSystem, SByte someBaseUnitNumber, String someName, String someSymbol)
            : this(someUnitSystem, someBaseUnitNumber, new NamedSymbol(someName, someSymbol))
        {
        }

        public BaseUnit(SByte someBaseUnitNumber, String someName, String someSymbol)
            : this(null, someBaseUnitNumber, someName, someSymbol)
        {
        }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString() => this.Symbol;

        public override Unit GetAsNamedUnit() => this;

        /// <summary>
        ///
        /// </summary>
        public override Boolean IsLinearConvertible() => true;

        public override Quantity ConvertToBaseUnit() => new Quantity((Unit)this);

        public override Quantity ConvertToDerivedUnit() => new Quantity((Unit)this);

        public override Quantity ConvertToBaseUnit(Double value) => new Quantity(value, (Unit)this);

        public override Quantity ConvertToBaseUnit(Quantity physicalQuantity) => physicalQuantity.ConvertTo((Unit)this);
    }

    public class DerivedUnit : SystemUnit, IDerivedUnit
    {
        private readonly SByte[] exponents;

        public override UnitKind Kind => UnitKind.DerivedUnit;

        public override SByte[] Exponents => exponents;

        public DerivedUnit(IUnitSystem someSystem, SByte[] someExponents = null)
            : base(someSystem)
        {
            this.exponents = someExponents;
        }


        public DerivedUnit(SByte[] someExponents)
            : this(Physics.SI_Units, someExponents)
        {
        }


        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            Debug.Assert(this.Kind == UnitKind.DerivedUnit, "The 'this.Kind' must be UnitKind.DerivedUnit");

            String ExponentsStr = "";
#if DEBUG  // Error traces only included in debug build
            Boolean UnitIsMissingSystem = false;
#endif
            int index = 0;
            foreach (SByte exponent in Exponents)
            {
                if (exponent != 0)
                {
                    if (!String.IsNullOrEmpty(ExponentsStr))
                    {
                        ExponentsStr += '·'; // center dot '\0x0B7' (Char)183 U+00B7
                    }
                    IUnitSystem thisExponentsSystem = this.ExponentsSystem;
                    if (thisExponentsSystem != null)
                    {
                        ExponentsStr += thisExponentsSystem.BaseUnits[index].Symbol;
                    }
                    else
                    {
#if DEBUG // Error traces only included in debug build
                        UnitIsMissingSystem = true;
#endif
                        ExponentsStr += "<" + index.ToString() + ">";
                    }
                    if (exponent != 1)
                    {
                        ExponentsStr += exponent.ToString();
                    }
                }

                index++;
            }

#if DEBUG // Error traces only included in debug build
            if (UnitIsMissingSystem)
            {
                // Do some trace of error
                Debug.WriteLine(global::System.Reflection.Assembly.GetExecutingAssembly().ToString() + " Unit " + this.Kind.ToString() + " { " + ExponentsStr + "} missing unit system.");
            }
#endif

            return ExponentsStr;
        }

        public override Boolean IsLinearConvertible() => true;

        public override Quantity ConvertToBaseUnit() => new Quantity((Unit)this);

        public override Quantity ConvertToBaseUnit(Double value) => new Quantity(value, (Unit)this);

        public override Quantity ConvertToBaseUnit(Quantity physicalQuantity) => physicalQuantity.ConvertTo((Unit)this);

        public override Quantity ConvertToDerivedUnit() => new Quantity((Unit)this);

        public override Unit GetAsNamedUnit()
        {
            IUnitSystem unitSystem = SimpleSystem;
            if (unitSystem != null && !unitSystem.IsCombinedUnitSystem)
            {
                Unit namedDerivatedUnit = (Unit)unitSystem.NamedDerivedUnitFromUnit(this);
                if (namedDerivatedUnit != null)
                {
                    return namedDerivatedUnit;
                }
            }

            return this;
        }

        public override Unit Multiply(Unit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");

            if (physicalUnit.Kind != UnitKind.CombinedUnit)
            {
                if (physicalUnit.SimpleSystem != this.SimpleSystem)
                {
                    Quantity pq_pu = physicalUnit.ConvertTo(this.SimpleSystem);
                    if (pq_pu != null)
                    {
                        IQuantity pq = this.Multiply(pq_pu);
                        if (pq.Value.Equals(1.0))
                        {
                            return pq.Unit;
                        }
                    }
                }

                if (physicalUnit.SimpleSystem == this.SimpleSystem)
                {
                    if (physicalUnit.Kind == UnitKind.BaseUnit)
                    {
                        IBaseUnit bu = physicalUnit as IBaseUnit;
                        return new DerivedUnit(this.SimpleSystem, this.Exponents.Multiply(bu.Exponents));
                    }

                    if (physicalUnit.Kind == UnitKind.DerivedUnit)
                    {
                        IDerivedUnit du = physicalUnit as IDerivedUnit;
                        return new DerivedUnit(this.SimpleSystem, this.Exponents.Multiply(du.Exponents));
                    }
                }
            }

            return this.CombineMultiply(physicalUnit);
        }

        public override Unit Divide(Unit physicalUnit)
        {
            Debug.Assert(physicalUnit != null, "The 'physicalUnit' parameter must be specified");

            if (physicalUnit.Kind != UnitKind.CombinedUnit)
            {
                if (physicalUnit.SimpleSystem != this.SimpleSystem)
                {
                    Quantity pq_pu = physicalUnit.ConvertTo(this.SimpleSystem);
                    if (pq_pu != null)
                    {
                        Quantity pq = this.Divide(pq_pu);
                        if (pq.Value.Equals(1.0))
                        {
                            return pq.Unit;
                        }
                    }
                }

                if (physicalUnit.SimpleSystem == this.SimpleSystem)
                {
                    if (physicalUnit.Kind == UnitKind.BaseUnit)
                    {
                        IBaseUnit bu = physicalUnit as IBaseUnit;
                        return new DerivedUnit(this.SimpleSystem, this.Exponents.Divide(bu.Exponents));
                    }

                    if (physicalUnit.Kind == UnitKind.DerivedUnit)
                    {
                        IDerivedUnit du = physicalUnit as IDerivedUnit;
                        return new DerivedUnit(this.SimpleSystem, this.Exponents.Divide(du.Exponents));
                    }
                }
            }

            return this.CombineDivide(physicalUnit);
        }

    }

    public class NamedDerivedUnit : DerivedUnit, INamedSymbol, INamedDerivedUnit // , IPrefixedUnit
    {
        private readonly NamedSymbol namedSymbol;

        public String Name => this.namedSymbol.Name;
        public String Symbol => this.namedSymbol.Symbol;

        public NamedDerivedUnit(UnitSystem someSystem, NamedSymbol someNamedSymbol, SByte[] someExponents = null)
            : base(someSystem, someExponents)
        {
            this.namedSymbol = someNamedSymbol;
        }

        public NamedDerivedUnit(UnitSystem someSystem, String someName, String someSymbol, SByte[] someExponents = null)
            : this(someSystem, new NamedSymbol(someName, someSymbol), someExponents)
        {
        }

        public NamedDerivedUnit(UnitSystem someSystem, String someName, String someSymbol, IDerivedUnit du)
            : this(someSystem, new NamedSymbol(someName, someSymbol), du.Exponents)
        {
            Debug.Assert(du.SimpleSystem == someSystem);
        }

        public static Unit operator *(NamedDerivedUnit u, IUnitPrefix up)
        {
            Debug.Assert(up != null, "The " + nameof(up) + " parameter must be specified");

            return new PrefixedUnit(up, u);
        }

        public static Unit operator *(IUnitPrefix up, NamedDerivedUnit u)
        {
            Debug.Assert(up != null, "The " + nameof(up) + " parameter must be specified");

            return new PrefixedUnit(up, u);
        }

        /// <summary>
        /// String PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString() => ReducedUnitString();

        /// <summary>
        /// String formatted by use of named derived unit symbols when possible(without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public override String ReducedUnitString()
        {
            Debug.Assert(this.Kind == UnitKind.DerivedUnit, "The 'this.Kind' must be UnitKind.DerivedUnit");

            return Symbol;
        }


        /// <summary>
        ///
        /// </summary>
        public override Quantity ConvertToBaseUnit() => this.ConvertToBaseUnit(1);

        /// <summary>
        ///
        /// </summary>
        public override Quantity ConvertToBaseUnit(double quantity)
        {
            IUnitSystem system = this.SimpleSystem;
            Debug.Assert(system != null, "The 'System' must be valid for this unit");
            return new Quantity(quantity, new DerivedUnit(system, this.Exponents));
        }

        /// <summary>
        ///
        /// </summary>
        public override Quantity ConvertToBaseUnit(Quantity physicalQuantity)
        {
            Quantity pq = physicalQuantity.ConvertTo(this);
            Debug.Assert(pq != null, "The 'physicalQuantity' must be valid and convertible to this unit");
            return this.ConvertToBaseUnit(pq.Value);
        }

        public override Unit GetAsNamedUnit() => this;
    }

    public class ConvertibleUnit : SystemUnit, INamedSymbol, IConvertibleUnit
    {
        private readonly NamedSymbol namedSymbol;

        public String Name => this.namedSymbol.Name;
        public String Symbol => this.namedSymbol.Symbol;

        private readonly Unit primaryunit;
        private readonly IValueConversion conversion;

        public Unit PrimaryUnit => primaryunit;
        public IValueConversion Conversion => conversion;

        public ConvertibleUnit(NamedSymbol someNamedSymbol, Unit somePrimaryUnit = null, ValueConversion someConversion = null)
            : base(somePrimaryUnit != null ? somePrimaryUnit.SimpleSystem : null)
        {
            this.namedSymbol = someNamedSymbol;
            primaryunit = somePrimaryUnit;
            conversion = someConversion;

            if (this.namedSymbol == null)
            {
                String name;
                if (someConversion == null || someConversion.LinearOffset == 0)
                {
                    name = this.ConvertToPrimaryUnit().ToPrintString();
                }
                else
                {
                    name = this.primaryunit.ToPrintString();
                    if (someConversion.LinearScale != 1)
                    {
                        name = name + "/" + someConversion.LinearScale;
                    }

                    if (someConversion.LinearOffset >= 0)
                    {
                        name = name + " + " + someConversion.LinearOffset;
                    }
                    else
                    {
                        name = name + " - " + -someConversion.LinearOffset;
                    }
                }
                this.namedSymbol = new NamedSymbol(name, name);
            }

            Debug.Assert(this.namedSymbol != null, "The 'someNamedSymbol' must be valid and not null");
        }

        public ConvertibleUnit(String someName, String someSymbol, Unit somePrimaryUnit = null, ValueConversion someConversion = null)
            : this(new NamedSymbol(someName, someSymbol), somePrimaryUnit, someConversion)
        {
        }

        public override UnitKind Kind => UnitKind.ConvertibleUnit;

        public override SByte[] Exponents => PrimaryUnit.Exponents;
        public override IUnitSystem ExponentsSystem => PrimaryUnit.ExponentsSystem;
        public override IUnitSystem SimpleSystem => PrimaryUnit.SimpleSystem;

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString() => ReducedUnitString();

        /// <summary>
        /// String with formatted by use of named derived unit symbols when possible(without system name prefixed).
        /// without debug asserts.
        /// </summary>
        public override String ReducedUnitString()
        {
            Debug.Assert(this.Kind == UnitKind.ConvertibleUnit, "The 'this.Kind' must be valid and an UnitKind.ConvertibleUnit");

            return this.namedSymbol.Symbol;
        }

        public override Unit GetAsNamedUnit() => this;

        /// <summary>
        ///
        /// </summary>
        public override Boolean IsLinearConvertible()
        {
            Debug.Assert(conversion != null, "The '_conversion' must be valid and not null");
            return conversion.LinearOffset == 0;
        }

        public Quantity ConvertFromPrimaryUnit() => new Quantity(Conversion.ConvertFromPrimaryUnit(), (Unit)this);

        public Quantity ConvertToPrimaryUnit() => new Quantity(Conversion.ConvertToPrimaryUnit(), PrimaryUnit);

        public Quantity ConvertFromPrimaryUnit(Double value) => new Quantity(Conversion.ConvertFromPrimaryUnit(value), (Unit)this);

        public Quantity ConvertToPrimaryUnit(Double value)
        {
            IValueConversion temp_conversion = Conversion;
            Unit temp_primaryunit = PrimaryUnit;
            Double convertedValue = temp_conversion.ConvertToPrimaryUnit(value);
            return new Quantity(convertedValue, temp_primaryunit);
        }

        public override Quantity ConvertToSystemUnit(ref Double value)
        {
            Quantity pq = this.ConvertToPrimaryUnit(value);
            pq = pq.ConvertToSystemUnit();
            return pq;
        }


        public override Quantity ConvertToBaseUnit()
        {
            Quantity pq = this.ConvertToPrimaryUnit();
            pq = pq.Unit.ConvertToBaseUnit().Multiply(pq.Value);
            return pq;
        }

        public override Quantity ConvertToBaseUnit(double value) => PrimaryUnit.ConvertToBaseUnit(new Quantity(value, (Unit)this));

        public override Quantity ConvertToBaseUnit(Quantity physicalQuantity)
        {
            Quantity pq = physicalQuantity.ConvertTo((Unit)this);
            Debug.Assert(pq != null, "The 'physicalQuantity' must be valid and convertible to this unit");
            return PrimaryUnit.ConvertToBaseUnit(pq);
        }

        public override Quantity ConvertToDerivedUnit() => this.ConvertToPrimaryUnit().ConvertToDerivedUnit();

        public override Quantity ConvertTo(Unit convertToUnit)
        {
            if (convertToUnit == this)
            {
                return new Quantity(1, (Unit)this);
            }
            else
            {
                Quantity pq = this.ConvertToPrimaryUnit();
                if (convertToUnit == PrimaryUnit)
                {
                    return pq;
                }
                Quantity pq_toUnit = pq.Unit.ConvertTo(convertToUnit);
                if (pq_toUnit != null)
                {
                    return pq_toUnit.Multiply(pq.Value);
                }
                //// throw new ArgumentException("Physical unit is not convertible to a " + convertToUnit.ToString());
                return null;
            }
        }

        public override Unit Power(SByte exponent)
        {
            Quantity pq = this.ConvertToPrimaryUnit().ConvertToSystemUnit().Pow(exponent);
            CombinedUnit cu = new CombinedUnit(pq.Value, pq.Unit);
            return cu;
        }

        public override Unit Root(SByte exponent)
        {
            Quantity pq = this.ConvertToPrimaryUnit().ConvertToSystemUnit().Rot(exponent);
            CombinedUnit cu = new CombinedUnit(pq.Value, pq.Unit);
            return cu;
        }
    }

    #region Combined Unit Classes

    public class PrefixedUnit : Unit, IPrefixedUnit
    {
        private readonly IUnitPrefix prefix;
        private readonly INamedSymbolUnit unit;

        public IUnitPrefix Prefix => prefix;
        public INamedSymbolUnit Unit => unit;


        public override IUnitSystem SimpleSystem { get { return unit.SimpleSystem; } set { /* unit.SimpleSystem = value; */ } }
        public override IUnitSystem ExponentsSystem => unit.ExponentsSystem;

        public override UnitKind Kind => UnitKind.PrefixedUnit;

        public override SByte[] Exponents => unit.Exponents;

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString() => Prefix.PrefixChar + Unit.Symbol;

        public override Unit GetAsNamedUnit() => this;

        public PrefixedUnit(IUnitPrefix somePrefix, INamedSymbolUnit someUnit)
        {
            this.prefix = somePrefix;
            this.unit = someUnit;
        }

        public override Quantity AsPhysicalQuantity(Double value) => new Quantity(value, (Unit)this);

        public override Boolean IsLinearConvertible() => unit.IsLinearConvertible();

        public static implicit operator Quantity(PrefixedUnit prefixedUnit) => prefixedUnit.AsQuantity();

        public override Quantity ConvertToSystemUnit()
        {
            Quantity pq = unit.ConvertToSystemUnit();
            if (pq != null && prefix != null && prefix.Exponent != 0)
            {
                pq = pq.Multiply(prefix.Value);
            }
            return pq;
        }

        public override Quantity ConvertToSystemUnit(ref Double value)
        {
            // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
            Quantity pq = unit.ConvertToSystemUnit(ref value);
            if (pq != null && prefix != null && prefix.Exponent != 0)
            {
                pq = pq.Multiply(prefix.Value);
            }
            return pq;
        }

        public override Quantity ConvertToBaseUnit()
        {
            Quantity pq = unit.ConvertToBaseUnit();
            if (pq != null && prefix != null && prefix.Exponent != 0)
            {
                pq = pq.Multiply(prefix.Value);
            }
            return pq;
        }

        public override Quantity ConvertToDerivedUnit()
        {
            Quantity pq = unit.ConvertToDerivedUnit();
            if (pq != null && prefix != null && prefix.Exponent != 0)
            {
                pq = pq.Multiply(prefix.Value);
            }
            return pq;
        }

        public override Quantity ConvertToBaseUnit(Double value)
        {
            Quantity pq = this.ConvertToBaseUnit();
            pq = pq.Multiply(value);
            return pq;
        }

        public override String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String combinedUnitString = Prefix.PrefixChar + Unit.Symbol;

            return combinedUnitString;
        }

        /// <inheritdoc />
        public override Unit Power(sbyte exponent)
        {
            return new CombinedUnit(this).Power(exponent);
        }
    }

    public class PrefixedUnitExponent : Unit, IPrefixedUnitExponent
    {
        private readonly IPrefixedUnit prefixedUnit;
        private readonly SByte exponent;

        public SByte Exponent => exponent;


        public PrefixedUnitExponent(INamedSymbolUnit someUnit)
            : this(null, someUnit, 1)
        {
        }

        public PrefixedUnitExponent(INamedSymbolUnit someUnit, SByte someExponent)
            : this(null, someUnit, someExponent)
        {
        }


        public PrefixedUnitExponent(IPrefixedUnitExponent prefixedUnitExponent)
            : this(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, prefixedUnitExponent.Exponent)
        {
        }

        public PrefixedUnitExponent(IUnitPrefix somePrefix, INamedSymbolUnit someUnit, SByte someExponent)
            : this(new PrefixedUnit(somePrefix, someUnit), someExponent)
        {
        }

        public PrefixedUnitExponent(IPrefixedUnit somePrefixedUnit, SByte someExponent)
        {
            this.prefixedUnit = somePrefixedUnit;
            this.exponent = someExponent;

            Debug.Assert(someExponent != 0, "The 'exponent' must be valid and not zero");
        }

        public IUnitPrefix Prefix => prefixedUnit.Prefix;
        public INamedSymbolUnit Unit => prefixedUnit.Unit;


        public override UnitKind Kind => UnitKind.PrefixedUnitExponent;


        public override IUnitSystem SimpleSystem { get { return prefixedUnit.SimpleSystem; } set { /* prefixedUnit.SimpleSystem = value; */ } }
        public override IUnitSystem ExponentsSystem => prefixedUnit.ExponentsSystem;


        public override SByte[] Exponents
        {
            get
            {
                SByte[] exponents = prefixedUnit.Exponents;
                if (exponent != 1)
                {
                    exponents = exponents.Power(exponent);
                }
                return exponents;
            }
        }

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString()
        {
            IUnit pu = prefixedUnit;
            if (exponent != 1)
            {
                pu = pu.Pow(exponent);
            }
            String unitString = pu.PureUnitString();
            return unitString;
        }

        public override Boolean IsLinearConvertible() => prefixedUnit.IsLinearConvertible();

        public override Quantity ConvertToSystemUnit()
        {
            Quantity pq = prefixedUnit.ConvertToSystemUnit();
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            return pq;
        }

        public override Quantity ConvertToSystemUnit(ref Double value)
        {
            // Conversion value is specified. Must assume Specific conversion e.g. specific temperature.
            Quantity pq = prefixedUnit.ConvertToSystemUnit(ref value);
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            return pq;
        }

        public override Quantity ConvertToBaseUnit()
        {
            Quantity pq = prefixedUnit.ConvertToBaseUnit();
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            return pq;
        }

        public override Quantity ConvertToDerivedUnit()
        {
            Quantity pq = prefixedUnit.ConvertToDerivedUnit();
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            return pq;
        }

        public override Quantity ConvertToBaseUnit(double quantity)
        {
            Quantity pq = prefixedUnit.ConvertToBaseUnit();
            if (exponent != 1)
            {
                pq = pq.Pow(exponent);
            }
            pq = pq.Multiply(quantity);
            return pq;
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public override String ToString() => this.CombinedUnitString();

        public override String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String Str = "";
            Debug.Assert(exponent != 0, "The '_Exponent' must be valid and not zero");
            if (prefixedUnit.Unit != null)
            {
                string UnitStr = prefixedUnit.Unit.UnitString();

                if (prefixedUnit.Prefix != null && prefixedUnit.Prefix.Exponent != 0)
                {
                    Char PrefixChar = prefixedUnit.Prefix.PrefixChar;
                    UnitStr = PrefixChar + UnitStr;
                }

                SByte expo = Exponent;
                if (invertExponents)
                {
                    expo = (SByte)(-expo);
                }

                if ((UnitStr.IndexOf('·') > -1 || UnitStr.IndexOf('/') > -1 || UnitStr.IndexOf('^') > -1) && (expo != 1))
                {
                    Str = "(" + UnitStr + ")";
                }
                else
                {
                    Str = UnitStr;
                }

                if (expo != 1)
                {
                    Str += expo.ToString();
                }

            }
            else
            {
                if (prefixedUnit.Prefix != null && prefixedUnit.Prefix.Exponent != 0)
                {
                    SByte expo = Exponent;
                    if (invertExponents)
                    {
                        expo = (SByte)(-expo);
                    }

                    expo = (SByte)(prefixedUnit.Prefix.Exponent * expo);

                    Str = "10";
                    if (expo != 1)
                    {
                        Str += "^" + expo.ToString();
                    }
                }
            }

            return Str;
        }

        public override Quantity AsQuantity()
        {
            Quantity pue_pq = prefixedUnit.AsQuantity();
            if (exponent != 1)
            {
                pue_pq = pue_pq.Pow(exponent);
            }
            return pue_pq;
        }


        public PrefixedUnitExponent CombinePrefixAndExponents(SByte outerPUE_PrefixExponent, SByte outerPUE_Exponent, out SByte scaleExponent, out Double scaleFactor)
        {
            SByte combinedPrefixExponent = 0;
            if (this.Exponent == 1 || outerPUE_PrefixExponent == 0)
            {
                //
                scaleFactor = 1;
                scaleExponent = 0;
                combinedPrefixExponent = (SByte)(outerPUE_PrefixExponent + this.prefixedUnit.Prefix.Exponent);
            }
            else
            {
                int reminder;
                combinedPrefixExponent = (SByte)MathUtils.DivRem(outerPUE_PrefixExponent, this.Exponent, out reminder);
                if (reminder != 0)
                {
                    scaleFactor = Math.Pow(10, 1.0 * reminder);
                    scaleExponent = (SByte)reminder;
                }
                else
                {
                    scaleFactor = 1;
                    scaleExponent = 0;
                }
                combinedPrefixExponent += this.prefixedUnit.Prefix.Exponent;
            }

            IUnitPrefix combinedUnitPrefix;
            SByte combinedScaleFactorExponent;

            Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(combinedPrefixExponent, out combinedUnitPrefix, out combinedScaleFactorExponent);


            IUnitPrefixExponent pe = new UnitPrefixExponent((SByte)(combinedPrefixExponent + this.prefixedUnit.Prefix.Exponent));

            IUnitPrefix up = null;
            if (Physics.UnitPrefixes.GetUnitPrefixFromExponent(pe, out up))
            {
                PrefixedUnitExponent CombinedPUE = new PrefixedUnitExponent(up, this.prefixedUnit.Unit, (SByte)(this.Exponent * outerPUE_Exponent));
                return CombinedPUE;
            }
            else
            {
                // TO DO: Handle to make result as IPrefixedUnitExponent
                Debug.Assert(false);
                return null;
            }
        }

        public static implicit operator Quantity(PrefixedUnitExponent prefixedUnitExponent) => prefixedUnitExponent.AsQuantity();
    }

    public class PrefixedUnitExponentList : List<IPrefixedUnitExponent>, IPrefixedUnitExponentList
    {
        public PrefixedUnitExponentList()
        {
        }

        public PrefixedUnitExponentList(IEnumerable<IPrefixedUnitExponent> elements)
            : base(elements.Where(elm => elm != null))
        {
        }


        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String str = "";

            foreach (IPrefixedUnitExponent ue in this)
            {
                Debug.Assert(ue.Exponent != 0, "ue.Exponent must be <> 0");
                if (!String.IsNullOrEmpty(str))
                {
                    str += '·';  // center dot '\0x0B7' (Char)183 U+00B7
                }

                str += ue.CombinedUnitString(mayUseSlash, invertExponents);
            }
            return str;
        }

        public IPrefixedUnitExponentList Power(SByte exponent)
        {
            PrefixedUnitExponentList result = new PrefixedUnitExponentList();
            Double factorValue = 1.0;
            foreach (IPrefixedUnitExponent pue in this)
            {
                SByte newPrefixExponent = 0;
                SByte newExponent = (SByte)(pue.Exponent * exponent);

                if (pue.Prefix != null && pue.Prefix.Exponent != 0)
                {
                    newPrefixExponent = pue.Prefix.Exponent;
                    Debug.Assert((pue.Prefix.Exponent == 0) || (exponent == 1) || (exponent == -1), "Power: pue.Prefix.PrefixExponent must be 0. " + pue.CombinedUnitString() + "^" + exponent);
                }

                IUnitPrefix newUnitPrefix = null;
                if (newPrefixExponent != 0)
                {
                    SByte scaleFactorExponent;

                    Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(newPrefixExponent, out newUnitPrefix, out scaleFactorExponent);

                    if (scaleFactorExponent != 0)
                    {
                        factorValue *= Math.Pow(10, scaleFactorExponent * newExponent);
                    }
                }
                PrefixedUnitExponent result_pue = new PrefixedUnitExponent(newUnitPrefix, pue.Unit, newExponent);

                result.Add(result_pue);
            }
            Debug.Assert(factorValue == 1.0);
            return result;
        }

        public IPrefixedUnitExponentList Root(SByte someExponent)
        {
            PrefixedUnitExponentList result = new PrefixedUnitExponentList();
            Double factorValue = 1.0;
            foreach (IPrefixedUnitExponent pue in this)
            {
                SByte newPrefixExponent = 0;
                int remainder;
                int newExponent = MathUtils.DivRem(pue.Exponent, someExponent, out remainder);
                if (remainder != 0)
                {
                    Debug.Assert(remainder == 0);
                    return null;
                }

                if (pue.Prefix != null && pue.Prefix.Exponent != 0)
                {
                    newPrefixExponent = pue.Prefix.Exponent;
                    Debug.Assert((pue.Prefix.Exponent == 0) || (someExponent == 1) || (someExponent == -1), "Root: pue.Prefix.PrefixExponent must be 0. " + pue.CombinedUnitString() + "^(1/" + someExponent + ")");
                }

                IUnitPrefix newUnitPrefix = null;
                if (newPrefixExponent != 0)
                {
                    SByte scaleFactorExponent;
                    Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(newPrefixExponent, out newUnitPrefix, out scaleFactorExponent);
                    if (scaleFactorExponent != 0)
                    {
                        factorValue *= Math.Pow(10, scaleFactorExponent * newExponent);
                    }
                }
                PrefixedUnitExponent result_pue = new PrefixedUnitExponent(newUnitPrefix, pue.Unit, (SByte)newExponent);
                result.Add(result_pue);
            }
            Debug.Assert(factorValue == 1.0);
            return result;
        }
    }

    public class CombinedUnit : Unit, ICombinedUnit
    {
        private Double scaleFactor = 1;

        private IPrefixedUnitExponentList numerators;
        private IPrefixedUnitExponentList denominators;

        public IPrefixedUnitExponentList Numerators => numerators as IPrefixedUnitExponentList;
        public IPrefixedUnitExponentList Denominators => denominators as IPrefixedUnitExponentList;

        public CombinedUnit()
            : this(new PrefixedUnitExponentList(), new PrefixedUnitExponentList())
        {
        }

        public CombinedUnit(IPrefixedUnitExponentList someNumerators)
            : this(someNumerators, new PrefixedUnitExponentList())
        {
        }

        public CombinedUnit(IPrefixedUnitExponentList someNumerators, IPrefixedUnitExponentList someDenominators)
        {
            if ((someNumerators == null || someNumerators.Count == 0) && (someDenominators != null && someDenominators.Count > 0))
            {
                someNumerators = new PrefixedUnitExponentList(someDenominators.Select(pue => new PrefixedUnitExponent(pue.Prefix, pue.Unit, (SByte)(-pue.Exponent))));
                someDenominators = null;
            }

            this.numerators = someNumerators != null ? someNumerators : new PrefixedUnitExponentList();
            this.denominators = someDenominators != null ? someDenominators : new PrefixedUnitExponentList();
        }

        public CombinedUnit(Double someScaleFactor, IPrefixedUnitExponentList someNumerators, IPrefixedUnitExponentList someDenominators)
            : this(someNumerators, someDenominators)
        {
            this.scaleFactor = someScaleFactor;
        }

        public CombinedUnit(ICombinedUnit combinedUnit)
            : this(combinedUnit.FactorValue, combinedUnit.Numerators, combinedUnit.Denominators)
        {

        }

        public CombinedUnit(IUnitPrefix prefix, CombinedUnit combinedUnit)
        {
            IUnitPrefix somePrefix = prefix;
            Double someFactorValue = combinedUnit.FactorValue;

            Boolean found = false;
            PrefixedUnitExponentList someNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList someDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent pue in combinedUnit.Numerators)
            {
                Boolean added = false;

                IUnitPrefix tempPrefix = null;
                SByte ScaleFactorExponent = 0;
                Double somePrefixExponentDiff = 0;

                if (!found )
                {
                    if (someFactorValue != 1)
                    {
                        // Try to combine specified prefix with combinedUnit valueFactor to a prefix
                        Double tempSomeFactorValue = Math.Pow(10, somePrefix.Exponent) * someFactorValue;
                        Double somePrefixExponentD = Math.Log10(tempSomeFactorValue);
                        SByte somePrefixExponent = (SByte)Math.Ceiling(somePrefixExponentD);
                        somePrefixExponentDiff = somePrefixExponentD - somePrefixExponent;

                        Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(somePrefixExponent, out tempPrefix, out ScaleFactorExponent);
                    }
                    else
                    {
                        // Just use specified prefix
                        tempPrefix = somePrefix;
                    }
                }

                if (!found && tempPrefix != null)
                {
                    IPrefixedUnitExponent tempPue = new PrefixedUnitExponent(tempPrefix, pue.Unit, pue.Exponent);
                    someNumerators.Add(tempPue);
                    somePrefix = null;
                    if (ScaleFactorExponent != 0)
                    {
                        someFactorValue = Math.Pow(10, ScaleFactorExponent + somePrefixExponentDiff);
                    }

                    found = true;
                    added = true;
                }

                if (!added)
                {
                    someNumerators.Add(pue);
                }
            }

            foreach (IPrefixedUnitExponent pue in combinedUnit.Denominators)
            {
                someDenominators.Add(pue);
            }

            this.scaleFactor = someFactorValue;
            this.numerators = someNumerators;
            this.denominators = someDenominators;
        }

        public CombinedUnit(IPrefixedUnitExponent prefixedUnitExponent)
            : this(new PrefixedUnitExponentList(), new PrefixedUnitExponentList())
        {
            this.numerators.Add(prefixedUnitExponent);
        }

        public CombinedUnit(IUnit physicalUnit)
        {
            ICombinedUnit cu = null;

            INamedSymbolUnit nsu = physicalUnit as INamedSymbolUnit;
            if (nsu != null)
            {
                cu = new CombinedUnit(nsu);
            }
            else
            {
                switch (physicalUnit.Kind)
                {
                    case UnitKind.PrefixedUnit:
                        IPrefixedUnit pu = physicalUnit as IPrefixedUnit;
                        Debug.Assert(pu != null);
                        cu = new CombinedUnit(pu);
                        break;
                    case UnitKind.PrefixedUnitExponent:
                        IPrefixedUnitExponent pue = physicalUnit as IPrefixedUnitExponent;
                        Debug.Assert(pue != null);
                        cu = new CombinedUnit(pue);
                        break;
                    case UnitKind.DerivedUnit:
                        IDerivedUnit du = physicalUnit as IDerivedUnit;
                        Debug.Assert(du != null);
                        cu = new CombinedUnit(du);
                        break;
                    case UnitKind.MixedUnit:
                        IMixedUnit mu = physicalUnit as IMixedUnit;
                        Debug.Assert(mu != null);
                        cu = new CombinedUnit(mu.MainUnit);
                        break;
                    case UnitKind.CombinedUnit:
                        ICombinedUnit cu2 = physicalUnit as ICombinedUnit;
                        Debug.Assert(cu2 != null);
                        cu = new CombinedUnit(cu2);
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }
            }

            if (cu != null)
            {
                this.scaleFactor = cu.FactorValue;
                this.numerators = cu.Numerators;
                this.denominators = cu.Denominators;
            }
            else
            {
                // TO DO: Convert physicalUnit to CombinedUnit
                Debug.Assert(false);
            }
        }

        public CombinedUnit(Double someScaleFactor, IUnit physicalUnit)
            : this(physicalUnit)
        {
            this.scaleFactor *= someScaleFactor;
        }

        public CombinedUnit(INamedSymbolUnit namedSymbolUnit)
            : this(new PrefixedUnitExponent(null, namedSymbolUnit, 1))
        {
        }

        public CombinedUnit(IPrefixedUnit prefixedUnit)
            : this(new PrefixedUnitExponent(prefixedUnit.Prefix, prefixedUnit.Unit, 1))
        {
        }


        public CombinedUnit(IDerivedUnit derivedUnit)
        {
            IPrefixedUnitExponentList someNumerators = new PrefixedUnitExponentList();
            IPrefixedUnitExponentList someDenominators = new PrefixedUnitExponentList();

            IUnitSystem system = derivedUnit.ExponentsSystem;

            int length = derivedUnit.Exponents.Length;
            foreach (Byte i in Enumerable.Range(0, length))
            {
                SByte exp = derivedUnit.Exponents[i];
                if (exp != 0)
                {
                    if (exp > 0)
                    {
                        someNumerators.Add(new PrefixedUnitExponent(null, system.BaseUnits[i], exp));
                    }
                    else
                    {
                        someDenominators.Add(new PrefixedUnitExponent(null, system.BaseUnits[i], (sbyte)(-exp)));
                    }

                }
            }

            this.scaleFactor = derivedUnit.FactorValue;
            this.numerators = someNumerators;
            this.denominators = someDenominators;
        }

        public override UnitKind Kind => UnitKind.CombinedUnit;

        public override IUnitSystem SimpleSystem
        {
            get
            {
                IUnitSystem system = null; // No unit system
                foreach (IPrefixedUnitExponent pue in Numerators.Union(Denominators))
                {
                    if (pue.Unit == null)
                    {
                        // This could be at prefix only
                        // Just ignore this pue; it can't affect unit system info
                        // return null;
                    }
                    else
                    {
                        IUnitSystem subsystem = pue.Unit.SimpleSystem;

                        if (system == null)
                        {
                            system = subsystem;
                        }
                        else
                        {
                            if (system != subsystem)
                            {
                                // Multiple unit systems
                                return null;
                            }
                        }
                    }
                }

                return system;  // The one and only system for all sub units
            }
            set {  /* Just do nothing */ throw new NotImplementedException(); }
        }

        public override IUnitSystem ExponentsSystem
        {
            get
            {
                IUnitSystem system = null; // No unit system
                List<IUnitSystem> subUnitSystems = null;
                foreach (IPrefixedUnitExponent pue in Numerators.Union(Denominators))
                {
                    if (pue.Unit != null)
                    {
                        if (system == null)
                        {
                            system = pue.Unit.ExponentsSystem;
                        }
                        else
                        {
                            IUnitSystem pue_system = pue.Unit.ExponentsSystem; // pue.Unit.SomeSimpleSystem;
                            if (system != pue_system
                                && ((!system.IsCombinedUnitSystem && !pue_system.IsCombinedUnitSystem)
                                    || (system.IsCombinedUnitSystem
                                        && !pue_system.IsCombinedUnitSystem
                                        && !((CombinedUnitSystem)system).ContainsSubUnitSystem(pue_system))
                                    /*
                                        We must still include pue_system sub unit systems
                                        || (   !system.IsCombinedUnitSystem
                                            && pue_system.IsCombinedUnitSystem
                                            && !((CombinedUnitSystem)pue_system).ContainsSubUnitSystem(system))
                                    */
                                    || (system.IsCombinedUnitSystem
                                        && pue_system.IsCombinedUnitSystem
                                        && !((CombinedUnitSystem)system).ContainsSubUnitSystems(((CombinedUnitSystem)pue_system).UnitSystemes))
                                   )
                                )
                            {
                                // Multiple unit systems and some could be an isolated unit system
                                if (subUnitSystems == null)
                                {   // First time we have found a second system. Add system as first system in list of systems
                                    subUnitSystems = new List<IUnitSystem>();
                                    if (!system.IsCombinedUnitSystem)
                                    {
                                        subUnitSystems.Add(system);
                                    }
                                    else
                                    {
                                        CombinedUnitSystem cus = (CombinedUnitSystem)system;
                                        subUnitSystems.AddRange(cus.UnitSystemes);
                                    }
                                }

                                {   // Add pue_system to list of systems
                                    if (!pue_system.IsCombinedUnitSystem)
                                    {
                                        subUnitSystems.Add(pue_system);
                                    }
                                    else
                                    {
                                        CombinedUnitSystem cus = (CombinedUnitSystem)pue_system;
                                        subUnitSystems.AddRange(cus.UnitSystemes);
                                    }
                                }
                            }
                        }
                    }
                }

                if (subUnitSystems != null)
                {
                    if (subUnitSystems.Any(us => us.IsIsolatedUnitSystem))
                    {   // Must combine the unit systems into one unit system
                        system = CombinedUnitSystem.GetCombinedUnitSystem(subUnitSystems.Distinct().ToArray());
                    }
                    else
                    {
                        IUnitSystem DefaultUnitSystem = Physics.CurrentUnitSystems.Default;
                        if (subUnitSystems.Contains(DefaultUnitSystem))
                        {
                            system = DefaultUnitSystem;
                        }
                        else
                        {
                            // system = SubUnitSystems.First(us => !us.IsIsolatedUnitSystem);
                            system = subUnitSystems.First();
                        }
                    }
                }
                if (system == null)
                {
                    system = Physics.CurrentUnitSystems.Default;
                }

                Debug.Assert(system != null, "CombinedUnit.ExponentsSystem is null");
                return system;
            }
        }


        public IUnitSystem SomeSimpleSystem
        {
            get
            {
                foreach (IPrefixedUnitExponent pue in Numerators.Union(Denominators))
                {
                    if (pue.Unit != null)
                    {
                        IUnit pu = pue.Unit;
                        IUnitSystem somesystem = pu.SimpleSystem;
                        if (somesystem != null)
                        {
                            return somesystem;
                        }
                        else if (pu.Kind == UnitKind.CombinedUnit)
                        {
                            ICombinedUnit cu = (ICombinedUnit)pu;
                            somesystem = cu.SomeSimpleSystem;
                            if (somesystem != null)
                            {
                                return somesystem;
                            }
                        }
                    }

                }

                return null;
            }
        }

        public override SByte[] Exponents
        {
            get
            {
                SByte[] exponents = null;
                int elementCount = Numerators.Count + Denominators.Count;
                if (elementCount == 0)
                {   // No exponents at all; return array of zeros
                    exponents = new SByte[1];
                    exponents[0] = 0;
                }
                else
                {
                    IUnitSystem anySystem = this.ExponentsSystem;
                    if (anySystem == null)
                    {
                        Debug.WriteLine("CombinedUnit.Exponents() missing ExponentsSystem");
                        Debug.Assert(anySystem != null, "CombinedUnit.Exponents() missing ExponentsSystem");
                        anySystem = this.SomeSimpleSystem;
                        if (anySystem == null)
                        {
                            Debug.WriteLine("CombinedUnit.Exponents() missing also SomeSystem");
                            Debug.Assert(anySystem != null, "CombinedUnit.Exponents() missing also SomeSystem");

                            anySystem = Physics.SI_Units;
                        }
                    }
                    if (anySystem != null)
                    {
                        IQuantity baseUnit_pq = null;
                        IUnit baseUnit_pu = null;

                        try
                        {
                            baseUnit_pq = this.ConvertToDerivedUnit();
                            if (baseUnit_pq != null)
                            {
                                baseUnit_pu = baseUnit_pq.Unit;
                                if (baseUnit_pu != null)
                                {
                                    IUnitSystem system = this.ExponentsSystem;
                                    Debug.Assert(system != null);

                                    if (system.IsCombinedUnitSystem)
                                    {
                                        ICombinedUnitSystem cus = system as ICombinedUnitSystem;

                                        exponents = cus.UnitExponents(this);
                                    }
                                    else
                                    {
                                        UnitKind uk = baseUnit_pu.Kind;
                                        exponents = baseUnit_pu.Exponents;
                                    }
                                }
                            }
                            Debug.Assert(exponents != null, "CombinedUnit.ConvertToDerivedUnit() are missing base unit and exponents");
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("CombinedUnit.ConvertToBaseUnit() failed and unit are missing exponents: " + e.Message);
                            Debug.Assert(false, "CombinedUnit.ConvertToBaseUnit() failed and unit are missing exponents: " + e.Message);
                        }
                    }
                    else
                    {
                        Debug.WriteLine("CombinedUnit.ConvertToBaseUnit() missing exponents");
                        Debug.Assert(false, "CombinedUnit.ConvertToBaseUnit() missing exponents");
                    }
                }
                return exponents;
            }
        }

        public CombinedUnit OnlySingleSystemUnits(IUnitSystem us)
        {
            PrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
            PrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                if (((pue.Unit != null) && (pue.Unit.SimpleSystem == us))
                    || ((pue.Unit == null) && (us == null)))
                {
                    // pue has the specified system; Include in result
                    tempNumerators.Add(pue);
                }
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                if (((pue.Unit != null) && (pue.Unit.SimpleSystem == us))
                    || ((pue.Unit == null) && (us == null)))
                {
                    // pue has the specified system; Include in result
                    tempDenominators.Add(pue);
                }
            }
            CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
            return cu;
        }


        public override Double FactorValue => scaleFactor;

        public override Unit PureUnit
        {
            get
            {
                Unit pureunit = this;
                if (scaleFactor != 0)
                {
                    pureunit = new CombinedUnit(Numerators, Denominators);
                }
                return pureunit;
            }
        }
        public override Unit GetAsNamedUnit()
        {
            Quantity pq = this.ConvertToDerivedUnit();
            Quantity pqNamedUnit = pq.AsNamedUnit;

            Unit namedUnit = Quantity.GetAsNamedUnit(pqNamedUnit);
            if (namedUnit != null)
            {
                // return namedUnit as INamedSymbolUnit or PrefixedUnit;
                Debug.Assert(namedUnit is BaseUnit || namedUnit is NamedDerivedUnit || namedUnit is ConvertibleUnit || namedUnit is PrefixedUnit);
                return namedUnit;
            }
            return this;
        }


        /// <summary>
        ///
        /// </summary>
        public override Boolean IsLinearConvertible()
        {
            if (Numerators.Count == 1 && Denominators.Count == 0)
            {
                IPrefixedUnitExponent pue = Numerators[0];
                if (pue.Exponent == 1)
                {
                    IUnit unit = pue.Unit;
                    if (unit != null)
                    {
                        return unit.IsLinearConvertible();
                    }
                }
            }
            return true;
        }

        // Relative conversion

        public override Quantity ConvertToSystemUnit()
        {
            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
                system = this.SomeSimpleSystem;
                if (system == null)
                {
                    system = this.ExponentsSystem;
                }
                Debug.Assert(system != null);
                Quantity pq = this.ConvertTo(system);
                if (pq == null)
                {
                    //Debug.Assert(pq == null || pq.Unit.SimpleSystem == system);
                    //Debug.Assert(pq != null);
                }
                return pq;
            }
            return new Quantity(1, (Unit)this);
        }

        // Absolute conversion
        public override Quantity ConvertToSystemUnit(ref Double value)
        {
            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
                system = Physics.CurrentUnitSystems.Default;
            }
            Debug.Assert(system != null);
            Quantity pq = this.ConvertTo(ref value, system);
            Debug.Assert(pq == null || pq.Unit.SimpleSystem != null);
            return pq;
        }


        public override Quantity ConvertToBaseUnit()
        {

            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
                system = this.ExponentsSystem;
                Debug.Assert(system != null);

                if (system.IsCombinedUnitSystem)
                {
                    ICombinedUnitSystem cus = system as ICombinedUnitSystem;

                    return cus.ConvertToBaseUnit(this);
                }

                // This happens for combined unit with sub units of different but convertible systems Debug.Assert(false);

                // Combined unit with sub units of different but convertible systems
                Quantity pq = this.ConvertToSystemUnit();

                if (pq != null)
                {
                    pq = pq.ConvertToBaseUnit();
                }
                return pq;
            }

            Debug.Assert(system != null);

            Double value = scaleFactor;
            Unit unit = null;

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                Quantity pq_baseunit = pue.ConvertToBaseUnit();
                value *= pq_baseunit.Value;
                Unit baseunit = pq_baseunit.Unit;
                if (unit == null)
                {
                    unit = baseunit;
                }
                else
                {
                    unit = unit.Multiply(baseunit);
                }
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                Quantity pq_baseunit = pue.ConvertToBaseUnit();
                value *= pq_baseunit.Value;
                Unit baseunit = pq_baseunit.Unit;
                if (unit == null)
                {
                    unit = baseunit.CombinePow(-1);
                }
                else
                {
                    unit = unit.Divide(baseunit);
                }
            }

            return new Quantity(value, unit);
        }

        public override Quantity ConvertToBaseUnit(double quantity)
        {
            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
               Double ScaledQuantity = scaleFactor * quantity;
                Quantity pq1 = this.ConvertToSystemUnit(ref ScaledQuantity).ConvertToBaseUnit();
                Debug.Assert(ScaledQuantity == 1.0);
                return pq1;
            }
            Debug.Assert(system != null);

            Quantity pq = new Quantity(quantity);

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                Quantity pue_pq = pue.ConvertToBaseUnit();
                pq = pq.Multiply(pue_pq);
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                Quantity pue_pq = pue.ConvertToBaseUnit();
                pq = pq.Divide(pue_pq);
            }

            return pq;
        }


        public override Quantity ConvertToDerivedUnit()
        {
            IUnitSystem system = this.SimpleSystem;
            if (system == null)
            {
                Double scaledQuantity = scaleFactor;
                Quantity pq1 = this.ConvertToSystemUnit();

                if (pq1 != null)
                {
                    // Simple system DerivedUnit
                    // Debug.Assert(scaledQuantity == 1.0);
                    Debug.Assert(pq1.Unit.SimpleSystem != null);
                }
                else
                {
                    // Combined system DerivedUnit
                    system = this.ExponentsSystem;
                    Debug.Assert(system != null && system.IsCombinedUnitSystem);

                    pq1 = this.ConvertToBaseUnit();
                }

                pq1 = pq1.ConvertToDerivedUnit();
                return pq1;
            }

            Debug.Assert(system != null);

            Quantity pq = new Quantity(scaleFactor, system.Dimensionless);

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                Quantity pue_pq = pue.ConvertToDerivedUnit();
                pq = pq.Multiply(pue_pq);
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                Quantity pue_pq = pue.ConvertToDerivedUnit();
                pq = pq.Divide(pue_pq);
            }

            return pq;
        }

        public override Quantity ConvertTo(Unit convertToUnit)
        {
            Debug.Assert(convertToUnit != null);

            IUnitSystem system = this.SimpleSystem;
            IUnitSystem convertToSystem = convertToUnit.SimpleSystem;
            if (system == null || system != convertToSystem)
            {
                if (convertToSystem == null)
                {
                    if (convertToUnit.Kind == UnitKind.CombinedUnit)
                    {
                        ICombinedUnit cu = convertToUnit as ICombinedUnit;
                        convertToSystem = cu.ExponentsSystem;
                    }
                }

                if (convertToSystem != null)
                {
                    IQuantity this_as_ToSystemUnit = this.ConvertTo(convertToSystem);
                    if (this_as_ToSystemUnit != null)
                    {
                        if (this_as_ToSystemUnit.Unit != null
                            && this_as_ToSystemUnit.Unit.ExponentsSystem != convertToSystem
                            && (!convertToSystem.IsCombinedUnitSystem || !((ICombinedUnitSystem)convertToSystem).ContainsSubUnitSystem(this_as_ToSystemUnit.Unit.ExponentsSystem)))
                        {
                            Debug.Assert(this_as_ToSystemUnit.Unit == null || this_as_ToSystemUnit.Unit.ExponentsSystem == convertToSystem || (convertToSystem.IsCombinedUnitSystem && ((ICombinedUnitSystem)convertToSystem).ContainsSubUnitSystem(this_as_ToSystemUnit.Unit.ExponentsSystem)), "PRE this_as_ToSystemUnit.Unit.ExponentsSystem != convertToSystem");
                        }
                        Debug.Assert(this_as_ToSystemUnit.Unit == null || this_as_ToSystemUnit.Unit.ExponentsSystem == convertToSystem || (convertToSystem.IsCombinedUnitSystem && ((ICombinedUnitSystem)convertToSystem).ContainsSubUnitSystem(this_as_ToSystemUnit.Unit.ExponentsSystem)), "this_as_ToSystemUnit.Unit.ExponentsSystem != convertToSystem");
                        return this_as_ToSystemUnit.ConvertTo(convertToUnit);
                    }
                    return null;
                }
                Debug.Assert(false);
                return null;
            }
            Debug.Assert(system != null && system == convertToSystem);

            Quantity pq_tounit = null;
            Quantity pq_baseunit = null;

            if (convertToUnit.Kind == UnitKind.CombinedUnit)
            {
                CombinedUnit convertToUnit_cu = convertToUnit as CombinedUnit;
                CombinedUnit relativeUnit_cu = this.CombineDivide(convertToUnit_cu);
                Quantity pq = relativeUnit_cu.ConvertToDerivedUnit();
                if (pq.IsDimensionless)
                {
                    return new Quantity(pq.Value, convertToUnit);
                }
                return null;
            }
            else
            {
                pq_baseunit = this.ConvertToDerivedUnit();
                Debug.Assert(pq_baseunit.Unit != this || pq_baseunit.Unit.Kind != this.Kind); // Some reduction must be performed; else infinite recursive calls can occur
                pq_tounit = pq_baseunit.Unit.ConvertTo(convertToUnit);
            }

            if (pq_tounit != null)
            {
                // Valid conversion
                return new Quantity(pq_baseunit.Value * pq_tounit.Value, pq_tounit.Unit);
            }
            // Invalid conversion
            return null;
        }

        public override Quantity ConvertTo(IUnitSystem convertToUnitSystem)
        {
           Double value = this.FactorValue;
            Unit unit = null;

            Debug.Assert(convertToUnitSystem != null);

            foreach (IPrefixedUnitExponent pue in Numerators)
            {
                IQuantity pue_pq = pue.ConvertTo(convertToUnitSystem);
                if (pue_pq == null)
                {
                    return null;
                }

                value *= pue_pq.Value;

                if (unit == null)
                {
                    unit = pue_pq.Unit;
                }
                else
                {
                    var e = pue_pq.Unit.Exponents;
                    unit = unit.CombineMultiply(pue_pq.Unit);
                }
            }

            foreach (IPrefixedUnitExponent pue in Denominators)
            {
                IQuantity pue_pq = pue.ConvertTo(convertToUnitSystem);
                if (pue_pq == null)
                {
                    return null;
                }

                value /= pue_pq.Value;

                if (unit == null)
                {
                    unit = pue_pq.Unit.CombinePow(-1);
                }
                else
                {
                    unit = unit.CombineDivide(pue_pq.Unit);
                }
            }

            if (unit == null)
            { // dimension less unit
                unit = convertToUnitSystem.Dimensionless;
            }
            return new Quantity(value, unit);
        }

        public override Quantity ConvertTo(ref Double value, IUnitSystem convertToUnitSystem)
        {
            Quantity pq = this.ConvertTo(convertToUnitSystem);
            if (pq != null)
            {
                pq = pq.Multiply(value);
            }
            return pq;
        }

        public Quantity ConvertFrom(Quantity physicalQuantity)
        {
            Quantity pq_unit = physicalQuantity;
            if (Numerators.Count == 1 && Denominators.Count == 0)
            {
                IPrefixedUnitExponent pue = Numerators[0];

                Debug.Assert(pue.Exponent == 1);

                pq_unit = pq_unit.ConvertTo((Unit)pue.Unit);

                Debug.Assert(pq_unit != null);

                if (pq_unit != null)
                {
                    if (pue.Prefix.Exponent != 0)
                    {
                        pq_unit = pq_unit.Multiply(Math.Pow(10, -pue.Prefix.Exponent));
                    }

                    pq_unit = new Quantity(pq_unit.Value, (Unit)this);
                }
            }
            else
            {
                // TODO: Not implemented yet
                Debug.Assert(false);
            }

            return pq_unit;
        }

        #region IPhysicalUnitMath Members

        public override Unit Dimensionless => new CombinedUnit();

        public override Boolean IsDimensionless
        {
            get
            {
                if (Numerators.Count == 0 && Denominators.Count == 0)
                {
                    return true;
                }
                return base.IsDimensionless;
            }
        }

        private void MultiplyPUEL(IPrefixedUnitExponent prefixedUnitExponent, CombineExponentsFunc cef, IPrefixedUnitExponentList inPuel, ref IPrefixedUnitExponentList outPuel1,
                  ref Boolean primaryUnitFound, ref SByte prefixExponent, ref SByte prefixedUnitExponentScaleing,
                  ref SByte multExponent, ref Boolean changedExponentSign)
        {
            foreach (IPrefixedUnitExponent ue in inPuel)
            {
                if (!primaryUnitFound && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    primaryUnitFound = true;

                    if (!prefixExponent.Equals(ue.Prefix.Exponent))
                    {   // Convert prefixedUnitExponent to have same PrefixExponent as ue; Move difference in scaling to prefixedUnitExponentScaleing
                        prefixedUnitExponentScaleing = (SByte)((ue.Prefix.Exponent - prefixExponent) * multExponent);
                        prefixExponent = ue.Prefix.Exponent;
                    }

                    // Reduce the found CombinedUnit exponent with ue2´s exponent;
                    SByte newExponent = cef(ue.Exponent, multExponent);
                    if (newExponent > 0)
                    {
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, newExponent);
                        outPuel1.Add(temp_pue);
                        // Done
                    }
                    else
                    {   // Convert to opposite Numerator/Denominator
                        multExponent = cef(0, newExponent);
                        changedExponentSign = true;
                    }
                }
                else
                {
                    outPuel1.Add(ue);
                }
            }
        }

        public override Unit Multiply(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            if (prefixedUnitExponent.Unit.Kind == UnitKind.CombinedUnit)
            {
                CombinedUnit cue = ((CombinedUnit)(prefixedUnitExponent.Unit));
                if (prefixedUnitExponent.Prefix.Exponent != 0)
                {
                    IPrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, null, 1);
                    cue = cue.CombineMultiply(temp_pue);
                }
                cue = cue.CombinePow(prefixedUnitExponent.Exponent);
                CombinedUnit unit = this.CombineMultiply(cue);
                return unit;
            }

            SByte multExponent = prefixedUnitExponent.Exponent;
            SByte prefixExponent = prefixedUnitExponent.Prefix.Exponent;
            SByte prefixedUnitExponentScaleing = 1;


            IPrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
            IPrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

            Boolean primaryUnitFound = false;
            Boolean changedExponentSign = false;
            //// Check if pue2.Unit is already among our Numerators or Denominators

            MultiplyPUEL(prefixedUnitExponent, SByte_Sub, Denominators, ref tempDenominators,
                  ref primaryUnitFound, ref prefixExponent, ref prefixedUnitExponentScaleing,
                  ref multExponent, ref changedExponentSign);

            MultiplyPUEL(prefixedUnitExponent, SByte_Add, Numerators, ref tempNumerators,
                  ref primaryUnitFound, ref prefixExponent, ref prefixedUnitExponentScaleing,
                  ref multExponent, ref changedExponentSign);

            if (!primaryUnitFound || changedExponentSign)
            {   // pue2.Unit is not among our Numerators or Denominators (or has changed from Numerators to Denominators)

                if (multExponent > 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, multExponent);
                    tempNumerators.Add(temp_pue);
                }
                else if (multExponent < 0)
                {
                    multExponent = (SByte)(-multExponent);
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, multExponent);
                    tempDenominators.Add(temp_pue);
                }
            }

            CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
            return cu;
        }

        public override Unit Divide(IPrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            SByte newExponent = (SByte)(-prefixedUnitExponent.Exponent);
            PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, newExponent);
            return temp_pue;
        }


        public override Quantity Multiply(double quantity) => this * quantity;

        public override Quantity Divide(double quantity) => this / quantity;

        public override Unit Power(SByte exponent)
        {
            return new CombinedUnit(Math.Pow(FactorValue, exponent), Numerators.Power(exponent), Denominators.Power(exponent));
        }

        public override Unit Root(SByte exponent)
        {
            IPrefixedUnitExponentList tempNumerators;
            IPrefixedUnitExponentList tempDenominators = null;
            tempNumerators = Numerators.Root(exponent);
            if (tempNumerators != null)
            {
                tempDenominators = Denominators.Root(exponent);
            }

            if ((tempNumerators != null) && (tempDenominators != null))
            {
                CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
                return cu;
            }
            else
            {
                SByte[] newExponents = this.Exponents;
                if (newExponents != null)
                {
                    newExponents = newExponents.Root(exponent);
                    Debug.Assert(this.ExponentsSystem != null);
                    DerivedUnit du = new DerivedUnit(this.ExponentsSystem, newExponents);
                    return du;
                }
                else
                {
                    Debug.Assert(newExponents != null);
                    //if (throwExceptionOnUnitMathError) {
                    throw new PhysicalUnitMathException("The result of the math operation on the Unit argument can't be represented by this implementation of PhysicalMeasure: (" + this.ToPrintString() + ").Root(" + exponent.ToString() + ")");
                    //}
                    //return null;
                }
            }
        }

        #endregion IPhysicalUnitMath Members

        #region Combine IPhysicalUnitMath Members


        public override CombinedUnit CombineMultiply(double quantity)
        {
            Double factor = this.FactorValue * quantity;
            CombinedUnit result = new CombinedUnit(factor, this.Numerators, this.Denominators);
            return result;
        }

        public override CombinedUnit CombineDivide(double quantity)
        {
            Double factor = this.FactorValue / quantity;
            CombinedUnit result = new CombinedUnit(factor, this.Numerators, this.Denominators);
            return result;
        }
        public override CombinedUnit CombineMultiply(IUnitPrefixExponent prefixExponent) => this.CombineMultiply(prefixExponent.Value);

        public override CombinedUnit CombineDivide(IUnitPrefixExponent prefixExponent) => this.CombineDivide(prefixExponent.Value);

        private void CombineFactorPUEL(IPrefixedUnitExponent prefixedUnitExponent, CombineExponentsFunc cef, IPrefixedUnitExponentList inPuel, ref IPrefixedUnitExponentList outPuel1, ref IPrefixedUnitExponentList outPuel2,
                  ref Boolean primaryUnitFound, ref SByte pue_prefixExp, ref sbyte scalingPrefixExponent, ref SByte scalingExponent,
                  ref SByte multPrefixExponent, ref SByte multExponent, ref Boolean ChangedExponentSign)
        {
            foreach (IPrefixedUnitExponent ue in inPuel)
            {
                if (!primaryUnitFound && prefixedUnitExponent.Unit != null && ue.Unit != null && prefixedUnitExponent.Unit.Equals(ue.Unit))
                {
                    primaryUnitFound = true;

                    // Reduce the found CombinedUnit exponent with ue2´s exponent;
                    SByte newExponent = (SByte)(cef(ue.Exponent, prefixedUnitExponent.Exponent));

                    SByte ue_prefixExp = 0; // 10^0 = 1
                    if (ue.Prefix != null)
                    {
                        ue_prefixExp = ue.Prefix.Exponent;
                    }

                    if (!pue_prefixExp.Equals(ue_prefixExp))
                    {   // Convert prefixedUnitExponent to have same PrefixExponent as ue; Move difference in scaling to prefixedUnitExponentScaleing
                        scalingPrefixExponent = (SByte)(pue_prefixExp - ue_prefixExp);
                        scalingExponent = prefixedUnitExponent.Exponent;
                    }

                    if (newExponent > 0)
                    {   // Still some exponent left for a denominator element
                        PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, newExponent);
                        outPuel1.Add(temp_pue);
                        // Done
                    }
                    else
                    if (newExponent< 0)
                    {   // Convert to Numerator
                        multPrefixExponent = ue.Prefix.Exponent;
                        multExponent = (SByte)(-newExponent);
                        ChangedExponentSign = true;
                    }
                }
                else
                {
                    if (ue.Exponent > 0)
                    {
                        outPuel1.Add(ue);
                    }
                    else
                    {
                        outPuel2.Add(new PrefixedUnitExponent(ue.Prefix, ue.Unit, (SByte)(-ue.Exponent)));
                    }
                }
            }
        }

        public override CombinedUnit CombineMultiply(PrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            if (prefixedUnitExponent.Unit != null && prefixedUnitExponent.Unit.Kind == UnitKind.CombinedUnit)
            {
                CombinedUnit cue = ((CombinedUnit)(prefixedUnitExponent.Unit));
                if (prefixedUnitExponent.Prefix.Exponent != 0)
                {
                    IPrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, null, 1);
                    cue = cue.CombineMultiply(temp_pue);
                }
                cue = cue.CombinePow(prefixedUnitExponent.Exponent);
                CombinedUnit unit = this.CombineMultiply(cue);
                return unit;
            }

            SByte multPrefixExponent = 0; // 10^0 = 1
            SByte multExponent = 1;

            SByte scalingPrefixExponent = 0;  // 10^0 = 1
            SByte scalingExponent = 1;

            SByte pue_prefixExp = 0; // 10^0 = 1
            if (prefixedUnitExponent.Prefix != null)
            {
                pue_prefixExp = prefixedUnitExponent.Prefix.Exponent;
            }

            IPrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
            IPrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

            Boolean primaryUnitFound = false;
            Boolean changedExponentSign = false;

            CombineFactorPUEL(prefixedUnitExponent, SByte_Sub, Denominators, ref tempDenominators, ref tempNumerators,
                  ref primaryUnitFound, ref pue_prefixExp, ref scalingPrefixExponent, ref scalingExponent,
                  ref multPrefixExponent, ref multExponent, ref changedExponentSign);
            CombineFactorPUEL(prefixedUnitExponent, SByte_Add, Numerators, ref tempNumerators, ref tempDenominators,
                  ref primaryUnitFound, ref pue_prefixExp, ref scalingPrefixExponent, ref scalingExponent,
                  ref multPrefixExponent, ref multExponent, ref changedExponentSign);

            if (!primaryUnitFound || changedExponentSign)
            {
                if (!primaryUnitFound)
                {
                    if (prefixedUnitExponent.Prefix != null)
                    {
                        multPrefixExponent = prefixedUnitExponent.Prefix.Exponent;
                    }
                    multExponent = prefixedUnitExponent.Exponent;
                }

                IUnitPrefix unitPrefix = null;
                SByte restMultPrefixExponent = 0;
                if (multPrefixExponent != 0)
                {
                    Physics.UnitPrefixes.GetFloorUnitPrefixAndScaleFactorFromExponent(multPrefixExponent, out unitPrefix, out restMultPrefixExponent);
                    multPrefixExponent = restMultPrefixExponent;
                }

                if (multExponent > 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(unitPrefix, prefixedUnitExponent.Unit, multExponent);
                    tempNumerators.Add(temp_pue);
                }
                else if (multExponent < 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(unitPrefix, prefixedUnitExponent.Unit, (SByte)(-multExponent));
                    tempDenominators.Add(temp_pue);
                }
            }

            Double resScaleFactor = scaleFactor;
            if (scalingPrefixExponent != 0 && scalingExponent != 0)
            {   // Add scaling factor without unit
                sbyte exp = (sbyte)(scalingPrefixExponent * scalingExponent);

                resScaleFactor = scaleFactor * Math.Pow(10, exp);
            }

            CombinedUnit cu = new CombinedUnit(resScaleFactor, tempNumerators, tempDenominators);
            return cu;
        }

        public override CombinedUnit CombineDivide(PrefixedUnitExponent prefixedUnitExponent)
        {
            Debug.Assert(prefixedUnitExponent != null);

            SByte newExponent = (SByte)(-prefixedUnitExponent.Exponent);
            IPrefixedUnitExponent temp_pue = new PrefixedUnitExponent(prefixedUnitExponent.Prefix, prefixedUnitExponent.Unit, newExponent);
            return this.CombineMultiply(temp_pue);
        }



        public override CombinedUnit CombineMultiply(PrefixedUnit prefixedUnit)
        {
            CombinedUnit result = this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(prefixedUnit.Prefix, prefixedUnit.Unit, 1));
            return result;
        }

        public override CombinedUnit CombineDivide(PrefixedUnit prefixedUnit)
        {
            CombinedUnit result = this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(prefixedUnit.Prefix, prefixedUnit.Unit, -1));
            return result;
        }

        public override CombinedUnit CombineMultiply(BaseUnit physicalUnit) => this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(null, physicalUnit, 1));

        public override CombinedUnit CombineDivide(BaseUnit physicalUnit) => this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(null, physicalUnit, -1));

        public override CombinedUnit CombineMultiply(NamedDerivedUnit physicalUnit) => this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(null, physicalUnit, 1));

        public override CombinedUnit CombineDivide(NamedDerivedUnit physicalUnit) => this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(null, physicalUnit, -1));

        public override CombinedUnit CombineMultiply(ConvertibleUnit physicalUnit) => this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(null, physicalUnit, 1));

        public override CombinedUnit CombineDivide(ConvertibleUnit physicalUnit) => this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(null, physicalUnit, -1));

        public override CombinedUnit CombineMultiply(INamedSymbolUnit namedSymbolUnit)
        {
            CombinedUnit result = this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(null, namedSymbolUnit, 1));
            return result;
        }

        public override CombinedUnit CombineDivide(INamedSymbolUnit namedSymbolUnit)
        {
            CombinedUnit result = this.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(null, namedSymbolUnit, -1));
            return result;
        }

        public override CombinedUnit CombineMultiply(Unit physicalUnit)
        {
            if (physicalUnit.Kind == UnitKind.CombinedUnit)
            {
                CombinedUnit cu = physicalUnit as CombinedUnit;
                Debug.Assert(cu != null);
                return this.CombineMultiply(cu);
            }

            if (physicalUnit.Kind == UnitKind.BaseUnit || physicalUnit.Kind == UnitKind.ConvertibleUnit)
            {
                INamedSymbolUnit nsu = physicalUnit as INamedSymbolUnit;
                Debug.Assert(nsu != null);
                return this.CombineMultiply(new PrefixedUnitExponent(null, nsu, 1));
            }

            if (physicalUnit.Kind == UnitKind.MixedUnit)
            {
                MixedUnit mu = physicalUnit as MixedUnit;
                Debug.Assert(mu != null);
                return this.CombineMultiply(mu.MainUnit);
            }

            if (physicalUnit.Kind == UnitKind.DerivedUnit)
            {
                DerivedUnit du = physicalUnit as DerivedUnit;
                Debug.Assert(du != null);
                return this.CombineMultiply(du);
            }

            if (physicalUnit.Kind == UnitKind.PrefixedUnit)
            {
                PrefixedUnit pu = physicalUnit as PrefixedUnit;
                Debug.Assert(pu != null);
                return this.CombineMultiply(pu);
            }

            if (physicalUnit.Kind == UnitKind.PrefixedUnitExponent)
            {
                PrefixedUnitExponent pue = physicalUnit as PrefixedUnitExponent;
                Debug.Assert(pue != null);
                return this.CombineMultiply(pue);
            }

            // PrefixedUnitExponent will not accept an IUnit: return this.CombinedUnitMultiply(new PrefixedUnitExponent(null, physicalUnit, 1));
            // Will make recursive call without reduction: return this.CombinedUnitMultiply(physicalUnit);
            //return this.CombinedUnitMultiply(new PrefixedUnitExponent(null, physicalUnit, 1));

            // Just try to use as INamedSymbolUnit
            INamedSymbolUnit nsu2 = physicalUnit as INamedSymbolUnit;
            Debug.Assert(nsu2 != null);
            return this.CombineMultiply(nsu2);
        }

        public override CombinedUnit CombineDivide(Unit physicalUnit)
        {
            if (physicalUnit.Kind == UnitKind.CombinedUnit)
            {
                CombinedUnit cu = physicalUnit as CombinedUnit;
                Debug.Assert(cu != null);
                return this.CombineDivide(cu);
            }

            if (physicalUnit.Kind == UnitKind.BaseUnit || physicalUnit.Kind == UnitKind.ConvertibleUnit
                || (physicalUnit.Kind == UnitKind.DerivedUnit && physicalUnit as INamedSymbolUnit != null))
            {
                INamedSymbolUnit nsu = physicalUnit as INamedSymbolUnit;
                Debug.Assert(nsu != null);
                return this.CombineDivide((IPrefixedUnitExponent)new PrefixedUnitExponent(null, nsu, 1));
            }

            if (physicalUnit.Kind == UnitKind.MixedUnit)
            {
                IMixedUnit mu = physicalUnit as IMixedUnit;
                Debug.Assert(mu != null);
                return this.CombineDivide(mu.MainUnit);
            }

            if (physicalUnit.Kind == UnitKind.DerivedUnit)
            {
                DerivedUnit du = physicalUnit as DerivedUnit;
                Debug.Assert(du != null);
                return this.CombineDivide(du);
            }

            if (physicalUnit.Kind == UnitKind.PrefixedUnit)
            {
                IPrefixedUnit pu = physicalUnit as IPrefixedUnit;
                Debug.Assert(pu != null);
                return this.CombineDivide(pu);
            }

            if (physicalUnit.Kind == UnitKind.PrefixedUnitExponent)
            {
                PrefixedUnitExponent pue = physicalUnit as PrefixedUnitExponent;
                Debug.Assert(pue != null);
                return this.CombineDivide(pue);
            }
            // PrefixedUnitExponent will not accept an IUnit: return this.CombinedUnitDivide(new PrefixedUnitExponent(null, physicalUnit, 1));
            // Will make recursive call without reduction: return this.CombinedUnitDivide(physicalUnit);
            // return this.CombinedUnitDivide(new PrefixedUnitExponent(null, physicalUnit, 1));

            // Just try to use as INamedSymbolUnit
            INamedSymbolUnit nsu2 = physicalUnit as INamedSymbolUnit;
            Debug.Assert(nsu2 != null);
            return this.CombineDivide(nsu2);
        }

        public override CombinedUnit CombineMultiply(IUnit physicalUnit)
        {
            Unit pu = (Unit)physicalUnit;
            Debug.Assert((pu != null) == (physicalUnit != null));
            CombinedUnit result = this.CombineMultiply(pu);
            return result;
        }

        public override CombinedUnit CombineDivide(IUnit physicalUnit)
        {
            Unit pu = (Unit)physicalUnit;
            Debug.Assert((pu != null) == (physicalUnit != null));
            CombinedUnit result = this.CombineDivide(pu);
            return result;
        }


        public CombinedUnit CombineMultiply(IDerivedUnit derivedUnit)
        {
            CombinedUnit result = new CombinedUnit(this);
            int baseUnitIndex = 0;
            IUnitSystem sys = derivedUnit.ExponentsSystem;
            foreach (SByte exp in derivedUnit.Exponents)
            {
                if (exp != 0)
                {
                    result = result.CombineMultiply((IPrefixedUnitExponent)new PrefixedUnitExponent(null, sys.BaseUnits[baseUnitIndex], exp));
                }
                baseUnitIndex++;
            }

            return result;
        }

        public CombinedUnit CombineDivide(IDerivedUnit derivedUnit)
        {
            CombinedUnit result = new CombinedUnit(this);
            int baseUnitIndex = 0;
            IUnitSystem sys = derivedUnit.ExponentsSystem;
            foreach (SByte exp in derivedUnit.Exponents)
            {
                if (exp != 0)
                {
                    result = result.CombineDivide((IPrefixedUnitExponent)new PrefixedUnitExponent(null, sys.BaseUnits[baseUnitIndex], exp));
                }
                baseUnitIndex++;
            }

            return result;
        }


        protected void CombineExponentPUEL(SByte exponent, CombineExponentsFunc cef, IPrefixedUnitExponentList inPuel, ref IPrefixedUnitExponentList outPuel1, ref IPrefixedUnitExponentList outPuel2)
        {
            foreach (IPrefixedUnitExponent ue in inPuel)
            {
                SByte NewExponent = (SByte)(cef(ue.Exponent , exponent));
                if (NewExponent > 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, NewExponent);
                    outPuel1.Add(temp_pue);
                }
                if (NewExponent< 0)
                {
                    PrefixedUnitExponent temp_pue = new PrefixedUnitExponent(ue.Prefix, ue.Unit, (sbyte)(-NewExponent));
                    outPuel2.Add(temp_pue);
                }
            }
        }


        public override CombinedUnit CombinePow(SByte exponent)
        {
            if (exponent == 1)
            {
                return this;
            }
            else
            {
                IPrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
                IPrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

                CombineExponentPUEL(exponent, SByte_Mult, Numerators, ref tempNumerators, ref tempDenominators);
                CombineExponentPUEL(exponent, SByte_Mult, Denominators, ref tempDenominators, ref tempNumerators);

                CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
                return cu;
            }
        }

        public override CombinedUnit CombineRot(SByte exponent)
        {
            if (exponent == 1)
            {
                return this;
            }
            else
            {
                IPrefixedUnitExponentList tempNumerators = new PrefixedUnitExponentList();
                IPrefixedUnitExponentList tempDenominators = new PrefixedUnitExponentList();

                CombineExponentPUEL(exponent, SByte_Div, Numerators, ref tempNumerators, ref tempDenominators);
                CombineExponentPUEL(exponent, SByte_Div, Denominators, ref tempDenominators, ref tempNumerators);

                CombinedUnit cu = new CombinedUnit(tempNumerators, tempDenominators);
                return cu;
            }
        }

        public CombinedUnit CombineMultiply(ICombinedUnit cu2)
        {
            if (this.IsDimensionless)
            {
                return cu2.CombineMultiply(this.FactorValue);
            }

            CombinedUnit cu1 = new CombinedUnit(this.FactorValue * cu2.FactorValue, this.Numerators, this.Denominators);

            foreach (IPrefixedUnitExponent pue in cu2.Numerators)
            {
                cu1 = cu1.CombineMultiply(pue);
            }

            foreach (IPrefixedUnitExponent pue in cu2.Denominators)
            {
                cu1 = cu1.CombineDivide(pue);
            }

            return cu1;
        }

        public CombinedUnit CombineDivide(ICombinedUnit cu2)
        {
            CombinedUnit cu1 = new CombinedUnit(this.FactorValue / cu2.FactorValue, this.Numerators, this.Denominators);

            foreach (IPrefixedUnitExponent pue in cu2.Numerators)
            {
                cu1 = cu1.CombineDivide(pue);
            }

            foreach (IPrefixedUnitExponent pue in cu2.Denominators)
            {
                cu1 = cu1.CombineMultiply(pue);
            }

            return cu1;
        }

        #endregion IPhysicalUnitMath Members

        #region IEquatable<IPhysicalUnit> Members

        public override Int32 GetHashCode() => numerators.GetHashCode() + denominators.GetHashCode();

        public override Boolean Equals(object other)
        {
            if (other == null)
                return false;

            Unit otherIPU = other as Unit;

            if (otherIPU == null)
                return false;

            return this.Equals(otherIPU);
        }

        #endregion IEquatable<IPhysicalUnit> Members

        /// <summary>
        /// String with PrefixedUnitExponent formatted symbol (without system name prefixed).
        /// </summary>
        public override String PureUnitString() => CombinedUnitString(mayUseSlash: true, invertExponents: false);

        public override String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false)
        {
            String unitName = "";
            Boolean nextLevelMayUseSlash = mayUseSlash && Denominators.Count == 0;
            if (Numerators.Count > 0)
            {
                unitName = Numerators.CombinedUnitString(nextLevelMayUseSlash, invertExponents);
            }
            else
            {
                if (Denominators.Count > 0)
                {
                    //UnitName = "1";
                }
            }

            if (Denominators.Count > 0)
            {
                if (mayUseSlash && !String.IsNullOrWhiteSpace(unitName))
                {
                    unitName += "/" + Denominators.CombinedUnitString(false, invertExponents);
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(unitName))
                    {
                        // center dot '\0x0B7' (Char)183 U+00B7
                        unitName += '·' + Denominators.CombinedUnitString(false, !invertExponents);
                    }
                    else
                    {
                        unitName = Denominators.CombinedUnitString(false, !invertExponents);
                    }
                }
            }
            return unitName;
        }
        public override String ToString() => UnitString();

    }

    #endregion Combined Unit Classes

    #region Mixed Unit Classes

    public class MixedUnit : Unit, IMixedUnit
    {
        protected readonly Unit mainUnit;
        protected readonly Unit fractionalUnit;

        protected readonly String separator;
        protected readonly String fractionalValueFormat;
        protected readonly Boolean inlineUnitFormat;


        public Unit MainUnit
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return this.mainUnit;
            }
        }

        public Unit FractionalUnit
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return this.fractionalUnit;
            }
        }

        public String Separator
        {
            get
            {
                Debug.Assert(separator != null);
                return this.separator;
            }
        }

        public MixedUnit(Unit someMainUnit, String someSeparator, Unit someFractionalUnit, String someFractionalValueFormat, Boolean someInlineUnitFormat)
        {
            this.mainUnit = someMainUnit;
            this.separator = someSeparator;
            this.fractionalUnit = someFractionalUnit;
            this.fractionalValueFormat = someFractionalValueFormat;
            this.inlineUnitFormat = someInlineUnitFormat;
        }

        public MixedUnit(Unit someMainUnit, String someSeparator, Unit someFractionalUnit, String someFractionalValueFormat)
            : this(someMainUnit, someSeparator, someFractionalUnit, someFractionalValueFormat, false)

        {
        }

        public MixedUnit(Unit someMainUnit, String separator, Unit someFractionalUnit)
            : this(someMainUnit, separator, someFractionalUnit, "00.################")
        {
        }

        public MixedUnit(Unit someMainUnit, Unit someFractionalUnit)
            : this(someMainUnit, ":", someFractionalUnit)
        {
        }

        public override UnitKind Kind => UnitKind.MixedUnit;

        public override IUnitSystem SimpleSystem
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return MainUnit.SimpleSystem;
            }
            set
            {
                Debug.Assert(mainUnit != null);
                /* Just do nothing */
                Debug.Assert(MainUnit.SimpleSystem == value);

            }
        }

        public override IUnitSystem ExponentsSystem
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return MainUnit.ExponentsSystem;
            }
        }

        public override SByte[] Exponents
        {
            get
            {
                Debug.Assert(mainUnit != null);
                return MainUnit.Exponents;
            }
        }


        /// <summary>
        ///
        /// </summary>
        public override Boolean IsLinearConvertible()
        {
            Debug.Assert(mainUnit != null);
            return mainUnit.IsLinearConvertible();
        }


        public override Quantity ConvertToSystemUnit(ref Double value)
        {
            Debug.Assert(mainUnit != null);
            return MainUnit.ConvertToSystemUnit(ref value);
        }

        public override Quantity ConvertToSystemUnit()
        {
            Debug.Assert(mainUnit != null);
            return MainUnit.ConvertToSystemUnit();
        }

        public override Quantity ConvertToBaseUnit() => this.ConvertToBaseUnit(1);

        public override Quantity ConvertToDerivedUnit() => this.ConvertToBaseUnit();

        public override Quantity ConvertToBaseUnit(Double value) => this.ConvertToSystemUnit(ref value).ConvertToBaseUnit();

        public override string PureUnitString()
        {
            Debug.Assert(mainUnit != null);

            string us = MainUnit.UnitString();
            if (FractionalUnit != null)
            {
                us = us + this.Separator + FractionalUnit.UnitString();
            }
            return us;
        }

        public override string ValueString(Double value) => ValueString(value, null, null);

        public override string ValueString(Double value, String format, IFormatProvider formatProvider)
        {
            Debug.Assert(mainUnit != null);

            string valStr;
            if (FractionalUnit == null)
            {
                valStr = MainUnit.ValueString(value, format, formatProvider);
            }

            Double integralValue = Math.Truncate(value);
            Double fractionalValue = value - integralValue;
            Quantity fracPQ = new Quantity(fractionalValue, this.MainUnit);
            Quantity fracPQConv = fracPQ.ConvertTo(this.FractionalUnit);
            if (fracPQConv != null)
            {
                valStr = MainUnit.ValueString(integralValue, format, formatProvider) + separator + FractionalUnit.ValueString(fracPQConv.Value, fractionalValueFormat, null);
            }
            else
            {
                valStr = MainUnit.ValueString(value, format, formatProvider);
            }
            return valStr;
        }

        public override string ValueAndUnitString(double value) => ValueAndUnitString(value, null, null);

        public override string ValueAndUnitString(double value, String format, IFormatProvider formatProvider)
        {
            Debug.Assert(mainUnit != null);

            string resultStr;
            if (FractionalUnit == null)
            {
                resultStr = MainUnit.ValueAndUnitString(value, format, formatProvider);
                return resultStr;
            }

            Double integralValue = Math.Truncate(value);
            Double fractionalValue = value - integralValue;
            Quantity fracPQ = new Quantity(fractionalValue, this.MainUnit);
            Quantity fracPQConv = fracPQ.ConvertTo(this.FractionalUnit);

            if (inlineUnitFormat)
            {
                if (fracPQConv != null)
                {
                    resultStr = MainUnit.ValueAndUnitString(integralValue, format, formatProvider)
                                + separator
                                + FractionalUnit.ValueAndUnitString(fracPQConv.Value, fractionalValueFormat, null);
                }
                else
                {
                    resultStr = MainUnit.ValueAndUnitString(value, format, formatProvider);
                }

                return resultStr;
            }

            String valStr;
            if (fracPQConv != null)
            {
                valStr = MainUnit.ValueString(integralValue, format, formatProvider) + separator + FractionalUnit.ValueString(fracPQConv.Value, fractionalValueFormat, null);
            }
            else
            {
                valStr = MainUnit.ValueString(value, format, formatProvider);
            }

            String unitStr = this.ToString();
            if (String.IsNullOrEmpty(unitStr))
            {
                resultStr = valStr;
            }
            else
            {
                resultStr = valStr + " " + unitStr;
            }

            return resultStr;
        }
    }

    #endregion Mixed Unit Classes

    #endregion Physical Unit Classes

    #region Physical Unit System Classes

    public abstract class AbstractUnitSystem : NamedObject, IUnitSystem
    {
        public abstract UnitPrefixTable UnitPrefixes { get; }
        public abstract BaseUnit[] BaseUnits { get; /* */ set; /* */ }
        public abstract NamedDerivedUnit[] NamedDerivedUnits { get; /* */ set; /* */ }
        public abstract ConvertibleUnit[] ConvertibleUnits { get; /* */ set; /* */ }

        public abstract Boolean IsIsolatedUnitSystem { get; }
        public abstract Boolean IsCombinedUnitSystem { get; }

        protected Unit dimensionless;
        public virtual Unit Dimensionless
        {
            get
            {
                if (dimensionless == null)
                {
                    dimensionless = new DerivedUnit(this, new SByte[] { 0 });
                }
                return dimensionless;
            }
        }

        public AbstractUnitSystem(String someName)
            : base(someName)
        {
        }

        public override String ToString() => this.Name;

        protected static INamedSymbolUnit UnitFromName(INamedSymbolUnit[] units, String unitname)
        {
            if (units != null)
            {
                foreach (INamedSymbolUnit u in units)
                {
                    if (u.Name.Equals(unitname, StringComparison.OrdinalIgnoreCase))
                    {
                        return u;
                    }
                }
            }
            return null;
        }

        protected static INamedSymbolUnit UnitFromSymbol(INamedSymbolUnit[] units, String unitsymbol)
        {
            if (units != null)
            {
                foreach (INamedSymbolUnit u in units)
                {
                    // StringComparison must consider case))
                    if (u.Symbol.Equals(unitsymbol, StringComparison.Ordinal))
                    {
                        return u;
                    }
                }
            }
            return null;
        }

        public INamedSymbolUnit UnitFromName(String unitName)
        {
            INamedSymbolUnit unit;

            unit = UnitFromName(this.BaseUnits, unitName);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromName(this.NamedDerivedUnits, unitName);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromName(this.ConvertibleUnits, unitName);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromName(Physics.ExtraTimeUnits, unitName);
            return unit;
        }

        public INamedSymbolUnit UnitFromSymbol(String unitSymbol)
        {
            INamedSymbolUnit unit;

            unit = UnitFromSymbol(this.BaseUnits, unitSymbol);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromSymbol(this.NamedDerivedUnits, unitSymbol);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromSymbol(this.ConvertibleUnits, unitSymbol);
            if (unit != null)
            {
                return unit;
            }

            unit = UnitFromSymbol(Physics.ExtraTimeUnits, unitSymbol);

            return unit;
        }

        public Unit ScaledUnitFromSymbol(String scaledUnitSymbol)
        {
            INamedSymbolUnit symbolUnit = UnitFromSymbol(scaledUnitSymbol);
            if (scaledUnitSymbol.Length > 1)
            {   // Check for prefixed unit
                Char prefixchar = scaledUnitSymbol[0];
                IUnitPrefix unitPrefix;
                if (UnitPrefixes.GetUnitPrefixFromPrefixChar(prefixchar, out unitPrefix))
                {
                    INamedSymbolUnit symbolUnit2 = UnitFromSymbol(scaledUnitSymbol.Substring(1));
                    if (symbolUnit2 != null)
                    {   // Found both a prefix and an unit; Must be the right unit.
                        if (symbolUnit != null)
                        {
                            // symbolUnit = SI.Kg         <-> symbolUnit2 = SI_prefix.K·SI.g    Prefer (non-prefixed) symbolUnit, discharged symbolUnit2
                            // symbolUnit = SI.K (Kelvin) <-> symbolUnit2 = SI_prefix.K·...     Prefer (prefixed) symbolUnit2, discharged symbolUnit

                            if (ReferenceEquals(symbolUnit, SI.Kg) && prefixchar == 'K' && ReferenceEquals(symbolUnit2, SI.g))
                            {   // Prefer (non-prefixed) symbolUnit, discharged symbolUnit2

                                //Debug.Assert(symbolUnit == null); // For debug. Manually check if the discharged symbolUnit2 is a better choice than the returned symbolUnit.
                                return (Unit)symbolUnit;
                            }
                            // Prefer (prefixed) symbolUnit2, discharged symbolUnit
                            // Discharged symbolUnit even if set by non-prefixed unit (first call to UnitFromSymbol())
                            Debug.Assert(symbolUnit == null); // For debug. Manually check if the discharged symbolUnit could be a better choice than the returned symbolUnit2.
                        }

                        // Found both a prefix and an unit; Must be the right unit.
                        Debug.Assert(unitPrefix != null); // GetUnitPrefixFromPrefixChar must have returned a valid unitPrefix.
                        Unit pu = new PrefixedUnit(unitPrefix, symbolUnit2);
                        return pu;
                    }
                }
            }
            return (Unit)symbolUnit;
        }

        public Unit UnitFromExponents(SByte[] exponents)
        {
            SByte noOfNonZeroExponents = 0;
            SByte noOfNonOneExponents = 0;
            SByte firstNonZeroExponent = -1;

            SByte i = 0;
            foreach (SByte exponent in exponents)
            {
                if (exponent != 0)
                {
                    if (firstNonZeroExponent == -1)
                    {
                        firstNonZeroExponent = i;
                    }
                    noOfNonZeroExponents++;
                    if (exponent != 1)
                    {
                        noOfNonOneExponents++;
                    }
                }

                i++;
            }

            return UnitFromUnitInfo(exponents, noOfNonZeroExponents, noOfNonOneExponents, firstNonZeroExponent);
        }

        public Unit UnitFromUnitInfo(SByte[] exponents, SByte noOfNonZeroExponents, SByte noOfNonOneExponents, SByte firstNonZeroExponent)
        {
            Unit unit;

            if ((noOfNonZeroExponents == 1) && (noOfNonOneExponents == 0))
            {
                // BaseUnit
                unit = (Unit)BaseUnits[firstNonZeroExponent];
            }
            else
            {
                // Check if it is a NamedDerivedUnit
                unit = null;
                if (NamedDerivedUnits != null)
                {
                    int namedderivedunitsindex = 0;

                    while ((namedderivedunitsindex < NamedDerivedUnits.Length)
                           && !NamedDerivedUnits[namedderivedunitsindex].Exponents.DimensionEquals(exponents))
                    {
                        namedderivedunitsindex++;
                    }

                    if (namedderivedunitsindex < NamedDerivedUnits.Length)
                    {
                        // NamedDerivedUnit
                        unit = (Unit)NamedDerivedUnits[namedderivedunitsindex];
                    }
                }
                if (unit == null)
                {
                    // DerivedUnit
                    unit = new DerivedUnit(this, exponents);
                }
            }

            return unit;
        }

        public INamedSymbolUnit NamedDerivedUnitFromUnit(IDerivedUnit derivedUnit)
        {
            SByte[] exponents = derivedUnit.Exponents;
            int noOfDimensions = exponents.NoOfDimensions();
            if (noOfDimensions > 1)
            {
                INamedSymbolUnit ns = NamedDerivedUnits.FirstOrNull(namedderivedunit => exponents.DimensionEquals(namedderivedunit.Exponents));
                return ns;
            }

            return null;
        }

        public INamedSymbolUnit NamedDerivedUnitFromUnit(Unit derivedUnit)
        {
            Quantity pq = derivedUnit.ConvertToDerivedUnit();
            if (!Quantity.IsPureUnit(pq))
            {
                Unit unit = Quantity.GetAsNamedUnit(pq);
                return unit as INamedSymbolUnit;
            }
            if (Quantity.IsPureUnit(pq))
            {
                Unit derunit = Quantity.PureUnit(pq);
                SByte[] exponents = derunit.Exponents;
                int noOfDimensions = exponents.NoOfDimensions();
                if (noOfDimensions > 1 && this.NamedDerivedUnits != null)
                {
                    foreach (NamedDerivedUnit namedderivedunit in this.NamedDerivedUnits)
                    {
                        if (exponents.DimensionEquals(namedderivedunit.Exponents))
                        {
                            return namedderivedunit;
                        }
                    }
                }
            }
            return null;
        }


        public INamedSymbolUnit this[String unitSymbol] => UnitFromSymbol(unitSymbol);

        public Quantity ConvertTo(Unit convertFromUnit, Unit convertToUnit)
        {
            // Relative conversion is assumed
            // Handle relative unit e.g. temperature interval ....

            Debug.Assert(convertFromUnit != null);
            Debug.Assert(convertToUnit != null);

            if (convertFromUnit == null)
            {
                throw new ArgumentNullException(nameof(convertFromUnit));
            }

            if (convertToUnit == null)
            {
                throw new ArgumentNullException(nameof(convertToUnit));
            }
            Double quotient = 1;  // 0 means not equivalent unit
            Boolean isEquivalentUnit = convertFromUnit.Equivalent(convertToUnit, out quotient);
            if (isEquivalentUnit)
            {
                return new Quantity(quotient, convertToUnit);
            }
            else
            {
                if (convertFromUnit.Kind == UnitKind.MixedUnit)
                {
                    MixedUnit imu = (MixedUnit)convertFromUnit;
                    Quantity pq = ConvertTo(imu.MainUnit, convertToUnit);
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.MixedUnit)
                {
                    MixedUnit imu = (MixedUnit)convertToUnit;
                    Quantity pq = ConvertTo(convertFromUnit, imu.MainUnit);
                    Debug.Assert(pq != null);
                    return new Quantity(pq.Value, convertToUnit);
                }
                else if (convertFromUnit.Kind == UnitKind.ConvertibleUnit)
                {
                    ConvertibleUnit icu = (ConvertibleUnit)convertFromUnit;
                    Quantity pq_prim = icu.ConvertToPrimaryUnit();
                    Quantity pq = pq_prim.Unit.ConvertTo(convertToUnit);
                    if (pq != null)
                    {
                        pq = pq.Multiply(pq_prim.Value);
                    }
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.ConvertibleUnit)
                {
                    ConvertibleUnit icu = (ConvertibleUnit)convertToUnit;
                    Quantity converted_fromunit = convertFromUnit.ConvertTo(icu.PrimaryUnit);
                    if (converted_fromunit != null)
                    {
                        converted_fromunit = icu.ConvertFromPrimaryUnit(converted_fromunit.Value);
                    }
                    return converted_fromunit;
                }
                else if (convertFromUnit.Kind == UnitKind.PrefixedUnit)
                {
                    PrefixedUnit icu = (PrefixedUnit)convertFromUnit;
                    Quantity pq_derivedUnit = icu.ConvertToDerivedUnit();
                    Quantity pq = pq_derivedUnit.ConvertTo(convertToUnit);
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.PrefixedUnit)
                {
                    IPrefixedUnit icu = (PrefixedUnit)convertToUnit;
                    Quantity pq_unit = convertFromUnit.ConvertTo((Unit)icu.Unit);
                    if (pq_unit != null)
                    {
                        IQuantity pq = pq_unit.Divide(icu.Prefix.Value);
                        if (pq != null)
                        {
                            return new Quantity(pq.Value, convertToUnit);
                        }
                    }
                    return null;
                }
                else if (convertFromUnit.Kind == UnitKind.PrefixedUnitExponent)
                {
                    PrefixedUnitExponent pue = (PrefixedUnitExponent)convertFromUnit;
                    Quantity pq_derivedUnit = pue.ConvertToDerivedUnit();
                    Quantity pq = pq_derivedUnit.ConvertTo(convertToUnit);
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.PrefixedUnitExponent)
                {
                    PrefixedUnitExponent pue = (PrefixedUnitExponent)convertToUnit;
                    Quantity pue_derivedUnit = pue.ConvertToDerivedUnit();
                    Quantity converted_fromunit = convertFromUnit.ConvertTo(pue_derivedUnit.Unit);
                    if (converted_fromunit != null)
                    {
                        return new Quantity(converted_fromunit.Value / pue_derivedUnit.Value, convertToUnit);
                    }
                    return null;
                }
                else if (convertFromUnit.Kind == UnitKind.CombinedUnit)
                {
                    CombinedUnit icu = (CombinedUnit)convertFromUnit;
                    Quantity pq = icu.ConvertTo(convertToUnit);
                    return pq;
                }
                else if (convertToUnit.Kind == UnitKind.CombinedUnit)
                {
                    CombinedUnit icu = (CombinedUnit)convertToUnit;
                    Quantity pqToUnit;
                    pqToUnit = icu.ConvertTo(convertFromUnit);
                    if (pqToUnit != null)
                    {
                        Unit pu = convertFromUnit.Divide(pqToUnit.Unit);
                        if (pu == null || pu.Exponents.IsDimensionless())
                        {
                            return new Quantity(1 / pqToUnit.Value, convertToUnit);
                        }
                    }

                    return null;
                }

                // From some simple system to some simple system
                if ((convertFromUnit.SimpleSystem == this) && (convertToUnit.SimpleSystem == this))
                {   // Intra unit system conversion
                    Debug.Assert((convertFromUnit.Kind == UnitKind.BaseUnit) || (convertFromUnit.Kind == UnitKind.DerivedUnit));
                    Debug.Assert((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit));

                    if (!((convertFromUnit.Kind == UnitKind.BaseUnit) || (convertFromUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must have a unit of BaseUnit or DerivedUnit", nameof(convertFromUnit));
                    }

                    if (!((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must be a unit of BaseUnit or DerivedUnit", nameof(convertToUnit));
                    }

                    if (convertFromUnit.Exponents.DimensionEquals(convertToUnit.Exponents))
                    {
                        return new Quantity(1, convertToUnit);
                    }
                }
                else
                {   // Inter unit system conversion
                    UnitSystemConversion usc = Physics.UnitSystemConversions.GetUnitSystemConversion(convertFromUnit.SimpleSystem, convertToUnit.SimpleSystem);
                    if (usc != null)
                    {
                        return usc.ConvertTo(convertFromUnit, convertToUnit);
                    }
                }
                return null;
            }
        }


        public virtual Quantity ConvertTo(Unit convertFromUnit, IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(convertFromUnit != null);
            Debug.Assert(convertToUnitSystem != null);

            IUnitSystem convertFromUnitSystem = convertFromUnit.SimpleSystem;

            if (convertFromUnitSystem == convertToUnitSystem
                || (convertToUnitSystem.IsCombinedUnitSystem && ((CombinedUnitSystem)convertToUnitSystem).ContainsSubUnitSystem(convertFromUnitSystem)))
            {
                return new Quantity(1, convertFromUnit);
            }


            {   // Inter unit system conversion
                UnitSystemConversion usc = Physics.UnitSystemConversions.GetUnitSystemConversion(convertFromUnitSystem, convertToUnitSystem);
                if (usc != null)
                {
                    return usc.ConvertTo(convertFromUnit.ConvertToBaseUnit(), convertToUnitSystem);
                }

                if (convertFromUnitSystem.IsIsolatedUnitSystem || convertToUnitSystem.IsIsolatedUnitSystem)
                {
                    // Unit system declared to be isolated (user defined) without conversion to other (physical) unit systems.
                    return null;
                }

                /* Missing unit system conversion from physicalquantity.Unit.System to ToUnitSystem */
                /* TO DO Find intermediate systems with conversions between physicalquantity.Unit.System and convertToUnitSystem */
                Debug.Assert(false, "Missing unit system conversion from " + convertFromUnitSystem.Name + " to " + convertToUnitSystem.Name);

                return null;
            }
        }

        public Quantity ConvertTo(Quantity physicalQuantity, Unit convertToUnit)
        {
            // return RelativeConvertTo(physicalQuantity, convertToUnit);
            // We need to use specific conversion of unit, if either convertFromUnit or convertToUnit are a pure linear scaled unit.
            Boolean physicalQuantityUnitRelativeconversion = physicalQuantity.Unit.IsLinearConvertible();
            Boolean convertToUnitRelativeconversion = convertToUnit.IsLinearConvertible();
            Boolean relativeconversion = physicalQuantityUnitRelativeconversion && convertToUnitRelativeconversion;
            if (relativeconversion)
            {
                Quantity pq = this.ConvertTo(physicalQuantity.Unit, convertToUnit);
                if (pq != null)
                {
                    pq = pq.Multiply(physicalQuantity.Value);
                }
                return pq;
            }
            else
            {
                return SpecificConvertTo(physicalQuantity, convertToUnit);
            }
        }

        public Quantity SpecificConvertTo(Quantity physicalQuantity, Unit convertToUnit)
        {
            Debug.Assert(physicalQuantity.Unit != null);
            Debug.Assert(convertToUnit != null);

            if (physicalQuantity.Unit == null)
            {
                throw new ArgumentException("Must have a unit", nameof(physicalQuantity));
            }

            if (convertToUnit == null)
            {
                throw new ArgumentNullException(nameof(convertToUnit));
            }

            if (physicalQuantity.Unit == convertToUnit)
            {
                return physicalQuantity;
            }

            if (physicalQuantity.Unit.SimpleSystem != null && physicalQuantity.Unit.SimpleSystem != this)
            {
                return physicalQuantity.Unit.SimpleSystem.SpecificConvertTo(physicalQuantity, convertToUnit);
            }
            else
            {
                IUnitSystem convertfromunitsystem = physicalQuantity.Unit.SimpleSystem;
                IUnitSystem converttounitsystem = convertToUnit.SimpleSystem;

                if (convertfromunitsystem == null)
                {
                    string physicalQuantityUnitKind_debug_trace_str = physicalQuantity.Unit.Kind.ToString();
                    string physicalQuantity_debug_trace_str = physicalQuantity.ToPrintString();
                    Debug.Assert(physicalQuantity.Unit.Kind == UnitKind.CombinedUnit);

                    IUnitSystem tempconverttounitsystem = converttounitsystem;
                    if (tempconverttounitsystem == null && (physicalQuantity.Unit.Kind == UnitKind.CombinedUnit))
                    {   // Find some system to convert into

                        ICombinedUnit icu = (ICombinedUnit)physicalQuantity.Unit;
                        Debug.Assert(icu != null);

                        tempconverttounitsystem = icu.SomeSimpleSystem;
                    }
                    if (tempconverttounitsystem != null)
                    {
                        physicalQuantity = physicalQuantity.ConvertTo(tempconverttounitsystem);

                        if (physicalQuantity != null)
                        {
                            convertfromunitsystem = physicalQuantity.Unit.SimpleSystem;
                        }
                        else
                        {
                            // ?? What TO DO here ??
                            Debug.Assert(false);

                            return null;
                        }
                    }
                    else
                    {
                        // ?? What TO DO here ??
                        Debug.Assert(false);

                        return null;
                    }
                }

                if (converttounitsystem == null)
                {
                    Debug.Assert(convertToUnit.Kind == UnitKind.CombinedUnit);

                    ICombinedUnit icu = (ICombinedUnit)convertToUnit;
                    Debug.Assert(icu != null);

                    IUnitSystem tempconverttounitsystem;
                    tempconverttounitsystem = icu.SomeSimpleSystem;
                    Debug.Assert(tempconverttounitsystem != null);

                    // ?? What TO DO here ??
                    Debug.Assert(false);
                }

                if (converttounitsystem != null && convertfromunitsystem != converttounitsystem)
                {   // Inter unit system conversion

                    if (physicalQuantity.Unit.Kind == UnitKind.ConvertibleUnit)
                    {
                        ConvertibleUnit icu = (ConvertibleUnit)physicalQuantity.Unit;
                        Double d = physicalQuantity.Value;
                        physicalQuantity = icu.ConvertToSystemUnit(ref d);
                    }

                    Quantity pq = this.ConvertTo(physicalQuantity, converttounitsystem);
                    if (pq != null)
                    {
                        physicalQuantity = pq;
                        convertfromunitsystem = physicalQuantity.Unit.SimpleSystem;
                    }
                    else
                    {
                        return null;
                    }
                }

                if (convertfromunitsystem != null && convertfromunitsystem == converttounitsystem)
                {   // Intra unit system conversion

                    if (physicalQuantity.Unit.Kind == UnitKind.MixedUnit)
                    {
                        MixedUnit imu = (MixedUnit)physicalQuantity.Unit;

                        Quantity pq = new Quantity(physicalQuantity.Value, imu.MainUnit);
                        pq = pq.ConvertTo(convertToUnit);
                        return pq;
                    }
                    else if (physicalQuantity.Unit.Kind == UnitKind.CombinedUnit)
                    {
                        CombinedUnit icu = (CombinedUnit)physicalQuantity.Unit;
                        Double d = physicalQuantity.Value;
                        Quantity pq = icu.ConvertToSystemUnit(ref d);
                        Debug.Assert(pq != null);
                        pq = pq.ConvertTo(convertToUnit);
                        return pq;
                    }
                    else if (physicalQuantity.Unit.Kind == UnitKind.ConvertibleUnit)
                    {
                        ConvertibleUnit icu = (ConvertibleUnit)physicalQuantity.Unit;
                        Quantity prim_pq = icu.ConvertToPrimaryUnit(physicalQuantity.Value);
                        return ConvertTo(prim_pq, convertToUnit);
                    }
                    else if (convertToUnit.Kind == UnitKind.MixedUnit)
                    {
                        MixedUnit imu = (MixedUnit)convertToUnit;

                        Quantity pq = ConvertTo(physicalQuantity, imu.MainUnit);
                        if (pq != null)
                        {
                            pq = new Quantity(pq.Value, convertToUnit);
                        }
                        return pq;
                    }
                    else if (convertToUnit.Kind == UnitKind.CombinedUnit)
                    {
                        CombinedUnit icu = (CombinedUnit)convertToUnit;
                        Quantity pq = icu.ConvertFrom(physicalQuantity);
                        return pq;
                    }
                    else if (convertToUnit.Kind == UnitKind.ConvertibleUnit)
                    {
                        ConvertibleUnit icu = (ConvertibleUnit)convertToUnit;
                        Quantity pq = ConvertTo(physicalQuantity, icu.PrimaryUnit);
                        if (pq != null)
                        {
                            pq = icu.ConvertFromPrimaryUnit(pq.Value);
                        }
                        return pq;
                    }

                    Debug.Assert((physicalQuantity.Unit.Kind == UnitKind.BaseUnit) || (physicalQuantity.Unit.Kind == UnitKind.DerivedUnit));
                    Debug.Assert((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit));

                    if (!((physicalQuantity.Unit.Kind == UnitKind.BaseUnit) || (physicalQuantity.Unit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must have a unit of BaseUnit or a DerivedUnit", nameof(physicalQuantity));
                    }

                    if (!((convertToUnit.Kind == UnitKind.BaseUnit) || (convertToUnit.Kind == UnitKind.DerivedUnit)))
                    {
                        throw new ArgumentException("Must be a unit of BaseUnit or a DerivedUnit", nameof(convertToUnit));
                    }

                    if (physicalQuantity.Unit.Exponents.DimensionEquals(convertToUnit.Exponents))
                    {
                        return new Quantity(physicalQuantity.Value, convertToUnit);
                    }
                }

                return null;
            }
        }

        public Quantity ConvertTo(Quantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            IUnitSystem convertFromUnitSystem = physicalQuantity.Unit.SimpleSystem;
            if (convertFromUnitSystem == convertToUnitSystem)
            {
                return physicalQuantity;
            }

            if (convertFromUnitSystem.IsIsolatedUnitSystem || convertToUnitSystem.IsIsolatedUnitSystem)
            {
                // Unit system declared to be isolated (user defined) without conversion to other (physical) unit systems.
                return null;
            }
            else
            {   // Inter unit system conversion
                UnitSystemConversion usc = Physics.UnitSystemConversions.GetUnitSystemConversion(convertFromUnitSystem, convertToUnitSystem);
                if (usc != null)
                {
                    return usc.ConvertTo(physicalQuantity.ConvertToBaseUnit(), convertToUnitSystem);
                }

                /* Missing unit system conversion from  physicalquantity.Unit.System to ToUnitSystem */
                /* TO DO Find intermediate systems with conversions between physicalquantity.Unit.System and convertToUnitSystem */
                Debug.Assert(false);

                return null;
            }
        }
    }

    public class UnitSystem : AbstractUnitSystem
    {
        private /* readonly */ UnitPrefixTable unitPrefixes;
        private /* readonly */ BaseUnit[] baseUnits;
        private /* readonly */ NamedDerivedUnit[] namedDerivedUnits;
        private /* readonly */ ConvertibleUnit[] convertibleUnits;

        private /* readonly */ Boolean isIsolated;

        public override UnitPrefixTable UnitPrefixes => unitPrefixes;
        public override BaseUnit[] BaseUnits { get { return baseUnits; } /* */ set { baseUnits = (BaseUnit[])value; CheckBaseUnitSystem(); } /* */ }
        public override NamedDerivedUnit[] NamedDerivedUnits { get { return namedDerivedUnits; } /* */ set { namedDerivedUnits = (NamedDerivedUnit[])value; CheckNamedDerivedUnitSystem(); } /* */  }
        public override ConvertibleUnit[] ConvertibleUnits { get { return convertibleUnits; } /* */ set { convertibleUnits = (ConvertibleUnit[])value; CheckConvertibleUnitSystem(); } /* */  }

        public override Boolean IsIsolatedUnitSystem => isIsolated;
        public override Boolean IsCombinedUnitSystem => false;

        public UnitSystem(String someName, Boolean someIsIsolated)
            : base(someName)
        {
            this.isIsolated = someIsIsolated;
        }

        public UnitSystem(String someName)
            : this(someName, true)
        {
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes)
            : this(someName, false)
        {
            this.unitPrefixes = someUnitPrefixes;
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit[] someBaseUnits)
            : this(someName, someUnitPrefixes)
        {
            this.baseUnits = someBaseUnits;

            CheckBaseUnitSystem();
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit[] someBaseUnits, NamedDerivedUnit[] someNamedDerivedUnits)
            : this(someName, someUnitPrefixes, someBaseUnits)
        {
            this.namedDerivedUnits = someNamedDerivedUnits;

            CheckNamedDerivedUnitSystem();
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit[] someBaseUnits, NamedDerivedUnit[] someNamedDerivedUnits, ConvertibleUnit[] someConvertibleUnits)
            : this(someName, someUnitPrefixes, someBaseUnits, someNamedDerivedUnits)
        {
            this.convertibleUnits = someConvertibleUnits;

            CheckConvertibleUnitSystem();
        }

        public UnitSystem(String someName, UnitPrefixTable someUnitPrefixes, BaseUnit someBaseUnit, NamedDerivedUnit[] someNamedDerivedUnits, ConvertibleUnit[] someConvertibleUnits)
            : this(someName, someUnitPrefixes, new BaseUnit[] { someBaseUnit }, someNamedDerivedUnits)
        {
            this.isIsolated = someBaseUnit.BaseUnitNumber == (SByte)MonetaryBaseQuantityKind.Currency;
            this.convertibleUnits = someConvertibleUnits;

            CheckConvertibleUnitSystem();
        }

        //public override INamedSymbolUnit this[String unitSymbol] => UnitFromSymbol(unitSymbol);

        private void CheckBaseUnitSystem()
        {
            Debug.Assert(this.baseUnits != null);

            foreach (BaseUnit aBaseUnit in this.baseUnits)
            {
                Debug.Assert(aBaseUnit.Kind == UnitKind.BaseUnit);
                if (aBaseUnit.Kind != UnitKind.BaseUnit)
                {
                    throw new ArgumentException("Must only contain units with Kind = UnitKind.BaseUnit", "BaseUnits");
                }
                if (aBaseUnit.SimpleSystem != this)
                {
                    Debug.Assert(aBaseUnit.SimpleSystem == null);
                    aBaseUnit.SimpleSystem = this;
                }
            }
        }

        private void CheckNamedDerivedUnitSystem()
        {
            if (this.namedDerivedUnits != null)
            {
                foreach (NamedDerivedUnit namedderivedunit in this.namedDerivedUnits)
                {
                    Debug.Assert(namedderivedunit.Kind == UnitKind.DerivedUnit);
                    if (namedderivedunit.Kind != UnitKind.DerivedUnit)
                    {
                        throw new ArgumentException("Must only contain units with Kind = UnitKind.DerivedUnit", "someNamedDerivedUnits");
                    }
                    if (namedderivedunit.SimpleSystem != this)
                    {
                        namedderivedunit.SimpleSystem = this;
                    }
                }
            }
        }

        private void CheckConvertibleUnitSystem()
        {
            if (this.convertibleUnits != null)
            {
                foreach (ConvertibleUnit convertibleunit in this.convertibleUnits)
                {
                    Debug.Assert(convertibleunit.Kind == UnitKind.ConvertibleUnit);
                    if (convertibleunit.Kind != UnitKind.ConvertibleUnit)
                    {
                        throw new ArgumentException("Must only contain units with Kind = UnitKind.DerivedUnit", "someConvertibleUnits");
                    }
                    if (convertibleunit.SimpleSystem != this)
                    {
                        convertibleunit.SimpleSystem = this;
                    }
                    if (convertibleunit.PrimaryUnit.SimpleSystem == null)
                    {
                        (convertibleunit.PrimaryUnit as Unit).SimpleSystem = this;
                    }
                }
            }
        }
    }


    public class CombinedUnitSystem : AbstractUnitSystem, ICombinedUnitSystem
    {
        private /* readonly */ IUnitSystem[] unitSystemes;

        private /* readonly */ UnitPrefixTable unitprefixes;
        private /* readonly */ BaseUnit[] baseunits;
        private /* readonly */ NamedDerivedUnit[] namedderivedunits;
        private /* readonly */ ConvertibleUnit[] convertibleunits;


        public IUnitSystem[] UnitSystemes => unitSystemes;

        public override UnitPrefixTable UnitPrefixes => unitprefixes;

        public override BaseUnit[] BaseUnits { get { return baseunits; }  /* */ set { throw new NotImplementedException(); } /* */ }

        public override NamedDerivedUnit[] NamedDerivedUnits { get { return namedderivedunits; } /* */ set { throw new NotImplementedException(); } /* */ }
        public override ConvertibleUnit[] ConvertibleUnits { get { return convertibleunits; } /* */ set { throw new NotImplementedException(); } /* */ }


        public override Unit Dimensionless => UnitSystemes[0].Dimensionless;

        public override Boolean IsIsolatedUnitSystem => UnitSystemes.All(us => us.IsIsolatedUnitSystem);
        public override Boolean IsCombinedUnitSystem => true;

        public Boolean ContainsSubUnitSystem(IUnitSystem unitsystem) => UnitSystemes.Contains(unitsystem);

        public Boolean ContainsSubUnitSystems(IEnumerable<IUnitSystem> someUnitSystems) => UnitSystemes.Union(someUnitSystems).Count() == UnitSystemes.Count();

        private static List<ICombinedUnitSystem> combinedUnitSystems = new List<ICombinedUnitSystem>();

        public static ICombinedUnitSystem GetCombinedUnitSystem(IUnitSystem[] subUnitSystems)
        {
            Debug.Assert(!subUnitSystems.Any(us => us.IsCombinedUnitSystem));
            IUnitSystem[] sortedSubUnitSystems = subUnitSystems.OrderByDescending(us => us.BaseUnits.Length).ToArray();
            ICombinedUnitSystem cus = null;

            if (combinedUnitSystems.Count() > 0)
            {
                IEnumerable<ICombinedUnitSystem> tempUnitSystems = combinedUnitSystems.Where(us => us.UnitSystemes.SequenceEqual(sortedSubUnitSystems));
                if (tempUnitSystems.Count() >= 1)
                {
                    Debug.Assert(tempUnitSystems.Count() == 1);
                    cus = tempUnitSystems.First();
                }
            }
            if (cus == null)
            {
                lock (combinedUnitSystems)
                {
                    IEnumerable<ICombinedUnitSystem> tempUnitSystems = combinedUnitSystems.Where(us => us.UnitSystemes.SequenceEqual(sortedSubUnitSystems));
                    if (tempUnitSystems.Count() >= 1)
                    {
                        cus = tempUnitSystems.First();
                    }
                    else
                    {
                        cus = new CombinedUnitSystem(null, sortedSubUnitSystems);
                        combinedUnitSystems.Add(cus);
                    }
                }
            }

            return cus;
        }

        public CombinedUnitSystem(String someName, IUnitSystem us1, IUnitSystem us2)
            : this(someName, new IUnitSystem[] { us1, us2 })
        {

        }

        public CombinedUnitSystem(String someName, IUnitSystem[] someSubUnitSystems)
            : base(someName != null ? someName : "<" + someSubUnitSystems.Aggregate("", ((str, us) => String.IsNullOrWhiteSpace(str) ? us.Name : str + ", " + us.Name)) + ">")
        {
            SetupCombinedUnitSystem(someSubUnitSystems.OrderByDescending(us => us.BaseUnits.Length).ToArray());
        }

        public void SetupCombinedUnitSystem(IUnitSystem[] subUnitSystems)
        {
            unitSystemes = subUnitSystems;

            UnitPrefix[] tempUnitprefixes = null;
            BaseUnit[] tempBaseUnits = null;
            NamedDerivedUnit[] tempNamedDerivedUnits = null;
            ConvertibleUnit[] tempConvertibleUnits = null;

            foreach (IUnitSystem us in unitSystemes)
            {
                tempUnitprefixes = ArrayExtensions.Concat<UnitPrefix>(tempUnitprefixes, us.UnitPrefixes?.UnitPrefixes);

                tempBaseUnits = ArrayExtensions.Concat<BaseUnit>(tempBaseUnits, us.BaseUnits);
                tempNamedDerivedUnits = ArrayExtensions.Concat<NamedDerivedUnit>(tempNamedDerivedUnits, us.NamedDerivedUnits);
                tempConvertibleUnits = ArrayExtensions.Concat<ConvertibleUnit>(tempConvertibleUnits, us.ConvertibleUnits);
            }

            unitprefixes = new UnitPrefixTable(tempUnitprefixes);

            baseunits = tempBaseUnits;
            namedderivedunits = tempNamedDerivedUnits;
            convertibleunits = tempConvertibleUnits;
        }


        public SByte[] UnitExponents(CombinedUnit cu)
        {
            int noOfSubUnitSystems = UnitSystemes.Length;
            SByte[] unitSystemExponentsLength = new sbyte[noOfSubUnitSystems];
            SByte[] unitSystemExponentsOffsets = new sbyte[noOfSubUnitSystems];

            SByte noOfDimensions = 0;
            SByte index = 0;
            foreach (IUnitSystem us in UnitSystemes)
            {
                unitSystemExponentsOffsets[index] = noOfDimensions;
                unitSystemExponentsLength[index] = (sbyte)us.BaseUnits.Length;
                noOfDimensions += unitSystemExponentsLength[index];

            }

            SByte[] resExponents = new sbyte[0];

            // Split cu in to parts for each sub unit system in UnitSystemes
            ICombinedUnit[] subUnitParts = new ICombinedUnit[noOfSubUnitSystems];

            // Split into subUnitParts
            for (int i = 0; i < noOfSubUnitSystems; i++)
            {
                subUnitParts[i] = cu.OnlySingleSystemUnits(UnitSystemes[i]);
                SByte[] us_exponents = subUnitParts[i].Exponents;
                if (us_exponents.Length < unitSystemExponentsLength[index])
                {
                    us_exponents = us_exponents.AllExponents(unitSystemExponentsLength[index]);
                }
                resExponents = resExponents.Concat(us_exponents).ToArray();
            }

            return resExponents;
        }

        public Quantity ConvertToBaseUnit(CombinedUnit cu)
        {
            int noOfSubUnitSystems = UnitSystemes.Length;
            SByte[] unitSystemExponentsLength = new sbyte[noOfSubUnitSystems];
            SByte[] unitSystemExponentsOffsets = new sbyte[noOfSubUnitSystems];

            SByte noOfDimensions = 0;
            SByte index = 0;
            foreach (IUnitSystem us in UnitSystemes)
            {
                unitSystemExponentsOffsets[index] = noOfDimensions;
                unitSystemExponentsLength[index] = (sbyte)us.BaseUnits.Length;
                noOfDimensions += unitSystemExponentsLength[index];
                index++;
            }

            SByte[] resExponents = new sbyte[0];

            // Split cu in to parts for each sub unit system in UnitSystemes
            ICombinedUnit[] subUnitParts = new ICombinedUnit[noOfSubUnitSystems + 1];

            // Split into subUnitParts
           Double resValue = cu.FactorValue;

            for (int i = 0; i < noOfSubUnitSystems; i++)
            {
                subUnitParts[i] = cu.OnlySingleSystemUnits(UnitSystemes[i]);

                Quantity pq = subUnitParts[i].ConvertToBaseUnit(UnitSystemes[i]);
                resValue *= pq.Value;
                Debug.Assert(pq.Unit != null);
                SByte[] us_exponents = pq.Unit.Exponents;
                if (us_exponents.Length < unitSystemExponentsLength[i])
                {
                    us_exponents = us_exponents.AllExponents(unitSystemExponentsLength[i]);
                }
                resExponents = resExponents.Concat(us_exponents).ToArray();
            }

            // Handle part of cu without (sub-)unit system
            subUnitParts[noOfSubUnitSystems] = cu.OnlySingleSystemUnits(null); // no unit system

            Quantity pq2 = subUnitParts[noOfSubUnitSystems].ConvertToBaseUnit();
            resValue *= pq2.Value;

            Unit derivatedUnit = new DerivedUnit(this, resExponents);
            return new Quantity(resValue, derivatedUnit);
        }

        public override int GetHashCode()
        {
            if (unitSystemes == null)
            {
                return base.GetHashCode();
            }
            return unitSystemes.GetHashCode();
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
                return false;

            ICombinedUnitSystem ICombinedUnitSystemObj = obj as ICombinedUnitSystem;
            if (ICombinedUnitSystemObj == null)
                return false;
            else
                return Equals(ICombinedUnitSystemObj);
        }

        public Boolean Equals(ICombinedUnitSystem other) => Equals(this.UnitSystemes, other.UnitSystemes);
    }

    #endregion Physical Unit System Classes

    #region Physical Unit System Conversion Classes

    public class UnitSystemConversion
    {
        private readonly IUnitSystem baseUnitSystem;
        private readonly IUnitSystem convertedUnitSystem;
        public readonly ValueConversion[] BaseUnitConversions;

        public IUnitSystem BaseUnitSystem => baseUnitSystem;
        public IUnitSystem ConvertedUnitSystem => convertedUnitSystem;


        public UnitSystemConversion(IUnitSystem someBaseUnitsystem, IUnitSystem someConvertedUnitsystem, ValueConversion[] someBaseUnitConversions)
        {
            this.baseUnitSystem = someBaseUnitsystem;
            this.convertedUnitSystem = someConvertedUnitsystem;
            this.BaseUnitConversions = someBaseUnitConversions;
        }

        public Quantity Convert(IUnit convertUnit, Boolean backwards = false)
        {
            Debug.Assert(convertUnit.Kind == UnitKind.BaseUnit || convertUnit.Kind == UnitKind.DerivedUnit);

            SByte[] fromUnitExponents = convertUnit.Exponents;

           Double convertproduct = 1;

            SByte noOfNonZeroExponents = 0;
            SByte noOfNonOneExponents = 0;
            SByte firstNonZeroExponent = -1;

            SByte i = 0;
            foreach (SByte exponent in fromUnitExponents)
            {
                if (exponent != 0)
                {
                    if (firstNonZeroExponent == -1)
                    {
                        firstNonZeroExponent = i;
                    }
                    noOfNonZeroExponents++;
                    if (exponent != 1)
                    {
                        noOfNonOneExponents++;
                    }
                    ValueConversion vc = BaseUnitConversions[i];
                    if (vc != null)
                    {
                       Double baseunitconvertedvalue = vc.Convert(1, backwards);
                       Double baseunitfactor = Math.Pow(baseunitconvertedvalue, exponent);
                        convertproduct = convertproduct * baseunitfactor;
                    }
                    else
                    {
                        /* throw new ArgumentException("object's physical unit is not convertible to a " + ConvertedUnitSystem.name + " unit. " + ConvertedUnitSystem.name + " does not "); */
                        return null;
                    }
                }

                i++;
            }
           Double value = convertproduct;

            IUnitSystem unitsystem = (backwards ? BaseUnitSystem : ConvertedUnitSystem);

            Unit unit = unitsystem.UnitFromUnitInfo(fromUnitExponents, noOfNonZeroExponents, noOfNonOneExponents, firstNonZeroExponent);
            return new Quantity(value, unit);
        }

        public Quantity Convert(IQuantity physicalQuantity, Boolean backwards = false)
        {
            Debug.Assert(physicalQuantity != null);

            IQuantity pq = Convert(physicalQuantity.Unit, backwards);
            return new Quantity(physicalQuantity.Value * pq.Value, pq.Unit);
        }

        public Quantity ConvertFromBaseUnitSystem(IUnit convertUnit) => Convert(convertUnit, false);

        public Quantity ConvertToBaseUnitSystem(IUnit convertUnit) => Convert(convertUnit, true);

        public Quantity ConvertFromBaseUnitSystem(IQuantity physicalQuantity) => Convert(physicalQuantity, false);

        public Quantity ConvertToBaseUnitSystem(IQuantity physicalQuantity) => Convert(physicalQuantity, true);

        public Quantity ConvertTo(IUnit convertFromUnit, IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(convertFromUnit != null);

            if ((convertFromUnit.SimpleSystem == BaseUnitSystem) && (convertToUnitSystem == ConvertedUnitSystem))
            {
                return this.ConvertFromBaseUnitSystem(convertFromUnit);
            }
            else
                if ((convertFromUnit.SimpleSystem == ConvertedUnitSystem) && (convertToUnitSystem == BaseUnitSystem))
            {
                return this.ConvertToBaseUnitSystem(convertFromUnit);
            }

            return null;
        }

        public Quantity ConvertTo(IQuantity physicalQuantity, IUnitSystem convertToUnitSystem)
        {
            Debug.Assert(physicalQuantity != null);

            if ((physicalQuantity.Unit.SimpleSystem == BaseUnitSystem) && (convertToUnitSystem == ConvertedUnitSystem))
            {
                return this.ConvertFromBaseUnitSystem(physicalQuantity);
            }
            else
                if ((physicalQuantity.Unit.SimpleSystem == ConvertedUnitSystem) && (convertToUnitSystem == BaseUnitSystem))
            {
                return this.ConvertToBaseUnitSystem(physicalQuantity);
            }

            return null;
        }

        public Quantity ConvertTo(Unit convertFromUnit, Unit convertToUnit)
        {
            Debug.Assert(convertToUnit != null);

            Quantity pq = this.ConvertTo(convertFromUnit, convertToUnit.SimpleSystem);
            if (pq != null)
            {
                pq = pq.ConvertTo(convertToUnit);
            }
            return pq;
        }

        public Quantity ConvertTo(Quantity physicalQuantity, Unit convertToUnit)
        {
            Debug.Assert(convertToUnit != null);

            Quantity pq = this.ConvertTo(physicalQuantity, convertToUnit.SimpleSystem);
            if (pq != null)
            {
                pq = pq.ConvertTo(convertToUnit);
            }
            return pq;
        }
    }

    #endregion Physical Unit System Conversions

    #region Physical Quantity Classes

    public class Quantity : IQuantity
    {
        // The value holders
        private readonly Double value;
        private readonly Unit unit;

        public Double Value => this.value;

        public Unit Unit => this.unit;

        public Unit Dimensionless => unit.Dimensionless;
        public Boolean IsDimensionless => unit == null || unit.IsDimensionless;

        public Quantity()
            : this(0)
        {
        }

        public Quantity(Double somevalue)
            : this(somevalue, Physics.dimensionless)
        {
        }

        public Quantity(Unit someunit)
            : this(1, someunit)
        {
        }

        public Quantity(Double somevalue, Unit someunit)
        {
            this.value = somevalue;

            if (someunit != null)
            {
                this.unit = someunit;
            }
            else
            {
                this.unit = Physics.dimensionless;
            }
        }


        public Quantity(Double somevalue, BaseUnit someunit)
        {
            this.value = somevalue;

            if (someunit != null)
            {
                this.unit = someunit;
            }
            else
            {
                this.unit = Physics.dimensionless;
            }
        }

        public Quantity(Double somevalue, DerivedUnit someunit)
        {
            this.value = somevalue;

            if (someunit != null)
            {
                this.unit = someunit;
            }
            else
            {
                this.unit = Physics.dimensionless;
            }
        }

        public Quantity(Double somevalue, NamedDerivedUnit someunit)
        {
            this.value = somevalue;

            if (someunit != null)
            {
                this.unit = someunit;
            }
            else
            {
                this.unit = Physics.dimensionless;
            }
        }

        public Quantity(Double somevalue, INamedSymbolUnit someunit)
        {
            this.value = somevalue;

            if (someunit != null)
            {
                this.unit = (Unit)someunit;
            }
            else
            {
                this.unit = Physics.dimensionless;
            }
        }

        public Quantity(Double somevalue, ConvertibleUnit someunit)
        {
            this.value = somevalue;

            if (someunit != null)
            {
                this.unit = someunit;
            }
            else
            {
                this.unit = Physics.dimensionless;
            }
        }

        public Quantity(Double somevalue, CombinedUnit someunit)
        {
            this.value = somevalue;

            if (someunit != null)
            {
                this.unit = someunit;
            }
            else
            {
                this.unit = Physics.dimensionless;
            }
        }


        public Quantity(Quantity somephysicalquantity)
        {
            if (somephysicalquantity != null)
            {
                this.value = somephysicalquantity.Value;
                this.unit = somephysicalquantity.Unit;
            }
            else
            {
                this.value = 0;
                this.unit = Physics.dimensionless;
            }
        }

        public Quantity(Double somevalue, Quantity somephysicalquantity)
            : this(somevalue * somephysicalquantity.Value, somephysicalquantity.Unit)
        {
        }

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is Quantity)
            {
                Quantity temp = (Quantity)obj;

                Quantity tempconverted = temp.ConvertTo(this.Unit);
                if (tempconverted != null)
                {
                    return value.EpsilonCompareTo(tempconverted.Value);
                }

                throw new ArgumentException("object's physical unit " + temp.Unit.ToPrintString() + " is not convertible to a " + this.Unit.ToPrintString());
            }

            throw new ArgumentException("object is not a IQuantity");
        }

        /// <summary>
        /// IFormattable.ToString implementation.
        /// </summary>
        public String ToString(String format, IFormatProvider formatProvider)
        {
            Double unitValue = this.Unit.FactorValue;
            Unit pureUnit = this.Unit.PureUnit;
            return pureUnit.ValueAndUnitString(this.Value * unitValue, format, formatProvider);
        }

        public String ToString(String format)
        {
            Double unitValue = this.Unit.FactorValue;
            Unit pureUnit = this.Unit.PureUnit;
            return pureUnit.ValueAndUnitString(this.Value * unitValue, format);
        }

        public override String ToString()
        {
            Double unitValue = this.Unit.FactorValue;
            Unit pureUnit = this.Unit.PureUnit;
            return pureUnit.ValueAndUnitString(this.Value * unitValue);
        }

        public virtual String ToPrintString()
        {
           Double unitValue = this.Unit.FactorValue;
            Unit pureUnit = this.Unit.PureUnit;
            String valStr = pureUnit.ValueString(this.Value * unitValue);
            String unitStr = pureUnit.ToString();

            if (String.IsNullOrEmpty(unitStr))
            {
                return valStr;
            }
            else
            {
                return valStr + " " + unitStr;
            }
        }

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// // [whitespace] [number] [whitespace] [prefix] [unitsymbol] [whitespace]
        /// [whitespace] [number] [whitespace] [unit] [whitespace]
        /// </summary>
        public static Boolean TryParse(String physicalQuantityStr, System.Globalization.NumberStyles styles, IFormatProvider provider, out Quantity result)
        {
            result = null;

            String[] strings = physicalQuantityStr.Trim().Split(' ');

            if (strings.GetLength(0) > 0)
            {
                // Parse numerical value
                String numValueStr = strings[0];
                Double numValue;

                if (!Double.TryParse(numValueStr, styles, provider, out numValue))
                {
                    if (!Double.TryParse(numValueStr, styles, null, out numValue)) // Try  to use Default Format Provider
                    {
                        if (!Double.TryParse(numValueStr, styles, NumberFormatInfo.InvariantInfo, out numValue))     // Try  to use invariant Format Provider
                        {
                            return false;
                        }
                    }
                }

                Unit unit = null;

                if (strings.GetLength(0) > 1)
                {
                    // Parse unit
                    String unitStr = strings[1];
                    unit = Unit.Parse(unitStr);
                }
                else
                {
                    unit = Physics.dimensionless;
                }

                result = new Quantity(numValue, unit);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Parses the physical quantity from a string in form
        /// // [whitespace] [number] [whitespace] [prefix] [unitSymbol] [whitespace]
        /// [whitespace] [number] [whitespace] [unit] [whitespace]
        /// </summary>
        public static Boolean TryParse(String physicalQuantityStr, System.Globalization.NumberStyles styles, out Quantity result) => TryParse(physicalQuantityStr, styles, null, out result);

        public static Boolean TryParse(String physicalQuantityStr, out Quantity result) => TryParse(physicalQuantityStr, System.Globalization.NumberStyles.Float, null, out result);

        public static Quantity Parse(String physicalQuantityStr, System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.Float, IFormatProvider provider = null)
        {
            Quantity result;
            if (!TryParse(physicalQuantityStr, styles, provider, out result))
            {
                throw new ArgumentException("Not a valid physical quantity format", nameof(physicalQuantityStr));
            }

            return result;
        }

        public Quantity Zero => new Quantity(0, this.Unit.Dimensionless);
        public Quantity One => new Quantity(1, this.Unit.Dimensionless);

        public override Int32 GetHashCode() => this.Value.GetHashCode() + this.Unit.GetHashCode();

        public Quantity ConvertToSystemUnit()
        {
            if (this.Unit.SimpleSystem != null)
            {
                return this;
            }

            Double d = this.Value;
            Quantity pq = this.Unit.ConvertToSystemUnit(ref d);
            return pq;
        }

        public Quantity ConvertToBaseUnit()
        {
            Quantity pq = this.Unit.ConvertToBaseUnit(this.Value);
            return pq;
        }

        public Quantity ConvertToDerivedUnit()
        {
            Quantity pq_baseunit = this.Unit.ConvertToBaseUnit(this.Value);
            Quantity pq_derivedunit = pq_baseunit.Unit.ConvertToDerivedUnit().Multiply(pq_baseunit.Value);
            return pq_derivedunit;
        }


        // Auto detecting if specific or relative unit conversion

        public Quantity this[BaseUnit convertToUnit] => this.ConvertTo(convertToUnit);
        public Quantity this[DerivedUnit convertToUnit] => this.ConvertTo(convertToUnit);
        public Quantity this[NamedDerivedUnit convertToUnit] => this.ConvertTo(convertToUnit);
        public Quantity this[ConvertibleUnit convertToUnit] => this.ConvertTo(convertToUnit);
        public Quantity this[CombinedUnit convertToUnit] => this.ConvertTo(convertToUnit);
        public Quantity this[MixedUnit convertToUnit] => this.ConvertTo(convertToUnit);
        public Quantity this[Unit convertToUnit] => this.ConvertTo(convertToUnit);

        public Quantity this[Quantity convertToUnit] => this.ConvertTo(convertToUnit);

        public Quantity ConvertTo(Unit convertToUnit)
        {
            if (Object.ReferenceEquals(this.Unit, convertToUnit))
            {
                // Convert to its own unit; No conversion needed
                return this;
            }

            if (this.Unit == null)
            {
                if (convertToUnit == null || (convertToUnit.IsDimensionless)) // || convertToUnit == 1)) // One ))
                {   // Any dimensionless can be converted to any systems dimensionless
                    Debug.Assert(convertToUnit != null);
                    Quantity quantity = new Quantity(this.Value * 1 / convertToUnit.FactorValue, convertToUnit);
                    return quantity;
                }
                else
                {   // No dimensionless can be converted to or from any non dimensionless.
                    return null;
                }
            }

            Debug.Assert(this.Unit != null);
            Debug.Assert(convertToUnit != null);

            if (convertToUnit == null)
            {
                throw new ArgumentNullException(nameof(convertToUnit));
            }

            if (this.Unit.Kind != UnitKind.CombinedUnit && convertToUnit.Kind != UnitKind.CombinedUnit)
            {
                Boolean thisIsDimensionless = this.Unit.IsDimensionless;
                Boolean toIsDimensionless = convertToUnit.IsDimensionless;
                if (thisIsDimensionless != toIsDimensionless)
                {   // No dimensionless can be converted to or from any non-dimensionless.
                    return null;
                }
                if (thisIsDimensionless && toIsDimensionless)
                {
                    // Any dimensionless can be converted to any systems dimensionless
                    Quantity quantity = new Quantity(this.Value * this.Unit.FactorValue / convertToUnit.FactorValue, convertToUnit);
                    return quantity;
                }
            }
            else
            if (this.Unit.Kind == UnitKind.CombinedUnit && convertToUnit.Kind == UnitKind.CombinedUnit)
            {

            }
            else
            {


            }

            IUnitSystem convertToUnitsystem = convertToUnit.SimpleSystem;
            if (convertToUnitsystem == null)
            {
                convertToUnitsystem = convertToUnit.ExponentsSystem;
                Debug.WriteLine("convertToUnitsystem assigned from convertToUnit.ExpresionsSystem");
            }

            Debug.Assert(convertToUnitsystem != null);
            if (convertToUnitsystem == null)
            {
                convertToUnitsystem = this.Unit.SimpleSystem;
                Debug.WriteLine("convertToUnitsystem assigned from this.Unit.System");
            }
            if (convertToUnitsystem == null)
            {
                Debug.WriteLine("convertToUnitsystem assigned from Physics.Default_UnitSystem");
                convertToUnitsystem = Physics.CurrentUnitSystems.Default;
            }

            if (convertToUnitsystem != null)
            {
                // Let convertToUnitsystem do auto detecting of specific or relative unit conversion
                Quantity quantity = convertToUnitsystem.ConvertTo(this, convertToUnit);
                return quantity;
            }

            return null;
        }

        public Quantity ConvertTo(BaseUnit convertToUnit) => this.ConvertTo((Unit)convertToUnit);
        public Quantity ConvertTo(DerivedUnit convertToUnit) => this.ConvertTo((Unit)convertToUnit);
        public Quantity ConvertTo(ConvertibleUnit convertToUnit) => this.ConvertTo((Unit)convertToUnit);
        public Quantity ConvertTo(CombinedUnit convertToUnit) => this.ConvertTo((Unit)convertToUnit);
        public Quantity ConvertTo(MixedUnit convertToUnit) => this.ConvertTo((Unit)convertToUnit);

        public Quantity ConvertTo(Quantity convertToUnit) => this.ConvertTo(convertToUnit.Unit);

        public Quantity ConvertTo(IUnitSystem convertToUnitSystem)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;

            if (convertFromUnitsystem == convertToUnitSystem)
            {
                return this;
            }

            Debug.Assert(this.Unit != null);
            Debug.Assert(convertToUnitSystem != null);

            if (this.Unit == null)
            {
                throw new InvalidOperationException("Must have a unit to convert it");
            }

            if (convertToUnitSystem == null)
            {
                throw new ArgumentNullException(nameof(convertToUnitSystem));
            }

            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion
                Quantity quantity = convertFromUnitsystem.ConvertTo(this, convertToUnitSystem);
                return quantity;
            }

            return null;
        }

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)1
        public Quantity RelativeConvertTo(Unit convertToUnit)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;
            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion
                Quantity quantity = convertFromUnitsystem.ConvertTo(this.Unit, convertToUnit).Multiply(this.Value);
                return quantity;
            }
            return null;
        }

        public Quantity RelativeConvertTo(IUnitSystem convertToUnitSystem)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;
            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion
                Quantity quantity = convertFromUnitsystem.ConvertTo(this.Unit, convertToUnitSystem).Multiply(this.Value);
                return quantity;
            }
            return null;
        }

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        public Quantity SpecificConvertTo(Unit convertToUnit)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;
            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion
                Quantity quantity = convertFromUnitsystem.ConvertTo(this, convertToUnit);
                return quantity;
            }
            return null;
        }

        public Quantity SpecificConvertTo(IUnitSystem convertToUnitSystem)
        {
            IUnitSystem convertFromUnitsystem = this.Unit.SimpleSystem;
            if (convertFromUnitsystem != null)
            {
                // Let unit's unit system do auto detecting of specific or relative unit conversion
                Quantity quantity = convertFromUnitsystem.ConvertTo(this, convertToUnitSystem);
                return quantity;
            }
            return null;
        }


        public static Boolean IsPureUnit(Quantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);

            return physicalQuantity.Value == 1;
        }

        public static Unit AsPureUnit(Quantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);

            if (IsPureUnit(physicalQuantity))
            {
                return physicalQuantity.Unit;
            }

            Double prefixExponentD = Math.Log10(physicalQuantity.Value);
            SByte prefixExponent = (SByte)Math.Floor(prefixExponentD);
            if (prefixExponentD - prefixExponent == 0)
            {
                SByte newPrefixExponent = prefixExponent;
                Unit newUnit = physicalQuantity.Unit;
                if (physicalQuantity.Unit.Kind == UnitKind.PrefixedUnit)
                {
                    IPrefixedUnit prefixUnit = physicalQuantity.Unit as PrefixedUnit;
                    newPrefixExponent = (SByte)(prefixExponent + prefixUnit.Prefix.Exponent);
                    newUnit = (Unit)prefixUnit.Unit;
                }

                INamedSymbolUnit namedSymbolUnit = newUnit as INamedSymbolUnit;
                if (namedSymbolUnit != null)
                {
                    if (newPrefixExponent == 0)
                    {
                        return (Unit)namedSymbolUnit;
                    }

                    IUnitPrefix unitPrefix;
                    if (physicalQuantity.Unit.SimpleSystem.UnitPrefixes.GetUnitPrefixFromExponent(new UnitPrefixExponent(newPrefixExponent), out unitPrefix))
                    {
                        return new PrefixedUnit(unitPrefix, namedSymbolUnit);
                    }
                }
                if (physicalQuantity.Unit.Kind == UnitKind.CombinedUnit)
                {
                    CombinedUnit combinedUnit = (CombinedUnit)physicalQuantity.Unit;
                    IUnitPrefix unitPrefix;
                    if (physicalQuantity.Unit.SimpleSystem.UnitPrefixes.GetUnitPrefixFromExponent(new UnitPrefixExponent(prefixExponent), out unitPrefix))
                    {
                        return new CombinedUnit(unitPrefix, combinedUnit);
                    }
                }
            }

            return null;
        }

        public static Unit GetAsNamedUnit(Quantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);

            Double prefixExponentD = Math.Log10(physicalQuantity.Value);
            SByte prefixExponent = (SByte)Math.Floor(prefixExponentD);
            if (prefixExponentD - prefixExponent == 0)
            {
                SByte newPrefixExponent = prefixExponent;
                Unit newUnit = physicalQuantity.Unit;

                if (physicalQuantity.Unit.Kind == UnitKind.CombinedUnit)
                {
                    CombinedUnit combinedUnit = (CombinedUnit)physicalQuantity.Unit;
                    Unit namedDerivatedUnit = (Unit)combinedUnit.SomeSimpleSystem.NamedDerivedUnitFromUnit(combinedUnit);
                    if (namedDerivatedUnit != null)
                    {
                        newUnit = namedDerivatedUnit;
                    }
                }

                if (physicalQuantity.Unit.Kind == UnitKind.DerivedUnit)
                {
                    DerivedUnit derivedUnit = physicalQuantity.Unit as DerivedUnit;
                    Unit namedDerivatedUnit = (Unit)derivedUnit.SimpleSystem.NamedDerivedUnitFromUnit(derivedUnit);
                    newUnit = namedDerivatedUnit;
                }

                if (physicalQuantity.Unit.Kind == UnitKind.PrefixedUnit)
                {
                    IPrefixedUnit prefixUnit = physicalQuantity.Unit as PrefixedUnit;
                    newPrefixExponent = (SByte)(prefixExponent + prefixUnit.Prefix.Exponent);
                    newUnit = (Unit)prefixUnit.Unit;
                }

                INamedSymbolUnit namedSymbolUnit = newUnit as INamedSymbolUnit;
                if (namedSymbolUnit != null)
                {
                    if (newPrefixExponent == 0)
                    {
                        return (Unit)namedSymbolUnit;
                    }

                    IUnitPrefix unitPrefix;
                    if (physicalQuantity.Unit.SimpleSystem.UnitPrefixes.GetUnitPrefixFromExponent(new UnitPrefixExponent(newPrefixExponent), out unitPrefix))
                    {
                        return new PrefixedUnit(unitPrefix, namedSymbolUnit);
                    }
                }
            }

            return null;
        }

        public static Unit PureUnit(Quantity physicalQuantity)
        {
            Unit pureUnit = AsPureUnit(physicalQuantity);
            if (pureUnit == null)
            {
                throw new ArgumentException("Physical quantity is not a pure unit; but has a value = " + physicalQuantity.Value.ToString());
            }

            return pureUnit;
        }

        public static PrefixedUnit AsPrefixedUnit(Quantity physicalQuantity)
        {
            Debug.Assert(physicalQuantity != null);

            Unit unit = AsPureUnit(physicalQuantity);
            if (unit == null)
            {
                return null;
            }

            PrefixedUnit prefixedUnit = unit as PrefixedUnit;
            return prefixedUnit;
        }

        public Quantity AsNamedUnit
        {
            get
            {
                Unit namedUnit = Unit.AsNamedUnit;

                if (namedUnit != null)
                {
                    return new Quantity(this.Value, namedUnit);
                }

                return this;
            }
        }

        public static Unit operator !(Quantity physicalQuantity) => PureUnit(physicalQuantity);

        //public static implicit operator Unit(Quantity physicalQuantity) => PureUnit(physicalQuantity) as Unit;

        public Boolean Equivalent(Quantity other, out Double quotient)
        {
            Debug.Assert(other != null);
            Debug.Assert(this.Unit != null);
            Debug.Assert(other.Unit != null);

            if (Object.ReferenceEquals(null, other))
            {
                quotient = 0;
                return false;
            }

            Quantity other2 = other.ConvertTo(this.Unit);
            if (Object.ReferenceEquals(null, other2))
            {
                quotient = 0;
                return false;
            }

            quotient = this.Value / other2.Value;
            return true;
        }

        public Boolean Equivalent(Quantity other)
        {
            Double quotient;
            return Equivalent(other, out quotient);
        }

        public Boolean Equals(Quantity other)
        {
            if (Object.ReferenceEquals(null, other))
            {
                return false;
            }

            Debug.Assert(other != null);
            Debug.Assert(this.Unit != null);
            // Debug.Assert(other.Unit != null);

            Quantity other2 = other.ConvertTo(this.Unit);
            if (Object.ReferenceEquals(null, other2))
            {
                return false;
            }
            return this.Value.EpsilonCompareTo(other2.Value) == 0;
        }


        public Boolean Equals(Unit other)
        {
            Quantity otherPhysicalQuantity = new Quantity(1, other);
            return this.Equals(otherPhysicalQuantity);
        }

        public Boolean Equals(double other)
        {
            Quantity otherPhysicalQuantity = new Quantity(other);
            return this.Equals(otherPhysicalQuantity);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
            {
                return base.Equals(obj);
            }

            Quantity pq = obj as Quantity;
            if (pq != null)
            {
                return Equals(pq);
            }

            Unit pu = obj as Unit;
            if (pu != null)
            {
                return Equals(pu);
            }

            //throw new InvalidCastException("The 'obj' argument is not a IQuantity object or IUnit object.");
            Debug.Assert(obj is Quantity || obj is Unit);

            return false;
        }

        public static Boolean operator ==(Quantity pq1, Quantity pq2)
        {
            if ((Object)pq1 == null)
            {
                return (Object)pq2 == null;
            }

            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.Equals(pq2);
        }

        public static Boolean operator ==(Quantity pq1, IQuantity pq2)
        {
            if ((Object)pq1 == null)
            {
                return (Object)pq2 == null;
            }

            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.Equals(pq2);
        }


        public static Boolean operator !=(Quantity pq1, Quantity pq2)
        {
            if ((Object)pq1 == null)
            {
                return (Object)pq2 != null;
            }

            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return !pq1.Equals(pq2);
        }

        public static Boolean operator !=(Quantity pq1, IQuantity pq2)
        {
            if ((Object)pq1 == null)
            {
                return (Object)pq2 != null;
            }

            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return !pq1.Equals(pq2);
        }

        public static Boolean operator <(Quantity pq1, Quantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) < 0;
        }

        public static Boolean operator <=(Quantity pq1, Quantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) <= 0;
        }

        public static Boolean operator >(Quantity pq1, Quantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) > 0;
        }

        public static Boolean operator >=(Quantity pq1, Quantity pq2)
        {
            Debug.Assert(!Object.ReferenceEquals(null, pq1));

            return pq1.CompareTo(pq2) >= 0;
        }

        #region Physical Quantity static operator methods

        protected delegate Double CombineValuesFunc(Double v1, Double v2);
        protected delegate IUnit CombineUnitsFunc(IUnit u1, IUnit u2);

        protected static Quantity CombineValues(IQuantity pq1, IQuantity pq2, CombineValuesFunc cvf)
        {
            if (pq1.Unit != pq2.Unit)
            {
                Quantity temp_pq2 = pq2.ConvertTo(pq1.Unit);
                if (temp_pq2 == null)
                {
                    throw new ArgumentException("object's physical unit " + pq2.Unit.ToPrintString() + " is not convertible to unit " + pq1.Unit.ToPrintString());
                }

                pq2 = temp_pq2;
            }
            return new Quantity(cvf(pq1.Value, pq2.Value), pq1.Unit);
        }

        protected static Quantity CombineUnitsAndValues(IQuantity pq1, IQuantity pq2, CombineValuesFunc cvf, CombineExponentsFunc cef)
        {
            Debug.Assert(pq1.Unit.Kind != UnitKind.CombinedUnit);
            Debug.Assert(pq2.Unit.Kind != UnitKind.CombinedUnit);

            if (pq1.Unit.Kind == UnitKind.CombinedUnit)
            {
                CombinedUnit pg1_unit = (CombinedUnit)pq1.Unit;
                pq1 = pq1.ConvertToSystemUnit();
            }

            if (pq2.Unit.Kind == UnitKind.CombinedUnit)
            {
                CombinedUnit pg2_unit = (CombinedUnit)pq2.Unit;
                pq2 = pq2.ConvertToSystemUnit();
            }

            while (pq1.Unit.Kind == UnitKind.ConvertibleUnit)
            {
                ConvertibleUnit pg1_unit = (ConvertibleUnit)pq1.Unit;
                pq1 = pq1.ConvertTo(pg1_unit.PrimaryUnit);
            }
            while (pq2.Unit.Kind == UnitKind.ConvertibleUnit)
            {
                ConvertibleUnit pg2_unit = (ConvertibleUnit)pq2.Unit;
                pq2 = pq2.ConvertTo(pg2_unit.PrimaryUnit);
            }

            IUnitSystem pq2UnitSystem = pq2.Unit.SimpleSystem;
            IUnitSystem pq1UnitSystem = pq1.Unit.SimpleSystem;
            if (pq2UnitSystem != pq1UnitSystem)
            {   // Must be same unit system
                pq2 = pq2.ConvertTo(pq1UnitSystem);
                Debug.Assert(pq2 != null);
            }

            SByte minNoOfBaseUnits = (SByte)Math.Min(pq1.Unit.Exponents.Length, pq2.Unit.Exponents.Length);
            SByte maxNoOfBaseUnits = (SByte)Math.Max(pq1.Unit.Exponents.Length, pq2.Unit.Exponents.Length);
            Debug.Assert(maxNoOfBaseUnits <= Physics.NoOfBaseQuanties);

            SByte[] someexponents = new SByte[Physics.NoOfBaseQuanties];

            for (int i = 0; i < minNoOfBaseUnits; i++)
            {
                someexponents[i] = cef(pq1.Unit.Exponents[i], pq2.Unit.Exponents[i]);
            }

            for (int i = minNoOfBaseUnits; i < maxNoOfBaseUnits; i++)
            {
                if (pq1.Unit.Exponents.Length > pq2.Unit.Exponents.Length)
                {
                    someexponents[i] = cef(pq1.Unit.Exponents[i], 0);
                }
                else
                {
                    someexponents[i] = cef(0, pq2.Unit.Exponents[i]);
                }
            }

            Debug.Assert(pq1.Unit.ExponentsSystem != null);
            Unit pu = new DerivedUnit(pq1.Unit.ExponentsSystem, someexponents);
            return new Quantity(cvf(pq1.Value, pq2.Value), pu);
        }

        public static Quantity operator +(Quantity pq1, Quantity pq2) => new Quantity(pq1.Add(pq2));

        public static Quantity operator -(Quantity pq1, Quantity pq2) => new Quantity(pq1.Subtract(pq2));

        public static Quantity operator *(Quantity pq1, Quantity pq2)
        {
            Quantity pq = pq1.Unit.Multiply(pq1.Value, pq2);
            return pq;
        }

        public static Quantity operator /(Quantity pq1, Quantity pq2)
        {
            Quantity pq = pq1.Unit.Divide(pq1.Value, pq2);
            return pq;
        }

        public static Quantity operator *(Quantity pq, IUnitPrefix up) => new Quantity(pq.Value * up.Value, pq.Unit);

        public static Quantity operator *(IUnitPrefix up, Quantity pq) => new Quantity(pq.Value * up.Value, pq.Unit);

        public static Quantity operator *(Quantity pq, Double d) => new Quantity(pq.Value * d, pq.Unit);

        public static Quantity operator /(Quantity pq, Double d) => new Quantity(pq.Value / d, pq.Unit);

        public static Quantity operator *(Double d, Quantity pq) => new Quantity(pq.Multiply(d));

        public static Quantity operator /(Double d, Quantity pq) => new Quantity(new Quantity(d).Divide(pq));

        public static Quantity operator *(Quantity pq, NamedDerivedUnit pu) => new Quantity(pq.Value, pq.Unit.Multiply((INamedSymbolUnit)pu));

        public static Quantity operator /(Quantity pq, NamedDerivedUnit pu) => new Quantity(pq.Value, pq.Unit.Divide((INamedSymbolUnit)pu));

        public static Quantity operator *(Quantity pq, Unit pu) => new Quantity(pq.Value, pq.Unit.Multiply(pu));

        public static Quantity operator /(Quantity pq, Unit pu) => new Quantity(pq.Value, pq.Unit.Divide(pu));

        public static Quantity operator *(Unit pu, Quantity pq) => new Quantity(pq.Value, pu.Multiply(pq.Unit));

        public static Quantity operator /(Unit pu, Quantity pq) => new Quantity(pq.Value, pu.Divide(pq.Unit));

        public static Quantity operator ^(Quantity pq, SByte exponent) => pq.Power(exponent);

        public static Quantity operator %(Quantity pq, SByte exponent) => pq.Root(exponent);

        public static Quantity operator |(Quantity pq, SByte exponent) => pq.Root(exponent);

        #endregion Physical Quantity static operator methods

        public Quantity Power(SByte exponent)
        {
            Unit pu = this.Unit;
            if (pu == null)
            {
                pu = Physics.CurrentUnitSystems.Default.Dimensionless;
            }
            Unit pu_pow = pu.Pow(exponent);
            Double value = System.Math.Pow(this.Value, exponent);
            return new Quantity(value, pu_pow);
        }

        public Quantity Root(SByte exponent)
        {
            Unit pu = this.Unit;
            if (pu == null)
            {
                pu = Physics.CurrentUnitSystems.Default.Dimensionless;
            }
            Unit pu_rot = pu.Rot(exponent);
            Double value = System.Math.Pow(this.Value, 1.0 / exponent);
            return new Quantity(value, pu_rot);
        }

        #region Physical Quantity IPhysicalUnitMath implementation

        public Quantity Add(Quantity physicalQuantity) => CombineValues(this, physicalQuantity, (Double v1, Double v2) => v1 + v2);

        public Quantity Subtract(Quantity physicalQuantity) => CombineValues(this, physicalQuantity, (Double v1, Double v2) => v1 - v2);

        public Quantity Abs() => new Quantity(Math.Abs(this.Value), this.Unit);


        public Quantity Multiply(INamedSymbolUnit physicalUnit) => this.Multiply(new PrefixedUnitExponent(null, physicalUnit, 1));

        public Quantity Divide(INamedSymbolUnit physicalUnit) => this.Divide(new PrefixedUnitExponent(null, physicalUnit, 1));

        public Quantity Multiply(Unit physicalUnit) => this.Unit.Multiply(physicalUnit).Multiply(this.Value);

        public Quantity Divide(Unit physicalUnit) => this.Unit.Divide(physicalUnit).Multiply(this.Value);

        public Quantity Multiply(Quantity physicalQuantity) => this.Unit.Multiply(this.Value, physicalQuantity);

        public Quantity Divide(Quantity physicalQuantity) => this.Unit.Divide(this.Value, physicalQuantity);

        public Quantity Multiply(Double value) => new Quantity(this.Value * value, this.Unit);

        public Quantity Divide(Double value) => new Quantity(this.Value / value, this.Unit);

        public Quantity Pow(SByte exponent) => this.Power(exponent);

        public Quantity Rot(SByte exponent) => this.Root(exponent);

        public Quantity Multiply(PrefixedUnit prefixedUnit)
        {
            Unit pu = this.Unit.Multiply(prefixedUnit);
            return new Quantity(this.Value, pu);
        }

        public Quantity Divide(PrefixedUnit prefixedUnit)
        {
            Unit pu = this.Unit.Divide(prefixedUnit);
            return new Quantity(this.Value, pu);
        }


        public Quantity Multiply(PrefixedUnitExponent prefixedUnitExponent)
        {
            Unit pu = this.Unit.Multiply((IPrefixedUnitExponent)prefixedUnitExponent);
            return new Quantity(Value, pu);
        }

        public Quantity Divide(PrefixedUnitExponent prefixedUnitExponent)
        {
            Unit pu = this.Unit.Divide((IPrefixedUnitExponent)prefixedUnitExponent);
            return new Quantity(this.Value, pu);
        }

        public Quantity Multiply(IUnitPrefixExponent prefix)
        {
            Unit pu = this.Unit.Multiply(prefix);
            return new Quantity(this.Value, pu);
        }

        public Quantity Divide(IUnitPrefixExponent prefix)
        {
            Unit pu = this.Unit.Divide(prefix);
            return new Quantity(this.Value, pu);
        }

        public Quantity Multiply(Double value, Quantity physicalQuantity)
        {
            Quantity pq = this.Unit.Multiply(this.Value * value, physicalQuantity);
            return pq;
        }

        public Quantity Divide(Double value, Quantity physicalQuantity)
        {
            Quantity pq = this.Unit.Divide(value, physicalQuantity).Multiply(this.Value);
            return pq;
        }

        #endregion Physical Quantity IPhysicalUnitMath implementation
    }

    #endregion Physical Quantity Classes


    public class UnitSystemStack
    {
        protected Stack<IUnitSystem> default_UnitSystem_Stack = new Stack<IUnitSystem>();

        public IUnitSystem Default
        {
            get
            {
                if (default_UnitSystem_Stack == null || default_UnitSystem_Stack.Count <= 0)
                {
                    return Physics.SI_Units;
                }
                else
                {
                    return default_UnitSystem_Stack.Peek();
                }
            }
        }

        public Boolean Use(IUnitSystem newUnitSystem)
        {
            if (Default != newUnitSystem)
            {
                default_UnitSystem_Stack.Push(newUnitSystem);
                return true;
            }
            return false;
        }

        public Boolean Unuse(IUnitSystem oldUnitSystem)
        {
            if (default_UnitSystem_Stack != null && default_UnitSystem_Stack.Count > 0 && default_UnitSystem_Stack.Peek() == oldUnitSystem)
            {
                default_UnitSystem_Stack.Pop();
                return true;
            }
            return false;
        }

        public void Reset()
        {
            default_UnitSystem_Stack.Clear();
        }
    }

    public class UnitLookup //(SingleUnitSystem[] unitSystems)
    {
        protected UnitSystem[] unitSystems;

        public UnitLookup(UnitSystem[] someUnitSystems)
        {
            unitSystems = someUnitSystems;
        }

        public Unit UnitFromName(String namestr)
        {
            foreach (UnitSystem us in unitSystems)
            {
                INamedSymbolUnit unit = us.UnitFromName(namestr);
                if (unit != null)
                {
                    return (Unit)unit;
                }
            }
            return null;
        }

        public Unit UnitFromSymbol(String symbolstr)
        {
            foreach (UnitSystem us in unitSystems)
            {
                INamedSymbolUnit unit = us.UnitFromSymbol(symbolstr);
                if (unit != null)
                {
                    return (Unit)unit;
                }
            }
            return null;
        }

        public Unit ScaledUnitFromSymbol(String scaledsymbolstr)
        {
            foreach (UnitSystem us in unitSystems)
            {
                Unit scaledunit = us.ScaledUnitFromSymbol(scaledsymbolstr);
                if (scaledunit != null)
                {
                    return scaledunit;
                }
            }
            return null;
        }

        public IUnitSystem UnitSystemFromName(String UnitSystemsymbolstr)
        {
            IUnitSystem result_us = unitSystems.FirstOrNull<IUnitSystem>(us => us.Name == UnitSystemsymbolstr);
            return result_us;
        }
    }

    public class UnitSystemConversionLookup
    {

        public IList<UnitSystemConversion> UnitSystemConversions;

        public UnitSystemConversionLookup(IList<UnitSystemConversion> unitSystemConversions)
        {
            UnitSystemConversions = unitSystemConversions;
        }

        public UnitSystemConversion GetUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2)
        {
            UnitSystemConversion usc = GetDirectUnitSystemConversion(unitsystem1, unitsystem2);
            if (usc != null)
            {
                return usc;
            }

            /*  No direct unit system conversion from  unitsystem1 to unitsystem2.
             *  Try to find an intermediate unit system with conversion to/from unitsystem1 and unitsystem2 */
            return GetIntermediateUnitSystemConversion(unitsystem1, unitsystem2);
        }

        public UnitSystemConversion GetDirectUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2)
        {
            foreach (UnitSystemConversion usc in UnitSystemConversions)
            {
                if ((usc.BaseUnitSystem == unitsystem1 && usc.ConvertedUnitSystem == unitsystem2)
                    || (usc.BaseUnitSystem == unitsystem2 && usc.ConvertedUnitSystem == unitsystem1))
                {
                    return usc;
                }
            }

            return null;
        }

        public UnitSystemConversion GetIntermediateUnitSystemConversion(IUnitSystem unitsystem1, IUnitSystem unitsystem2)
        {
            /*  No direct unit system conversion from  unitsystem1 to unitsystem2.
             *  Try to find an intermediate unit system with conversion to/from unitsystem1 and unitsystem2 */

            IList<IUnitSystem> oldUnitsystems1 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems2
            IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>() { unitsystem1 };
            IList<IUnitSystem> oldUnitsystems2 = new List<IUnitSystem>() { }; // NoDiretConversionTounitsystems1
            IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>() { unitsystem2 };
            return GetIntermediateUnitSystemConversion(UnitSystemConversions, oldUnitsystems1, newUnitSystemsConvertableToUnitsystems1, oldUnitsystems2, newUnitSystemsConvertableToUnitsystems2);
        }


        public UnitSystemConversion GetIntermediateUnitSystemConversion(IList<UnitSystemConversion> unitSystemConversions,
                                                                        IList<IUnitSystem> oldUnitsystems1, IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems1,
                                                                        IList<IUnitSystem> oldUnitsystems2, IList<IUnitSystem> newUnitSystemsConvertableToUnitsystems2)
        {
            IList<IUnitSystem> unitSystemsConvertableToUnitsystems1 = new List<IUnitSystem>();
            IList<IUnitSystem> unitSystemsConvertableToUnitsystems2 = new List<IUnitSystem>();

            IList<UnitSystemConversion> unitsystems1Conversions = new List<UnitSystemConversion>();
            IList<UnitSystemConversion> unitsystems2Conversions = new List<UnitSystemConversion>();

            foreach (UnitSystemConversion usc in UnitSystemConversions)
            {
                Boolean BUS_in_US1 = newUnitSystemsConvertableToUnitsystems1.Contains(usc.BaseUnitSystem);
                Boolean CUS_in_US1 = newUnitSystemsConvertableToUnitsystems1.Contains(usc.ConvertedUnitSystem);

                Boolean BUS_in_US2 = newUnitSystemsConvertableToUnitsystems2.Contains(usc.BaseUnitSystem);
                Boolean CUS_in_US2 = newUnitSystemsConvertableToUnitsystems2.Contains(usc.ConvertedUnitSystem);

                if (BUS_in_US1 || CUS_in_US1)
                {
                    if (BUS_in_US2 || CUS_in_US2)
                    {
                        return usc;
                    }

                    Debug.Assert(!unitsystems1Conversions.Contains(usc));
                    unitsystems1Conversions.Add(usc);

                    if (!(BUS_in_US1 && CUS_in_US1))
                    {
                        if (BUS_in_US1)
                        {
                            unitSystemsConvertableToUnitsystems1.Add(usc.ConvertedUnitSystem);
                        }
                        else
                        {
                            unitSystemsConvertableToUnitsystems1.Add(usc.BaseUnitSystem);
                        }
                    }
                }
                else if (BUS_in_US2 || CUS_in_US2)
                {
                    Debug.Assert(!unitsystems2Conversions.Contains(usc));
                    unitsystems2Conversions.Add(usc);

                    if (!(BUS_in_US2 && CUS_in_US2))
                    {
                        if (BUS_in_US2)
                        {
                            unitSystemsConvertableToUnitsystems2.Add(usc.ConvertedUnitSystem);
                        }
                        else
                        {
                            unitSystemsConvertableToUnitsystems2.Add(usc.BaseUnitSystem);
                        }
                    }
                }
            }

            /*  No direct unit system conversion from  unitsystems1 to unitsystems2.
             *  Try to find an intermediate unit system with conversion to/from unitsystems1 and unitsystems2 */

            if (unitSystemsConvertableToUnitsystems1.Count > 0 || unitSystemsConvertableToUnitsystems2.Count > 0)
            {
                IList<IUnitSystem> unitsystems1 = (IList<IUnitSystem>)oldUnitsystems1.Union(newUnitSystemsConvertableToUnitsystems1).ToList();
                IList<IUnitSystem> unitsystems2 = (IList<IUnitSystem>)oldUnitsystems2.Union(newUnitSystemsConvertableToUnitsystems2).ToList();

                UnitSystemConversion subIntermediereUnitSystemConversion = null;

                IList<IUnitSystem> intersectUnitsystemsList = unitSystemsConvertableToUnitsystems1.Intersect(unitSystemsConvertableToUnitsystems2).ToList();

                if (intersectUnitsystemsList.Count > 0)
                {
                    IUnitSystem intersectUnitsystem = intersectUnitsystemsList[0];
                    subIntermediereUnitSystemConversion = GetIntermediateUnitSystemConversion(unitsystems1Conversions, new List<IUnitSystem>() { }, newUnitSystemsConvertableToUnitsystems1, new List<IUnitSystem>() { }, new List<IUnitSystem>() { intersectUnitsystem });
                    Debug.Assert(subIntermediereUnitSystemConversion != null);
                }
                else
                {
                    IList<UnitSystemConversion> notIntermediereUnitSystemConversions = (IList<UnitSystemConversion>)unitSystemConversions.Except(unitsystems1Conversions.Union(unitsystems2Conversions)).ToList();
                    if (notIntermediereUnitSystemConversions.Count > 0)
                    {
                        subIntermediereUnitSystemConversion = GetIntermediateUnitSystemConversion(notIntermediereUnitSystemConversions, unitsystems1, unitSystemsConvertableToUnitsystems1, unitsystems2, unitSystemsConvertableToUnitsystems2);
                    }
                }
                if (subIntermediereUnitSystemConversion != null)
                {
                    if (!unitsystems1.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem)
                        && !unitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem))
                    {
                        // Combine system conversion from some unit system in unitsystems1 to one of subIntermediereUnitSystemConversion's systems
                        // Find the first and second UnitSystemConversions which will be combined into a two step conversion

                        IUnitSystem combinedUnitSystemConversionBaseUnitSystem;
                        IUnitSystem combinedUnitSystemConversionIntermedierUnitSystem;
                        IUnitSystem combinedUnitSystemConversionConvertedUnitSystem;

                        UnitSystemConversion secondUnitSystemConversion = subIntermediereUnitSystemConversion;
                        Boolean secondValueConversionDirectionInverted = !unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem);
                        if (!secondValueConversionDirectionInverted)
                        {
                            combinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                            combinedUnitSystemConversionConvertedUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            Debug.Assert(unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem));
                            combinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                            combinedUnitSystemConversionConvertedUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                        }

                        UnitSystemConversion firstUnitSystemConversion = GetIntermediateUnitSystemConversion(unitsystems1Conversions, new List<IUnitSystem>() { }, unitsystems1, new List<IUnitSystem>() { }, new List<IUnitSystem>() { combinedUnitSystemConversionIntermedierUnitSystem });
                        Boolean firstValueConversionDirectionInverted = firstUnitSystemConversion.BaseUnitSystem == combinedUnitSystemConversionIntermedierUnitSystem;

                        if (!firstValueConversionDirectionInverted)
                        {
                            combinedUnitSystemConversionBaseUnitSystem = firstUnitSystemConversion.BaseUnitSystem;
                        }
                        else
                        {
                            combinedUnitSystemConversionBaseUnitSystem = firstUnitSystemConversion.ConvertedUnitSystem;
                        }

                        // Make the Combined unit system conversion
                        ValueConversion[] CombinedValueConversions = new ValueConversion[] {  new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[0], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[0], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[1], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[1], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[2], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[2], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[3], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[3], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[4], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[4], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[5], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[5], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[6], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[6], secondValueConversionDirectionInverted)
                                                                                                };

                        subIntermediereUnitSystemConversion = new UnitSystemConversion(combinedUnitSystemConversionBaseUnitSystem, combinedUnitSystemConversionConvertedUnitSystem, CombinedValueConversions);
                    }

                    if (!unitsystems2.Contains(subIntermediereUnitSystemConversion.BaseUnitSystem)
                        && !unitsystems2.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem))
                    {
                        // Combine system conversion from one of subIntermediereUnitSystemConversion's systems to some unit system in unitsystems2
                        // Find Post UnitSystemConversion

                        IUnitSystem combinedUnitSystemConversionBaseUnitSystem;
                        IUnitSystem combinedUnitSystemConversionIntermedierUnitSystem;
                        IUnitSystem combinedUnitSystemConversionConvertedUnitSystem;

                        UnitSystemConversion firstUnitSystemConversion = subIntermediereUnitSystemConversion;
                        Boolean firstValueConversionDirectionInverted = !unitSystemsConvertableToUnitsystems2.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem);
                        if (!firstValueConversionDirectionInverted)
                        {
                            combinedUnitSystemConversionBaseUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                            combinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            Debug.Assert(unitSystemsConvertableToUnitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem) || unitsystems1.Contains(subIntermediereUnitSystemConversion.ConvertedUnitSystem));

                            combinedUnitSystemConversionBaseUnitSystem = subIntermediereUnitSystemConversion.ConvertedUnitSystem;
                            combinedUnitSystemConversionIntermedierUnitSystem = subIntermediereUnitSystemConversion.BaseUnitSystem;
                        }

                        UnitSystemConversion secondUnitSystemConversion = GetIntermediateUnitSystemConversion(unitsystems2Conversions, new List<IUnitSystem>() { }, new List<IUnitSystem>() { combinedUnitSystemConversionIntermedierUnitSystem }, new List<IUnitSystem>() { }, unitsystems2);
                        Boolean secondValueConversionDirectionInverted = secondUnitSystemConversion.ConvertedUnitSystem == combinedUnitSystemConversionIntermedierUnitSystem;

                        if (!secondValueConversionDirectionInverted)
                        {
                            combinedUnitSystemConversionConvertedUnitSystem = secondUnitSystemConversion.ConvertedUnitSystem;
                        }
                        else
                        {
                            combinedUnitSystemConversionConvertedUnitSystem = secondUnitSystemConversion.BaseUnitSystem;
                        }

                        // Make the Combined unit system conversion
                        ValueConversion[] combinedValueConversions = new ValueConversion[] {  new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[0], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[0], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[1], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[1], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[2], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[2], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[3], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[3], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[4], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[4], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[5], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[5], secondValueConversionDirectionInverted),
                                                                                                    new CombinedValueConversion(firstUnitSystemConversion.BaseUnitConversions[6], firstValueConversionDirectionInverted, secondUnitSystemConversion.BaseUnitConversions[6], secondValueConversionDirectionInverted)
                                                                                                };

                        subIntermediereUnitSystemConversion = new UnitSystemConversion(combinedUnitSystemConversionBaseUnitSystem, combinedUnitSystemConversionConvertedUnitSystem, combinedValueConversions);
                    }
                    return subIntermediereUnitSystemConversion;
                }
            }

            return null;
        }
    }

    #endregion Physical Measure Classes
}


