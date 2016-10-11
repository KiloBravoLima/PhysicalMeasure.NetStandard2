/*   http://physicalmeasure.codeplex.com                          */
/*   http://en.wikipedia.org/wiki/International_System_of_Units   */
/*   http://en.wikipedia.org/wiki/Physical_quantity               */
/*   http://en.wikipedia.org/wiki/Physical_constant               */

namespace PhysicalMeasure
{
    #region Physical Constants Statics

    public static partial class Constants
    {
        /* http://en.wikipedia.org/wiki/Physical_constant 
         
            Table of universal constants
                Quantity                            Symbol      Value                               Relative Standard Uncertainty 
                speed of light in vacuum            c           299792458 m·s−1                     defined 
                Newtonian constant of gravitation   G           6.67428(67)×10−11 m3·kg−1·s−2       1.0 × 10−4 
                Planck constant                     h           6.62606896(33) × 10−34 J·s          5.0 × 10−8 
                reduced Planck constant             h/2 pi      1.054571628(53) × 10−34 J·s         5.0 × 10−8 
         */
        public static readonly Quantity c = new Quantity(299792458, SI.m / SI.s);
        public static readonly Quantity G = new Quantity(6.67428E-11, (SI.m^3) / (SI.Kg * (SI.s ^ 2)));
        public static readonly Quantity h = new Quantity(6.62E-34, SI.J * SI.s);
        public static readonly Quantity h_bar = new Quantity(1.054571628E-34, SI.J * SI.s);

        /*
            Table of electromagnetic constants
                Quantity                            Symbol      Value                           (SI units)              Relative Standard Uncertainty 
                magnetic constant 
                     (vacuum permeability)          my0         4π × 10−7                       N·A−2 
                                                                   = 1.256637061× 10−6          N·A−2                   defined 
                electric constant 
                     (vacuum permittivity)          epsilon0    8.854187817×10−12               F·m−1                   defined 
                characteristic impedance of vacuum  Z0          376.730313461                   Ω                       defined 
                Coulomb's constant                  ke          8.987551787×109                 N·m²·C−2                defined 
                elementary charge                   e           1.602176487×10−19               C                       2.5 × 10−8 
                Bohr magneton                       myB         9.27400915(23)×10−24            J·T−1                   2.5 × 10−8 
                conductance quantum                 G0          7.7480917004(53)×10−5           S                       6.8 × 10−10 
                inverse conductance quantum         1/G0        12906.4037787(88)               Ω                       6.8 × 10−10 
                Josephson constant                  KJ          4.83597891(12)×1014             Hz·V−1                  2.5 × 10−8 
                magnetic flux quantum               phi0        2.067833667(52)×10−15           Wb                      2.5 × 10−8 
                nuclear magneton                    myN         5.05078343(43)×10−27            J·T−1                   8.6 × 10−8 
                von Klitzing constant               RK          25812.807557(18)                Ω                       6.8 × 10−10 
        */
        public static readonly Quantity my0 = new Quantity(1.256637061E-6, SI.N / (SI.A^2));
        public static readonly Quantity epsilon0 = new Quantity(8.854187817E-12, SI.F / SI.m);
        public static readonly Quantity Z0 = new Quantity(376.730313461, SI.Ohm);
        public static readonly Quantity ke = new Quantity(8.987551787E9, SI.N * (SI.m^2) /(SI.C^2));
        public static readonly Quantity e = new Quantity(1.602176487E-19, SI.C);
        public static readonly Quantity myB = new Quantity(9.27400915E-24, SI.J / SI.T);
        public static readonly Quantity G0 = new Quantity(7.7480917004E-5, SI.S);
        public static readonly Quantity KJ = new Quantity(4.83597891E14, SI.Hz / SI.V);
        public static readonly Quantity phi0 = new Quantity(2.067833667E-15, SI.Wb);
        public static readonly Quantity myN = new Quantity(5.05078343E-27, SI.J / SI.T);
        public static readonly Quantity RK = new Quantity(25812.807557, SI.Ohm);

        /*
            Table of atomic and nuclear constants
                Quantity                            Symbol      Value                           (SI units)              Relative Standard Uncertainty 
                Bohr radius                         a0          5.291772108(18)×10−11           m                       3.3 × 10−9 
                classical electron radius           re          2.8179402894(58)×10−15          m                       2.1 × 10−9 
                electron mass                       me          9.10938215(45)×10−31            kg                      5.0 × 10−8 
                Fermi coupling constant             GF          1.16639(1)×10−5                 GeV−2                   8.6 × 10−6 
                fine-structure constant             alpha       7.2973525376(50)×10−3                                   6.8 × 10−10 
                Hartree energy                      Eh          4.35974417(75)×10−18            J                       1.7 × 10−7 
                proton mass                         mp          1.672621637(83)×10−27           kg                      5.0 × 10−8 
                quantum of circulation              h2me        3.636947550(24)×10−4            m² s−1                  6.7 × 10−9 
                Rydberg constant                    Rinf        10973731.568525(73)             m−1                     6.6 × 10−12 
                Thomson cross section               tcs         6.65245873(13)×10−29            m²                      2.0 × 10−8 
                weak mixing angle                   ThetaW      0.22215(76)                                             3.4 × 10−3 
        */
        public static readonly Quantity a0 = new Quantity(5.291772108E-11, SI.m);
        public static readonly Quantity re = new Quantity(2.8179402894E-15, SI.m);
        public static readonly Quantity me = new Quantity(9.10938215E-31, SI.Kg);
        /** */
        public static readonly Quantity GF = new Quantity(1.16639E-5, (Prefix.G * Constants.e * SI.V) ^ -2);
        /* **/
        public static readonly Quantity alpha = new Quantity(7.2973525376E-3, Physics.dimensionless);
        public static readonly Quantity Eh = new Quantity(4.35974417E-18, SI.J);
        public static readonly Quantity mp = new Quantity(1.672621637E-27, SI.Kg);
        public static readonly Quantity h2me = new Quantity(3.636947550E-4, (SI.m ^ 2) / SI.s);
        public static readonly Quantity Rinf = new Quantity(10973731.568525, SI.m ^ -1);
        public static readonly Quantity tcs = new Quantity(6.65245873E-29, SI.m ^ 2);
        public static readonly Quantity ThetaW = new Quantity(0.22215, Physics.dimensionless);
        /*
            Other usefull constants
        */
        public static readonly Quantity g = new Quantity(9.82, SI.N/SI.Kg );  /// Gravitational constant at earth surface 9.82 N/Kg

    }
    #endregion Physical Constants Statics
}

