using System.Diagnostics;

record Range(int Start, int End);

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        int result = 0;
        bool isPrint = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            Range[] ranges = line.Split(',').Select(x => { var pair = x.Split('-').Select(int.Parse).ToArray(); return new Range(pair[0], pair[1]); }).ToArray();

            if (IsFullContain(ranges[0], ranges[1])
                || IsFullContain(ranges[1], ranges[0]))
            {
                ++result;
            }
        }

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static bool IsFullContain(Range range1, Range range2)
    {
        if (range2.Start >= range1.Start && range2.End <= range1.End)
        {
            return true;
        }

        return false;
    }
}