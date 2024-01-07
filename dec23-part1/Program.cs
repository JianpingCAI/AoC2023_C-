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
    public Pos EndPos { get; set; }
    public Direction StartDir { get; set; }

    public int Length { get; set; }

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

        // paths and connections
        Path rootPath = BuildPathNetwork_BFS(mat);

        // traverse paths
        int result = FindLongestPath(rootPath);

        sw.Stop();

        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    record Path_CurrentTotalLength(Path path, int curTotalLength);

    // check all the paths to the end, and find the max length
    private static int FindLongestPath(Path rootPath)
    {
        PriorityQueue<Path_CurrentTotalLength, int> pq = new();
        pq.Enqueue(new Path_CurrentTotalLength(rootPath, rootPath.Length), rootPath.Length);

        int maxLength = int.MinValue;
        while (pq.Count > 0)
        {
            Path_CurrentTotalLength qItem = pq.Dequeue();
            Path curPath = qItem.path;
            int curTotalLength = qItem.curTotalLength;

            // not to the end yet
            if (curPath.NextPaths.Count >= 1)
            {
                foreach (Path nextPath in curPath.NextPaths)
                {
                    pq.Enqueue(new Path_CurrentTotalLength(nextPath, curTotalLength + nextPath.Length), curTotalLength + nextPath.Length);
                }
            }
            // reach the end, check the total length
            else
            {
                if (curTotalLength > maxLength)
                {
                    maxLength = curTotalLength;
                }
                Console.WriteLine($"{curTotalLength - 1}");
            }
        }

        return maxLength - 1;
    }

    // breadth first traverse to build the network of paths
    private static Path BuildPathNetwork_BFS(string[] map)
    {
        Path startPath = new()
        {
            StartPos = new Pos(_start_i, _start_j),
            StartDir = Direction.Down
        };

        Queue<Path> que = [];
        que.Enqueue(startPath);

        while (que.Count > 0)
        {
            Path curPath = que.Dequeue();

            // walk until encounter junction
            (Pos curEndPos, int length, List<Tuple<Pos, Direction>> nextPathStartInfos) = WalkTillJunction(curPath.StartPos, curPath.StartDir, map);

            curPath.EndPos = curEndPos;
            curPath.Length = length;

            foreach (Tuple<Pos, Direction> nextStart in nextPathStartInfos)
            {
                Path nextPath = new() { StartPos = nextStart.Item1, StartDir = nextStart.Item2 };
                curPath.NextPaths.Add(nextPath);

                que.Enqueue(nextPath);
            }
        }

        return startPath;
    }

    private static (Pos curEndPos, int length, List<Tuple<Pos, Direction>> nextStartPos) WalkTillJunction(Pos startPos, Direction startDir, string[] map)
    {
        int length = 0;

        Direction curDir = startDir;
        int i = startPos.i;
        int j = startPos.j;
        List<Direction> nextDirs = [];

        // find segment end point (with startPos)
        // stop if 1) exit the map, or 2) find a junction
        bool isContinue = true;
        while (i < map.Length && isContinue)
        {
            while (j < map[i].Length)
            {
                ++length;

                nextDirs.Clear();
                if (i == _ROWs - 1 && j == _COLs - 2)
                {
                    isContinue = false;
                    break;
                }

                // next step directions
                nextDirs = GetNextDirection(i, j, curDir, map);
                if (nextDirs.Count == 0)
                {
                    isContinue = false;

                    break;
                }

                Direction? nextDir = null;

                // continue with a single path
                if (nextDirs.Count == 1)
                {
                    nextDir = nextDirs[0];
                }
                // there is a junction
                else if (nextDirs.Count > 1)
                {
                    isContinue = false;

                    break;
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
        }

        List<Tuple<Pos, Direction>> nextTurnPoints = [];

        // exit the map
        if (nextDirs.Count == 0)
        {
            //nextTurnPoints.Add(new Tuple<Pos, Direction>(new Pos(i, j), Direction.Down));
        }
        // there is a junction
        else
        {
            if (nextDirs.Count < 2)
            {
                throw new Exception();
            }

            foreach (Direction nextDir in nextDirs)
            {
                switch (nextDir)
                {
                    case Direction.Left:
                        {
                            nextTurnPoints.Add(new Tuple<Pos, Direction>(new Pos(i, j - 1), Direction.Left));
                        }
                        break;

                    case Direction.Right:
                        {
                            nextTurnPoints.Add(new Tuple<Pos, Direction>(new Pos(i, j + 1), Direction.Right));
                        }
                        break;

                    case Direction.Up:
                        {
                            nextTurnPoints.Add(new Tuple<Pos, Direction>(new Pos(i - 1, j), Direction.Up));
                        }
                        break;

                    case Direction.Down:
                        {
                            nextTurnPoints.Add(new Tuple<Pos, Direction>(new Pos(i + 1, j), Direction.Down));
                        }
                        break;
                }
            }
        }

        Pos curEndPos = new(i, j);

        return (curEndPos, length, nextTurnPoints);
    }

    private static List<Direction> GetNextDirection(int i, int j, Direction curDir, string[] map)
    {
        List<Direction> nextDirs = [];
        switch (curDir)
        {
            case Direction.Left:
                {
                    // continue
                    if (j - 1 >= 0 && map[i][j - 1] != '#' && map[i][j - 1] != '>')
                    {
                        nextDirs.Add(Direction.Left);
                    }

                    //Up
                    if (i - 1 >= 0 && map[i - 1][j] != '#' && map[i - 1][j] != 'v')
                    {
                        nextDirs.Add(Direction.Up);
                    }

                    //Down

                    if (i + 1 < _ROWs && map[i + 1][j] != '#' /*&& map[i - 1][j] != '^'*/)
                    {
                        nextDirs.Add(Direction.Down);
                    }
                }
                break;

            case Direction.Right:
                {
                    if (j + 1 < _COLs && map[i][j + 1] != '#' && map[i][j - 1] != '<')
                    {
                        nextDirs.Add(Direction.Right);
                    }

                    //Up
                    if (i - 1 >= 0 && map[i - 1][j] != '#' && map[i - 1][j] != 'v')
                    {
                        nextDirs.Add(Direction.Up);
                    }

                    //Down
                    if (i + 1 < _ROWs && map[i + 1][j] != '#' /*&& map[i - 1][j] != '^'*/)
                    {
                        nextDirs.Add(Direction.Down);
                    }
                }

                break;

            case Direction.Up:
                {
                    if (i - 1 >= 0 && map[i - 1][j] != '#' && map[i][j - 1] != 'v')
                    {
                        nextDirs.Add(Direction.Up);
                    }

                    //Left
                    if (j - 1 >= 0 && map[i][j - 1] != '#' && map[i][j - 1] != '>')
                    {
                        nextDirs.Add(Direction.Left);
                    }

                    //Right
                    if (j + 1 < _COLs && map[i][j + 1] != '#' && map[i - 1][j] != '<')
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
                    if (j - 1 >= 0 && map[i][j - 1] != '#' && map[i][j - 1] != '>')
                    {
                        nextDirs.Add(Direction.Left);
                    }

                    //Right
                    if (j + 1 < _COLs && map[i][j + 1] != '#' && map[i - 1][j] != '<')
                    {
                        nextDirs.Add(Direction.Right);
                    }
                }
                break;
        }

        return nextDirs;
    }
}