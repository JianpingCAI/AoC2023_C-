using System.Diagnostics;

internal class Program_trial
{
    private static readonly int STEPS = 6;
    private static int ROWs = 0;
    private static int COLs = 0;
    private static Dictionary<int, long> Dict_Steps_Count = [];
    record Pos(int row, int col);

    private static void Main(string[] args)
    {
        string filePath = "input2.txt";
        string[] lines = File.ReadAllLines(filePath);
        ROWs = lines.Length;
        COLs = lines[0].Length;

        Stopwatch sw = Stopwatch.StartNew();

        // step1
        Pos startPos = LocateStartPos(lines);

        // step2
        Dict_Steps_Count = CountWithNoBlocks(STEPS);

        // step3
        long result = CountWithBlocks(startPos, lines);

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static long CountWithBlocks(Pos startPos, string[] mat)
    {
        long result = 0;
        for (int steps = 1; steps <= STEPS; steps++)
        {
            for (int x = 0; x <= steps; x++)
            {
                int y = steps - x;

                //(+x,+y)
                if (startPos.row + x < ROWs && startPos.col + y < COLs)
                {
                    if (mat[startPos.row + x][startPos.col + y] == '#')
                    {
                        MakeCorrection(steps);
                    }
                }

                //(-x,-y)
                if (startPos.row - x >= 0 && startPos.col - y >= 0)
                {
                    if (mat[startPos.row - x][startPos.col - y] == '#')
                    {
                        MakeCorrection(steps);
                    }
                }

                //(+y, +x)
                if (startPos.row + y < ROWs && startPos.col + x < COLs)
                {
                    if (mat[startPos.row + y][startPos.col + x] == '#')
                    {
                        MakeCorrection(steps);
                    }
                }

                //(-y,-x)
                if (startPos.row - y >= 0 && startPos.col - x >= 0)
                {
                    if (mat[startPos.row - y][startPos.col - x] == '#')
                    {
                        MakeCorrection(steps);
                    }
                }
            }
        }

        for (int i = 2; i <= STEPS; i += 2)
        {
            result += Dict_Steps_Count[i];
        }

        result += 1;

        return result;
    }

    private static void MakeCorrection(int steps)
    {
        Dict_Steps_Count[steps]--;
        if (steps + 2 <= STEPS)
        {
            Dict_Steps_Count[steps + 2]++;
        }
    }

    private static void CountWithBlockedDistance(int steps, Pos startPos, string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            for (int j = 0; j < lines[i].Length; j++)
            {
                if (Math.Abs(i - startPos.row) + Math.Abs(j - startPos.col) == steps
                    && lines[i][j] == '#')
                {
                    Dict_Steps_Count[steps]--;
                    if (steps < STEPS)
                    {
                        Dict_Steps_Count[steps + 2]++;
                    }
                }
            }
        }

        // Math.Abs(i - startPos.row) + Math.Abs(j - startPos.col) == steps
    }

    private static Pos LocateStartPos(string[] lines)
    {
        int i;
        int j = 0;
        for (i = 0; i < lines.Length; i++)
        {
            for (j = 0; j < lines[i].Length; j++)
            {
                if (lines[i][j] == 'S')
                {
                    return new Pos(i, j);
                }
            }
        }

        throw new Exception();
    }

    private static Dictionary<int, long> CountWithNoBlocks(int steps)
    {
        Dictionary<int, long> dict_steps_count = [];

        long sum = 0;
        long curCount = 0;
        for (int i = 1; i <= steps; i++)
        {
            curCount += 4;
            sum += curCount;

            dict_steps_count.Add(i, sum);
        }

        return dict_steps_count;
    }
}