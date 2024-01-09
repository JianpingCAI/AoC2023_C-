using System.Diagnostics;

record Node(int I, int J, int Dir, int DirCount);

internal class Program
{
    private static bool _isPart1Quesion = true;

    private static string[] lines = [];
    private static int[,] mat;

    private static int _ROWs = 0;
    private static int COLs = 0;

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

        _ROWs = lines.Length;
        COLs = lines[0].Length;

        mat = FormMatrix(lines);

        int result = Dijkstra(mat);

        sw.Stop();

        // 179 (low), 720 (too high),765, 770 (high), 773, 742(correct)
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static int[,] FormMatrix(string[] lines)
    {
        int[,] mat = new int[_ROWs, COLs];

        for (int i = 0; i < _ROWs; i++)
        {
            for (int j = 0; j < COLs; j++)
            {
                mat[i, j] = lines[i][j] - '0';
            }
        }

        return mat;
    }

    /// <summary>
    /// Find shortest path via Dijkstra
    /// - priority queue
    /// - visited check
    /// - move to the neighbors
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    private static int Dijkstra(int[,] mat)
    {
        //<(i,j, direction, curLossSum, dirCount), curLossSum>
        PriorityQueue<Tuple<Node, int>, int> pq = new();

        //// E - east
        //pq.Enqueue(new Tuple<int, int, int, int, int>(0, 0, 0, 0, 1), 0);
        //// S - south
        //pq.Enqueue(new Tuple<int, int, int, int, int>(0, 0, 3, 0, 1), 0);

        //int heatLoss = FindMinimumHeatLss(mat, pq);
        //return heatLoss;

        pq.Enqueue(new Tuple<Node, int>(new Node(0, 0, 0, 1), 0), 0);
        int heatLoss1 = FindMinimumHeatLoss(mat, pq);

        pq.Clear();
        _visited.Clear();

        pq.Enqueue(new Tuple<Node, int>(new Node(0, 0, 3, 1), 0), 0);
        int heatLoss2 = FindMinimumHeatLoss(mat, pq);

        return int.Min(heatLoss1, heatLoss2);
    }

    private static int FindMinimumHeatLoss(int[,] mat, PriorityQueue<Tuple<Node, int>, int> pq)
    {
        while (pq.Count > 0)
        {
            var cur = pq.Dequeue();
            int cur_i = cur.Item1.I;
            int cur_j = cur.Item1.J;
            int curLossSum = cur.Item2;

            Node node = new(cur_i, cur_j, cur.Item1.Dir, cur.Item1.DirCount);

            // (i, j, dir) is visited
            if (_visited.Contains(node))
            {
                continue;
            }
            _visited.Add(node);

            // find the shortest path
            if (cur_i == _ROWs - 1 && cur_j == COLs - 1)
            {
                Console.WriteLine($"{curLossSum}");

                return curLossSum;
            }

            List<Tuple<int, int, int>> validNexts = (_isPart1Quesion)
                ? GetValidNextNeighbors(cur_i, cur_j, cur.Item1.Dir, cur.Item1.DirCount)
                : GetValidNextNeighbors2(cur_i, cur_j, cur.Item1.Dir, cur.Item1.DirCount);

            foreach (Tuple<int, int, int> validNext in validNexts)
            {
                int next_i = validNext.Item1;
                int next_j = validNext.Item2;
                int next_dir = validNext.Item3;
                int next_DirCount = (cur.Item1.Dir == next_dir) ? cur.Item1.DirCount + 1 : 1;

                //if (next_DirCount > 3)
                //{
                //    Console.WriteLine("wrong1");
                //}
                int nextLossSum = curLossSum + mat[next_i, next_j];
                pq.Enqueue(new Tuple<Node, int>
                    (new Node(next_i, next_j, next_dir, next_DirCount), nextLossSum), nextLossSum);
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

        // cannot continue the same direction
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
        if (i >= 0 && i < _ROWs && j >= 0 && j < COLs)
        {
            return true;
        }

        return false;
    }
}