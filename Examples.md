**Examples**

The PhysicalMeasure library can be used to represent and handle physical measures and calculations of physical properties:

```
using PhysicalMeasure;

public void CalculateEnergyIn1Gram()
{
   PhysicalQuantity m = new PhysicalQuantity(0.001, SI.kg);
   PhysicalQuantity c = new PhysicalQuantity(299792458, SI.m / SI.s);

   PhysicalQuantity E = m * c.Pow(2);

   PhysicalQuantity expected = new PhysicalQuantity(0.001 * 299792458 * 299792458, SI.J);
   Assert.AreEqual(expected, E);
}
```

PhysicalMeasure also supports user defined units and unit systems:

```
using PhysicalMeasure;
using static PhysicalMeasure.SI;

public void CalculatePriceInEuroForEnergiConsumed()
{
   BaseUnit Euro = new BaseUnit(null, (SByte)MonetaryBaseQuantityKind.Currency, "Euro", "€");
   ConvertibleUnit Cent = new ConvertibleUnit("Euro-cent", "¢", Euro, 
                                              new ScaledValueConversion(100)); 
                                                    /* [¢](¢) = 100 * [€](€) */

   UnitSystem EuroUnitSystem = new UnitSystem("Euros", Physics.UnitPrefixes, Euro, null, 
                                              new ConvertibleUnit[]() { Cent } );
   PhysicalUnit EurosAndCents = new MixedUnit(Euro, " ", Cent,"00", true);

   PhysicalUnit kWh = Prefix.k * W * SI.h; // Kilo Watt hour
   PhysicalQuantity EnergyUnitPrice = 31.75 * Cent / kWh;
   PhysicalQuantity EnergyConsumed = 1234.56 * kWh;
   PhysicalQuantity PriceEnergyConsumed = EnergyConsumed * EnergyUnitPrice;
   Double PriceInEuroForEnergyConsumed = PriceEnergyConsumed.ConvertTo(Euro).Value;

   Assert.AreEqual(PriceInEuroForEnergyConsumed, 31.75/100 * 1234.56 );

   IPhysicalQuantity PriceEnergyConsumedEurosAndCents 
                                        = PriceEnergyConsumed.ConvertTo(EurosAndCents);
   String PriceInEuroForEnergyConsumedStr = PriceEnergyConsumedEurosAndCents.ToString();
   Assert.AreEqual(PriceInEuroForEnergyConsumedStr, "391 € 97 ¢");
}
```

