/*   http://physicalmeasure.codeplex.com                          */
/*   http://en.wikipedia.org/wiki/International_System_of_Units   */
/*   http://en.wikipedia.org/wiki/Physical_quantity               */
/*   http://en.wikipedia.org/wiki/Physical_constant               */

using System;
using System.Collections.Generic;

namespace PhysicalMeasure
{

    #region Physical Measure Constants

    /**
    public static partial class Physics
    {
        public const int NoOfBaseQuanties = 7;
    }
    
    public static partial class Economics
    {
        public const int NoOfBaseQuanties = 1;
    }
    **/

    public enum PhysicalBaseQuantityKind
    {
        Length,
        Mass,
        Time,
        ElectricCurrent,
        ThermodynamicTemperature,
        AmountOfSubstance,
        LuminousIntensity
    }

    public enum MonetaryBaseQuantityKind
    {
        Currency // Monetary unit
    }

    public enum UnitKind
    {
        BaseUnit,
        DerivedUnit,
        ConvertibleUnit,

        PrefixedUnit,
        PrefixedUnitExponent,
        CombinedUnit,
        MixedUnit
    }
    
    #endregion Physical Measure Constants

    #region Physical Measure Interfaces

    public interface INamed
    {
        String Name { get; }
    }

    public interface INamedSymbol : INamed
    {
        String Symbol { get; }
    }

    public interface ISystemItem
    {
        /** Returns the unit system that this item is defined as a part of. 
         *  Must be a single simple unit system (not a combined unit system). 
         *  Can be null if and only if a combined unit combines sub units from different single in-convertible unit systems.
         **/
        IUnitSystem SimpleSystem { get; }
    }

    public interface IUnitPrefixExponent
    {
        SByte Exponent { get; }
        Double Value { get; }

        //  IUnitPrefixExponentMath
        IUnitPrefixExponent Multiply(IUnitPrefixExponent prefix);
        IUnitPrefixExponent Divide(IUnitPrefixExponent prefix);

        IUnitPrefixExponent Power(SByte exponent);
        IUnitPrefixExponent Root(SByte exponent);
    }

    public interface IUnitPrefix : INamed, IUnitPrefixExponent
    {
        Char PrefixChar { get; }
    }

    public interface IUnitPrefixTable
    {
        UnitPrefix[] UnitPrefixes { get; }

        Boolean GetUnitPrefixFromExponent(IUnitPrefixExponent someExponent, out IUnitPrefix unitPrefix);

        Boolean GetUnitPrefixFromPrefixChar(Char somePrefixChar, out IUnitPrefix unitPrefix);

        // 
        Boolean GetExponentFromPrefixChar(Char somePrefixChar, out IUnitPrefixExponent exponent);

        Boolean GetPrefixCharFromExponent(IUnitPrefixExponent someExponent, out Char prefixChar);

        IUnitPrefix UnitPrefixFromPrefixChar(Char somePrefixChar);

        IUnitPrefixExponent ExponentFromPrefixChar(Char somePrefixChar);
    }

    public interface INamedSymbolUnit : INamedSymbol, IUnit //  <BaseUnit | NamedDerivedUnit | ConvertiableUnit>
    {
        // Unprefixed unit, coherent unit symbol
    }

    public interface IPrefixedUnit : IUnit
    {
        IUnitPrefix Prefix { get; }
        INamedSymbolUnit Unit { get; }
    }

    public interface ICombinedUnitFormat
    {
        String CombinedUnitString(Boolean mayUseSlash = true, Boolean invertExponents = false);
    }

    public interface IAsQuantity
    {
        Quantity AsQuantity();
    }

    public interface IPrefixedUnitExponent : IPrefixedUnit, ICombinedUnitFormat, IAsQuantity
    {
        SByte Exponent { get; }

        PrefixedUnitExponent CombinePrefixAndExponents(SByte outerPUE_PrefixExponent, SByte outerPUE_Exponent, out SByte scaleExponent, out Double scaleFactor);
    }

    public interface IPrefixedUnitExponentList : IList<IPrefixedUnitExponent>, ICombinedUnitFormat
    {
        IPrefixedUnitExponentList Root(SByte exponent);
        IPrefixedUnitExponentList Power(SByte exponent);
    }

