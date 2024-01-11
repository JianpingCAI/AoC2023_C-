using System.Diagnostics;

internal class Program
{
    record Record(char Opp, char You);

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        Dictionary<char, char> mapABC2XYZ = [];
        mapABC2XYZ.Add('A', 'X');
        mapABC2XYZ.Add('B', 'Y');
        mapABC2XYZ.Add('C', 'Z');

        Dictionary<Record, int> map_Record_Score = [];
        map_Record_Score.Add(new Record('X', 'Y'), 6 + 2);
        map_Record_Score.Add(new Record('X', 'Z'), 0 + 3);

        map_Record_Score.Add(new Record('Y', 'X'), 0 + 1);
        map_Record_Score.Add(new Record('Y', 'Z'), 6 + 3);

        map_Record_Score.Add(new Record('Z', 'X'), 6 + 1);
        map_Record_Score.Add(new Record('Z', 'Y'), 0 + 2);

        map_Record_Score.Add(new Record('X', 'X'), 3 + 1);
        map_Record_Score.Add(new Record('Y', 'Y'), 3 + 2);
        map_Record_Score.Add(new Record('Z', 'Z'), 3 + 3);

        Record[] records = new Record[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            char[] record = line.Split(' ').Select(x => x[0]).ToArray();
            records[i] = new Record(mapABC2XYZ[record[0]], record[1]);
        }

        int result = 0;

        foreach (Record record in records)
        {
            result += map_Record_Score[record];
        }

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }
}