string filePath = "input.txt"; //4,8,10,1 ???input2
string[] lines = File.ReadAllLines(filePath);

int result = 0;

string[] mat = new string[lines.Length];

for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    mat[i] = line;
}

int R = mat.Length;
int C = mat[0].Length;

bool[,] visited = new bool[R, C];

// S
Tuple<int, int> S = new(0, 0);
for (int i = 0; i < mat.Length; i++)
{
    for (int j = 0; j < mat[0].Length; j++)
    {
        if (mat[i][j] == 'S')
        {
            S = new Tuple<int, int>(i, j);
            visited[i, j] = true;
            break;
        }
    }
}

// queue
Queue<Tuple<int, int, List<Tuple<int, int>>>> que = new();
//Up
if (S.Item1 - 1 >= 0
    && mat[S.Item1 - 1][S.Item2] != '.'
    && mat[S.Item1 - 1][S.Item2] != '-'
    && mat[S.Item1 - 1][S.Item2] != 'L'
    && mat[S.Item1 - 1][S.Item2] != 'J')
{
    que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(S.Item1 - 1, S.Item2,
        [new(S.Item1, S.Item2)]));
}

//Down
if (S.Item1 + 1 < R
    && mat[S.Item1 + 1][S.Item2] != '.'
    && mat[S.Item1 + 1][S.Item2] != '-'
    && mat[S.Item1 + 1][S.Item2] != 'F'
    && mat[S.Item1 + 1][S.Item2] != '7')
{
    que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(S.Item1 + 1, S.Item2,
        [new(S.Item1, S.Item2)]));
}

//Left
if (S.Item2 - 1 >= 0
    && mat[S.Item1][S.Item2 - 1] != '.'
    && mat[S.Item1][S.Item2 - 1] != '|'
    && mat[S.Item1][S.Item2 - 1] != 'J'
    && mat[S.Item1][S.Item2 - 1] != '7')
{
    que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(S.Item1, S.Item2 - 1,
        [new(S.Item1, S.Item2)]));
}

//Right
if (S.Item2 + 1 < C
    && mat[S.Item1][S.Item2 + 1] != '.'
    && mat[S.Item1][S.Item2 + 1] != '|'
    && mat[S.Item1][S.Item2 + 1] != 'L'
    && mat[S.Item1][S.Item2 + 1] != 'F')
{
    que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(S.Item1, S.Item2 + 1,
        [new(S.Item1, S.Item2)]));
}

// paths
int countLayers = 0;

List<List<Tuple<int, int>>> loopPaths = [];
// traverse
while (que.Count > 0)
{
    ++countLayers;
    int layerCount = que.Count;

    Tuple<int, int>? loopNode = containsDuplicatedElements(que);
    if (null != loopNode)
    {
        result = countLayers;
    }

    // each layer
    for (int L = 0; L < layerCount; L++)
    {
        Tuple<int, int, List<Tuple<int, int>>> q = que.Dequeue();
        int i = q.Item1;
        int j = q.Item2;

        List<Tuple<int, int>> path = q.Item3.ToList();
        path.Add(new Tuple<int, int>(i, j));

        if (null != loopNode && i == loopNode.Item1 && j == loopNode.Item2)
        {
            loopPaths.Add(path);
        }

        if (i < 0 || visited[i, j])
        {
            continue;
        }

        visited[i, j] = true;

        if (mat[i][j] == 'F')
        {
            // j+1
            if (j + 1 < C && !visited[i, j + 1] && mat[i][j + 1] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i, j + 1, path));
            }
            //i+1
            if (i + 1 < R && !visited[i + 1, j] && mat[i + 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i + 1, j, path));
            }
        }
        else
        if (mat[i][j] == 'J')
        {
            // j-1
            if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i, j - 1, path));
            }
            //i-1
            if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i - 1, j, path));
            }
        }
        else
            if (mat[i][j] == '7')
        {
            // j-1
            if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i, j - 1, path));
            }
            //i+1
            if (i + 1 < R && !visited[i + 1, j] && mat[i + 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i + 1, j, path));
            }
        }
        else
            if (mat[i][j] == 'L')
        {
            // j+1
            if (j + 1 < C && !visited[i, j + 1] && mat[i][j + 1] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i, j + 1, path));
            }
            //i-1
            if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i - 1, j, path));
            }
        }
        else
            if (mat[i][j] == '|')
        {
            //i-1
            if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i - 1, j, path));
            }
            //i+1
            if (i + 1 < R && !visited[i + 1, j] && mat[i + 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i + 1, j, path));
            }
        }
        else
            if (mat[i][j] == '-')
        {
            // j-1
            if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i, j - 1, path));
            }
            // j+1
            if (j + 1 < C && !visited[i, j + 1] && mat[i][j + 1] != '.')
            {
                que.Enqueue(new Tuple<int, int, List<Tuple<int, int>>>(i, j + 1, path));
            }
        }
    }
}

