namespace PhysicalMeasure
{
    public static class MathUtils
    {
        public static int DivRem(int a, int b, out int result)
        {
            return System.Math.DivRem(a, b, out result);
        }
    }
}