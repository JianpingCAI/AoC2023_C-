using System.Diagnostics;

internal class Program
{
    record Node(int I, int J, int Dir, int DirCount);

    private static string[] lines = [];

    private static int ROWs = 0;
    private static int COLs = 0;

    //private static bool[,,] _visited;
    //!!!! the most important part
    private static readonly HashSet<Node> _visited = [];

    private static bool isPrint = false;

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        if (filePath == "input2.txt")
        {
            isPrint = true;
        }
        lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        ROWs = lines.Length;
        COLs = lines[0].Length;

        int[,] mat = FormMatrix(lines);

        long result = Dijkstra(mat);

        sw.Stop();

        // 918
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static int[,] FormMatrix(string[] lines)
    {
        int[,] mat = new int[ROWs, COLs];

        for (int i = 0; i < ROWs; i++)
        {
            for (int j = 0; j < COLs; j++)
            {
                mat[i, j] = lines[i][j] - '0';
            }
        }

        return mat;
    }

    private static long Dijkstra(int[,] mat)
    {
        //<(i,j, direction, curLossSum, curDirsCount), curLossSum>
        PriorityQueue<Tuple<int, int, int, long, int[]>, long> pq = new();

        //E,W,N,S - 0,1,2,3
        // E
        pq.Enqueue(new Tuple<int, int, int, long, int[]>(0, 0, 0, 0, [1, 0, 0, 0]), 0);
        pq.Enqueue(new Tuple<int, int, int, long, int[]>(0, 0, 3, 0, [0, 0, 0, 1]), 0);

        long heatLoss = FindMinimumHeatLoss(mat, pq);
        return heatLoss;

        //_visited.Clear();
        //pq.Enqueue(new Tuple<int, int, int, long, , int[]>(0, 0, 0, 0, 1, [1, 0, 0, 0]), 0);
        //long heatLoss1 = FindMinimumHeatLss(mat, pq);

        //pq.Clear();
        //_visited.Clear();

        //pq.Enqueue(new Tuple<int, int, int, long, , int[]>(0, 0, 3, 0, 1, [0, 0, 0, 1]), 0);
        //long heatLoss2 = FindMinimumHeatLss(mat, pq);

        //return long.Min(heatLoss1, heatLoss2);
    }

    private static long FindMinimumHeatLoss(int[,] mat, PriorityQueue<Tuple<int, int, int, long, int[]>, long> pq)
    {
        while (pq.Count > 0)
        {
            Tuple<int, int, int, long, int[]> cur = pq.Dequeue();
            int cur_i = cur.Item1;
            int cur_j = cur.Item2;
            int curDir = cur.Item3;
            long curLossSum = cur.Item4;
            int[] curDirsCounts = cur.Item5;

            int curDirCount = curDirsCounts[curDir];
            Node node = new(cur_i, cur_j, curDir, curDirsCounts[curDir]);

            // (i, j, dir) is visited
            if (_visited.Contains(node))
            {
                continue;
            }
            _visited.Add(node);

            if (cur_i == ROWs - 1 && cur_j == COLs - 1)
            {
                Console.WriteLine($"{curLossSum}");

                return curLossSum;
            }

            List<Tuple<int, int, int>> validNexts = GetValidNextNeighbors2(cur_i, cur_j, curDir, curDirsCounts[curDir]);

            foreach (Tuple<int, int, int> validNext in validNexts)
            {
                int next_i = validNext.Item1;
                int next_j = validNext.Item2;
                int next_dir = validNext.Item3;

                long nextLossSum = curLossSum + mat[next_i, next_j];

                int[] next_DirsCount = new int[4];
                if (curDir == next_dir)
                {
                    next_DirsCount[next_dir] = curDirsCounts[curDir] + 1;
                }
                else
                {
                    next_DirsCount[next_dir] = 1;
                }

                if (next_DirsCount[next_dir] > 10)
                {
                    Console.WriteLine("wrong1");
                }

                pq.Enqueue(new Tuple<int, int, int, long, int[]>
                    (next_i, next_j, next_dir, nextLossSum, next_DirsCount), nextLossSum);
            }
        }

        return int.MaxValue;
    }

