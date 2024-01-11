using System.Diagnostics;

internal class Program
{
    record Record(char Opp, char You);
    private static Dictionary<char, char> map_ABC_WIN = [];
    private static Dictionary<char, char> map_ABC_LOSE = [];

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        //R, P, S
        map_ABC_WIN.Add('A', 'B');
        map_ABC_WIN.Add('B', 'C');
        map_ABC_WIN.Add('C', 'A');

        map_ABC_LOSE.Add('A', 'C');
        map_ABC_LOSE.Add('B', 'A');
        map_ABC_LOSE.Add('C', 'B');

        Dictionary<char, char> map_XYZ_ABC = [];
        map_XYZ_ABC.Add('X', 'A');
        map_XYZ_ABC.Add('Y', 'B');
        map_XYZ_ABC.Add('Z', 'C');

        Dictionary<Record, int> map_Record_Score = [];
        map_Record_Score.Add(new Record('A', 'B'), 6 + 2);
        map_Record_Score.Add(new Record('A', 'C'), 0 + 3);

        map_Record_Score.Add(new Record('B', 'A'), 0 + 1);
        map_Record_Score.Add(new Record('B', 'C'), 6 + 3);

        map_Record_Score.Add(new Record('C', 'A'), 6 + 1);
        map_Record_Score.Add(new Record('C', 'B'), 0 + 2);

        map_Record_Score.Add(new Record('A', 'A'), 3 + 1);
        map_Record_Score.Add(new Record('B', 'B'), 3 + 2);
        map_Record_Score.Add(new Record('C', 'C'), 3 + 3);

        Record[] records = new Record[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            char[] record = line.Split(' ').Select(x => x[0]).ToArray();
            records[i] = new Record(record[0], GetChoice(record[1], record[0]));
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

    // lose, draw, win
    private static char GetChoice(char v, char opp)
    {
        switch (v)
        {
            case 'X':
                return map_ABC_LOSE[opp];

            case 'Y':
                return opp;

            case 'Z':
                return map_ABC_WIN[opp];
        }

        return ' ';
    }
}