    public interface IUnitKindAccess
    {
        UnitKind Kind { get; }
    }

    public interface IUnitExponentsAccess
    {
        /** Returns the same unit system as property ISystemItem.SimpleSystem if all sub units are from that same system. 
         *  Can be a combined unit system if sub units reference different unit systems which are not convertible to each other (neither directly or indirectly). 
         *  Can not be null.
         **/
        IUnitSystem ExponentsSystem { get; }

        SByte[] Exponents { get; }
    }

    public interface IUnitStringFormatting
    {
        String PureUnitString();

        String UnitString();

        String UnitPrintString();

        String ToPrintString();

        String ReducedUnitString();

        String ValueString(Double value);
        String ValueString(Double value, String format, IFormatProvider formatProvider);

        String ValueAndUnitString(Double value);
        String ValueAndUnitString(Double value, String format, IFormatProvider formatProvider);

        Double FactorValue { get; }
    }

    public interface IPureUnitAccess
    {
        Unit PureUnit { get; }
    }

    public interface ISystemUnit : ISystemItem, IUnitKindAccess, IUnitExponentsAccess, IUnitStringFormatting, IPureUnitAccess
    {

    }

    public interface IPureUnitMath
    {
        Unit Multiply(IUnitPrefixExponent prefix);
        Unit Divide(IUnitPrefixExponent prefix);

        Unit Multiply(INamedSymbolUnit namedSymbolUnit);
        Unit Divide(INamedSymbolUnit namedSymbolUnit);


        Unit Multiply(PrefixedUnit prefixedUnit);
        Unit Divide(PrefixedUnit prefixedUnit);

        Unit Multiply(IPrefixedUnitExponent prefixedUnitExponent);
        Unit Divide(IPrefixedUnitExponent prefixedUnitExponent);


        Unit Multiply(Unit physicalUnit);
        Unit Divide(Unit physicalUnit);

        Unit Pow(SByte exponent);
        Unit Rot(SByte exponent);
    }

    public interface ICombinedUnitMath
    {
        CombinedUnit CombineMultiply(Double factor);
        CombinedUnit CombineDivide(Double factor);

        CombinedUnit CombineMultiply(IUnitPrefixExponent prefix);
        CombinedUnit CombineDivide(IUnitPrefixExponent prefix);

        CombinedUnit CombineMultiply(INamedSymbolUnit namedSymbolUnit);
        CombinedUnit CombineDivide(INamedSymbolUnit namedSymbolUnit);


        CombinedUnit CombineMultiply(PrefixedUnit prefixedUnit);
        CombinedUnit CombineDivide(PrefixedUnit prefixedUnit);

        CombinedUnit CombineMultiply(PrefixedUnitExponent prefixedUnitExponent);
        CombinedUnit CombineDivide(PrefixedUnitExponent prefixedUnitExponent);

        CombinedUnit CombineMultiply(Unit physicalUnit);
        CombinedUnit CombineDivide(Unit physicalUnit);

        CombinedUnit CombinePow(SByte exponent);
        CombinedUnit CombineRot(SByte exponent);
    }


    public interface IUnitItemMath
    {
        Unit Dimensionless { get; }
        Boolean IsDimensionless { get; }

        Quantity Multiply(Quantity physicalQuantity);
        Quantity Divide(Quantity physicalQuantity);

        Quantity Multiply(Double value);
        Quantity Divide(Double value);

        Quantity Multiply(Double value, Quantity physicalQuantity);
        Quantity Divide(Double value, Quantity physicalQuantity);
    }

    public interface IEquivalence<T>
    {
        // Like Equal, but allow to be off by a factor: this.Equivalent(other, out quotient) means (this == other * quotient) is true.
        Boolean Equivalent(T other, out Double quotient);
        Boolean Equivalent(T other);

        // Double Quotient(T other);   // quotient = 0 means not equivalent
    }

    public interface IUnitMath : IEquatable<Unit>, IEquivalence<Unit>, IPureUnitMath, ICombinedUnitMath, IUnitItemMath
    {

    }

