using System.Diagnostics;

internal class Program_naive
{
    private static int ROWs = 0;
    private static int COLs = 0;
    private static readonly int MaxDepth = 26501365;

    private static readonly bool IsDebugPrint = true;

    record Pos(int row, int col)
    {
        internal Pos Left()
        {
            return this with { col = col - 1 };
        }

        internal Pos Right()
        {
            return (this with { col = col + 1 });
        }

        internal Pos Up()
        {
            return this with { row = row - 1 };
        }

        internal Pos Down()
        {
            return this with { row = row + 1 };
        }
    }

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();
        ROWs = lines.Length;
        COLs = lines[0].Length;

        // step1. Locate the start position
        Pos startPos = LocateStartPos(lines);

        // step. BFS traverse
        Dictionary<int, int> dict_depth_count = BFS(startPos, lines);

        if (IsDebugPrint)
        {
            foreach (KeyValuePair<int, int> depth_count in dict_depth_count)
            {
                Console.WriteLine($"{depth_count.Key}: {depth_count.Value}");
            }
        }

        // step. calculate
        long result = 0;
        for (int d = 0; d <= MaxDepth; d += 2)
        {
            result += dict_depth_count[d];
        }

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static Dictionary<int, int> BFS(Pos startPos, string[] lines)
    {
        Dictionary<int, int> dict_depth_count = [];
        for (int i = 0; i <= MaxDepth; i++)
        {
            dict_depth_count.Add(i, 0);
        }

        HashSet<Pos> visited = [];
        Queue<Tuple<Pos, int>> que_pos_depth = new();
        que_pos_depth.Enqueue(new Tuple<Pos, int>(startPos, 0));

        while (que_pos_depth.Count > 0)
        {
            Tuple<Pos, int> qItem = que_pos_depth.Dequeue();
            Pos curPos = qItem.Item1;
            int depth = qItem.Item2;

            if (visited.Contains(curPos))
            {
                continue;
            }
            visited.Add(curPos);

            dict_depth_count[depth]++;

            //L,R,U,D
            if (depth < MaxDepth)
            {
                Pos next = curPos.Left();
                Pos p = GetMappedPosition(next);
                if (lines[p.row][p.col] == '.')
                {
                    que_pos_depth.Enqueue(new Tuple<Pos, int>(next, depth + 1));
                }

                next = curPos.Right();
                p = GetMappedPosition(next);
                if (lines[p.row][p.col] == '.')
                {
                    que_pos_depth.Enqueue(new Tuple<Pos, int>(next, depth + 1));
                }

                next = curPos.Up();
                p = GetMappedPosition(next);
                if (lines[p.row][p.col] == '.')
                {
                    que_pos_depth.Enqueue(new Tuple<Pos, int>(next, depth + 1));
                }

                next = curPos.Down();
                p = GetMappedPosition(next);
                if (lines[p.row][p.col] == '.')
                {
                    que_pos_depth.Enqueue(new Tuple<Pos, int>(next, depth + 1));
                }
            }
        }

        return dict_depth_count;
    }

    private static Pos GetMappedPosition(Pos p)
    {
        int row = p.row % ROWs;
        if (row < 0)
        {
            row += ROWs;
        }

        int col = p.col % COLs;
        if (col < 0)
        {
            col += COLs;
        }

        return new Pos(row, col);
    }

    private static Pos LocateStartPos(string[] lines)
    {
        int i;
        for (i = 0; i < lines.Length; i++)
        {
            int j;
            for (j = 0; j < lines[i].Length; j++)
            {
                if (lines[i][j] == 'S')
                {
                    lines[i] = lines[i].Replace('S', '.');
                    return new Pos(i, j);
                }
            }
        }

        throw new Exception();
    }
}