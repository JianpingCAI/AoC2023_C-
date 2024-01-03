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

List<SortedSet<int>> validIndices = new(ROW);
for (int i = 0; i < ROW; i++)
{
    validIndices.Add([]);
}

// step1
Dictionary<int, List<int>> starCols_byRow = [];

for (int i = 0; i < mat.Count; i++)
{
    for (int j = 0; j < mat[i].Length; j++)
    {
        if (!char.IsDigit(mat[i][j]) && mat[i][j] == '*')
        {
            setFlagPositions(i, j);
        }

        if (mat[i][j] == '*')
        {
            if (starCols_byRow.TryGetValue(i, out List<int>? value))
            {
                value.Add(j);
            }
            else
            {
                starCols_byRow[i] = [j];
            }
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
            if (r + i >= 0 && r + i < ROW
                && c + j >= 0 && c + j < COL
                && char.IsDigit(mat[r + i][c + j]))
            {
                validIndices[r + i].Add(c + j);

                //before
                int b = c + j - 1;
                while (b >= 0 && char.IsDigit(mat[r + i][b]))
                {
                    validIndices[r + i].Add(b);
                    b--;
                }

                //after
                int a = c + j + 1;
                while (a < COL && char.IsDigit(mat[r + i][a]))
                {
                    validIndices[r + i].Add(a);
                    a++;
                }
            }
        }
    }
}

// step3
// (row, (startIndex, endIndex, Number)
Dictionary<int, List<Tuple<int, int, int>>> numInfo_ByRow = [];

for (int r = 0; r < ROW; r++)
{
    numInfo_ByRow[r] = [];

    string rawLineString = mat[r];
    List<int> validColIndices = validIndices[r].ToList();

    if (validColIndices.Count > 0)
    {
        List<int> curRowNums = [];

        StringBuilder str = new();
        str.Append(rawLineString[validColIndices[0]]);

        for (int i = 1; i < validColIndices.Count; i++)
        {
            if (validColIndices[i] - validColIndices[i - 1] == 1)
            {
                str.Append(rawLineString[validColIndices[i]]);
            }
            else
            {
                string numStr = str.ToString();
                curRowNums.Add(int.Parse(numStr));
                numInfo_ByRow[r].Add(new Tuple<int, int, int>(validColIndices[i - 1] - numStr.Length + 1, validColIndices[i - 1], curRowNums.Last()));

                str = new StringBuilder();
                str.Append(rawLineString[validColIndices[i]]);
            }
        }

        if (!string.IsNullOrEmpty(str.ToString()))
        {
            string numStr = str.ToString();
            curRowNums.Add(int.Parse(numStr));

            numInfo_ByRow[r].Add(new Tuple<int, int, int>(validColIndices.Last() - numStr.Length + 1, validColIndices.Last(), curRowNums.Last()));
        }
    }
}

// step4
//Dictionary<int, List<int>> starCols_byRow = new();
//Dictionary<int/*,*/ List<Tuple<int, int, int>>> numLocations_byRow = new();
foreach (KeyValuePair<int, List<int>> star in starCols_byRow)
{
    int star_i = star.Key;

    //get number if any
    foreach (int star_j in star.Value)
    {
        List<int> starGears = [];

        // up
        if (star_i > 0)
        {
            if (numInfo_ByRow[star_i - 1].Count > 0)
            {
                starGears.AddRange(getGearNumbers(star_j, numInfo_ByRow[star_i - 1]));

                if (starGears.Count > 2)
                {
                    break;
                }
            }
        }

        //same row
        if (numInfo_ByRow[star_i].Count > 0)
        {
            starGears.AddRange(getGearNumbers(star_j, numInfo_ByRow[star_i]));

            if (starGears.Count > 2)
            {
                break;
            }
        }

        // below
        if (star_i + 1 < ROW)
        {
            if (numInfo_ByRow[star_i + 1].Count > 0)
            {
                starGears.AddRange(getGearNumbers(star_j, numInfo_ByRow[star_i + 1]));

                if (starGears.Count > 2)
                {
                    break;
                }
            }
        }

        // result
        if (starGears.Count == 2)
        {
            result += starGears[0] * starGears[1];
        }
    }
}

List<int> getGearNumbers(int i, List<Tuple<int, int, int>> numLocations)
{
    List<int> ngNums = [];
    foreach (Tuple<int, int, int> numLocation in numLocations)
    {
        // if i is in the number's start-end range
        if (!(i < numLocation.Item1 - 1 || i > numLocation.Item2 + 1))
        {
            ngNums.Add(numLocation.Item3);
        }
    }

    return ngNums;
}

Console.WriteLine($"Result = {result}");