    public interface IUnitConversion
    {
        Boolean IsLinearConvertible();


        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        Quantity this[Unit convertToUnit] { get; }
        Quantity this[Quantity convertToUnit] { get; }


        Quantity ConvertTo(Unit convertToUnit);
        Quantity ConvertTo(IUnitSystem convertToUnitSystem);

        Quantity ConvertToSystemUnit();   /// Unique defined simple unit system this unit is part of
        Quantity ConvertToBaseUnit();     /// Express the unit by base units only; No ConvertibleUnit, MixedUnit or NamedDerivatedUnit.
        Quantity ConvertToBaseUnit(IUnitSystem convertToUnitSystem);

        Quantity ConvertToDerivedUnit();


        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        Quantity ConvertTo(ref Double value, Unit convertToUnit);
        Quantity ConvertTo(ref Double value, IUnitSystem convertToUnitSystem);

        Quantity ConvertToSystemUnit(ref Double value);

        Quantity ConvertToBaseUnit(Double value);
        Quantity ConvertToBaseUnit(Quantity physicalQuantity);

        Quantity ConvertToBaseUnit(Double value, IUnitSystem convertToUnitSystem);
        Quantity ConvertToBaseUnit(Quantity physicalQuantity, IUnitSystem convertToUnitSystem);
    }

    public interface INamedUnit
    {
        Unit AsNamedUnit { get; }
    }

    public interface IUnit : ISystemUnit, IUnitMath, IUnitConversion, INamedUnit, IAsQuantity /*  : <BaseUnit | DerivedUnit | ConvertibleUnit | CombinedUnit | MixedUnit> */
    {
    }

    public interface IQuantityFactorMath
    {
        Quantity Multiply(IUnitPrefixExponent prefix);
        Quantity Divide(IUnitPrefixExponent prefix);

        Quantity Multiply(INamedSymbolUnit namedSymbolUnit);
        Quantity Divide(INamedSymbolUnit namedSymbolUnit);


        Quantity Multiply(PrefixedUnit prefixedUnit);
        Quantity Divide(PrefixedUnit prefixedUnit);

        Quantity Multiply(PrefixedUnitExponent prefixedUnitExponent);
        Quantity Divide(PrefixedUnitExponent prefixedUnitExponent);

        Quantity Multiply(Unit physicalUnit);
        Quantity Divide(Unit physicalUnit);

        Quantity Pow(SByte exponent);
        Quantity Rot(SByte exponent);
    }

    public interface IQuantityMath : IComparable, IEquivalence<Quantity>, IEquatable<Double>, IEquatable<Unit>, IEquatable<Quantity>, IQuantityFactorMath, IUnitItemMath
    {
        Quantity Zero { get; }
        Quantity One { get; }

        Quantity Add(Quantity physicalQuantity);
        Quantity Subtract(Quantity physicalQuantity);

        Quantity Abs();
    }

    public interface IQuantityConversion
    {
        // Auto detecting if specific or relative unit conversion 
        Quantity this[Unit convertToUnit] { get; }
        Quantity this[Quantity convertToUnit] { get; }

        Quantity ConvertTo(Unit convertToUnit);
        Quantity ConvertTo(IUnitSystem convertToUnitSystem);

        Quantity ConvertToSystemUnit();
        Quantity ConvertToBaseUnit();

        Quantity ConvertToDerivedUnit();

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        Quantity RelativeConvertTo(Unit convertToUnit);
        Quantity RelativeConvertTo(IUnitSystem convertToUnitSystem);

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        Quantity SpecificConvertTo(Unit convertToUnit);
        Quantity SpecificConvertTo(IUnitSystem convertToUnitSystem);
    }

    public interface IBaseUnit : IUnit, INamedSymbolUnit
    {
        SByte BaseUnitNumber { get; }
    }

    public interface IDerivedUnit : IUnit
    {
    }

    public interface INamedDerivedUnit : INamedSymbolUnit, IDerivedUnit
    {
    }

    public interface IValueConversion
    {
        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        Double Convert(Boolean backwards = false);
        Double ConvertToPrimaryUnit();
        Double ConvertFromPrimaryUnit();

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        Double Convert(Double value, Boolean backwards = false);
        Double ConvertToPrimaryUnit(Double value);
        Double ConvertFromPrimaryUnit(Double value);

