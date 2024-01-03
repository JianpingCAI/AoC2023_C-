using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

record Pos2D(int X, int Y);
record Pos3D(int X, int Y, int Z);

internal enum Type
{
    AlongX,
    AlongY,
    AlongZ,
};

internal class Brick(string name)
{
    public Pos3D? P1 { get; set; }
    public Pos3D? P2 { get; set; }
    public Type? Type { get; set; }

    public List<Brick> UpBricks { get; set; } = [];
    public List<Brick> DownBricks { get; set; } = [];
    public string Name { get; } = name;

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine($"{P1.X} {P1.Y} {P1.Z}");
        stringBuilder.AppendLine($"{P2.X} {P2.Y} {P2.Z}");

        return stringBuilder.ToString();
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        // step. get all bricks in z order
        List<Brick> sortedBricks = GenerateAllBricks(lines);

        // step. drop one by one from the lowest, by check existing cube
        // add it to the support list of its support (if any) (double linked)
        // the height map
        DropAllBricks_OneByOne(sortedBricks);

        //** step. remove bricks
        int result = CountRemovableBricks(sortedBricks);

        sw.Stop();
        // 459
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static int CountRemovableBricks(List<Brick> dropedBricks)
    {
        int result = 0;
        HashSet<string> names = [];
        List<Brick> freeBricks = [];
        foreach (Brick curBrick in dropedBricks)
        {
            //Console.WriteLine("Brick");
            //Console.WriteLine($"{curBrick}");

            if (curBrick.UpBricks.Count == 0)
            {
                //names.Add(curBrick.Name);
                //Console.WriteLine("** free brick");
                result++;

                freeBricks.Add(curBrick);
                continue;
            }

            if (curBrick.UpBricks.Count > 0)
            {
                bool canRemoveCurrent = curBrick.UpBricks.All(x => x.DownBricks.Count >= 2);
                if (canRemoveCurrent)
                {
                    names.Add(curBrick.Name);
                    result++;
                    //Console.WriteLine("free brick");

                    freeBricks.Add(curBrick);
                }
            }
        }

        foreach (var item in freeBricks)
        {
            Console.WriteLine($"free: {item}");
        }
        //result = names.Count;
        return result;
    }

    private static void DropAllBricks_OneByOne(List<Brick> sortedBricks)
    {
        Dictionary<Pos2D, Brick> dict_pos2D_highestBrick = [];

        // drop one by one in z order
        foreach (Brick curBrick in sortedBricks)
        {
            // find overlapped bricks if any
            List<Brick> downBricks = FindHighestDownBricks(dict_pos2D_highestBrick, curBrick);

            // update relation
            if (downBricks.Count > 0)
            {
                curBrick.DownBricks.AddRange(downBricks);
                foreach (Brick downBrick in downBricks)
                {
                    downBrick.UpBricks.Add(curBrick);
                }
            }

            // update its current location ( in Z direction)
            int curZ = 1;
            if (downBricks.Count > 0)
            {
                curZ = downBricks[0].P2.Z + 1;

                if (downBricks.Count(x => x.P2.Z != downBricks[0].P2.Z) > 1)
                {
                    throw new Exception();
                }
            }

            int originLenDiff = curBrick.P2.Z - curBrick.P1.Z;

            // drop the current brick
            curBrick.P1 = curBrick.P1 with { Z = curZ };
            curBrick.P2 = curBrick.P2 with { Z = curZ + originLenDiff };

            //  update the height map
            switch (curBrick.Type)
            {
                case Type.AlongX:
                    {
                        for (int x = curBrick.P1.X; x <= curBrick.P2.X; x++)
                        {
                            Pos2D pos2D = new(x, curBrick.P1.Y);
                            dict_pos2D_highestBrick[pos2D] = curBrick;
                        }
                    }
                    break;

                case Type.AlongY:
                    {
                        for (int y = curBrick.P1.Y; y <= curBrick.P2.Y; y++)
                        {
                            Pos2D pos2D = new(curBrick.P1.X, y);
                            dict_pos2D_highestBrick[pos2D] = curBrick;
                        }
                    }
                    break;

                case Type.AlongZ:
                    {
                        Pos2D pos2D = new(curBrick.P1.X, curBrick.P1.Y);
                        dict_pos2D_highestBrick[pos2D] = curBrick;
                    }
                    break;

                default:
                    break;
            }
        }

        foreach (var item in sortedBricks)
        {
            item.UpBricks = item.UpBricks.DistinctBy(x => x.P1).ToList();
            item.DownBricks = item.DownBricks.DistinctBy(x => x.P1).ToList();
        }

        foreach (var item in sortedBricks)
        {
            if (item.P1.X == 0 && item.P1.Y == 5 && item.P1.Z == 2)
            {
                Console.WriteLine($"Found {item}");
                break;
            }
        }
    }

