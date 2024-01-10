namespace AocLib
{
    public class DataConverter
    {
        public static long BinaryToLong(string binString)
        {
            return Convert.ToInt64(binString, 2);
        }

        public static int BinaryToInt32(char[] binChars)
        {
            return Convert.ToInt32(new string(binChars), 2);
        }
    }
}