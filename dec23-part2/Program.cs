using System.Diagnostics;

record Pos(int i, int j);

public enum Direction
{
    Left,
    Right,
    Up,
    Down
}

internal class Path
{
    public Pos StartPos { get; set; }
    public Pos? EndPos { get; set; } = null;
    public Direction StartDir { get; set; }

    public int Length { get; set; } = 0;

    public Path? PrevPath { get; set; } = null;
    public List<Path> NextPaths { get; set; } = [];
}

internal class PathConnection
{
    public Path Path { get; set; }

    public List<Path> NextPaths { get; set; } = [];
}

internal class Program
{
    private static readonly int _start_i = 0;
    private static readonly int _start_j = 1;

    private static int _ROWs = 0;
    private static int _COLs = 0;

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        string[] mat = lines;
        _ROWs = mat.Length;
        _COLs = mat[0].Length;

        Path rootPath = new()
        {
            StartPos = new(_start_i, _start_j),
            StartDir = Direction.Down,
        };

        int result = GetMaxLength_DFS(rootPath, mat) - 1;

        sw.Stop();

        // too high: 6387
        // 6302
        // C# version: Time = 433.4578745 seconds
        // C++ one:    Time = 4.337 seconds
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");

        Console.WriteLine($"MAX = {result}");
    }

    private static HashSet<Pos> _curVisitedStartEndList = [];

    private static int GetMaxLength_DFS(Path curPath, string[] map)
    {
        // walk until encounter junction
        (Pos curEndPos, int curPathLength, List<Tuple<Pos, Direction>> nextPathStartInfos)
            = WalkTillJunction_Part2(curPath.StartPos, curPath.StartDir, map);

        curPath.EndPos = curEndPos;
        curPath.Length = curPathLength;

        if (IsMapExit(curPath.EndPos))
        {
            //Console.WriteLine($"\n*({curPath.StartPos},{curPath.StartPos}): {curPath.Length}");
            return curPath.Length;
        }
        if (_curVisitedStartEndList.Contains(curPath.EndPos) || _curVisitedStartEndList.Contains(curPath.StartPos))
        {
            return -int.MaxValue;
        }

        _curVisitedStartEndList.Add(curPath.StartPos);
        _curVisitedStartEndList.Add(curPath.EndPos);

        int maxTotalLength = -int.MaxValue;
        // find the max length of its sub paths
        foreach (Tuple<Pos, Direction> nextStart in nextPathStartInfos)
        {
            // avoid loop
            if (!_curVisitedStartEndList.Contains(nextStart.Item1))
            {
                Path nextPath = new()
                {
                    StartPos = nextStart.Item1,
                    StartDir = nextStart.Item2,
                    PrevPath = curPath
                };

                int len = GetMaxLength_DFS(nextPath, map);

                maxTotalLength = int.Max(maxTotalLength, curPath.Length + len);
            }
        }

        _curVisitedStartEndList.Remove(curPath.StartPos);
        _curVisitedStartEndList.Remove(curPath.EndPos);

        return maxTotalLength;
    }

    private static bool IsMapExit(Pos endPos)
    {
        return endPos.i == _ROWs - 1 && endPos.j == _COLs - 2;
    }

    private static (Pos curEndPos, int length, List<Tuple<Pos, Direction>> nextStartPos)
        WalkTillJunction_Part2(Pos startPos, Direction startDir, string[] map)
    {
        int length = 0;

        Direction curDir = startDir;
        int i = startPos.i;
        int j = startPos.j;

        // find path segment end point (with the input startPos)
        // stop if 1) exit the map, or 2) find a junction
        bool isContinue = true;
        List<Direction> nextDirs = [];

        // row
        //while (i < _ROWs)
        for (i = startPos.i; i < _ROWs;)
        {
            // column
            //while (j < _COLs)
            for (j = startPos.j; j < _COLs;)
            {
                ++length;

                nextDirs.Clear();

                //if get to the final exit
                if (i == _ROWs - 1 && j == _COLs - 2)
                {
                    isContinue = false;
                    break;
                }

                // next step directions
                nextDirs = GetNextDirections(i, j, curDir, map);

                // 1) no way to go
                //if (nextDirs.Count == 0)
                //{
                //    isContinue = false;
                //    break;
                //}

                Direction? nextDir = null;

                // 2) there is a junction
                if (nextDirs.Count > 1)
                {
                    isContinue = false;
                    break;
                }

                // 3) continue with a single path
                else if (nextDirs.Count == 1)
                {
                    nextDir = nextDirs[0];
                }

                switch (nextDir!.Value)
                {
                    case Direction.Left:
                        {
                            --j;
                        }
                        break;

                    case Direction.Right:
                        {
                            ++j;
                        }
                        break;

                    case Direction.Up:
                        {
                            --i;
                        }
                        break;

                    case Direction.Down:
                        {
                            ++i;
                        }
                        break;

                    default:
                        break;
                }

                curDir = nextDir!.Value;
            }

            if (!isContinue)
            {
                break;
            }
        }

        List<Tuple<Pos, Direction>> nextStartInfo = [];

        if (nextDirs.Count != 0)
        {
            foreach (Direction nextDir in nextDirs)
            {
                switch (nextDir)
                {
                    case Direction.Left:
                        {
                            nextStartInfo.Add(new Tuple<Pos, Direction>(new Pos(i, j - 1), Direction.Left));
                        }
                        break;

                    case Direction.Right:
                        {
                            nextStartInfo.Add(new Tuple<Pos, Direction>(new Pos(i, j + 1), Direction.Right));
                        }
                        break;

                    case Direction.Up:
                        {
                            nextStartInfo.Add(new Tuple<Pos, Direction>(new Pos(i - 1, j), Direction.Up));
                        }
                        break;

                    case Direction.Down:
                        {
                            nextStartInfo.Add(new Tuple<Pos, Direction>(new Pos(i + 1, j), Direction.Down));
                        }
                        break;
                }
            }
        }

        Pos curEndPos = new(i, j);

        return (curEndPos, length, nextStartInfo);
    }

    private static List<Direction> GetNextDirections(
        int i, int j,
        Direction curDir,
        string[] map)
    {
        List<Direction> nextDirs = [];
        switch (curDir)
        {
            case Direction.Left:
                {
                    // continue
                    if (j - 1 >= 0 && map[i][j - 1] != '#')
                    {
                        nextDirs.Add(Direction.Left);
                    }

                    //Up
                    if (i - 1 >= 0 && map[i - 1][j] != '#')
                    {
                        nextDirs.Add(Direction.Up);
                    }

                    //Down
                    if (i + 1 < _ROWs && map[i + 1][j] != '#')
                    {
                        nextDirs.Add(Direction.Down);
                    }
                }
                break;

            case Direction.Right:
                {
                    if (j + 1 < _COLs && map[i][j + 1] != '#')
                    {
                        nextDirs.Add(Direction.Right);
                    }

                    //Up
                    if (i - 1 >= 0 && map[i - 1][j] != '#')
                    {
                        nextDirs.Add(Direction.Up);
                    }

                    //Down
                    if (i + 1 < _ROWs && map[i + 1][j] != '#')
                    {
                        nextDirs.Add(Direction.Down);
                    }
                }

                break;

            case Direction.Up:
                {
                    if (i - 1 >= 0 && map[i - 1][j] != '#')
                    {
                        nextDirs.Add(Direction.Up);
                    }

                    //Left
                    if (j - 1 >= 0 && map[i][j - 1] != '#')
                    {
                        nextDirs.Add(Direction.Left);
                    }

                    //Right
                    if (j + 1 < _COLs && map[i][j + 1] != '#')
                    {
                        nextDirs.Add(Direction.Right);
                    }
                }
                break;

            case Direction.Down:
                {
                    if (i + 1 < _ROWs && map[i + 1][j] != '#')
                    {
                        nextDirs.Add(Direction.Down);
                    }

                    //Left
                    if (j - 1 >= 0 && map[i][j - 1] != '#')
                    {
                        nextDirs.Add(Direction.Left);
                    }

                    //Right
                    if (j + 1 < _COLs && map[i][j + 1] != '#')
                    {
                        nextDirs.Add(Direction.Right);
                    }
                }
                break;
        }

        return nextDirs;
    }
}