        Double LinearOffset { get; }
        Double LinearScale { get; }
    }

    public interface IConvertibleUnit : INamedSymbolUnit
    {
        Unit PrimaryUnit { get; }
        IValueConversion Conversion { get; }

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        Quantity ConvertToPrimaryUnit();
        Quantity ConvertFromPrimaryUnit();


        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        Quantity ConvertToPrimaryUnit(Double value);
        Quantity ConvertFromPrimaryUnit(Double value);
    }

    public interface ICombinedUnit : IUnit
    {
        IPrefixedUnitExponentList Numerators { get; }
        IPrefixedUnitExponentList Denominators { get; }


        /** Returns the same unit system as property SimpleSystem if all sub units are from that same system. 
         *  Can be a combined unit system if sub units reference different unit systems which are not convertible to each other (neither directly or indirectly). 
         *  Can not be null.
        IUnitSystem ExponentsSystem { get; }
         **/


        /** Returns the same unit system as property System if all sub units are from that same system. 
         *  Can be a combined unit system if sub units reference different unit systems. 
         *  Can not be null.
         **/
        IUnitSystem SomeSimpleSystem { get; }

        CombinedUnit OnlySingleSystemUnits(IUnitSystem us);

        // Specific conversion
        Quantity ConvertFrom(Quantity physicalQuantity);
    }

    public interface IMixedUnit : IUnit
    {
        Unit MainUnit { get; }
        Unit FractionalUnit { get; }
        String Separator { get; }
    }
    public interface IQuantityNamedUnit
    {
        Quantity AsNamedUnit { get; }
    }


    public interface IQuantity : IFormattable, IQuantityMath, IQuantityConversion, IQuantityNamedUnit
    {
        Double Value { get; }
        Unit Unit { get; }

        String ToPrintString();
    }

    public interface IUnitSystem : INamed
    {
        bool IsIsolatedUnitSystem { get; }
        bool IsCombinedUnitSystem { get; }

        UnitPrefixTable UnitPrefixes { get; }
        BaseUnit[] BaseUnits { get; }
        NamedDerivedUnit[] NamedDerivedUnits { get; }
        ConvertibleUnit[] ConvertibleUnits { get; }

        Unit Dimensionless { get; }
        INamedSymbolUnit UnitFromName(String unitName);
        INamedSymbolUnit UnitFromSymbol(String unitSymbol);

        Unit ScaledUnitFromSymbol(String scaledUnitSymbol);

        INamedSymbolUnit NamedDerivedUnitFromUnit(Unit derivedUnit);
        // Unit NamedUnitFromUnit(Unit derivedUnit);

        Unit UnitFromExponents(SByte[] exponents);
        Unit UnitFromUnitInfo(SByte[] exponents, SByte NoOfNonZeroExponents, SByte NoOfNonOneExponents, SByte FirstNonZeroExponent);

        // Unspecific/relative non-quantity unit conversion (e.g. temperature interval)
        Quantity ConvertTo(Unit convertFromUnit, Unit convertToUnit);
        Quantity ConvertTo(Unit convertFromUnit, IUnitSystem convertToUnitSystem);

        // Specific/absolute quantity unit conversion (e.g. specific temperature)
        Quantity ConvertTo(Quantity physicalQuantity, Unit convertToUnit);
        Quantity ConvertTo(Quantity physicalQuantity, IUnitSystem convertToUnitSystem);

        Quantity SpecificConvertTo(Quantity physicalQuantity, Unit convertToUnit);
    }

    public interface ICombinedUnitSystem : IUnitSystem, IEquatable<ICombinedUnitSystem>
    {
        SByte[] UnitExponents(CombinedUnit cu);
        Quantity ConvertToBaseUnit(CombinedUnit cu);

        IUnitSystem[] UnitSystemes { get; }
        Boolean ContainsSubUnitSystem(IUnitSystem unitsystem);
        Boolean ContainsSubUnitSystems(IEnumerable<IUnitSystem> unitsystems);
    }

    #endregion Physical Measure Interfaces

}

