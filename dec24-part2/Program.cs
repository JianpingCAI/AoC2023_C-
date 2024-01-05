using System.Diagnostics;
using Vec3DRecord = AocLib.DataTypes.Vec3DRecord<long>;

record V(int x, int y, int z)
{
    internal Vec3DRecord Multiple(long t)
    {
        return new Vec3DRecord(x * t, y * t, z * t);
    }

    internal V Minus(V v0)
    {
        return new V(x - v0.x, y - v0.y, z - v0.z);
    }
}

record Data(Vec3DRecord s, V v);

internal class Solver
{
    public static (Vec3DRecord?, bool isFuture) CalculateIntersectPos(Vec3DRecord s1, V v1, Vec3DRecord s2, V v2)
    {
        // solve for t1
        Vec3DRecord S = new(s2.x - s1.x, s2.y - s1.y, 0);
        var tmp = v1.x * v2.y - v1.y * v2.x;
        var t1 = (v2.y * S.x - v2.x * S.y) / tmp;

        Vec3DRecord interSectPos = s1 + (v1.Multiple(t1));

        return (interSectPos, true);
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        List<Data> dataList = new(lines.Length);
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] arr = line.Split('@').ToArray();

            long[] s = arr[0].Split(',', StringSplitOptions.TrimEntries).Select(long.Parse).ToArray();
            int[] v = arr[1].Split(',', StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();

            dataList.Add(new Data(new Vec3DRecord(s[0], s[1], s[2]), new V(v[0], v[1], v[2])));
        }

        SortedSet<int> prevSetX = GetCommon(dataList, 'x');
        Console.WriteLine(prevSetX.Count);
        Console.WriteLine(prevSetX.Min());

        SortedSet<int> prevSetY = GetCommon(dataList, 'y');
        Console.WriteLine(prevSetY.Count);
        Console.WriteLine(prevSetY.Min());

        SortedSet<int> prevSetZ = GetCommon(dataList, 'z');
        Console.WriteLine(prevSetZ.Count);
        Console.WriteLine(prevSetZ.Min());

        (Vec3DRecord? startPos, bool isFuture) = GetStartPos(dataList, new V(prevSetX.First(), prevSetY.First(), prevSetZ.First()));

        long result = 0;

        if (isFuture)
        {
            result = (startPos.x + startPos.y + startPos.z);
            Console.WriteLine($"{startPos}");
        }

        sw.Stop();

        //646810057104753
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static (Vec3DRecord?, bool isFuture) GetStartPos(List<Data> dataList, V V0)
    {
        SortedSet<long> prevSet = [];

        Vec3DRecord s1 = dataList[0].s;
        Vec3DRecord s2 = dataList[1].s;

        V v1 = dataList[0].v;
        v1 = v1.Minus(V0);
        V v2 = dataList[1].v;
        v2 = v2.Minus(V0);

        return Solver.CalculateIntersectPos(s1, v1, s2, v2);
    }

    private static SortedSet<long> GetIntegers2(long s1, int v1, int v0)
    {
        SortedSet<long> ints = [];

        for (int t = 0; t < 100000; t++)
        {
            ints.Add(s1 + (v1 - v0) * t);
        }

        return ints;
    }

    private static SortedSet<int> GetCommon(List<Data> dataList, char vd)
    {
        Dictionary<int, List<Data>> v_data_pairs = [];

        foreach (Data data in dataList)
        {
            int value = 0;
            switch (vd)
            {
                case 'x':
                    value = data.v.x;
                    break;

                case 'y':
                    value = data.v.y;

                    break;

                case 'z':
                    value = data.v.z;

                    break;

                default:
                    break;
            }
            if (!v_data_pairs.TryGetValue(value, out List<Data> list))
            {
                list = new List<Data>();
                v_data_pairs[value] = list;
            }

            list.Add(data);
        }

        SortedSet<int> prevSet = [];

        foreach (KeyValuePair<int, List<Data>> item in v_data_pairs)
        {
            if (item.Value.Count >= 2)
            {
                Vec3DRecord s1 = item.Value[0].s;
                Vec3DRecord s2 = item.Value[1].s;

                long s1v = 0;
                long s2v = 0;

                switch (vd)
                {
                    case 'x':
                        s1v = s1.x;
                        s2v = s2.x;
                        break;

                    case 'y':
                        s1v = s1.y;
                        s2v = s2.y;
                        break;

                    case 'z':
                        s1v = s1.z;
                        s2v = s2.z;
                        break;

                    default:
                        break;
                }

                SortedSet<int> set = GetIntegers(s1v, s2v, item.Key);

                if (prevSet.Count > 0)
                {
                    if (set.Count > 0)
                    {
                        var commonInts = prevSet.Intersect(set).ToList();
                        prevSet.Clear();

                        foreach (var common in commonInts)
                        {
                            prevSet.Add(common);
                        }
                    }
                    else
                    {
                        prevSet.Clear();
                        break;
                    }
                }
                else
                {
                    prevSet = set;
                    if (set.Count == 0)
                    {
                        break;
                    }
                }
            }
        }

        return prevSet;
    }

    private static SortedSet<int> GetIntegers(long s1, long s2, int v1)
    {
        SortedSet<int> ints = new SortedSet<int>();

        for (int v = -1000; v < 1000; v++)
        {
            if (v - v1 == 0)
            {
                ints.Add(v);
                continue;
            }

            if ((s1 - s2) % (v - v1) == 0)
            {
                ints.Add(v);
            }
        }

        return ints;
    }
}