using System.Diagnostics;

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

    private static int StartBeam(List<string> rows)
    {
        //E >:0,W <:1, N ^:2, S :3
        bool[,,] visited = new bool[rows.Count, rows[0].Length, 4];

        //(i,j, beam-direction)
        Queue<Tuple<int, int, int>> que = new();

        que.Enqueue(new Tuple<int, int, int>(0, 0, 0));

        while (que.Count() > 0)
        {
            Tuple<int, int, int> tile = que.Dequeue();

            int cur_dir = tile.Item3;

            if (visited[tile.Item1, tile.Item2, cur_dir])
            {
                continue;
            }
            visited[tile.Item1, tile.Item2, cur_dir] = true;

            // next direction
            char cur_char = rows[tile.Item1][tile.Item2];
            switch (cur_char)
            {
                case '.':
                    {
                        int next_dir = cur_dir;
                        Tuple<int, int>? next_loc = GetNextTileLocation(tile, next_dir);
                        if (null != next_loc)
                        {
                            que.Enqueue(new Tuple<int, int, int>(next_loc.Item1, next_loc.Item2, next_dir));
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
                                    Tuple<int, int>? next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tuple<int, int, int>(next_loc.Item1, next_loc.Item2, next_dir));
                                    }
                                }
                                break;

                            case 2:
                            case 3:
                                {
                                    int next_dir = 0;
                                    Tuple<int, int>? next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tuple<int, int, int>(next_loc.Item1, next_loc.Item2, next_dir));
                                    }

                                    next_dir = 1;
                                    next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tuple<int, int, int>(next_loc.Item1, next_loc.Item2, next_dir));
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
                                    Tuple<int, int>? next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tuple<int, int, int>(next_loc.Item1, next_loc.Item2, next_dir));
                                    }

                                    next_dir = 3;
                                    next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tuple<int, int, int>(next_loc.Item1, next_loc.Item2, next_dir));
                                    }
                                }
                                break;

                            case 2:
                            case 3:
                                {
                                    int next_dir = cur_dir;
                                    Tuple<int, int>? next_loc = GetNextTileLocation(tile, next_dir);
                                    if (null != next_loc)
                                    {
                                        que.Enqueue(new Tuple<int, int, int>(next_loc.Item1, next_loc.Item2, next_dir));
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

                        Tuple<int, int>? next_loc = GetNextTileLocation(tile, next_dir);
                        if (null != next_loc)
                        {
                            que.Enqueue(new Tuple<int, int, int>(next_loc.Item1, next_loc.Item2, next_dir));
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

                        Tuple<int, int>? next_loc = GetNextTileLocation(tile, next_dir);
                        if (null != next_loc)
                        {
                            que.Enqueue(new Tuple<int, int, int>(next_loc.Item1, next_loc.Item2, next_dir));
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

    private static Tuple<int, int>? GetNextTileLocation(Tuple<int, int, int> tile, int next_dir)
    {
        int i = 0, j = 0;
        switch (next_dir)
        {
            case 0:
                i = tile.Item1;
                j = tile.Item2 + 1;
                break;

            case 1:
                i = tile.Item1;
                j = tile.Item2 - 1;
                break;

            case 2:
                i = tile.Item1 - 1;
                j = tile.Item2;
                break;

            case 3:
                i = tile.Item1 + 1;
                j = tile.Item2;
                break;

            default:
                break;
        }

        if (i < 0 || i >= ROWs || j < 0 || j >= COLs)
        {
            return null;
        }

        return new Tuple<int, int>(i, j);
    }
}