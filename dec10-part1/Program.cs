string filePath = "input.txt";
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
Queue<Tuple<int, int>> que = new();
//T
if (S.Item1 - 1 >= 0 && mat[S.Item1 - 1][S.Item2] != '.' && mat[S.Item1 - 1][S.Item2] != '-')
{
    que.Enqueue(new Tuple<int, int>(S.Item1 - 1, S.Item2));
}

//B
if (S.Item1 + 1 < R && mat[S.Item1 + 1][S.Item2] != '.' && mat[S.Item1 + 1][S.Item2] != '-')
{
    que.Enqueue(new Tuple<int, int>(S.Item1 + 1, S.Item2));
}

//L
if (S.Item2 - 1 >= 0 && mat[S.Item1][S.Item2 - 1] != '.' && mat[S.Item1][S.Item2 - 1] != '|')
{
    que.Enqueue(new Tuple<int, int>(S.Item1, S.Item2 - 1));
}

//R
if (S.Item2 + 1 < C && mat[S.Item1][S.Item2 + 1] != '.' && mat[S.Item1][S.Item2 + 1] != '|')
{
    que.Enqueue(new Tuple<int, int>(S.Item1, S.Item2 + 1));
}

int countLayers = 0;

// traverse
while (que.Count > 0)
{
    ++countLayers;
    int layerCount = que.Count;

    if (containsDuplicatedElements(que))
    {
        result = countLayers;
    }

    for (int L = 0; L < layerCount; L++)
    {
        Tuple<int, int> p = que.Dequeue();
        int i = p.Item1;
        int j = p.Item2;

        if (visited[i, j])
        {
            continue;
        }
        visited[i, j] = true;

        if (mat[i][j] == 'F')
        {
            // j+1
            if (j + 1 < C && !visited[i, j + 1] && mat[i][j + 1] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i, j + 1));
            }
            //i+1
            if (i + 1 < R && !visited[i + 1, j] && mat[i + 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i + 1, j));
            }
        }
        else
        if (mat[i][j] == 'J')
        {
            // j-1
            if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i, j - 1));
            }
            //i-1
            if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i - 1, j));
            }
        }
        else
            if (mat[i][j] == '7')
        {
            // j-1
            if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i, j - 1));
            }
            //i+1
            if (i + 1 < R && !visited[i + 1, j] && mat[i + 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i + 1, j));
            }
        }
        else
            if (mat[i][j] == 'L')
        {
            // j+1
            if (j + 1 < C && !visited[i, j + 1] && mat[i][j + 1] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i, j + 1));
            }
            //i-1
            if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i - 1, j));
            }
        }
        else
            if (mat[i][j] == '|')
        {
            //i-1
            if (i - 1 >= 0 && !visited[i - 1, j] && mat[i - 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i - 1, j));
            }
            //i+1
            if (i + 1 < R && !visited[i + 1, j] && mat[i + 1][j] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i + 1, j));
            }
        }
        else
            if (mat[i][j] == '-')
        {
            // j-1
            if (j - 1 >= 0 && !visited[i, j - 1] && mat[i][j - 1] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i, j - 1));
            }
            // j+1
            if (j + 1 < C && !visited[i, j + 1] && mat[i][j + 1] != '.')
            {
                que.Enqueue(new Tuple<int, int>(i, j + 1));
            }
        }
    }
}

bool containsDuplicatedElements(Queue<Tuple<int, int>> que)
{
    HashSet<Tuple<int, int>> tuples = [];
    foreach (Tuple<int, int> item in que)
    {
        if (tuples.Contains(item))
        {
            return true;
        }

        tuples.Add(item);
    }

    return false;
}

//7001
Console.WriteLine($"Result = {result}");
Console.WriteLine($"Layers = {countLayers}");