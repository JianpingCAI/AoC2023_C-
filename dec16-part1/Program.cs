using System.Diagnostics;

record Tile(int i, int j, int dir);
record Pos(int i, int j);

internal class Program
{
    private static int ROWs = 0;
    private static int COLs = 0;
    private static List<string> rows = [];

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        // input
        rows = new List<string>(lines.Length);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            rows.Add(line);
        }
        ROWs = rows.Count;
        COLs = rows[0].Length;

        //
        int result = StartBeam(rows);

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    /// <summary>
    /// breadth first
    /// - queue
    ///
    /// directions: E:0,W:1, N:2, S:3
    ///
    /// </summary>
    /// <param name="rows"></param>
    /// <returns></returns>
    private static int StartBeam(List<string> rows)
    {
        //E >:0,W <:1, N ^:2, S :3
        bool[,,] visited = new bool[rows.Count, rows[0].Length, 4];

        //(i,j, beam-direction)
        Queue<Tile> que = new();

        que.Enqueue(new Tile(0, 0, 0));

        while (que.Count() > 0)
        {
            Tile tile = que.Dequeue();

            int cur_dir = tile.dir;

            if (visited[tile.i, tile.j, cur_dir])
            {
                continue;
            }
            visited[tile.i, tile.j, cur_dir] = true;

            // next direction
            char cur_char = rows[tile.i][tile.j];
            switch (cur_char)
            {
                case '.':
                    {
                        int next_dir = cur_dir;
                        Pos? next_loc = GetNextTileLocation(tile, next_dir);
                        if (null != next_loc)
                        {
                            que.Enqueue(new Tile(next_loc.i, next_loc.j, next_dir));
                        }
                    }
                    break;

                case '-':
                    {
                        switch (cur_dir)
                        {
                            case 0:
                            case 1:
                                {
                                    int next_dir = cur_dir;
                                    Pos? next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tile(next_loc.i, next_loc.j, next_dir));
                                    }
                                }
                                break;

                            case 2:
                            case 3:
                                {
                                    int next_dir = 0;
                                    Pos? next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tile(next_loc.i, next_loc.j, next_dir));
                                    }

                                    next_dir = 1;
                                    next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tile(next_loc.i, next_loc.j, next_dir));
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    break;

                case '|':
                    {
                        switch (cur_dir)
                        {
                            case 0:
                            case 1:
                                {
                                    int next_dir = 2;
                                    Pos? next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tile(next_loc.i, next_loc.j, next_dir));
                                    }

                                    next_dir = 3;
                                    next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tile(next_loc.i, next_loc.j, next_dir));
                                    }
                                }
                                break;

                            case 2:
                            case 3:
                                {
                                    int next_dir = cur_dir;
                                    Pos? next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tile(next_loc.i, next_loc.j, next_dir));
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                    }

                    break;

                case '/':
                    {
                        int next_dir = 0;
                        switch (cur_dir)
                        {
                            case 0:
                                {
                                    next_dir = 2;
                                }
                                break;

                            case 1:
                                {
                                    next_dir = 3;
                                }
                                break;

                            case 2:
                                {
                                    next_dir = 0;
                                }
                                break;

                            case 3:
                                {
                                    next_dir = 1;
                                }
                                break;

                            default:
                                break;
                        }

                        Pos? next_loc = GetNextTileLocation(tile, next_dir);
                        if (null != next_loc)
                        {
                            que.Enqueue(new Tile(next_loc.i, next_loc.j, next_dir));
                        }
                    }

                    break;

                case '\\':
                    {
                        int next_dir = 0;
                        switch (cur_dir)
                        {
                            case 0:
                                {
                                    next_dir = 3;
                                }
                                break;

                            case 1:
                                {
                                    next_dir = 2;
                                }
                                break;

                            case 2:
                                {
                                    next_dir = 1;
                                }
                                break;

                            case 3:
                                {
                                    next_dir = 0;
                                }
                                break;

                            default:
                                break;
                        }

                        Pos? next_loc = GetNextTileLocation(tile, next_dir);
                        if (null != next_loc)
                        {
                            que.Enqueue(new Tile(next_loc.i, next_loc.j, next_dir));
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        // count visited
        int count = 0;
        for (int i = 0; i < visited.GetLength(0); i++)
        {
            for (int j = 0; j < visited.GetLength(1); j++)
            {
                for (int k = 0; k < visited.GetLength(2); k++)
                {
                    if (visited[i, j, k])
                    {
                        ++count;
                        break;
                    }
                }
            }
        }

        return count;
    }

    private static Pos? GetNextTileLocation(Tile tile, int next_dir)
    {
        int i = 0, j = 0;
        switch (next_dir)
        {
            case 0:
                i = tile.i;
                j = tile.j + 1;
                break;

            case 1:
                i = tile.i;
                j = tile.j - 1;
                break;

            case 2:
                i = tile.i - 1;
                j = tile.j;
                break;

            case 3:
                i = tile.i + 1;
                j = tile.j;
                break;

            default:
                break;
        }

        if (i < 0 || i >= ROWs || j < 0 || j >= COLs)
        {
            return null;
        }

        return new Pos(i, j);
    }
}