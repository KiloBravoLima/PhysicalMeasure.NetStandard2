using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Reflection;
using PhysicalMeasure;


namespace PhysicalMeasureTest
{

    /// <summary>
    ///This is a test class for UnitTest and is intended
    ///to contain all UnitTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UnitTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region Unit StringTo tests

        /// <summary>
        ///A test for base unit ToString
        ///</summary>
        [TestMethod()]
        public void BaseUnitToStringTest()
        {
            Unit u = (Unit)(Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            String expected = "Kg";

            String actual = u.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for named derived unit ToString
        ///</summary>
        [TestMethod()]
        public void NamedDerivedUnitToStringTest()
        {
            Unit u = (Unit)(Physics.SI_Units.NamedDerivedUnits[5]);

            String expected = "J";

            String actual = u.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void DerivedUnitToStringTest()
        {
            Unit MeterPerSecond2 = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });

            //String expected = "SI.m·s-2";
            String expected = "m·s-2";

            String actual = MeterPerSecond2.ToString();

            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void DerivedUnitBaseUnitStringTest()
        {
            Unit MeterPerSecond2 = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });

            String expected = "m·s-2";

            IQuantity pq = MeterPerSecond2.ConvertToBaseUnit();

            Assert.AreEqual(pq.Value, 1d);

            String actual = pq.Unit.ToString();
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void NamedDerivedUnitBaseUnitStringTest()
        {
            IUnit Watt = Physics.SI_Units.NamedDerivedUnits[6];

            String expected = "1 m2·Kg·s-3";

            String actual = Watt.ConvertToBaseUnit().ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void NamedDerivedUnitReducedUnitStringTest()
        {
            IUnit Watt = Physics.SI_Units["W"];

            String expected = "W";

            String actual = Watt.ReducedUnitString().ToString();

            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for (unnamed) derived unit ToString
        ///</summary>
        [TestMethod()]
        public void MixedUnitBaseUnitStringTest()
        {
            IUnit HourMin= new MixedUnit((Unit)Physics.SI_Units["h"], ":",  (Unit)Physics.MGD_Units["min"]);

            String expected = "3600 s";

            String actual = HourMin.ConvertToBaseUnit().ToString();

            Assert.AreEqual(expected, actual);
        }


        #endregion Unit StringTo tests

        #region Unit math tests

        /// <summary>
        ///A test for mult operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_MultBaseunitAndDerivedUnitTest()
        {
            Unit pu1 = (Unit)Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)];
            Unit pu2 = (Unit)Physics.SI_Units.UnitFromSymbol("J"); // m2∙kg∙s−2

            Unit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 2, -2, 0, 0, 0, 0 });

