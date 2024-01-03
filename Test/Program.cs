internal class Program
{
    private static void Main()
    {
        //CanNotUseComplexTupleAsDictionaryKey();

        //Test_SortAndReassemble();
        Test_Alglib();
        Test_Alglib2();
    }

    private static void Test_Alglib2()
    {
        double[] x;
        alglib.densesolverlsreport rep;
        double[,] a = new double[,] { { 4, 2 }, { -1, 3 }, { 6, 5 } };
        double[] b = new double[] { 8, 5, 16 };
        alglib.rmatrixsolvels(a, 3, 2, b, 0.0, out int info, out rep, out x);
        System.Console.WriteLine("{0}", rep); // EXPECTED: 1
        System.Console.WriteLine("{0}", alglib.ap.format(x, 4)); // EXPECTED: [1.0000, 2.0000]
        System.Console.ReadLine();
    }

    private static void Test_Alglib()
    {
        // Define the matrix of coefficients (A) and the vector of constant terms (b)
        double[,] a = { { 1, 1 }, { 1, 2 }, { 1, 3 } }; // 3x2 matrix
        double[] b = { 1, 2, 3 }; // Vector of 3 elements

        // Create an object to store the solution and other information
        double[] x; // This will hold the solution
        int info;
        alglib.densesolverlsreport rep;

        // Solve the system
        alglib.rmatrixsolvels(a, 3, 2, b, 0.0, out info, out rep, out x);

        // Output the solution
        Console.WriteLine("Solution:");
        foreach (double xi in x)
        {
            Console.WriteLine(xi);
        }
    }

    private static void Test_SortAndReassemble()
    {
        string input = "#235#768##";
        string result = SortAndReassemble(input);
        Console.WriteLine(result);
    }

    public static string SortAndReassemble(string input)
    {
        // Splitting the string by '#'
        string[] parts = input.Split('#');

        // Extracting and sorting non-empty parts
        List<string> sortedParts = parts.Where(p => !string.IsNullOrEmpty(p))
                               .OrderBy(p => p)
                               .ToList();

        // Reassembling the string
        string result = "";
        int sortedIndex = 0;
        foreach (string part in parts)
        {
            if (string.IsNullOrEmpty(part))
            {
                result += "#";
            }
            else
            {
                result += sortedParts[sortedIndex++] + "#";
            }
        }

        // Append the trailing '#' if the input ends with it
        if (input.EndsWith("#"))
        {
            result += "#";
        }

        return result;
    }

    /// <summary>
    /// You cannot use a Tuple<char[], int[]> as a key of a Dictionary directly in C# because Tuple types are not suitable for use as keys in dictionaries due to their default implementations of GetHashCode and Equals. The default implementations of these methods rely on reference equality, which means that two different tuples with the same elements would not be considered equal as keys.
    /// </summary>
    private static void CanNotUseComplexTupleAsDictionaryKey()
    {
        // Create a Dictionary with Tuple<char[], int[]> as the key
        Dictionary<Tuple<char[], int[]>, string> keyValuePairs = [];

        // Create sample arrays
        char[] charArray1 = { 'a', 'b', 'c' };
        int[] intArray1 = { 1, 2, 3 };
        char[] charArray2 = { 'x', 'y', 'z' };
        int[] intArray2 = { 4, 5, 6 };

        // Add values to the dictionary
        keyValuePairs.Add(new Tuple<char[], int[]>(charArray1, intArray1), "First");
        keyValuePairs.Add(new Tuple<char[], int[]>(charArray2, intArray2), "Second");

        // Access values using the Tuple key
        string value = keyValuePairs[new Tuple<char[], int[]>(charArray1, intArray1)];
        Console.WriteLine(value); // Output: First

        // Iterate over the dictionary
        foreach (KeyValuePair<Tuple<char[], int[]>, string> kvp in keyValuePairs)
        {
            Console.WriteLine($"Key: ({string.Join(", ", kvp.Key.Item1)}), ({string.Join(", ", kvp.Key.Item2)}), Value: {kvp.Value}");
        }
    }
}