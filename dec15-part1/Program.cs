using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input2.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        long result = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            string[] inputs = line.Split(',').ToArray();

            foreach (string input in inputs)
            {
                int hash = GetHashCode(input);

                result += hash;
            }
        }

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static int GetHashCode(string input)
    {
        int value = 0;
        foreach (char c in input)
        {
            value += (int)c;
            value *= 17;
            value %= 256;
        }

        return value;
    }
}