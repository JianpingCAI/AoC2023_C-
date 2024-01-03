using System.Diagnostics;

internal class Program
{
    record Node(int I, int J, int Dir, int DirCount);

    private static string[] lines = [];
    private static int[,] mat;

    private static int ROWs = 0;
    private static int COLs = 0;

    //private static bool[,,] _visited;
    //!!!! the most important part
    private static HashSet<Node> _visited = [];

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

        mat = FormMatrix(lines);

        long result = Dijkstra(mat);

        sw.Stop();

        // 179 (low), 720 (too high),765, 770 (high), 773
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
        //<(i,j, direction, curLossSum, dirCount, curDirsCount, history), curLossSum>
        PriorityQueue<Tuple<int, int, int, long, int, int[], List<Tuple<int, int, int, int>>>, long> pq = new();

        //E,W,N,S - 0,1,2,3
        // E
        pq.Enqueue(new Tuple<int, int, int, long, int, int[], List<Tuple<int, int, int, int>>>(0, 0, 0, 0, 1, [1, 0, 0, 0], []), 0);
        // S
        pq.Enqueue(new Tuple<int, int, int, long, int, int[], List<Tuple<int, int, int, int>>>(0, 0, 3, 0, 1, [0, 0, 0, 1], []), 0);

        while (pq.Count > 0)
        {
            Tuple<int, int, int, long, int, int[], List<Tuple<int, int, int, int>>> cur = pq.Dequeue();
            int cur_i = cur.Item1;
            int cur_j = cur.Item2;
            int curDir = cur.Item3;
            long curLossSum = cur.Item4;
            int curDirCount = cur.Item5;
            int[] curDirsCounts = cur.Item6;

            //(i,j,dir,count)
            List<Tuple<int, int, int, int>> curHistory = cur.Item7.ToList();
            curHistory.Add(new Tuple<int, int, int, int>(cur_i, cur_j, curDir, curDirsCounts[curDir]));

            var node = new Node(cur_i, cur_j, curDir, curDirCount);
            // (i, j, dir) is visited
            if (_visited.Contains(node))
            {
                continue;
            }
            _visited.Add(node);

            // update
            // (i, j, dir) is visited
            //_visited[cur_i, cur_j, curDir] = true;

            if (cur_i == ROWs - 1 && cur_j == COLs - 1)
            {
                Console.WriteLine($"{curLossSum}");

                if (isPrint)
                {
                    Print(mat, curHistory);
                }

                return curLossSum;
            }

            List<Tuple<int, int, int>> validNexts = GetValidNextNeighbors(cur_i, cur_j, curDir, curDirsCounts[curDir]);

            foreach (var validNext in validNexts)
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

                if (next_DirsCount[next_dir] > 3)
                {
                    Console.WriteLine("wrong1");
                }

                pq.Enqueue(new Tuple<int, int, int, long, int, int[], List<Tuple<int, int, int, int>>>
                    (next_i, next_j, next_dir, nextLossSum, curDirCount + 1, next_DirsCount, curHistory), nextLossSum);
            }
        }

        return 0;
    }

    private static void Print(int[,] mat, List<Tuple<int, int, int, int>> curHistory)
    {
        char[,] charMat = new char[ROWs, COLs];
        for (int i = 0; i < ROWs; i++)
        {
            for (int j = 0; j < COLs; j++)
            {
                charMat[i, j] = mat[i, j].ToString()[0];
            }
        }

        int sum = 0;
        foreach (var item in curHistory)
        {
            sum += mat[item.Item1, item.Item2];
            mat[item.Item1, item.Item2] = 0;

            charMat[item.Item1, item.Item2] = getChar(item.Item3);
        }

        for (int i = 0; i < ROWs; i++)
        {
            for (int j = 0; j < COLs; j++)
            {
                Console.Write(charMat[i, j]);
            }
            Console.WriteLine();
        }

        Console.WriteLine(sum - 2);
    }

    private static char getChar(int dir)
    {
        switch (dir)
        {
            case 0:
                return '>';

            case 1:
                return '<';

            case 2:
                return '^';

            case 3:
                return '|';

            default:
                break;
        }

        return '#';
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

        //// there is a turn
        //switch (curDir)
        //{
        //    //E
        //    case 0:
        //    //W
        //    case 1:
        //        {
        //            //N
        //            next_i = i - 1;
        //            next_j = j;
        //            if (IsValid(next_i, next_j))
        //            {
        //                validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 2));
        //            }

        //            //S
        //            next_i = i + 1;
        //            next_j = j;
        //            if (IsValid(next_i, next_j))
        //            {
        //                validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 3));
        //            }
        //        }

        //        break;
        //    //N
        //    case 2:
        //    //S
        //    case 3:
        //        {
        //            //E
        //            next_i = i;
        //            next_j = j + 1;
        //            if (IsValid(next_i, next_j))
        //            {
        //                validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 0));
        //            }

        //            //W
        //            next_i = i;
        //            next_j = j - 1;
        //            if (IsValid(next_i, next_j))
        //            {
        //                validNeighbors.Add(new Tuple<int, int, int>(next_i, next_j, 1));
        //            }
        //        }
        //        break;

        //    default:
        //        break;
        //}

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