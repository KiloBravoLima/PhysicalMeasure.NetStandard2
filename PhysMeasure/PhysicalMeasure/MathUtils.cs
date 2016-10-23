using System;

namespace PhysicalMeasure
{
    public static class MathUtils
    {
        public static int DivRem(int a, int b, out int result)
        {
#if NETCORE
            result = a % b;
            return a / b;
#else
            return Math.DivRem(a, b, out result);
#endif
        }
    }
}