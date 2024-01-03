using System.Text;

string filePath = "input2.txt";
string[] lines = File.ReadAllLines(filePath);

long result = 0;
const int T = 5;

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
    int tempCount = GetFeasibleArrangeCount(inputPair.Item1, inputPair.Item2);
    result += tempCount;

    Console.WriteLine($"Count = {tempCount}");
}

int GetFeasibleArrangeCount(char[] record, int[] counts)
{
    int count = 0;

    GetFeasibleCount(counts, record, 0, ref count);

    return count;
}

void GetFeasibleCount(int[] counts, char[] record, int i, ref int count)
{
    if (i >= record.Length)
    {
        if (IsFeasibleArrangement(record, counts))
        {
            ++count;
            return;
        }
        return;
    }

    int SingleRecordLen = (record.Length + 1) / T - 1;
    int SingeCountLen = counts.Length / T;
    int[] subCounts = new int[SingeCountLen];
    Array.Copy(counts, subCounts, SingeCountLen);

    if (i == SingleRecordLen)
    {
        record[i] = '#';
        char[] subRecord = new char[SingleRecordLen];
        Array.Copy(record, subRecord, SingleRecordLen);

        if (IsFeasibleArrangement(subRecord, subCounts))
        {
            ++count;
            return;
        }
    }

    if (record[i] == '?')
    {
        var copy1 = new char[record.Length];
        Array.Copy(record, copy1, record.Length);
        var copy2 = new char[record.Length];
        Array.Copy(record, copy2, record.Length);

        // 1. check one
        copy1[i] = '.';

        // stop early strategy
        if (i == SingleRecordLen - 1)
        {
            char[] subRecord = new char[SingleRecordLen];
            Array.Copy(copy1, subRecord, SingleRecordLen);

            if (!IsFeasibleArrangement(subRecord, subCounts))
            {
                // stop checking on it
            }
            else
            {
                GetFeasibleCount(counts, copy1, i + 1, ref count);
            }
        }
        else
        if (IsSubRecordPossible(copy1, i, counts))
        {
            GetFeasibleCount(counts, copy1, i + 1, ref count);
        }

        // 2. check two
        copy2[i] = '#';

        char[] subRecord2 = new char[SingleRecordLen];
        Array.Copy(copy2, subRecord2, SingleRecordLen);
        if (i == SingleRecordLen - 1)
        {
            if (!IsFeasibleArrangement(subRecord2, subCounts))
            {
                //return;
            }
            else
            {
                GetFeasibleCount(counts, copy2, i + 1, ref count);
            }
        }
        else if (IsSubRecordPossible(copy2, i, counts))
        {
            GetFeasibleCount(counts, copy2, i + 1, ref count);
        }
    }
    else if (record[i] == '#')
    {
        var copy1 = new char[record.Length];
        Array.Copy(record, copy1, record.Length);

        if (IsSubRecordPossible(copy1, i, counts))
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

bool IsSubRecordPossible(char[] record, int i, int[] counts)
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