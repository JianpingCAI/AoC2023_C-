using System.Diagnostics;
using Vec3DRecord = AocLib.DataTypes.Vec3DRecord<double>;

record V(int x, int y, int z);

record Data(Vec3DRecord s, V v);

internal class Solver
{
    public static (Vec3DRecord?, bool isFuture) CalculateIntersectPos(Vec3DRecord s1, V v1, Vec3DRecord s2, V v2)
    {
        // check if parallel
        if (IsParallel_XYPlane(v1, v2))
        {
            return (null, false);
        }

        // not parallel
        Vec3DRecord S = new(s2.x - s1.x, s2.y - s1.y, 0);
        double tmp = v1.x * v2.y - v1.y * v2.x;
        double t1 = (v2.y * S.x - v2.x * S.y) / tmp;
        if (t1 <= 0)
        {
            return (null, false);
        }

        double t2 = (v1.y * S.x - v1.x * S.y) / tmp;
        if (t2 <= 0)
        {
            return (null, false);
        }

        Vec3DRecord interSectPos = GetPosition_XYPlane(s1, v1, t1);
        return (interSectPos, true);
    }

    private static Vec3DRecord GetPosition_XYPlane(Vec3DRecord s1, V v1, double t1)
    {
        return new Vec3DRecord(s1.x + v1.x * t1, s1.y + v1.y * t1, 0);
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

        List<Data> dataList = new(lines.Length);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] arr = line.Split('@').ToArray();

            double[] s = arr[0].Split(',', StringSplitOptions.TrimEntries).Select(double.Parse).ToArray();
            int[] v = arr[1].Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();

            dataList.Add(new Data(new Vec3DRecord(s[0], s[1], s[2]), new V(v[0], v[1], v[2])));
        }

        double MinValue = 200000000000000;
        double MaxValue = 400000000000000;

        Vec3DRecord MinPos = new(MinValue, MinValue, 0);
        Vec3DRecord MaxPos = new(MaxValue, MaxValue, 0);
        for (int i = 0; i < dataList.Count - 1; i++)
        {
            for (int j = i + 1; j < dataList.Count; j++)
            {
                (Vec3DRecord? pos, bool isFuture) = Solver.CalculateIntersectPos(dataList[i].s, dataList[i].v, dataList[j].s, dataList[j].v);

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

        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static bool IsInRange_XYPlane(Vec3DRecord pos, Vec3DRecord minPos, Vec3DRecord maxPos)
    {
        //x,y
        return pos.x >= minPos.x && pos.x <= maxPos.x
             && pos.y >= minPos.y && pos.y <= maxPos.y;
    }
}