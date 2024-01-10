internal class Program
{
    record Pos2D(int i, int j);

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);

        string[] mat = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            mat[i] = line;
        }
        int ROWS = mat.Length;
        int COLS = mat[0].Length;

        int[,] boundMat = GetLoopBoundary(mat);

        FillByRayCasting(mat, boundMat);

        int count = 0;
        for (int i = 0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++)
            {
                if (boundMat[i, j] == 0) // Inside the loop
                {
                    ++count;
                }
            }
        }

        Console.WriteLine($"Result = {count}");
    }

    private static int[,] GetLoopBoundary(string[] mat)
    {
        int ROWS = mat.Length;
        int COLS = mat[0].Length;
        bool[,] visited = new bool[ROWS, COLS];

        // S
        Pos2D startS = new(0, 0);
        for (int i = 0; i < mat.Length; i++)
        {
            for (int j = 0; j < mat[0].Length; j++)
            {
                if (mat[i][j] == 'S')
                {
                    startS = new Pos2D(i, j);
                    visited[i, j] = true;
                    break;
                }
            }
        }

        // queue
        Queue<Tuple<Pos2D, List<Pos2D>>> que = new();
        //Up
        if (startS.i - 1 >= 0
            && mat[startS.i - 1][startS.j] != '.'
            && mat[startS.i - 1][startS.j] != '-'
            && mat[startS.i - 1][startS.j] != 'L'
            && mat[startS.i - 1][startS.j] != 'J')
        {
            que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new Pos2D(startS.i - 1, startS.j), [new(startS.i, startS.j)]));
        }

        //Down
        if (startS.i + 1 < ROWS
            && mat[startS.i + 1][startS.j] != '.'
            && mat[startS.i + 1][startS.j] != '-'
            && mat[startS.i + 1][startS.j] != 'F'
            && mat[startS.i + 1][startS.j] != '7')
        {
            que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(startS.i + 1, startS.j), [new(startS.i, startS.j)]));
        }

        //Left
        if (startS.j - 1 >= 0
            && mat[startS.i][startS.j - 1] != '.'
            && mat[startS.i][startS.j - 1] != '|'
            && mat[startS.i][startS.j - 1] != 'J'
            && mat[startS.i][startS.j - 1] != '7')
        {
            que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(startS.i, startS.j - 1), [new(startS.i, startS.j)]));
        }

        //Right
        if (startS.j + 1 < COLS
            && mat[startS.i][startS.j + 1] != '.'
            && mat[startS.i][startS.j + 1] != '|'
            && mat[startS.i][startS.j + 1] != 'L'
            && mat[startS.i][startS.j + 1] != 'F')
        {
            que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(startS.i, startS.j + 1), [new(startS.i, startS.j)]));
        }

        // paths

        List<List<Pos2D>> loopPaths = [];
        // traverse
        while (que.Count > 0)
        {
            int curLayElementCount = que.Count;

            Pos2D? loopNode = ContainsDuplicatedElements(que);

            // each layer
            for (int L = 0; L < curLayElementCount; L++)
            {
                Tuple<Pos2D, List<Pos2D>> q = que.Dequeue();
                int i = q.Item1.i;
                int j = q.Item1.j;

                List<Pos2D> curPath = q.Item2.ToList();
                curPath.Add(q.Item1);

                if (null != loopNode && i == loopNode.i && j == loopNode.j)
                {
                    loopPaths.Add(curPath);
                }

                if (i < 0 || visited[i, j])
                {
                    continue;
                }

                visited[i, j] = true;

                if (mat[i][j] == 'F')
                {
                    // j+1
                    if (j + 1 < COLS && !visited[i, j + 1] && mat[i][j + 1] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i, j + 1), curPath));
                    }
                    //i+1
                    if (i + 1 < ROWS && !visited[i + 1, j] && mat[i + 1][j] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i + 1, j), curPath));
                    }
                }
                else
                if (mat[i][j] == 'J')
                {
                    // j-1
                    if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i, j - 1), curPath));
                    }
                    //i-1
                    if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i - 1, j), curPath));
                    }
                }
                else
                    if (mat[i][j] == '7')
                {
                    // j-1
                    if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i, j - 1), curPath));
                    }
                    //i+1
                    if (i + 1 < ROWS && !visited[i + 1, j] && mat[i + 1][j] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i + 1, j), curPath));
                    }
                }
                else
                    if (mat[i][j] == 'L')
                {
                    // j+1
                    if (j + 1 < COLS && !visited[i, j + 1] && mat[i][j + 1] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i, j + 1), curPath));
                    }
                    //i-1
                    if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i - 1, j), curPath));
                    }
                }
                else
                    if (mat[i][j] == '|')
                {
                    //i-1
                    if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i - 1, j), curPath));
                    }
                    //i+1
                    if (i + 1 < ROWS && !visited[i + 1, j] && mat[i + 1][j] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i + 1, j), curPath));
                    }
                }
                else
                    if (mat[i][j] == '-')
                {
                    // j-1
                    if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i, j - 1), curPath));
                    }
                    // j+1
                    if (j + 1 < COLS && !visited[i, j + 1] && mat[i][j + 1] != '.')
                    {
                        que.Enqueue(new Tuple<Pos2D, List<Pos2D>>(new(i, j + 1), curPath));
                    }
                }
            }
        }

        // two longest paths
        List<Pos2D> loop1 = loopPaths.Last();
        List<Pos2D> loop2 = loopPaths[^2];

        //find boundary elements
        int[,] bound = new int[ROWS, COLS];
        foreach (Pos2D b in loop1)
        {
            bound[b.i, b.j] = 1;
        }
        foreach (Pos2D b in loop2)
        {
            bound[b.i, b.j] = 1;
        }

        return bound;
    }

    private static Pos2D? ContainsDuplicatedElements(Queue<Tuple<Pos2D, List<Pos2D>>> que)
    {
        HashSet<Pos2D> tuples = [];
        foreach (Tuple<Pos2D, List<Pos2D>> item in que)
        {
            if (tuples.Contains(item.Item1))
            {
                return item.Item1;
            }

            tuples.Add(item.Item1);
        }

        return null;
    }

    /**
         mat:   2d matrix, original input
         bound: 2d matrix: 0 - non-boundary; 1 - boundary

         result: check all non-boundary elements, and set them to 2 if outside the boundary loop.
         */

    private static void FillByRayCasting(string[] mat, int[,] boundMat)
    {
        int numRows = boundMat.GetLength(0);
        for (int i = 0; i < numRows; i++)
        {
            RayCasting(mat, boundMat, i);
        }
    }

    /*
     * 1: bound
     * 2: outside
     * 3: inside
     */

    private static void RayCasting(string[] mat, int[,] boundMat, int i)
    {
        bool isInside = false;
        char prevB = ' ';
        char curB;

        int numColumns = boundMat.GetLength(1);
        for (int j = 0; j < numColumns; j++)
        {
            // is bounday
            if (boundMat[i, j] == 1)
            {
                curB = mat[i][j];
                switch (curB)
                {
                    case '|':
                    case 'F':
                    case 'L':
                    case 'S':
                        isInside = !isInside;
                        break;

                    case 'J':
                        if (prevB != 'F')
                        {
                            isInside = !isInside;
                        }
                        break;

                    case '7':
                        if (prevB != 'L')
                        {
                            isInside = !isInside;
                        }
                        break;

                    default:
                        break;
                }

                // update previous boundary type
                if (curB != '-')
                {
                    prevB = curB;
                }
            }
            else
            {
                // set outside as 2
                if (!isInside)
                {
                    boundMat[i, j] = 2;
                }
            }
        }
    }
}