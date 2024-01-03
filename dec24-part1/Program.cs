using System.Diagnostics;

record Pos(double x, double y, double z);
record V(int x, int y, int z);

record Data(Pos s, V v);

internal class Solver
{
    public static (Pos?, bool isFuture) CalculateIntersectPos(Pos s1, V v1, Pos s2, V v2)
    {
        // check if parallel
        if (IsParallel_XYPlane(v1, v2))
        {
            return (null, false);
        }

        // not parallel
        Pos S = new(s2.x - s1.x, s2.y - s1.y, 0);
        double tmp = v1.x * v2.y - v1.y * v2.x;
        double t1 = (v2.y * S.x - v2.x * S.y) / tmp;
        if (t1 <= 0)
        {
            return (null, false);
        }

        double t2 = (v1.y * S.x - v1.x * S.y) / tmp;//(v1.x * v2.y - v2.x * v1.y);
        if (t2 <= 0)
        {
            return (null, false);
        }

        Pos interSectPos = GetPosition_XYPlane(s1, v1, t1);
        return (interSectPos, true);
    }

    private static Pos GetPosition_XYPlane(Pos s1, V v1, double t1)
    {
        return new Pos(s1.x + v1.x * t1, s1.y + v1.y * t1, 0);
    }

    private static bool IsParallel_XYPlane(V v1, V v2)
    {
        return v1.x * v2.y == v1.y * v2.x;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        int result = 0;
        bool isPrint = false;

        List<Data> dataList = new(lines.Length);
        double scale = double.MaxValue;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] arr = line.Split('@').ToArray();

            double[] s = arr[0].Split(',', StringSplitOptions.TrimEntries).Select(double.Parse).ToArray();
            int[] v = arr[1].Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();

            dataList.Add(new Data(new Pos(s[0], s[1], s[2]), new V(v[0], v[1], v[2])));

            scale = double.Min(scale, s[0]);
        }

        scale = 1.0;

        for (int i = 0; i < dataList.Count; i++)
        {
            Data data = dataList[i];
            dataList[i] = data with { s = data.s with { x = data.s.x / scale, y = data.s.y / scale, z = 0.0 } };
        }

        //double MinValue = 7 / scale;
        //double MaxValue = 27 / scale;
        double MinValue = 200000000000000 / scale;
        double MaxValue = 400000000000000 / scale;

        Pos MinPos = new(MinValue, MinValue, 0);
        Pos MaxPos = new(MaxValue, MaxValue, 0);
        for (int i = 0; i < dataList.Count - 1; i++)
        {
            for (int j = i + 1; j < dataList.Count; j++)
            {
                (Pos? pos, bool isFuture) = Solver.CalculateIntersectPos(dataList[i].s, dataList[i].v, dataList[j].s, dataList[j].v);

                if (pos != null && isFuture)
                {
                    if (IsInRange_XYPlane(pos, MinPos, MaxPos))
                    {
                        ++result;
                    }
                }
            }
        }

        sw.Stop();

        // too high: 17311
        //           18866

        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static bool IsInRange_XYPlane(Pos pos, Pos minPos, Pos maxPos)
    {
        //x,y
        return pos.x >= minPos.x && pos.x <= maxPos.x
             && pos.y >= minPos.y && pos.y <= maxPos.y;
    }
}