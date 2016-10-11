/*   http://physicalmeasure.codeplex.com                          */
/*   http://en.wikipedia.org/wiki/International_System_of_Units   */
/*   http://en.wikipedia.org/wiki/Physical_quantity               */
/*   http://en.wikipedia.org/wiki/Physical_constant               */

using System;
using System.Collections.Generic;

namespace PhysicalMeasure
{
    #region Physical Measure Static Classes
    public static /* partial */ class Physics
    {
        #region Physical Measure Constants

        public const int NoOfBaseQuanties = 7;

        #endregion Physical Measure Constants

        /*  http://en.wikipedia.org/wiki/SI_prefix 
            The International System of Units specifies twenty SI prefixes:

            SI prefixes   
                Prefix Symbol   1000^m      10^n    Decimal                     Short scale     Long scale      Since
                yotta   Y       1000^8      10^24   1000000000000000000000000   Septillion      Quadrillion     1991 
                zetta   Z       1000^7      10^21   1000000000000000000000      Sextillion      Trilliard       1991 
                exa     E       1000^6      10^18   1000000000000000000         Quintillion     Trillion        1975 
                peta    P       1000^5      10^15   1000000000000000            Quadrillion     Billiard        1975 
                tera    T       1000^4      10^12   1000000000000               Trillion        Billion         1960 
                giga    G       1000^3      10^9    1000000000                  Billion         Milliard        1960 
                mega    M       1000^2      10^6    1000000                             Million                 1960 
                kilo    k       1000^1      10^3    1000                                Thousand                1795 
                hecto   h       1000^2⁄3    10^2    100                                 Hundred                 1795 
                deca    da      1000^1⁄3    10^1    10                                  Ten                     1795 
                                1000^0      10^0    1                                   One  
                deci    d       1000^−1⁄3   10^−1   0.1                                 Tenth                   1795 
                centi   c       1000^−2⁄3   10^−2   0.01                                Hundredth               1795 
                milli   m       1000^−1     10^−3   0.001                               Thousandth              1795 
                micro   μ       1000^−2     10^−6   0.000001                            Millionth               1960 
                nano    n       1000^−3     10^−9   0.000000001                 Billionth       Milliardth      1960 
                pico    p       1000^−4     10^−12  0.000000000001              Trillionth      Billionth       1960 
                femto   f       1000^−5     10^−15  0.000000000000001           Quadrillionth   Billiardth      1964 
                atto    a       1000^−6     10^−18  0.000000000000000001        Quintillionth   Trillionth      1964 
                zepto   z       1000^−7     10^−21  0.000000000000000000001     Sextillionth    Trilliardth     1991 
                yocto   y       1000^−8     10^−24  0.000000000000000000000001  Septillionth    Quadrillionth   1991 
        */

        public static readonly UnitPrefixTable UnitPrefixes = new UnitPrefixTable(new UnitPrefix[] {new UnitPrefix(UnitPrefixes, "yotta", 'Y', 24),
                                                                                                    new UnitPrefix(UnitPrefixes, "zetta", 'Z', 21),
                                                                                                    new UnitPrefix(UnitPrefixes, "exa",   'E', 18),
                                                                                                    new UnitPrefix(UnitPrefixes, "peta",  'P', 15),
                                                                                                    new UnitPrefix(UnitPrefixes, "tera",  'T', 12),
                                                                                                    new UnitPrefix(UnitPrefixes, "giga",  'G', 9),
                                                                                                    new UnitPrefix(UnitPrefixes, "mega",  'M', 6),
                                                                                                    new UnitPrefix(UnitPrefixes, "kilo",  'K', 3),   /* k */
                                                                                       /* extra */  new UnitPrefix(UnitPrefixes, "kilo",  'k', 3),   /* k */
                                                                                                    new UnitPrefix(UnitPrefixes, "hecto", 'H', 2),   /* h */
                                                                                       /* extra */  new UnitPrefix(UnitPrefixes, "hecto", 'h', 2),   /* h */
                                                                                                    new UnitPrefix(UnitPrefixes, "deca",  'D', 1),   /* da */
                                                                                                    new UnitPrefix(UnitPrefixes, "deci",  'd', -1),
                                                                                                    new UnitPrefix(UnitPrefixes, "centi", 'c', -2),
                                                                                                    new UnitPrefix(UnitPrefixes, "milli", 'm', -3),
                                                                                                    // new UnitPrefix(UnitPrefixes, "micro", 'μ', -6), // '\0x03BC' (Char)956  
                                                                                                    new UnitPrefix(UnitPrefixes, "micro", 'µ', -6),  // ANSI '\0x00B5' (Char)181   
                                                                                                    new UnitPrefix(UnitPrefixes, "nano",  'n', -9),
                                                                                                    new UnitPrefix(UnitPrefixes, "pico",  'p', -12),
                                                                                                    new UnitPrefix(UnitPrefixes, "femto", 'f', -15),
                                                                                                    new UnitPrefix(UnitPrefixes, "atto",  'a', -18),
                                                                                                    new UnitPrefix(UnitPrefixes, "zepto", 'z', -21),
                                                                                                    new UnitPrefix(UnitPrefixes, "yocto", 'y', -24) });
        /*  http://en.wikipedia.org/wiki/Category:SI_units 
            SI base units
                Name        Symbol  Measure 
                metre       m       length 
                kilogram    kg      mass
                second      s       time
                ampere      A       electric current
                kelvin      K       thermodynamic temperature
                mole        mol     amount of substance 
                candela     cd      luminous intensity
          
            http://en.wikipedia.org/wiki/SI_derived_unit
            Named units derived from SI base units 
                Name        Symbol  Quantity                            Expression in terms of other units      Expression in terms of SI base units 
                hertz       Hz      frequency                           1/s                                     s-1 
                radian      rad     angle                               m∙m-1                                   dimensionless 
                steradian   sr      solid angle                         m2∙m-2                                  dimensionless 
                newton      N       force, weight                       kg∙m/s2                                 kg∙m∙s−2 
                pascal      Pa      pressure, stress                    N/m2                                    m−1∙kg∙s−2 
                joule       J       energy, work, heat                  N∙m = C·V = W·s                         m2∙kg∙s−2 
                watt        W       power, radiant flux                 J/s = V·A                               m2∙kg∙s−3 
                coulomb     C       electric charge or electric flux    s∙A                                     s∙A 
                volt        V       voltage, 
                                    electrical potential difference, 
                                    electromotive force                 W/A = J/C                               m2∙kg∙s−3∙A−1 
                farad       F       electric capacitance                C/V                                     m−2∙kg−1∙s4∙A2 
                ohm         Ω       electric resistance,
                                    impedance, reactance                V/A                                     m2∙kg∙s−3∙A−2 
                siemens     S       electrical conductance              1/Ω                                     m−2∙kg−1∙s3∙A2 
                weber       Wb      magnetic flux                       J/A                                     m2∙kg∙s−2∙A−1 
                tesla       T       magnetic field strength, 
                                    magnetic flux density               V∙s/m2 = Wb/m2 = N/(A∙m)                kg∙s−2∙A−1 
                henry       H       inductance                          V∙s/A = Wb/A                            m2∙kg∙s−2∙A−2 
         
        
                Celsius     C       temperature                         K − 273.15                              K − 273.15 
                lumen       lm      luminous flux                       lx·m2                                   cd·sr 
                lux         lx      illuminance                         lm/m2                                   m−2∙cd∙sr 
                becquerel   Bq      radioactivity 
                                    (decays per unit time)              1/s                                     s−1 
                gray        Gy      absorbed dose 
                                    (of ionizing radiation)             J/kg                                    m2∙s−2 
                sievert     Sv      equivalent dose 
                                    (of ionizing radiation)             J/kg                                    m2∙s−2 
                katal       kat     catalytic activity                  mol/s                                   s−1∙mol 
         
        */

        public static readonly BaseUnit[] SI_BaseUnits 
            = new BaseUnit[] {  new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.Length, "meter", "m"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.Mass, "kilogram", "Kg"), /* kg */
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.Time, "second", "s"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.ElectricCurrent, "ampere", "A"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.ThermodynamicTemperature, "kelvin", "K"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.AmountOfSubstance, "mol", "mol"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.LuminousIntensity, "candela", "cd") };

        public static readonly NamedDerivedUnit[] SI_NamedDerivedUnits
            = new NamedDerivedUnit[] {  new NamedDerivedUnit(null, "hertz",     "Hz",   new SByte[] { 0, 0, -1, 0, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "radian",    "rad",  new SByte[] { 0, 0, 0, 0, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "steradian", "sr",   new SByte[] { 0, 0, 0, 0, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "newton",    "N",    new SByte[] { 1, 1, -2, 0, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "pascal",    "Pa",   new SByte[] { -1, 1, -2, 0, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "joule",     "J",    new SByte[] { 2, 1, -2, 0, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "watt",      "W",    new SByte[] { 2, 1, -3, 0, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "coulomb",   "C",    new SByte[] { 1, 0, 0, 1, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "volt",      "V",    new SByte[] { 2, 1, -3, -1, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "farad",     "F",    new SByte[] { -2, -1, 4, 2, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "ohm",       "Ω",    new SByte[] { 2, 1, -3, -2, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "siemens",   "S",    new SByte[] { -2, -1, 3, 2, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "weber",     "Wb",   new SByte[] { 2, 1, -2, -1, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "tesla",     "T",    new SByte[] { 0, 1, -2, -1, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "henry",     "H",    new SByte[] { 2, 1, -2, -2, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "lumen",     "lm",   new SByte[] { 0, 0, 0, 0, 0, 0, 1 }),
                                        new NamedDerivedUnit(null, "lux",       "lx",   new SByte[] { -2, 0, 0, 0, 0, 0, 1 }),
                                        new NamedDerivedUnit(null, "becquerel", "Bq",   new SByte[] { 0, 0, -1, 0, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "gray",      "Gy",   new SByte[] { 2, 0, -2, 0, 0, 0, 0 }),
                                        new NamedDerivedUnit(null, "katal",     "kat",  new SByte[] { 0, 0, -1, 0, 0, 1, 0 })};
        public static readonly ConvertibleUnit[] SI_ConvertibleUnits
            = new ConvertibleUnit[] {   new ConvertibleUnit("gram", "g", SI_BaseUnits[(int)PhysicalBaseQuantityKind.Mass], new ScaledValueConversion(1000)),  /* [g] = 1000 * [Kg] */
                                        new ConvertibleUnit("Celsius", "°C" /* degree sign:  C2 B0  (char)176 '\0x00B0' */ ,
                                                            SI_BaseUnits[(int)PhysicalBaseQuantityKind.ThermodynamicTemperature], new LinearValueConversion(-273.15, 1)),    /* [°C] = 1 * [K] - 273.15 */
                                        new ConvertibleUnit("liter", "l", SI_BaseUnits[(int)PhysicalBaseQuantityKind.Length].Pow(3), new ScaledValueConversion(1000) ),  /* [l] = 1000 * [m3] */
                                        new ConvertibleUnit("hour", "h", SI_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(1.0/3600)) }; /* [h] = 1/3600 * [s] */

        public static readonly ConvertibleUnit[] ExtraTimeUnits
            = new ConvertibleUnit[] {   new ConvertibleUnit("minute", "min", SI_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(1.0/60)),                /* [min] = 1/60 * [s] */
                                        new ConvertibleUnit("day", "d", SI_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(1.0/86400)),                  /* [d] = 1/86400 * [s] */
                                        new ConvertibleUnit("year", "y", SI_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(1.0/(86400 * 365.25))),      /* [y]    = 1/365.25 * [d] */
                                        new ConvertibleUnit("hour", "hour", SI_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new IdentityValueConversion()),                     /* [hour] = 1 * [h] */
                                        new ConvertibleUnit("day", "day", SI_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(1.0/86400)),                /* [day] = 1/86400 * [s] */
                                        new ConvertibleUnit("year", "year", SI_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(1.0/(86400 * 365.25)))    /* [year]    = 1/365.25 * [d] */
                         };

        public static readonly UnitSystem SI_Units 
            = new UnitSystem("SI", UnitPrefixes, SI_BaseUnits, SI_NamedDerivedUnits, SI_ConvertibleUnits);


        public static readonly Unit dimensionless = SI_Units.Dimensionless as Unit;

        public static readonly Unit[] MixedTimeUnits
            = new Unit[] {              new MixedUnit(ExtraTimeUnits[2], "y ", new MixedUnit(ExtraTimeUnits[1], "d ", new MixedUnit(SI_ConvertibleUnits[3], ":", new MixedUnit(ExtraTimeUnits[0], ":", SI_BaseUnits[2]))))};

        public static readonly UnitSystem CGS_Units 
            = new UnitSystem("CGS", UnitPrefixes,
                             new BaseUnit[] {new BaseUnit(CGS_Units, (SByte)PhysicalBaseQuantityKind.Length, "centimeter", "cm"),
                                             new BaseUnit(CGS_Units, (SByte)PhysicalBaseQuantityKind.Mass, "gram", "g"),
                                             new BaseUnit(CGS_Units, (SByte)PhysicalBaseQuantityKind.Time, "second", "s"),
                                             new BaseUnit(CGS_Units, (SByte)PhysicalBaseQuantityKind.ElectricCurrent, "ampere", "A"),
                                             new BaseUnit(CGS_Units, (SByte)PhysicalBaseQuantityKind.ThermodynamicTemperature, "kelvin", "K"),
                                             new BaseUnit(CGS_Units, (SByte)PhysicalBaseQuantityKind.AmountOfSubstance, "mol", "mol"),
                                             new BaseUnit(CGS_Units, (SByte)PhysicalBaseQuantityKind.LuminousIntensity, "candela", "cd")});

        public static readonly BaseUnit[] MGD_BaseUnits
            = new BaseUnit[] {  new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.Length, "meter", "m"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.Mass, "kilogram", "Kg"), /* kg */
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.Time, "day", "d"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.ElectricCurrent, "ampere", "A"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.ThermodynamicTemperature, "kelvin", "K"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.AmountOfSubstance, "mol", "mol"),
                                new BaseUnit(null, (SByte)PhysicalBaseQuantityKind.LuminousIntensity, "candela", "cd") };

        public static readonly UnitSystem MGD_Units 
            = new UnitSystem("MGD", UnitPrefixes, MGD_BaseUnits, null,
                             new ConvertibleUnit[] {new ConvertibleUnit("second", "sec", MGD_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(24 * 60 * 60)),  /* [sec]  = 24 * 60 * 60 * [d] */
                                                    new ConvertibleUnit("minute", "min", MGD_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(24 * 60)),       /* [min]  = 24 * 60 * [d] */
                                                    new ConvertibleUnit("hour", "hour", MGD_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(24)),             /* [hour] = 24 * [d] */
                                                    new ConvertibleUnit("day", "day", MGD_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new IdentityValueConversion()),               /* [day]  = 1 * [d] */
                                                    new ConvertibleUnit("year", "year", MGD_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(1.0/365.25)),     /* [year] = 1/365.25 * [d] */
                                                    new ConvertibleUnit("year", "y", MGD_BaseUnits[(int)PhysicalBaseQuantityKind.Time], new ScaledValueConversion(1.0/365.25)) });     /* [y]    = 1/365.25 * [d] */

        public static readonly UnitSystem MGM_Units 
            = new UnitSystem("MGM", UnitPrefixes,
                             new BaseUnit[] {new BaseUnit(MGM_Units, (SByte)PhysicalBaseQuantityKind.Length, "meter", "m"),
                                             new BaseUnit(MGM_Units, (SByte)PhysicalBaseQuantityKind.Mass, "kilogram", "Kg"), 
                                             
                                             new BaseUnit(MGM_Units, (SByte)PhysicalBaseQuantityKind.Time, "moment", "ø"),

                                             new BaseUnit(MGM_Units, (SByte)PhysicalBaseQuantityKind.ElectricCurrent, "ampere", "A"),
                                             new BaseUnit(MGM_Units, (SByte)PhysicalBaseQuantityKind.ThermodynamicTemperature, "kelvin", "K"),
                                             new BaseUnit(MGM_Units, (SByte)PhysicalBaseQuantityKind.AmountOfSubstance, "mol", "mol"),
                                             new BaseUnit(MGM_Units, (SByte)PhysicalBaseQuantityKind.LuminousIntensity, "candela", "cd") });

        public static UnitSystemStack CurrentUnitSystems = new UnitSystemStack();
        public static UnitLookup UnitSystems = new UnitLookup(new UnitSystem[] { SI_Units, CGS_Units, MGD_Units, MGM_Units });

        public static readonly UnitSystemConversion SItoCGSConversion
            = new UnitSystemConversion(SI_Units, CGS_Units,
                new ValueConversion[] { new ScaledValueConversion(100),       /* 1 m       <SI> = 100 cm        <CGS>  */
                                        new ScaledValueConversion(1000),      /* 1 Kg      <SI> = 1000 g        <CGS>  */
                                        new IdentityValueConversion(),        /* 1 s       <SI> = 1 s           <CGS>  */
                                        new IdentityValueConversion(),        /* 1 A       <SI> = 1 A           <CGS>  */
                                        new IdentityValueConversion(),        /* 1 K       <SI> = 1 K           <CGS>  */
                                        new IdentityValueConversion(),        /* 1 mol     <SI> = 1 mol         <CGS>  */
                                        new IdentityValueConversion(),        /* 1 candela <SI> = 1 candela     <CGS>  */
                                    });

        public static readonly UnitSystemConversion SItoMGDConversion 
            = new UnitSystemConversion(SI_Units, MGD_Units, 
                new ValueConversion[] {new IdentityValueConversion(),        /* 1 m       <SI> = 1 m           <MGD>  */
                                       new IdentityValueConversion(),                 /* 1 Kg      <SI> = 1 Kg          <MGD>  */
                                       new ScaledValueConversion(1.0/(24*60*60)),     /* 1 s       <SI> = 1/86400 d     <MGD>  */
                                       /* new ScaledValueConversion(10000/(24*60*60)),   /* 1 s       <SI> = 10000/86400 ø <MGD>  */
                                       new IdentityValueConversion(),                 /* 1 A       <SI> = 1 A           <MGD>  */
                                       new IdentityValueConversion(),                 /* 1 K       <SI> = 1 K           <MGD>  */
                                       new IdentityValueConversion(),                 /* 1 mol     <SI> = 1 mol         <MGD>  */
                                       new IdentityValueConversion(),                 /* 1 candela <SI> = 1 candela     <MGD>  */
                                     });

        public static readonly UnitSystemConversion MGDtoMGMConversion = 
            new UnitSystemConversion(MGD_Units, MGM_Units, 
                new ValueConversion[] {new IdentityValueConversion(),      /* 1 m       <MGD> = 1 m           <MGM>  */
                                       new IdentityValueConversion(),               /* 1 Kg      <MGD> = 1 Kg          <MGM>  */
                                       new ScaledValueConversion(10000),            /* 1 d       <MGD> = 10000 ø       <MGM>  */
                                       new IdentityValueConversion(),               /* 1 A       <MGD> = 1 A           <MGM>  */
                                       new IdentityValueConversion(),               /* 1 K       <MGD> = 1 K           <MGM>  */
                                       new IdentityValueConversion(),               /* 1 mol     <MGD> = 1 mol         <MGM>  */
                                       new IdentityValueConversion(),               /* 1 candela <MGD> = 1 candela     <MGM>  */
                                    });

        public static UnitSystemConversionLookup UnitSystemConversions = new UnitSystemConversionLookup(new List<UnitSystemConversion> { SItoCGSConversion, SItoMGDConversion, MGDtoMGMConversion});
    }

    public static class Prefix
    {
        /* SI unit prefixes */
        public static readonly UnitPrefix Y = (UnitPrefix)Physics.UnitPrefixes['Y'];
        public static readonly UnitPrefix Z = (UnitPrefix)Physics.UnitPrefixes['Z'];
        public static readonly UnitPrefix E = (UnitPrefix)Physics.UnitPrefixes['E'];
        public static readonly UnitPrefix P = (UnitPrefix)Physics.UnitPrefixes['P'];
        public static readonly UnitPrefix T = (UnitPrefix)Physics.UnitPrefixes['T'];
        public static readonly UnitPrefix G = (UnitPrefix)Physics.UnitPrefixes['G'];
        public static readonly UnitPrefix M = (UnitPrefix)Physics.UnitPrefixes['M'];
        public static readonly UnitPrefix K = (UnitPrefix)Physics.UnitPrefixes['K'];
        public static readonly UnitPrefix k = (UnitPrefix)Physics.UnitPrefixes['k'];
        public static readonly UnitPrefix H = (UnitPrefix)Physics.UnitPrefixes['H'];
        public static readonly UnitPrefix h = (UnitPrefix)Physics.UnitPrefixes['h'];
        public static readonly UnitPrefix D = (UnitPrefix)Physics.UnitPrefixes['D'];
        public static readonly UnitPrefix da = (UnitPrefix)Physics.UnitPrefixes['D']; // Extra
        public static readonly UnitPrefix d = (UnitPrefix)Physics.UnitPrefixes['d']; 
        public static readonly UnitPrefix c = (UnitPrefix)Physics.UnitPrefixes['c']; 
        public static readonly UnitPrefix m = (UnitPrefix)Physics.UnitPrefixes['m']; 
        public static readonly UnitPrefix my = (UnitPrefix)Physics.UnitPrefixes['µ']; 
        public static readonly UnitPrefix n = (UnitPrefix)Physics.UnitPrefixes['n'];
        public static readonly UnitPrefix p = (UnitPrefix)Physics.UnitPrefixes['p'];
        public static readonly UnitPrefix f = (UnitPrefix)Physics.UnitPrefixes['f'];
        public static readonly UnitPrefix a = (UnitPrefix)Physics.UnitPrefixes['a'];
        public static readonly UnitPrefix z = (UnitPrefix)Physics.UnitPrefixes['z'];
        public static readonly UnitPrefix y = (UnitPrefix)Physics.UnitPrefixes['y'];
    }


    public static class SI
    {
        /* SI base units */
        public static readonly BaseUnit m   = (BaseUnit)Physics.SI_Units["m"]; 
        public static readonly BaseUnit Kg  = (BaseUnit)Physics.SI_Units["Kg"]; 
        public static readonly BaseUnit s   = (BaseUnit)Physics.SI_Units["s"];
        public static readonly BaseUnit A   = (BaseUnit)Physics.SI_Units["A"];
        public static readonly BaseUnit K   = (BaseUnit)Physics.SI_Units["K"];
        public static readonly BaseUnit mol = (BaseUnit)Physics.SI_Units["mol"];
        public static readonly BaseUnit cd  = (BaseUnit)Physics.SI_Units["cd"]; 

        /* Named units derived from SI base units */
        public static readonly NamedDerivedUnit Hz  = (NamedDerivedUnit)Physics.SI_Units["Hz"]; 
        public static readonly NamedDerivedUnit rad = (NamedDerivedUnit)Physics.SI_Units["rad"]; 
        public static readonly NamedDerivedUnit sr  = (NamedDerivedUnit)Physics.SI_Units["sr"]; 
        public static readonly NamedDerivedUnit N   = (NamedDerivedUnit)Physics.SI_Units["N"]; 
        public static readonly NamedDerivedUnit Pa  = (NamedDerivedUnit)Physics.SI_Units["Pa"]; 
        public static readonly NamedDerivedUnit J   = (NamedDerivedUnit)Physics.SI_Units["J"]; 
        public static readonly NamedDerivedUnit W   = (NamedDerivedUnit)Physics.SI_Units["W"]; 
        public static readonly NamedDerivedUnit C   = (NamedDerivedUnit)Physics.SI_Units["C"]; 
        public static readonly NamedDerivedUnit V   = (NamedDerivedUnit)Physics.SI_Units["V"]; 
        public static readonly NamedDerivedUnit F   = (NamedDerivedUnit)Physics.SI_Units["F"]; 
        public static readonly NamedDerivedUnit Ohm = (NamedDerivedUnit)Physics.SI_Units["Ω"]; 
        public static readonly NamedDerivedUnit S   = (NamedDerivedUnit)Physics.SI_Units["S"]; 
        public static readonly NamedDerivedUnit Wb  = (NamedDerivedUnit)Physics.SI_Units["Wb"]; 
        public static readonly NamedDerivedUnit T   = (NamedDerivedUnit)Physics.SI_Units["T"]; 
        public static readonly NamedDerivedUnit H   = (NamedDerivedUnit)Physics.SI_Units["H"]; 
        public static readonly NamedDerivedUnit lm  = (NamedDerivedUnit)Physics.SI_Units["lm"];
        public static readonly NamedDerivedUnit lx  = (NamedDerivedUnit)Physics.SI_Units["lx"];
        public static readonly NamedDerivedUnit Bq  = (NamedDerivedUnit)Physics.SI_Units["Bq"];
        public static readonly NamedDerivedUnit Gy  = (NamedDerivedUnit)Physics.SI_Units["Gy"];
        public static readonly NamedDerivedUnit kat = (NamedDerivedUnit)Physics.SI_Units["kat"];

        /* Convertible units */
        public static readonly ConvertibleUnit g  = (ConvertibleUnit)Physics.SI_Units["g"];
        public static readonly ConvertibleUnit Ce = (ConvertibleUnit)Physics.SI_Units["°C"];
        public static readonly ConvertibleUnit h  = (ConvertibleUnit)Physics.SI_Units["h"];
        public static readonly ConvertibleUnit l  = (ConvertibleUnit)Physics.SI_Units["l"];
    }


    #endregion Physical Measure Static Classes
}