            Unit actual = pu1 * pu2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for div operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_DivBaseunitAndDerivedUnitTest()
        {
            Unit pu1 = (Unit)Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)];
            Unit pu2 = (Unit)Physics.SI_Units.UnitFromSymbol("J"); // m2∙kg∙s−2

            Unit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { -2, 0, 2, 0, 0, 0, 0 });

            Unit actual = pu1 / pu2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for div operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_DivDerivedunitAndBaseUnitTest()
        {
            Unit pu1 = (Unit)Physics.SI_Units.UnitFromSymbol("J"); // m2∙kg∙s−2
            Unit pu2 = (Unit)Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)];

            Unit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -2, 0, 0, 0, 0 });

            Unit actual = pu1 / pu2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_PowerOfBaseUnit()
        {
            Unit pu = (Unit)Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Length)];

            Unit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 3, 0, 0, 0, 0, 0, 0 });

            IUnit actual = pu ^ 3;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_PowerOfDerivedUnitTest()
        {
            Unit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -1, 0, 0, 0, 0 });

            Unit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 3, 0, -3, 0, 0, 0, 0 });

            IUnit actual = pu ^ 3;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator (for CombinedUnit having a prefix)
        ///</summary>
        [TestMethod()]
        public void QuantityTest_PowerOfCombinedUnitTest()
        {
            Unit cm = Prefix.c * SI.m;
            Assert.AreEqual((cm ^ 3) * 1000000, 1 * SI.m ^ 3);

            Unit cm3 = cm * cm * cm;
            Assert.AreEqual(cm3 * 1000000, 1 * SI.m ^ 3);
        }

        /// <summary>
        ///A test for power operator (for PrefixedUnit)
        ///</summary>
        [TestMethod()]
        public void QuantityTest_PowerOfPrefixedUnitTest()
        {
            Unit MHz = Prefix.M * SI.Hz;
            Assert.AreEqual((MHz.Pow(-1)) * 1, SI.s * 1e-6);

            Assert.AreEqual(((MHz.Pow(-2)) * 1).ConvertToBaseUnit(), (SI.s ^ 2) * 1e-12); // throws and Debug.Assert() in 
        }

        /// <summary>
        ///A test for root operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_RootOfDerivedUnitTest()
        {
            Unit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            Unit expected = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });

            IUnit actual = pu % 2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for mult and div operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_KmPrHourUnitTest()
        {
            Quantity KmPrHour_example1 = Prefix.K * SI.m / SI.h;

            Quantity Speed1 = 123 * KmPrHour_example1;

            //IUnit KmPrHour_example2 = Prefix.K * SI.m / SI.h;

            Unit Km = Unit.Parse("Km");
            Unit h = Unit.Parse("h");
            Unit KmPrHour_example2 = Km.Divide(h);
            Quantity Speed2 = new Quantity(123, KmPrHour_example2);

            string KmPrHour_Str = "Km/h";

            Unit KmPrHour_example3 = Unit.Parse(KmPrHour_Str);
            Quantity Speed3 = new Quantity(123, KmPrHour_example3);

            Quantity expected = new Quantity(123 * 1000.0/(60 * 60) , SI.m / SI.s);

            Assert.AreEqual(expected, Speed2, "Speed2");

            Assert.AreEqual(expected, Speed1, "Speed1");
            Assert.AreEqual(expected, Speed2, "Speed2");
            Assert.AreEqual(expected, Speed3, "Speed3");
        }

        /// <summary>
        ///A test for mult and div operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_WattHourUnitTest()
        {
            Unit WattHour_example1 = SI.W * SI.h;

            Unit WattHour_example2 = new ConvertibleUnit("WattHour", "Wh", SI.J, new ScaledValueConversion(1.0 / 3600)); /* [Wh] = 1/3600 * [J] */

            Quantity E_1 = new Quantity(1, WattHour_example1 ); // 1 Wh
            Quantity E_2 = new Quantity(0.001, Prefix.K * WattHour_example2); // 0.001 KWh
            //IQuantity actual_1 = E_1.ConvertTo(SI.J);
            //IQuantity actual_2 = E_2.ConvertTo(SI.J);

            Quantity expected = new Quantity(3600, SI.J);

            Assert.AreEqual(expected, E_1);
            Assert.AreEqual(expected, E_2);
        }

        /// <summary>
        ///A test for mult operator
        ///</summary>
        [TestMethod()]
        public void MultNumberWithUnitTest_KiloMeter()
        {
            Unit Km = Prefix.K * SI.m;
            Quantity len = new Quantity(123, Km);
            Quantity len2 = 123 * Km;

            String Len_String = len.ToString();
            String Len2_String = len2.ToString();

            // Check len properties
            Double len_value = 123.0;
            Assert.AreEqual(len.Value, len_value);
            Assert.AreSame(len.Unit, Km);


            // Check len2 properties
            Assert.AreEqual(len2.Value, len_value);
            Assert.AreSame(len2.Unit, Km);

            System.Reflection.Assembly assembly = typeof(Quantity).GetTypeInfo().Assembly;
            String VerStr = assembly.AssemblyInfo();


            System.Diagnostics.Debug.WriteLine("AssemblyExtensions.AssemblyInfo(assembly):" + VerStr);
            // Assert.AreEqual(VerStr, ".");
        }

        #endregion Unit math tests

        #region Unit Convert tests


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void UnitConvertToUnitSystemMGD()
        {
            Unit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            Unit expectedunit = new DerivedUnit(Physics.MGD_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });
            Quantity expected = new Quantity(1/Math.Pow(24*60*60, -4), expectedunit);

            IQuantity actual = pu.ConvertTo(Physics.MGD_Units);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void UnitConvertToUnitSystemSI()
        {
            Unit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            Unit expectedunit = new DerivedUnit(Physics.MGD_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });
            Quantity expected = new Quantity(1 / Math.Pow(24 * 60 * 60, -4), expectedunit);

            IQuantity actual = pu.ConvertTo(Physics.MGD_Units);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void UnitConvertToMGDUnit()
        {
            Unit pu = new DerivedUnit(Physics.SI_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            Unit expectedunit = new DerivedUnit(Physics.MGD_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });
            Quantity expected = new Quantity(1/Math.Pow(24 * 60 * 60, -4), expectedunit);

            IQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void UnitConvertToSIUnit()
        {
            Unit pu = new DerivedUnit(Physics.MGD_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            Unit expectedunit = new DerivedUnit(Physics.SI_Units, pu.Exponents);
            Quantity expected = new Quantity( Math.Pow(24 * 60 * 60, -4), expectedunit);

            IQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }




        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void UnitConvertToThroughOtherUnitSystems()
        {
            Unit pu = new DerivedUnit(Physics.MGM_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            Unit expectedunit = new DerivedUnit(Physics.CGS_Units, pu.Exponents);
            Quantity expected = new Quantity(Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4), expectedunit);

            IQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void UnitReversedConvertToThroughOtherUnitSystems()
        {
            Unit pu = new DerivedUnit(Physics.CGS_Units , new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            Unit expectedunit = new DerivedUnit(Physics.MGM_Units, pu.Exponents);
            Quantity expected = new Quantity(1D / (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), expectedunit);

            IQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void UnitReversedConvertToThroughOtherUnitSystems2()
        {
            Unit pu = new DerivedUnit(Physics.CGS_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            Unit expectedunit = new DerivedUnit(Physics.MGM_Units, pu.Exponents);
            
            // VS fails Quantity expected = 1D / new Quantity( (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), 1D/expectedunit);
            Quantity expected = new Quantity(1D / (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), expectedunit);

            IQuantity actual = pu.ConvertTo(expectedunit);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void UnitReversedConvertToThroughOtherUnitSystems2SquareOperator()
        {
            Unit pu = new DerivedUnit(Physics.CGS_Units, new SByte[] { 2, 0, -4, 0, 0, 0, 0 });

            Unit expectedunit = new DerivedUnit(Physics.MGM_Units, pu.Exponents);

            // VS fails Quantity expected = 1D / new Quantity( (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), 1D/expectedunit);
            Quantity expected = new Quantity(1D / (Math.Pow(100, 2) * Math.Pow(24 * 60 * 60, -4) / Math.Pow(10000, -4)), expectedunit);

            IQuantity actual = pu [expectedunit];
            Assert.AreEqual(expected, actual);
        }


        #endregion Unit Convert tests


        #region BaseUnit tests

        /// <summary>
        ///A test for BaseUnit BaseUnitNumber access
        ///</summary>
        [TestMethod()]
        public void BaseUnitTestBaseUnitNumberAccessMass()
        {
            BaseUnit u = (BaseUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            SByte expected = 1;

            SByte actual = u.BaseUnitNumber;

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BaseUnit BaseUnitNumber access
        ///</summary>
        [TestMethod()]
        public void BaseUnitTestBaseUnitNumberAccessLuminousIntensity()
        {
            BaseUnit u = (BaseUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.LuminousIntensity)]);

            SByte expected = 6;

            SByte actual = u.BaseUnitNumber;

            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for BaseUnit Name access
        ///</summary>
        [TestMethod()]
        public void BaseUnitTestNameAccessMass()
        {
            BaseUnit u = (BaseUnit)(Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            String expected = "kilogram";

            String actual = u.Name;

            Assert.AreEqual(expected, actual);
        }


        #endregion BaseUnit tests

    }

    /// <summary>
    ///This is a test class for QuantityTest and is intended
    ///to contain all QuantityTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QuantityTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region Quantity.Parse test

        /// <summary>
        ///A test for Parse
        ///</summary>
        [TestMethod()]
        public void TestMilliGramParseString()
        {
            String s = "123.000 mg"; 
            NumberStyles styles = NumberStyles.Float;
            IFormatProvider provider = NumberFormatInfo.InvariantInfo;
            //IQuantity expected = (IQuantity)(new Quantity(0.000123, (IUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)])));
            //IQuantity expected = (IQuantity)(new Quantity(123, (IUnit)(new PhysicalMeasure.CombinedUnit(new PrefixedUnitExponent(-3, PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.Mass)], 1)))));
            //IQuantity expected = (IQuantity)(new Quantity(123, (IUnit)(new PhysicalMeasure.CombinedUnit(new PrefixedUnitExponent(-3, PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("g"), 1)))));
            IQuantity expected = new Quantity(123, new CombinedUnit(new PrefixedUnitExponent(Prefix.m, Physics.SI_Units.UnitFromSymbol("g"), 1)));
            IQuantity actual;
            actual = Quantity.Parse(s, styles, provider);
            Assert.AreEqual(expected, actual);
        }

        #endregion Quantity.Parse test


        #region Quantity.Equal test

        /// <summary>
        ///A test for equal
        ///</summary>
        [TestMethod()]
        public void TestMilligramEqualKilogram()
        {
            //String s = "123.000 mg";
            IQuantity InKiloGram = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]); // In kilogram
            IQuantity InMilliGram = new Quantity(123, new CombinedUnit(new PrefixedUnitExponent(Prefix.m, Physics.SI_Units.UnitFromSymbol("g"), 1))); // In milli gram

            Assert.AreEqual(InKiloGram, InMilliGram);
        }

        /// <summary>
        ///A test for equal
        ///</summary>
        [TestMethod()]
        public void TestMilliKelvinEqualKiloCelsiusSpecificConversion()
        {
            //String s = "594.15 mK";
            //String s = "3.21 K°C";
            /*
            IQuantity InMilliKelvin = (IQuantity)(new Quantity(321273.15, (IUnit)(PhysicalMeasure.Physics.SI_Units.BaseUnits[(int)(MeasureKind.ThermodynamicTemperature)]))); // In Kelvin
            IQuantity InKiloCelsius = (IQuantity)(new Quantity(321, (IUnit)(new PhysicalMeasure.CombinedUnit(new PrefixedUnitExponent(3, PhysicalMeasure.Physics.SI_Units.UnitFromSymbol("°C"), 1))))); // In Kilo Celsius
            */
            IQuantity InMilliKelvin = new Quantity(321273.15, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.ThermodynamicTemperature)]); // In Kelvin
            IQuantity InKiloCelsius = new Quantity(321, new CombinedUnit(new PrefixedUnitExponent(Prefix.k, Physics.SI_Units.UnitFromSymbol("°C"), 1))); // In Kilo Celsius

            Assert.AreEqual(InMilliKelvin, InKiloCelsius);
        }

        /// <summary>
        ///A test for conversion between temperatures
        ///</summary>
        [TestMethod()]
        public void TestAdd2KelvinWithCelsiusSpecificConversion2()
        {

            IQuantity InMilliKelvin = new Quantity(321273150, new CombinedUnit(new PrefixedUnitExponent(Prefix.m, Physics.SI_Units.UnitFromSymbol("K"), 1))); // In miliKelvin
            IQuantity InKelvin      = new Quantity(321273.15, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.ThermodynamicTemperature)]); // In Kelvin
            IQuantity InKiloCelsius = new Quantity(321, new CombinedUnit(new PrefixedUnitExponent(Prefix.K, Physics.SI_Units.UnitFromSymbol("°C"), 1))); // In Kilo Celsius

            // Check all quantities in all combinations as first and second:
            Assert.AreEqual(InMilliKelvin, InKelvin, "(InMilliKelvin, InKelvin)");
            Assert.AreEqual(InKelvin, InMilliKelvin, "(InKelvin, InMilliKelvin)");
            Assert.AreEqual(InKelvin, InKiloCelsius, "(InKelvin, InKiloCelsius)");
            Assert.AreEqual(InKiloCelsius, InKelvin, "(InKiloCelsius, InKelvin)");
            Assert.AreEqual(InMilliKelvin, InKiloCelsius, "(InMilliKelvin, InKiloCelsius)");
            Assert.AreEqual(InKiloCelsius, InMilliKelvin, "(InKiloCelsius, InMilliKelvin)");
        }


        /// <summary>
        ///A test for conversion of temperatures from Celsius to Kelvin and back using CombineUnit
        ///</summary>
        [TestMethod()]
        public void TestKelvinPerSecondConvertedToCe_per_s()
        {
            // 2013-09-05  From CodePlex User JuricaGrcic

            // Define Celsius per second - °C/s
            Unit Ce_per_s = SI.Ce.CombineDivide(SI.s);

            // Define Kelvin per second - K/s
            Unit Kelvin_per_s = SI.K.CombineDivide(SI.s);

            // Create value in units °C/s
            Quantity valueOfCelsiusPerSecond = new Quantity(2, Ce_per_s);
            //Console.WriteLine("Base value : {0}", valueOfCelsiusPerSecond); 
            // prints 2 °C/s
            string valueOfCelsiusPerSecond_str = valueOfCelsiusPerSecond.ToString();
            string valueOfCelsiusPerSecond_str_expected = "2 °C/s";  
            
            // Convert °C/s to K/s
            IQuantity valueOfKelvinPerSecond = valueOfCelsiusPerSecond.ConvertTo(Kelvin_per_s);
            //Console.WriteLine("Base value converted to {0} : {1}", Ce_per_s, valueOfKelvinPerSecond);
            // prints 275.15 K/s - correct conversion or not??
            // 2013-10-29  Corrected to print 2 K/s
            string valueOfKelvinPerSecond_str = valueOfKelvinPerSecond.ToString();
            string valueOfKelvinPerSecond_str_expected = "2 K/s";

            // Convert K/s back to °C/s 
            IQuantity valueOfKelvinPerSecondConvertedToCe_per_s = valueOfKelvinPerSecond.ConvertTo(Ce_per_s);

            //Console.WriteLine("{0} converted back to {1}: {2}", Kelvin_per_s, Ce_per_s, valueOfKelvinPerSecond.ConvertTo(Ce_per_s));
            // prints 1.0036476381543 °C/s - should print 2 °C/s - incorrect conversion
            string valueOfKelvinPerSecondConvertedToCe_per_s_str = valueOfKelvinPerSecondConvertedToCe_per_s.ToString();
            string valueOfKelvinPerSecondConvertedToCe_per_s_str_expected = "2 °C/s";

            Assert.AreEqual(valueOfCelsiusPerSecond, valueOfKelvinPerSecondConvertedToCe_per_s);

            Assert.AreEqual(valueOfCelsiusPerSecond_str, valueOfCelsiusPerSecond_str_expected);
            Assert.AreEqual(valueOfKelvinPerSecond_str, valueOfKelvinPerSecond_str_expected);
            Assert.AreEqual(valueOfKelvinPerSecondConvertedToCe_per_s_str, valueOfKelvinPerSecondConvertedToCe_per_s_str_expected);
        }

        /// <summary>
        ///A test for conversion of temperatures from Celsius to Kelvin and back using CombineUnit
        ///</summary>
        [TestMethod()]
        public void TestMeterKelvinPerSecondConvertedToCe_per_s()
        {
            // 2013-09-05  From CodePlex User JuricaGrcic but modified to not have °C as first element in denominators

            // Define Celsius per second - m·°C/s
            Unit meter_Ce_per_s = SI.m.CombineMultiply(SI.Ce).CombineDivide(SI.s);

            // Define Kelvin per second - m·K/s
            Unit meter_Kelvin_per_s = SI.m.CombineMultiply(SI.K).CombineDivide(SI.s);

            // Create value in units m·°C/s
            Quantity valueOfmeterCelsiusPerSecond = new Quantity(2, meter_Ce_per_s);
            //Console.WriteLine("Base value : {0}", valueOfmeterCelsiusPerSecond); 
            // prints 2 m·°C/s

            // Convert m·°C/s to m·K/s
            Quantity valueOfmeterKelvinPerSecond = valueOfmeterCelsiusPerSecond.ConvertTo(meter_Kelvin_per_s);
            //Console.WriteLine("Base value converted to {0} : {1}", meter_Ce_per_s, valueOfmeterKelvinPerSecond);
            // prints 548.3 m·K/s - correct conversion ??

            // Convert m·K/s back to m·°C/s 
            IQuantity valueOfmeter_KelvinPerSecondConvertedToMeter_Ce_per_s = valueOfmeterKelvinPerSecond.ConvertTo(meter_Ce_per_s);

            //Console.WriteLine("{0} converted back to {1}: {2}", meter_Kelvin_per_s, meter_Ce_per_s, valueOfmeterKelvinPerSecond.ConvertTo(meter_Ce_per_s));
            // prints 1.0036476381543 m·°C/s - should print 2 m·°C/s - incorrect conversion ??

            Assert.AreEqual(valueOfmeterCelsiusPerSecond, valueOfmeter_KelvinPerSecondConvertedToMeter_Ce_per_s);
        }



        #endregion Quantity.Equal test


        #region Quantity ConvertTo test

        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void QuantityOfConvertibleUnitBasedOnDerivedUnitConvertToDerivedUnit_kWh()
        {
            Unit kWh = new ConvertibleUnit("kiloWattHour", "kWh", SI.J, new ScaledValueConversion(1.0/3600000)); /* [kWh] = 1/3600000 * [J] */
            Quantity consumed = new Quantity(1, kWh);
            Quantity actual = consumed.ConvertTo(SI.J);

            Quantity expected = new Quantity(3600000, SI.J);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void QuantityOfConvertibleUnitBasedOnDerivedUnitConvertToDerivedUnit_Wh()
        {
            Unit Wh = new ConvertibleUnit("WattHour", "Wh", SI.J, new ScaledValueConversion(1.0/3600)); /* [Wh] = 1/3600 * [J] */
            Quantity E_1 = new Quantity(1, Prefix.K * Wh);
            Quantity E_2 = new Quantity(0.001, Prefix.M * Wh);
            IQuantity actual_1 = E_1.ConvertTo(SI.J);
            IQuantity actual_2 = E_2.ConvertTo(SI.J);

            Quantity expected = new Quantity(3600000, SI.J);

            Assert.AreEqual(expected, actual_1);
            Assert.AreEqual(expected, actual_2);
        }


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void QuantityOfConvertibleUnitBasedOnConvertibleUnitConvertToDerivedUnit()
        {
            /* It is NOT encouraged to do like this. Just for test */
            Unit Wh = new ConvertibleUnit("WattHour", "Wh", SI.J, new ScaledValueConversion(1.0 / 3600)); /* [Wh] = 1/3600 * [J] */
            Unit kWh = new ConvertibleUnit("kiloWattHour", "kWh", Wh, new ScaledValueConversion(1.0 / 1000)); /* [kWh] = 1/1000 * [Wh] */
            Unit MWh = new ConvertibleUnit("MegaWattHour", "MWh", kWh, new ScaledValueConversion(1.0 / 1000)); /* [MWh] = 1/1000 * [kWh] */
            Quantity E_1 = new Quantity(1000, Wh);
            Quantity E_2 = new Quantity(1, kWh);
            Quantity E_3 = new Quantity(0.001, MWh);
            IQuantity actual_1 = E_1.ConvertTo(SI.J);
            IQuantity actual_2 = E_2.ConvertTo(SI.J);
            IQuantity actual_3 = E_3.ConvertTo(SI.J);

            Quantity expected = new Quantity(3600000, SI.J);

            Assert.AreEqual(expected, actual_1);
            Assert.AreEqual(expected, actual_2);
            Assert.AreEqual(expected, actual_3);
        }


        /// <summary>
        ///A test for ConvertTo()
        ///</summary>
        [TestMethod()]
        public void QuantityOfConvertibleUnitBasedOnConvertibleUnitConvertToDerivedUnitSquareOperator()
        {
            /* It is NOT encouraged to do like this. Just for test */
            Unit Wh = new ConvertibleUnit("WattHour", "Wh", SI.J, new ScaledValueConversion(1.0 / 3600)); /* [Wh] = 1/3600 * [J] */
            Unit kWh = new ConvertibleUnit("kiloWattHour", "kWh", Wh, new ScaledValueConversion(1.0 / 1000)); /* [kWh] = 1/1000 * [Wh] */
            Unit MWh = new ConvertibleUnit("MegaWattHour", "MWh", kWh, new ScaledValueConversion(1.0 / 1000)); /* [MWh] = 1/1000 * [kWh] */
            Quantity E_1 = new Quantity(1000, Wh);
            Quantity E_2 = new Quantity(1, kWh);
            Quantity E_3 = new Quantity(0.001, MWh);
            Quantity actual_1 = E_1 [SI.J];
            Quantity actual_2 = E_2 [SI.J];
            Quantity actual_3 = E_3 [SI.J];

            Quantity expected = new Quantity(3600000, SI.J);

            Assert.AreEqual(expected, actual_1);
            Assert.AreEqual(expected, actual_2);
            Assert.AreEqual(expected, actual_3);
        }

        #endregion Quantity ConvertTo test

        #region Quantity compare operation test

        /// <summary>
        ///A test for == operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_CompareOperatorEqualsTest()
        {
            Quantity pg1 = new Quantity(0.000123, (Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]));
            Quantity pg2 = new Quantity(456, (Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]));

            Quantity expected = new Quantity(456.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            IQuantity actual = pg1 + pg2;

            Assert.IsTrue(expected == actual);
        }

        /// <summary>
        ///A test for != operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_CompareOperatorNotEqualsTest()
        {
            Quantity pg1 = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            Quantity pg2 = new Quantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Quantity expected = new Quantity(456.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            IQuantity actual = pg1 + pg2;

            Assert.IsFalse(expected != actual);
        }

        /// <summary>
        ///A test for < operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_CompareOperatorLessTest()
        {
            Quantity pg1 = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            Quantity pg2 = new Quantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Assert.IsTrue(pg1 < pg2);
        }

        /// <summary>
        ///A test for <= operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_CompareOperatorLessOrEqualsTest()
        {
            Quantity pg1 = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            Quantity pg2 = new Quantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Assert.IsTrue(pg1 <= pg2);
        }

        /// <summary>
        ///A test for > operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_CompareOperatorLargerTest()
        {
            Quantity pg1 = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            Quantity pg2 = new Quantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Assert.IsFalse(pg1 > pg2);
        }

        /// <summary>
        ///A test for > operator
        ///</summary>
        [TestMethod()]
        public void QuantityTest_CompareOperatorLargerOrEqualTest()
        {
            Quantity pg1 = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            Quantity pg2 = new Quantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Assert.IsFalse(pg1 > pg2);
        }

        #endregion Quantity compare operation test

        #region Quantity math test

        /// <summary>
        ///A test for add operator
        ///</summary>
        [TestMethod()]
        public void AddKiloGramToMilliGram()
        {
            Quantity pg1 = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            Quantity pg2 = new Quantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Quantity expected = new Quantity(456.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            IQuantity actual = pg1 + pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for sub operator
        ///</summary>
        [TestMethod()]
        public void SubKiloGramFromMilliGram()
        {
            Quantity pg1 = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            Quantity pg2 = new Quantity(789, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Quantity expected = new Quantity(0.000123- 789, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Quantity actual = pg1 - pg2;
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for mult operator
        ///</summary>
        [TestMethod()]
        public void MultKiloGramToMilliGram()
        {
            Quantity pg1 = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            Quantity pg2 = new Quantity(456, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Unit MassSquared = new DerivedUnit(Physics.SI_Units, new SByte[] { 0, 2, 0, 0, 0, 0, 0 });

            Quantity expected = new Quantity(0.000123 * 456 , MassSquared);

            Quantity actual = pg1 * pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for div operator
        ///</summary>
        [TestMethod()]
        public void DivKiloGramFromMilliGram()
        {
            Quantity pg1 = new Quantity(0.000123, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);
            Quantity pg2 = new Quantity(789, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Quantity expected = new Quantity(0.000123 / 789, Physics.dimensionless);

            Quantity actual = pg1 / pg2;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for power operator
        ///</summary>
        [TestMethod()]
        public void PowerOperatorCalculateEnergyIn1Gram()
        {
            Quantity m = new Quantity(0.001, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Unit MeterPerSecond = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -1, 0, 0, 0, 0 });
            Quantity c = new Quantity(299792458, MeterPerSecond);

            Quantity expected = new Quantity(0.001 * 299792458 * 299792458, Physics.SI_Units.UnitFromSymbol("J"));

            Quantity E = m * c.Pow(2);
            Assert.AreEqual(expected, E);
        }


        /// <summary>
        ///A test for DerivedUnit with exponent of absolute value larger than 1
        ///</summary>
        [TestMethod()]
        public void CalculateEnergyOf1GramAfterFalling10MeterAtEarthSurface()
        {
            Quantity m = Quantity.Parse("1 g") as Quantity;
            Quantity h = Quantity.Parse("10 m") as Quantity;

            //!!! To do: make this work: Quantity g = Quantity.Parse("9.81 m/s^2");
            Unit MeterPerSecond2 = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 });
            Quantity g = new Quantity(9.81, MeterPerSecond2);

            Quantity expected = new Quantity(0.001 * 9.81 * 10, Physics.SI_Units.UnitFromSymbol("J"));

            Quantity E = m * g * h;

            Assert.AreEqual(expected, E);
        }

        /// <summary>
        ///A test for power operator in Quantity parse
        ///</summary>
        [TestMethod()]
        public void CalculateEnergyOf1GramAfterFalling10MeterAtEarthSurfaceParsePowerOperator()
        {
            Quantity m = Quantity.Parse("1 g") as Quantity;
            Quantity h = Quantity.Parse("10 m") as Quantity;

            Quantity g = Quantity.Parse("9.81 m/s^2");

            Quantity expected = new Quantity(0.001 * 9.81 * 10, Physics.SI_Units.UnitFromSymbol("J"));

            Quantity E = m * g * h;

            Assert.AreEqual(expected, E);
        }


        /// <summary>
        ///A test for adding quantities of different units
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentException))]
        public void AdditionOfDifferentUnitsTest()
        {
            Quantity m = Quantity.Parse("1 g") as Quantity;
            Quantity h = Quantity.Parse("10 m") as Quantity;

            Quantity m_plus_h = m + h;
        }

        /// <summary>
        ///A test for subtracting quantities of different units
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(System.ArgumentException))]
        public void SubtractionOfDifferentUnitsTest()
        {
            Quantity m = Quantity.Parse("1 g") as Quantity;
            Quantity h = Quantity.Parse("10 m") as Quantity;

            Quantity m_minus_h = m - h;
        }

        /// <summary>
        ///A test for adding quantity with a number
        ///</summary>
        [TestMethod()]
        public void AdditionOfUnitsWithNumbersTest()
        {
            Quantity m = Quantity.Parse("1 g");
#pragma warning disable 219
            double h = 10.0; 
#pragma warning restore 219

            // Must not compile:  
            // Quantity m_plus_h = m + h;

            // Must not compile:  
            // Quantity h_plus_m = h + m;

        }

        /// <summary>
        ///A test for adding quantity with a number
        ///</summary>
        [TestMethod()]
        public void SubtractionOfUnitsWithNumbersTest()
        {
            Quantity m = Quantity.Parse("1 g");
#pragma warning disable 219
            double h = 10.0;
#pragma warning restore 219

            // Must not compile:  
            // Quantity m_sub_h = m - h;

            // Must not compile:  
            // Quantity h_sub_m = h - m;

        }

        #endregion Quantity math test

        #region Quantity ToString test

        /// <summary>
        ///A test for base unit Quantity ToString
        ///</summary>
        [TestMethod()]
        public void BaseUnitQuantityToStringTest()
        {
            Quantity pq = new Quantity(123.4, Physics.SI_Units.UnitFromSymbol("Kg"));

            //String expected = (123.4).ToString()+" SI.Kg";
            //String expected = (123.4).ToString(CultureInfo.InvariantCulture) + " Kg";
            String expected = (123.4).ToString() + " Kg";

            String actual = pq.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for named derived unit Quantity ToString
        ///</summary>
        [TestMethod()]
        public void NamedDerivedUnitQuantityToStringTest()
        {
            Quantity pq = new Quantity(0.001 * 9.81 * 10, Physics.SI_Units.UnitFromSymbol("J"));

            //String expected = (0.0981).ToString()+" SI.J";
            //String expected = (0.0981).ToString(CultureInfo.InvariantCulture) + " J";
            String expected = (0.0981).ToString() + " J";

            String actual = pq.ToString();

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for (unnamed) derived unit Quantity ToString
        ///</summary>
        [TestMethod()]
        public void DerivedUnitQuantityToStringTest()
        {
            Quantity pq = new Quantity(0.00987654321, new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -2, 0, 0, 0, 0 }));

            //String expected = (0.00987654321).ToString() + " SI.ms-2";
            //String expected = (0.00987654321).ToString(CultureInfo.InvariantCulture) + " m·s-2";
            String expected = (0.00987654321).ToString() + " m·s-2";

            String actual = pq.ToString();

            Assert.AreEqual(expected, actual);
        }


        #endregion Quantity ToString test

        #region Quantity functions test

        /// <summary>
        ///A test for Quantity 
        ///</summary>
        [TestMethod()]
        public void QuantityTest_QuantityGeVTest()
        {

            Quantity GeV = Prefix.G * (Constants.e * SI.V);

            Quantity GeVPowMinus2 = GeV ^ -2;
            Quantity GF = new Quantity(1.16639E-5 * GeVPowMinus2.Value, GeVPowMinus2.Unit);

            Assert.IsNotNull(GF);
        }


        Quantity EnergyEquivalentOfMass(Quantity mass)
        {
            /* Assert. ...(mass.Unit). ... == MeasureKind.Mass); */
            Quantity E = mass * Constants.c.Pow(2);
            return E;
        }

        /// <summary>
        ///A test for Quantity 
        ///</summary>
        [TestMethod()]
        public void QuantityTest_QuantityFunctionTest()
        {
            Quantity m = new Quantity(0.001, Physics.SI_Units.BaseUnits[(int)(PhysicalBaseQuantityKind.Mass)]);

            Unit MeterPerSecond = new DerivedUnit(Physics.SI_Units, new SByte[] { 1, 0, -1, 0, 0, 0, 0 });
            Quantity c = new Quantity(299792458, MeterPerSecond);

            Quantity expected = new Quantity(0.001 * 299792458 * 299792458, Physics.SI_Units.UnitFromSymbol("J"));

            Quantity E = EnergyEquivalentOfMass(m);
            Assert.AreEqual(expected, E);
        }


        /// <summary>
        ///A test for Quantity 
        ///</summary>
        [TestMethod()]
        public void QuantityHectoLitreTest()
        {

            Unit cubicmeter = new NamedDerivedUnit(Physics.SI_Units, "cubicmeter", "m3", new SByte[] { 3, 0, 0, 0, 0, 0, 0 });

            // Unit hl = new ConvertibleUnit("hectolitre", "hl", SI.m3, new ScaledValueConversion(1/10));
            //Unit kWh = (Unit)new ConvertibleUnit("kiloWattHour", "kWh", Wh, new ScaledValueConversion(1.0 / 1000)); /* [kWh] = 1/1000 * [Wh] */
            Unit hl = new ConvertibleUnit("hectolitre", "hl", cubicmeter, new ScaledValueConversion(10)); /* [hl] = 10 * [cubicmeter] */

            Quantity _10_hectolitre = new Quantity(10, hl);

            //IQuantity hektoLiterIncubicmeters = _10_hectolitre.ConvertTo(SI.m3);
            IQuantity _10_hektoLiterIncubicmeters = _10_hectolitre.ConvertTo(cubicmeter);

            Quantity expected = new Quantity(1, Physics.SI_Units.UnitFromSymbol("m").Pow(3));
            Assert.AreEqual(expected, _10_hektoLiterIncubicmeters);
        }


        /// <summary>
        ///A test for Quantity 
        ///</summary>
        [TestMethod()]
        public void QuantityLitreTest()
        {

            Unit hl = new ConvertibleUnit("hectolitre", "hl", SI.l, new ScaledValueConversion(1.0/100)); /* [hl] = 1/100 * [l] */
        
            Quantity _10_hectolitre = new Quantity(10, hl);

            IUnit cubicmeter = SI.m^3;
            IQuantity _10_hektoLiterIncubicmeters = _10_hectolitre.ConvertTo(SI.m^3);

            Quantity expected = new Quantity(1, SI.m^3);
            Assert.AreEqual(expected, _10_hektoLiterIncubicmeters);
        }


        #endregion Quantity functions test

    }
}
