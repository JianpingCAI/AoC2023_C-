using System.Text;

string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int result = 0;
const int T = 3;

// input
List<Tuple<char[], int[]>> inputList = new(lines.Length);
for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    List<string> record = line.Split(' ').ToList();
    int[] counts = record.Last().Split(',').Select(int.Parse).ToArray();

    StringBuilder sb = new();
    for (int j = 0; j < T; j++)
    {
        sb.Append(record[0]);
        sb.Append('?');
    }
    sb.Remove(sb.Length - 1, 1);

    int[] duplicatedCounts = new int[counts.Length * T];
    for (int j = 0; j < T; j++)
    {
        Array.Copy(counts, 0, duplicatedCounts, j * counts.Length, counts.Length);
    }

    inputList.Add(new Tuple<char[], int[]>(sb.ToString().ToCharArray(), duplicatedCounts));
}

//
foreach (Tuple<char[], int[]> inputPair in inputList)
{
    int count = GetFeasibleArrangeCount(inputPair.Item1, inputPair.Item2);
    Console.WriteLine($"Count = {count}");

    result += count;
}

int GetFeasibleArrangeCount(char[] record, int[] counts)
{
    int count = 0;

    GetFeasibleCount(counts, record, 0, ref count);

    return count;
}

void GetFeasibleCount(int[] counts, char[] record, int i, ref int count)
{
    if (i > record.Length - 1)
    {
        if (IsFeasibleArrangement(record, counts))
        {
            ++count;
            return;
        }
        //++count;
        return;
    }

    int failCount = 0;
    if (record[i] == '?')
    {
        var copy1 = new char[record.Length];
        Array.Copy(record, copy1, record.Length);
        var copy2 = new char[record.Length];
        Array.Copy(record, copy2, record.Length);

        copy1[i] = '.';

        // stop early strategy
        if (IsSubRecordFeasible(copy1, i, counts))
        {
            GetFeasibleCount(counts, copy1, i + 1, ref count);
        }

        copy2[i] = '#';
        failCount++;
        if (IsSubRecordFeasible(copy2, i, counts))
        {
            GetFeasibleCount(counts, copy2, i + 1, ref count);
        }
    }
    else if (record[i] == '#')
    {
        var copy1 = new char[record.Length];
        Array.Copy(record, copy1, record.Length);

        if (IsSubRecordFeasible(copy1, i, counts))
        {
            GetFeasibleCount(counts, copy1, i + 1, ref count);
        }
    }
    else if (record[i] == '.')
    {
        var copy1 = new char[record.Length];
        Array.Copy(record, copy1, record.Length);

        GetFeasibleCount(counts, copy1, i + 1, ref count);
    }
}

bool IsSubRecordFeasible(char[] record, int i, int[] counts)
{
    char[] sub = new char[i + 1];
    Array.Copy(record, sub, i + 1);

    if (IsPossibleArrange(sub, counts))
        return true;

    return false;
}

bool IsPossibleArrange(char[] sub, int[] counts)
{
    int currentCount = 0;

    int checkIndex = 0;
    int i = 0;
    for (i = 0; i < sub.Length; i++)
    {
        char c = sub[i];

        if (c == '#')
        {
            currentCount++;
        }
        else
        {
            if (currentCount > 0)
            {
                // feasible
                if (checkIndex < counts.Length && currentCount == counts[checkIndex])
                {
                    //check next
                    ++checkIndex;
                    if (checkIndex == counts.Length)
                    {
                        currentCount = 0;

                        ++i;
                        break;
                    }
                }
                // not feasible
                else
                {
                    return false;
                }
            }

            currentCount = 0;
        }
    }

    // check the last one, feasible
    if (currentCount > 0)
    {
        if (checkIndex < counts.Length
        && currentCount <= counts[checkIndex])
        {
            //++checkIndex;
        }
        else
        {
            return false;
        }
    }

    return true;
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