    private static List<Tuple<int, int, int>> GetValidNextNeighbors(int i, int j, int curDir, int curDirCount)
    {
        if (!IsValid(i, j))
        {
            return [];
        }

        List<Tuple<int, int, int>> validNeighbors = [];
        int next_i = -1;
        int next_j = -1;

        // there is turn
        switch (curDir)
        {
            //E
            case 0:
            //W
            case 1:
                {
                    //N
                    next_i = i - 1;
                    next_j = j;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 2));
                    }

                    //S
                    next_i = i + 1;
                    next_j = j;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 3));
                    }
                }
                break;

            //N
            case 2:
            //S
            case 3:
                {
                    //E
                    next_i = i;
                    next_j = j + 1;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 0));
                    }

                    //W
                    next_i = i;
                    next_j = j - 1;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 1));
                    }
                }
                break;

            default:
                break;
        }

        if (curDirCount >= 3)
        {
            return validNeighbors;
        }

        // there is a no turn, same dir
        switch (curDir)
        {
            //E
            case 0:
                {
                    next_i = i;
                    next_j = j + 1;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, curDir));
                    }
                }
                break;
            //W
            case 1:
                {
                    next_i = i;
                    next_j = j - 1;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, curDir));
                    }
                }

                break;
            //N
            case 2:
                {
                    next_i = i - 1;
                    next_j = j;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, curDir));
                    }
                }
                break;
            //S
            case 3:
                {
                    next_i = i + 1;
                    next_j = j;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, curDir));
                    }
                }
                break;

            default:
                break;
        }

        return validNeighbors;
    }

    private static List<Tuple<int, int, int>> GetValidNextNeighbors2(int i, int j, int curDir, int curDirCount)
    {
        if (!IsValid(i, j))
        {
            return [];
        }

        List<Tuple<int, int, int>> validNeighbors = [];
        int next_i = -1;
        int next_j = -1;

        // there is turn
        if (curDirCount >= 4)
        {
            switch (curDir)
            {
                //E
                case 0:
                //W
                case 1:
                    {
                        //N
                        next_i = i - 1;
                        next_j = j;
                        if (IsValid(next_i, next_j))
                        {
                            validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 2));
                        }

                        //S
                        next_i = i + 1;
                        next_j = j;
                        if (IsValid(next_i, next_j))
                        {
                            validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 3));
                        }
                    }
                    break;

                //N
                case 2:
                //S
                case 3:
                    {
                        //E
                        next_i = i;
                        next_j = j + 1;
                        if (IsValid(next_i, next_j))
                        {
                            validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 0));
                        }

                        //W
                        next_i = i;
                        next_j = j - 1;
                        if (IsValid(next_i, next_j))
                        {
                            validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 1));
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        if (curDirCount >= 10)
        {
            return validNeighbors;
        }

        // there is a no turn, same dir
        switch (curDir)
        {
            //E
            case 0:
                {
                    next_i = i;
                    next_j = j + 1;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, curDir));
                    }
                }
                break;
            //W
            case 1:
                {
                    next_i = i;
                    next_j = j - 1;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, curDir));
                    }
                }

                break;
            //N
            case 2:
                {
                    next_i = i - 1;
                    next_j = j;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, curDir));
                    }
                }
                break;
            //S
            case 3:
                {
                    next_i = i + 1;
                    next_j = j;
                    if (IsValid(next_i, next_j))
                    {
                        validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, curDir));
                    }
                }
                break;

            default:
                break;
        }

        return validNeighbors;
    }

    private static bool IsValid(int i, int j)
    {
        if (i >= 0 && i < ROWs && j >= 0 && j < COLs)
        {
            return true;
        }

        return false;
    }
}