    private static List<Brick> GenerateAllBricks(string[] lines)
    {
        List<Brick> bricks = [];
        // parse
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] tmp1 = line.Split('~').ToArray();
            int[] p1 = tmp1[0].Split(',').Select(int.Parse).ToArray();
            int[] p2 = tmp1[1].Split(",").Select(int.Parse).ToArray();

            Brick brick = new((((char)('A' + i)).ToString()))
            {
                P1 = new Pos3D(p1[0], p1[1], p1[2]),
                P2 = new Pos3D(p2[0], p2[1], p2[2]),
            };

            if (p1[0] == p2[0] && p1[1] == p2[1] && p1[2] != p2[2])
            {
                brick.Type = Type.AlongZ;
            }
            else if (p1[0] == p2[0] && p1[1] != p2[1] && p1[2] == p2[2])
            {
                brick.Type = Type.AlongY;
            }
            else if (p1[0] != p2[0] && p1[1] == p2[1] && p1[2] == p2[2])
            {
                brick.Type = Type.AlongX;
            }
            else if (p1[0] == p2[0] && p1[1] == p2[1] && p1[2] == p2[2])
            {
                brick.Type = Type.AlongZ;
            }

            bricks.Add(brick);
        }

        // step. order by z
        bricks.Sort((a, b) => a.P1.Z.CompareTo(b.P1.Z));

        foreach (var item in bricks)
        {
            if (item.P1.Z > item.P2.Z)
            {
                throw new Exception();
            }
        }

        return bricks;
    }

    /// <summary>
    /// Find highest down bricks
    /// </summary>
    private static List<Brick> FindHighestDownBricks(
        Dictionary<Pos2D, Brick> dict_pos_highestBrick,
        Brick curBrick)
    {
        List<Brick> highestDownBricks = [];
        int maxHeight = 0;

        if (curBrick.P1.Z == 1)
        {
            return highestDownBricks;
        }

        switch (curBrick.Type)
        {
            case Type.AlongX:
                {
                    int y = curBrick.P1.Y;
                    for (int x = curBrick.P1.X; x <= curBrick.P2.X; x++)
                    {
                        Pos2D xyPos = new(x, y);

                        if (dict_pos_highestBrick.TryGetValue(xyPos, out Brick? downBrick))
                        {
                            if (downBrick.P2.Z > maxHeight)
                            {
                                maxHeight = downBrick.P2.Z;

                                highestDownBricks.Clear();
                                highestDownBricks.Add(downBrick);
                            }
                            else if (downBrick.P2.Z == maxHeight)
                            {
                                highestDownBricks.Add(downBrick);
                            }
                        }
                    }
                }
                break;

            case Type.AlongY:
                {
                    int x = curBrick.P1.X;
                    for (int y = curBrick.P1.Y; y <= curBrick.P2.Y; y++)
                    {
                        Pos2D xyPos = new(x, y);

                        if (dict_pos_highestBrick.TryGetValue(xyPos, out Brick? downBrick))
                        {
                            if (downBrick.P2.Z > maxHeight)
                            {
                                maxHeight = downBrick.P2.Z;

                                highestDownBricks.Clear();
                                highestDownBricks.Add(downBrick);
                            }
                            else if (downBrick.P2.Z == maxHeight)
                            {
                                highestDownBricks.Add(downBrick);
                            }
                        }
                    }
                }
                break;

            case Type.AlongZ:
                {
                    Pos2D xyPos = new(curBrick.P1.X, curBrick.P1.Y);

                    if (dict_pos_highestBrick.TryGetValue(xyPos, out Brick? downBrick))
                    {
                        highestDownBricks.Add(downBrick);
                    }
                }
                break;

            default:
                break;
        }

        return highestDownBricks;
    }
}