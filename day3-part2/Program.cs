using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        int result = 0;

        List<string> groupStrings = [];

        for (int i = 0; i < lines.Length; i++)
        {
            groupStrings.Add(lines[i]);

            if ((i + 1) % 3 == 0)
            {
                char groupType = GetGroupType(groupStrings);

                result += GetTypeValue(groupType);

                groupStrings.Clear();
            }
        }

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static char GetGroupType(List<string> groupStrings)
    {
        HashSet<char> charSet = new(groupStrings[0]);

        HashSet<char> common = [];

        foreach (char c in groupStrings[1])
        {
            if (charSet.Contains(c))
            {
                common.Add(c);
            }
        }

        foreach (char c in groupStrings[2])
        {
            if (common.Contains(c))
            {
                return c;
            }
        }

        throw new Exception();
    }

    private static int GetTypeValue(char type)
    {
        if (type >= 'a' && type <= 'z')
        {
            return type - 'a' + 1;
        }

        return type - 'A' + 27;
    }
}