// two longest paths
List<Tuple<int, int>> loop1 = loopPaths.Last();
List<Tuple<int, int>> loop2 = loopPaths[loopPaths.Count - 2];

//find boundary elements
int[,] bound = new int[R, C];
foreach (Tuple<int, int> b in loop1)
{
    bound[b.Item1, b.Item2] = 1;
}
foreach (Tuple<int, int> b in loop2)
{
    bound[b.Item1, b.Item2] = 1;
}

Tuple<int, int>? containsDuplicatedElements(Queue<Tuple<int, int, List<Tuple<int, int>>>> que)
{
    HashSet<Tuple<int, int>> tuples = [];
    foreach (Tuple<int, int, List<Tuple<int, int>>> item in que)
    {
        if (tuples.Contains(new Tuple<int, int>(item.Item1, item.Item2)))
        {
            return new Tuple<int, int>(item.Item1, item.Item2);
        }

        tuples.Add(new Tuple<int, int>(item.Item1, item.Item2));
    }

    return null;
}

CorrectByRayCasting(mat, bound);

Console.WriteLine("\n");
//PrintMatrix(bound);

int count = 0;
for (int i = 0; i < R; i++)
{
    for (int j = 0; j < C; j++)
    {
        if (bound[i, j] == 0) // Inside the loop
        {
            ++count;
        }
    }
}

// 395
//7012,
Console.WriteLine($"Result = {count}");
Console.WriteLine($"Layers = {countLayers}");

void PrintMatrix(int[,] m)
{
    int R = m.GetLength(0);
    int C = m.GetLength(1);

    for (int i = 0; i < R; i++)
    {
        for (int j = 0; j < C; j++)
        {
            Console.Write($"{m[i, j]}");
        }
        Console.Write("\n");
    }
}

/**
 mat:   2d matrix, original input 
 bound: 2d matrix: 0 - non-boundary; 1 - boundary

 result: check all non-boundary elements, and set them to 2 if outside the boundary loop.
 */
void CorrectByRayCasting(string[] mat, int[,] bound)
{
    int numRows = bound.GetLength(0);
    for (int i = 0; i < numRows; i++)
    {
        RayCasting(mat, bound, i);
    }
}

void RayCasting(string[] M, int[,] B, int r)
{
    bool isInside = false;
    char prevB = ' ';
    char curB;

    int numColums = B.GetLength(1);
    for (int j = 0; j < numColums; j++)
    {
        // is bounday
        if (B[r, j] == 1)
        {
            curB = M[r][j];
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
                B[r, j] = 2;
            }
        }
    }
}

bool IsEnclosedByLoop(int[,] matrix, int row, int col)
{
    int rows = matrix.GetLength(0);
    int cols = matrix.GetLength(1);
    if (row < 0 || col < 0 || row >= rows || col >= cols)
    {
        return true; // Encountered the boundary, so it's inside
    }

    if (matrix[row, col] == 1)
    {
        return true; // Encountered the loop, so it's inside
    }

    if (matrix[row, col] != 0)
    {
        return false; // Already marked cell, so not part of the inside area being checked
    }

    matrix[row, col] = -2; // Temporary mark to avoid re-checking this cell

    // Check all four directions
    bool enclosed = IsEnclosedByLoop(matrix, row + 1, col)
                    && IsEnclosedByLoop(matrix, row - 1, col)
                    && IsEnclosedByLoop(matrix, row, col + 1)
                    && IsEnclosedByLoop(matrix, row, col - 1);

    if (!enclosed)
    {
        matrix[row, col] = 0; // Reset if not enclosed
    }

    return enclosed;
}