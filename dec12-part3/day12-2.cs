using System.Diagnostics;
using System.Text;

Stopwatch sw = Stopwatch.StartNew();
string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

long result = 0;
const int COUNT_FOLDED = 5;

// input

#region input

List<Tuple<char[], int[]>> inputList = new(lines.Length);
for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    List<string> record = [.. line.Split(' ')];
    int[] nums = record.Last().Split(',').Select(int.Parse).ToArray();

    StringBuilder sb = new();
    for (int j = 0; j < COUNT_FOLDED; j++)
    {
        sb.Append(record[0]);
        sb.Append('?');
    }
    sb.Remove(sb.Length - 1, 1);

    int[] duplicatedCounts = new int[nums.Length * COUNT_FOLDED];
    for (int j = 0; j < COUNT_FOLDED; j++)
    {
        Array.Copy(nums, 0, duplicatedCounts, j * nums.Length, nums.Length);
    }

    inputList.Add(new Tuple<char[], int[]>(sb.ToString().ToCharArray(), duplicatedCounts));
}

#endregion input

#region single thread version

//foreach (var item in inputList)
//{
//    Dictionary<string, long> cache = [];

//    long count = GetFeasibleCount(item.Item1, item.Item2, cache);

//    //Console.WriteLine($"Count = {count}");

//    result += count;
//}

#endregion single thread version

#region parallel version

long[] counts = new long[inputList.Count];
Parallel.For(0, inputList.Count, i =>
{
    Dictionary<string, long> cache = [];

    long count = GetFeasibleCount(inputList[i].Item1, inputList[i].Item2, cache);

    counts[i] = count;
});
result = counts.Sum();

#endregion parallel version

// recursive
long GetFeasibleCount(char[] record, int[] nums, Dictionary<string, long> cache)
{
    if (record.Length == 0)
    {
        return nums.Length == 0 ? 1 : 0;
    }
    if (nums.Length == 0)
    {
        return record.Contains('#') ? 0 : 1;
    }

    string visited = ToString(record, nums);
    if (cache.TryGetValue(visited, out long value))
    {
        return value;
    }

    long count = 0;

    // consider '?' as '.'
    if (record[0] == '.' || record[0] == '?')
    {
        count += GetFeasibleCount(record.Skip(1).ToArray(), nums, cache);
    }

    // consider '?' as '#'
    if (record[0] == '#' || record[0] == '?')
    {
        // match the first batch of '#'s
        if (nums[0] <= record.Length
            && !record.Take(nums[0]).Contains('.') //should be all # (.All(x => x == '#') wrong)
            && (nums[0] == record.Length || record[nums[0]] != '#')//!!! no succeeded #
            )
        {
            count += GetFeasibleCount(record.Skip(nums[0] + 1).ToArray(), nums.Skip(1).ToArray(), cache);
        }
        //else
        //{
        //    count += 0;
        //}
    }

    cache[visited] = count;
    return count;
}

///
/// You cannot use a Tuple<char[], int[]> as a key of a Dictionary directly in C# because Tuple types are not suitable for use as keys in dictionaries due to their default implementations of GetHashCode and Equals. The default implementations of these methods rely on reference equality, which means that two different tuples with the same elements would not be considered equal as keys.
///
string ToString(char[] record, int[] nums)
{
    StringBuilder sb = new();
    sb.Append(string.Join("", record));
    sb.Append(string.Join("", nums));

    return sb.ToString();
}

sw.Stop();
Console.WriteLine($"Result = {result}");
Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");