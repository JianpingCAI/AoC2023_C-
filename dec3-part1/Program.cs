using System.Text;

string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int result = 0;
List<string> mat = [];

for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    mat.Add(line);
}

int ROW = mat.Count;
int COL = mat[0].Length;

List<SortedSet<int>> flags = new(ROW);
for (int i = 0; i < ROW; i++)
{
    flags.Add([]);
}

// step1
for (int i = 0; i < mat.Count; i++)
{
    for (int j = 0; j < mat[i].Length; j++)
    {
        if (!char.IsDigit(mat[i][j]) && mat[i][j] != '.')
        {
            setFlagPositions(i, j);
        }
    }
}

// step2
void setFlagPositions(int r, int c)
{
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            if ((r + i) >= 0 && (r + i) < ROW
                && (c + j) >= 0 && (c + j) < COL
                && char.IsDigit(mat[r + i][c + j]))
            {
                flags[r + i].Add(c + j);

                //before
                int b = c + j - 1;
                while (b >= 0 && char.IsDigit(mat[r + i][b]))
                {
                    flags[r + i].Add(b);
                    b--;
                }

                //after
                int a = c + j + 1;
                while (a < COL && char.IsDigit(mat[r + i][a]))
                {
                    flags[r + i].Add(a);
                    a++;
                }
            }
        }
    }
}

// step3
for (int r = 0; r < ROW; r++)
{
    string line = mat[r];
    List<int> rowIds = flags[r].ToList();
    int numDigits = rowIds.Count;

    if (numDigits > 0)
    {
        List<int> rowNums = [];

        StringBuilder str = new();
        str.Append(line[rowIds[0]]);

        for (int i = 1; i < numDigits; i++)
        {
            if (rowIds[i] - rowIds[i - 1] == 1)
            {
                str.Append(line[rowIds[i]]);
            }
            else
            {
                rowNums.Add(int.Parse(str.ToString()));

                str = new StringBuilder();
                str.Append(line[rowIds[i]]);
            }
        }

        if (!string.IsNullOrEmpty(str.ToString()))
        {
            rowNums.Add(int.Parse(str.ToString()));
        }

        foreach (int number in rowNums)
        {
            result += number;
        }
    }
}

Console.WriteLine($"Result = {result}");