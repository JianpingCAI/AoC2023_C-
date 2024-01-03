using System.Diagnostics;

internal class Program
{
    private static int ROWs = 0;
    private static int COLs = 0;
    private static readonly int MaxDepth = 64;

    private static readonly bool IsDebugPrint = true;

    record Pos(int row, int col)
    {
        internal Pos? Left()
        {
            return (this.col >= 1) ? (this with { col = this.col - 1 }) : null;
        }

        internal Pos? Right()
        {
            return (this.col + 1 < COLs) ? (this with { col = this.col + 1 }) : null;
        }

        internal Pos? Up()
        {
            return (this.row >= 1) ? (this with { row = this.row - 1 }) : null;
        }

        internal Pos? Down()
        {
            return (this.row + 1 < COLs) ? (this with { row = this.row + 1 }) : null;
        }

        internal int Dist(Pos startPos)
        {
            return Math.Abs(startPos.col - this.col) + Math.Abs(startPos.row - this.row);
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
                Pos? next = curPos.Left();
                if (next != null && lines[next.row][next.col] == '.')
                {
                    que_pos_depth.Enqueue(new Tuple<Pos, int>(next, depth + 1));
                }

                next = curPos.Right();
                if (next != null && lines[next.row][next.col] == '.')
                {
                    que_pos_depth.Enqueue(new Tuple<Pos, int>(next, depth + 1));
                }

                next = curPos.Up();
                if (next != null && lines[next.row][next.col] == '.')
                {
                    que_pos_depth.Enqueue(new Tuple<Pos, int>(next, depth + 1));
                }

                next = curPos.Down();
                if (next != null && lines[next.row][next.col] == '.')
                {
                    que_pos_depth.Enqueue(new Tuple<Pos, int>(next, depth + 1));
                }
            }
        }

        return dict_depth_count;
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
                    return new Pos(i, j);
                }
            }
        }

        throw new Exception();
    }
}