using System.Collections.Concurrent;
using System.Diagnostics;

Stopwatch sw = Stopwatch.StartNew();

string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

SortedDictionary<Tuple<long, long>, Tuple<long, long>> seed_soil_map = [];
SortedDictionary<Tuple<long, long>, Tuple<long, long>> soil_fert_map = [];
SortedDictionary<Tuple<long, long>, Tuple<long, long>> fert_water_map = [];
SortedDictionary<Tuple<long, long>, Tuple<long, long>> water_light_map = [];
SortedDictionary<Tuple<long, long>, Tuple<long, long>> light_temp_map = [];
SortedDictionary<Tuple<long, long>, Tuple<long, long>> temp_hum_map = [];
SortedDictionary<Tuple<long, long>, Tuple<long, long>> hum_loc_map = [];

List<SortedDictionary<Tuple<long, long>, Tuple<long, long>>> allStepMaps =
[
 seed_soil_map,soil_fert_map,fert_water_map,water_light_map,light_temp_map,temp_hum_map,hum_loc_map
];

string seedsLine = lines[0];

SortedSet<Tuple<long, long>> seed_ranges = getSeedRanges(seedsLine);

SortedSet<Tuple<long, long>> getSeedRanges(string line)
{
    SortedSet<Tuple<long, long>> seedRanges = [];
    List<long> nums = getLineNumbers(line);

    for (int i = 0; i + 1 < nums.Count; i += 2)
    {
        seedRanges.Add(new Tuple<long, long>(nums[i], nums[i] + nums[i + 1] - 1)); ;
    }

    return seedRanges;
}

for (long i = 2; i < lines.Length; i++)
{
    string line = lines[i];

    if (line.Contains("seed-to-soil"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            setRangeMap(lines[i], seed_soil_map);
            ++i;
        }
    }
    else if (line.Contains("soil-to-fertilizer"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            setRangeMap(lines[i], soil_fert_map);
            ++i;
        }
    }
    else if (line.Contains("fertilizer-to-water"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            setRangeMap(lines[i], fert_water_map);

            ++i;
        }
    }
    else if (line.Contains("water-to-light"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            setRangeMap(lines[i], water_light_map);

            ++i;
        }
    }
    else if (line.Contains("light-to-temperature"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            setRangeMap(lines[i], light_temp_map);

            ++i;
        }
    }
    else if (line.Contains("temperature-to-humidity"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            setRangeMap(lines[i], temp_hum_map);

            ++i;
        }
    }
    else if (line.Contains("humidity-to-location"))
    {
        ++i;
        while (i < lines.Length && !string.IsNullOrEmpty(lines[i]))
        {
            setRangeMap(lines[i], hum_loc_map);

            ++i;
        }
    }
}

SortedSet<Tuple<long, long>> inputRanges = seed_ranges;

for (int i = 0; i < allStepMaps.Count; i++)
{
    SortedDictionary<Tuple<long, long>, Tuple<long, long>> currentStepMap = allStepMaps[i];
    SortedSet<Tuple<long, long>> nextInputRanges = [];

    foreach (Tuple<long, long> inputRange in inputRanges)
    {
        SortedSet<Tuple<long, long>> mappedRanges = getMappedRanges(currentStepMap, inputRange);

        foreach (Tuple<long, long> mappedRange in mappedRanges)
        {
            nextInputRanges.Add(mappedRange);
        }
    }

    inputRanges = nextInputRanges;

    #region slower for the parallel version impl

    //ConcurrentBag<Tuple<long, long>> nextInputRanges2 = [];
    //Parallel.ForEach(inputRanges, (inputRange) =>
    //{
    //    SortedSet<Tuple<long, long>> mappedRanges = getMappedRanges(currentStepMap, inputRange);

    //    foreach (Tuple<long, long> mappedRange in mappedRanges)
    //    {
    //        nextInputRanges2.Add(mappedRange);
    //    }
    //});

    //inputRanges.Clear();
    //foreach (var item in nextInputRanges2)
    //{
    //    inputRanges.Add(item);
    //}

    #endregion slower for the parallel version impl
}

SortedSet<Tuple<long, long>> getMappedRanges(
    SortedDictionary<Tuple<long, long>, Tuple<long, long>> src_target_ranges,
    Tuple<long, long> inputRange)
{
    SortedSet<Tuple<long, long>> mapped = [];

    if (inputRange.Item1 > src_target_ranges.Last().Key.Item2
        || inputRange.Item2 < src_target_ranges.First().Key.Item1)
    {
        mapped.Add(inputRange);
        return mapped;
    }

    Queue<Tuple<long, long>> remainInputRanges = new();
    remainInputRanges.Enqueue(inputRange);

    bool isSkipMapCheck = false;
    bool isStop = false;
    foreach (KeyValuePair<Tuple<long, long>, Tuple<long, long>> s_t in src_target_ranges)
    {
        Tuple<long, long> srcRange = s_t.Key;
        Tuple<long, long> targetRange = s_t.Value;

        int unmappedCount = remainInputRanges.Count;
        while (unmappedCount > 0)
        {
            unmappedCount--;

            Tuple<long, long> remainRange = remainInputRanges.Dequeue();

            long start = remainRange.Item1;
            long end = remainRange.Item2;

            if (start > srcRange.Item2)
            {
                remainInputRanges.Enqueue(remainRange);

                isSkipMapCheck = true;
                break;
            }
            if (end < srcRange.Item1)
            {
                remainInputRanges.Enqueue(remainRange);

                isStop = true;
                break;
            }

            if (start < srcRange.Item1)
            {
                //1
                if (end > srcRange.Item2)
                {
                    mapped.Add(new Tuple<long, long>(targetRange.Item1, targetRange.Item2));

                    remainInputRanges.Enqueue(new Tuple<long, long>(srcRange.Item2 + 1, end));
                }
                //2
                else
                {
                    long off = end - start + 1;
                    mapped.Add(new Tuple<long, long>(targetRange.Item1, targetRange.Item1 + off));
                }

                mapped.Add(new Tuple<long, long>(start, srcRange.Item1 - 1));
            }
            else
            {
                //3
                if (end > srcRange.Item2)
                {
                    remainInputRanges.Enqueue(new Tuple<long, long>(srcRange.Item2 + 1, end));

                    long off = start - srcRange.Item1;
                    mapped.Add(new Tuple<long, long>(targetRange.Item1 + off, targetRange.Item2));
                }
                //4
                else
                {
                    long off1 = start - srcRange.Item1;
                    long off2 = end - srcRange.Item1;

                    mapped.Add(new Tuple<long, long>(targetRange.Item1 + off1, targetRange.Item1 + off2));
                }
            }
        }

        if (isSkipMapCheck)
        {
            isSkipMapCheck = false;
            continue;
        }
        if (isStop)
        {
            foreach (Tuple<long, long> item in remainInputRanges)
            {
                mapped.Add(item);
            }
            break;
        }
    }

    return mapped;
}

Console.WriteLine($"Result = {inputRanges.First().Item1}");
Console.WriteLine($"Done in {sw.ElapsedMilliseconds} ms");

static List<long> getLineNumbers(string line0)
{
    return line0.Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
}

static void setRangeMap(string line, SortedDictionary<Tuple<long, long>, Tuple<long, long>> range_map)
{
    List<long> nums = getLineNumbers(line);
    range_map[new Tuple<long, long>(nums[1], nums[1] + nums[2] - 1)] = new Tuple<long, long>(nums[0], nums[0] + nums[2] - 1);
}