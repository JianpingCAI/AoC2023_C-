internal class Program
{
    record Pos2D(int i, int j);

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);

        int result = 0;

        string[] mat = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            mat[i] = line;
        }

        // S
        Pos2D startS = new(0, 0);
        for (int i = 0; i < mat.Length; i++)
        {
            for (int j = 0; j < mat[0].Length; j++)
            {
                if (mat[i][j] == 'S')
                {
                    startS = new Pos2D(i, j);
                    break;
                }
            }
        }

        result = Traverse_BFS(mat, startS);

        Console.WriteLine($"Result = {result}");
    }

    private static int Traverse_BFS(string[] mat, Pos2D startS)
    {
        int ROWS = mat.Length;
        int COLS = mat[0].Length;

        bool[,] visited = new bool[ROWS, COLS];
        visited[startS.i, startS.j] = true;

        // queue
        Queue<Pos2D> que = new();
        //Top
        if (startS.i - 1 >= 0 && mat[startS.i - 1][startS.j] != '.' && mat[startS.i - 1][startS.j] != '-')
        {
            que.Enqueue(new Pos2D(startS.i - 1, startS.j));
        }
        //Bottom
        if (startS.i + 1 < ROWS && mat[startS.i + 1][startS.j] != '.' && mat[startS.i + 1][startS.j] != '-')
        {
            que.Enqueue(new Pos2D(startS.i + 1, startS.j));
        }
        //Left
        if (startS.j - 1 >= 0 && mat[startS.i][startS.j - 1] != '.' && mat[startS.i][startS.j - 1] != '|')
        {
            que.Enqueue(new Pos2D(startS.i, startS.j - 1));
        }
        //Right
        if (startS.j + 1 < COLS && mat[startS.i][startS.j + 1] != '.' && mat[startS.i][startS.j + 1] != '|')
        {
            que.Enqueue(new Pos2D(startS.i, startS.j + 1));
        }

        int layDepth = 0;

        // traverse
        while (que.Count > 0)
        {
            ++layDepth;
            int layerCount = que.Count;

            // detect a loop
            if (ContainsDuplicatedElements(que))
            {
                Console.WriteLine($"loop occurs: {layDepth}");
            }

            for (int L = 0; L < layerCount; L++)
            {
                Pos2D p = que.Dequeue();
                int i = p.i;
                int j = p.j;

                if (visited[i, j])
                {
                    continue;
                }
                visited[i, j] = true;

                if (mat[i][j] == 'F')
                {
                    // j+1
                    if (j + 1 < COLS && !visited[i, j + 1] && mat[i][j + 1] != '.')
                    {
                        que.Enqueue(new Pos2D(i, j + 1));
                    }
                    //i+1
                    if (i + 1 < ROWS && !visited[i + 1, j] && mat[i + 1][j] != '.')
                    {
                        que.Enqueue(new Pos2D(i + 1, j));
                    }
                }
                else if (mat[i][j] == 'J')
                {
                    // j-1
                    if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
                    {
                        que.Enqueue(new Pos2D(i, j - 1));
                    }
                    //i-1
                    if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
                    {
                        que.Enqueue(new Pos2D(i - 1, j));
                    }
                }
                else if (mat[i][j] == '7')
                {
                    // j-1
                    if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
                    {
                        que.Enqueue(new Pos2D(i, j - 1));
                    }
                    //i+1
                    if (i + 1 < ROWS && !visited[i + 1, j] && mat[i + 1][j] != '.')
                    {
                        que.Enqueue(new Pos2D(i + 1, j));
                    }
                }
                else if (mat[i][j] == 'L')
                {
                    // j+1
                    if (j + 1 < COLS && !visited[i, j + 1] && mat[i][j + 1] != '.')
                    {
                        que.Enqueue(new Pos2D(i, j + 1));
                    }
                    //i-1
                    if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
                    {
                        que.Enqueue(new Pos2D(i - 1, j));
                    }
                }
                else if (mat[i][j] == '|')
                {
                    //i-1
                    if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
                    {
                        que.Enqueue(new Pos2D(i - 1, j));
                    }
                    //i+1
                    if (i + 1 < ROWS && !visited[i + 1, j] && mat[i + 1][j] != '.')
                    {
                        que.Enqueue(new Pos2D(i + 1, j));
                    }
                }
                else if (mat[i][j] == '-')
                {
                    // j-1
                    if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
                    {
                        que.Enqueue(new Pos2D(i, j - 1));
                    }
                    // j+1
                    if (j + 1 < COLS && !visited[i, j + 1] && mat[i][j + 1] != '.')
                    {
                        que.Enqueue(new Pos2D(i, j + 1));
                    }
                }
            }
        }

        return layDepth;
    }

    // detect a loop
    private static bool ContainsDuplicatedElements(Queue<Pos2D> que)
    {
        HashSet<Pos2D> tuples = [];
        foreach (Pos2D item in que)
        {
            if (tuples.Contains(item))
            {
                return true;
            }

            tuples.Add(item);
        }

        return false;
    }
}