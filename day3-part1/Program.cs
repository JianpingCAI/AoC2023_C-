using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        int result = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string first = new(line.Take(line.Length / 2).ToArray());
            string second = new(line.Skip(line.Length / 2).ToArray());

            char type = FindSharedChar(first, second);
            int typeValue = GetTypeValue(type);

            result += typeValue;
        }

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static int GetTypeValue(char type)
    {
        if (type >= 'a' && type <= 'z')
        {
            return type - 'a' + 1;
        }

        return type - 'A' + 27;
    }

    private static char FindSharedChar(string first, string second)
    {
        HashSet<char> chars = new(first.ToArray());

        foreach (char c in second)
        {
            if (chars.Contains(c))
                return c;
        }

        throw new Exception();
    }
}