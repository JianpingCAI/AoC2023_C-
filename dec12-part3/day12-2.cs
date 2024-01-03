using System.Diagnostics;
using System.Text;

// 1537505634471

Stopwatch sw = Stopwatch.StartNew();
string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

long result = 0;
const int T = 5;

// input

#region input

List<Tuple<char[], int[]>> inputList = new(lines.Length);
for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    List<string> record = line.Split(' ').ToList();
    int[] nums = record.Last().Split(',').Select(int.Parse).ToArray();

    StringBuilder sb = new();
    for (int j = 0; j < T; j++)
    {
        sb.Append(record[0]);
        sb.Append('?');
    }
    sb.Remove(sb.Length - 1, 1);

    int[] duplicatedCounts = new int[nums.Length * T];
    for (int j = 0; j < T; j++)
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

long GetFeasibleCount(char[] cfg, int[] nums, Dictionary<string, long> cache)
{
    if (cfg.Length == 0)
    {
        return nums.Length == 0 ? 1 : 0;
    }
    if (nums.Length == 0)
    {
        return cfg.Contains('#') ? 0 : 1;
    }

    string key = ToString(cfg, nums);
    if (cache.TryGetValue(key, out long value))
    {
        return value;
    }

    long count = 0;

    // consider '?' as '.'
    if (cfg[0] == '.' || cfg[0] == '?')
    {
        count += GetFeasibleCount(cfg.Skip(1).ToArray(), nums, cache);
    }

    // consider '?' as '#'
    if (cfg[0] == '#' || cfg[0] == '?')
    {
        // match the first batch of '#'s
        if (nums[0] <= cfg.Length
            && !cfg.Take(nums[0]).Contains('.') //.All(x => x == '#') wrong
            && (nums[0] == cfg.Length || cfg[nums[0]] != '#')//!!!
            )
        {
            count += GetFeasibleCount(cfg.Skip(nums[0] + 1).ToArray(), nums.Skip(1).ToArray(), cache);
        }
        //else
        //{
        //    count += 0;
        //}
    }

    cache[key] = count;
    return count;
}

///
/// You cannot use a Tuple<char[], int[]> as a key of a Dictionary directly in C# because Tuple types are not suitable for use as keys in dictionaries due to their default implementations of GetHashCode and Equals. The default implementations of these methods rely on reference equality, which means that two different tuples with the same elements would not be considered equal as keys.
///
string ToString(char[] cfg, int[] nums)
{
    StringBuilder sb = new();
    sb.Append(string.Join("", cfg));
    sb.Append(string.Join("", nums));

    return sb.ToString();
}

sw.Stop();
Console.WriteLine($"Result = {result}");
Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");