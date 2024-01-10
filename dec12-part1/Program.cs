string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int result = 0;

// input
List<Tuple<char[], int[]>> inputList = new(lines.Length);
for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    List<string> input = line.Split(' ').ToList();
    int[] record = input.Last().Split(',').Select(int.Parse).ToArray();

    inputList.Add(new Tuple<char[], int[]>(input[0].ToCharArray(), record));
}

//
foreach (Tuple<char[], int[]> inputPair in inputList)
{
    int count = GetArrangementCount(inputPair.Item1, inputPair.Item2);

    result += count;
}

int GetArrangementCount(char[] record, int[] counts)
{
    int count = 0;

    List<char[]> possibleArrangements = [];

    // brute force: get all possible combinations
    GetPossibleArrangements(possibleArrangements, record, 0);

    foreach (char[] possible in possibleArrangements)
    {
        if (IsFeasibleArrangement(possible, counts))
        {
            ++count;
        }
    }

    return count;
}

// recursive
void GetPossibleArrangements(List<char[]> possibleArrangements, char[] record, int i)
{
    if (i > record.Length - 1)
    {
        possibleArrangements.Add(record);
        return;
    }

    if (record[i] == '?')
    {
        char[] copy1 = record.ToArray();
        char[] copy2 = record.ToArray();

        // consider '?' as '.'
        copy1[i] = '.';
        GetPossibleArrangements(possibleArrangements, copy1, i + 1);

        // consider '?' as '#'
        copy2[i] = '#';
        GetPossibleArrangements(possibleArrangements, copy2, i + 1);
    }
    else if (record[i] == '.' || record[i] == '#')
    {
        char[] copy1 = record.ToArray();

        GetPossibleArrangements(possibleArrangements, copy1, i + 1);
    }
}

bool IsFeasibleArrangement(char[] record, int[] counts)
{
    int currentCount = 0;

    int checkIndex = 0;
    int i = 0;
    for (i = 0; i < record.Length; i++)
    {
        char c = record[i];

        if (c == '#')
        {
            currentCount++;
        }
        else
        {
            if (currentCount > 0)
            {
                if (checkIndex < counts.Length && currentCount == counts[checkIndex])
                {
                    ++checkIndex;
                    if (checkIndex == counts.Length)
                    {
                        ++i;
                        break;
                    }
                }
                else
                {
                    return false;
                }
            }

            currentCount = 0;
        }
    }

    if (currentCount > 0 && checkIndex < counts.Length && currentCount == counts[checkIndex])
    {
        ++checkIndex;
    }

    if (checkIndex == counts.Length)
    {
        for (int j = i; j < record.Length; j++)
        {
            if (record[j] == '#')
            {
                return false;
            }
        }
        return true;
    }

    return false;
}

Console.WriteLine($"Result = {result}");