namespace AocMath
{
    public static class IntegerTypeConstraint
    {
        public static bool Check<T>()
        {
            return typeof(T) == typeof(int) ||
                   typeof(T) == typeof(long) ||
                   typeof(T) == typeof(short) ||
                   //typeof(T) == typeof(byte) ||
                   typeof(T) == typeof(uint) ||
                   typeof(T) == typeof(ulong) ||
                   typeof(T) == typeof(ushort);// || typeof(T) == typeof(sbyte);
        }
    }

    public class LCMSolver<T>
        where T : struct
    {
        public LCMSolver()
        {
            if (!IntegerTypeConstraint.Check<T>())
            {
                throw new InvalidOperationException("Type must be an integer type");
            }
        }

        // Greatest Common Divisor
        public T GCD(T a, T b)
        {
            dynamic da = a;
            dynamic db = b;
            while (db != 0)
            {
                dynamic temp = db;
                db = da % db;
                da = temp;
            }
            return da;
        }

        // Least Common Multiplier
        public T LCM(T a, T b)
        {
            dynamic da = a;
            dynamic db = b;
            return Math.Abs(da * db) / GCD(a, b);
        }

        public T LCM(T[] values)
        {
            if (values.Length < 2)
            {
                throw new ArgumentException();
            }

            T lcm = values[0];
            for (int i = 1; i < values.Length; i++)
            {
                lcm = LCM(lcm, values[i]);
            }

            return lcm;
        }
    }
}