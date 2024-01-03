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

    List<char[]> possibles = [];

    GetPossibleArrangements(possibles, record, 0);

    foreach (char[] possible in possibles)
    {
        if (IsFeasibleArrangement(possible, counts))
        {
            ++count;
        }
    }

    return count;
}

void GetPossibleArrangements(List<char[]> possibles, char[] record, int i)
{
    if (i > record.Length - 1)
    {
        possibles.Add(record);
        return;
    }

    if (record[i] == '?')
    {
        char[] copy1 = new char[record.Length];
        Array.Copy(record, copy1, record.Length);
        char[] copy2 = new char[record.Length];
        Array.Copy(record, copy2, record.Length);

        copy1[i] = '.';
        GetPossibleArrangements(possibles, copy1, i + 1);

        copy2[i] = '#';
        GetPossibleArrangements(possibles, copy2, i + 1);
    }
    else if (record[i] == '.' || record[i] == '#')
    {
        char[] copy1 = new char[record.Length];
        Array.Copy(record, copy1, record.Length);

        GetPossibleArrangements(possibles, copy1, i + 1);
    }
}

bool IsFeasibleArrangement(char[] record, int[] counts)
{
    int currentCount = 0;
    char prevC = '.';

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