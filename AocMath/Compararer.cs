namespace AocLib
{
    public class Comparer
    {
        public static bool IsOneBitDifferent<T>(long value1, long value2)
        {
            if (value1 == value2)
            {
                return false;
            }

            long n = value1 ^ value2;
            bool isOneBitDiff = (n > 0 && (n & (n - 1)) == 0);

            return isOneBitDiff;
        }

        public static bool IsOneBitDifferent<T>(int value1, int value2)
        {
            if (value1 == value2)
            {
                return false;
            }

            int n = value1 ^ value2;
            bool isOneBitDiff = (n > 0 && (n & (n - 1)) == 0);

            return isOneBitDiff;
        }
    }
}