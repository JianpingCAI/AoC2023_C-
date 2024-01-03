string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

long result = 0;

SortedDictionary<long, Tuple<long, long>> seed_soil_map = [];
SortedDictionary<long, Tuple<long, long>> soil_fert_map = [];
SortedDictionary<long, Tuple<long, long>> fert_water_map = [];
SortedDictionary<long, Tuple<long, long>> water_light_map = [];
SortedDictionary<long, Tuple<long, long>> light_temp_map = [];
SortedDictionary<long, Tuple<long, long>> temp_hum_map = [];
SortedDictionary<long, Tuple<long, long>> hum_loc_map = [];

string line0 = lines[0];

List<long> seeds = getLineNumbers(line0);

for (long i = 2; i < lines.Length; i++)
{
    string line = lines[i];

    if (line.Contains("seed-to-soil"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            List<long> nums = getLineNumbers(lines[i]);
            seed_soil_map[nums[1]] = new Tuple<long, long>(nums[0], nums[2]);
            ++i;
        }
    }
    else if (line.Contains("soil-to-fertilizer"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            List<long> nums = getLineNumbers(lines[i]);
            soil_fert_map[nums[1]] = new Tuple<long, long>(nums[0], nums[2]);
            ++i;
        }
    }
    else if (line.Contains("fertilizer-to-water"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            List<long> nums = getLineNumbers(lines[i]);
            fert_water_map[nums[1]] = new Tuple<long, long>(nums[0], nums[2]);
            ++i;
        }
    }
    else if (line.Contains("water-to-light"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            List<long> nums = getLineNumbers(lines[i]);
            water_light_map[nums[1]] = new Tuple<long, long>(nums[0], nums[2]);
            ++i;
        }
    }
    else if (line.Contains("light-to-temperature"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            List<long> nums = getLineNumbers(lines[i]);
            light_temp_map[nums[1]] = new Tuple<long, long>(nums[0], nums[2]);
            ++i;
        }
    }
    else if (line.Contains("temperature-to-humidity"))
    {
        ++i;
        while (!string.IsNullOrEmpty(lines[i]))
        {
            List<long> nums = getLineNumbers(lines[i]);
            temp_hum_map[nums[1]] = new Tuple<long, long>(nums[0], nums[2]);
            ++i;
        }
    }
    else if (line.Contains("humidity-to-location"))
    {
        ++i;
        while (i < lines.Length && !string.IsNullOrEmpty(lines[i]))
        {
            List<long> nums = getLineNumbers(lines[i]);
            hum_loc_map[nums[1]] = new Tuple<long, long>(nums[0], nums[2]);
            ++i;
        }
    }
}

result = long.MaxValue;
foreach (long seed in seeds)
{
    long soil = FindDest(seed_soil_map, seed);
    long fert = FindDest(soil_fert_map, soil);
    long water = FindDest(fert_water_map, fert);
    long light = FindDest(water_light_map, water);
    long temp = FindDest(light_temp_map, light);
    long hum = FindDest(temp_hum_map, temp);
    long loc = FindDest(hum_loc_map, hum);

    result = long.Min(result, loc);
}

Console.WriteLine($"Result = {result}");

static List<long> getLineNumbers(string line0)
{
    return line0.Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
}

static long FindLowerBound(List<long> sortedList, long value)
{
    int index = sortedList.BinarySearch(value);

    if (index >= 0)
    {
        //Exact value found, return its index.
        return sortedList[index];
    }
    else
    {
        //Value not found.Find the index where it would be inserted.
        int insertionIndex = ~index;

        //If the insertion index is 0, the value is smaller than all elements.
        if (insertionIndex == 0)
        {
            return -1; // No lower bound found.
        }

        //Return the index of the largest element smaller than the value.
        return sortedList[insertionIndex - 1];
    }
}

static long FindDest(SortedDictionary<long, Tuple<long, long>> src_des_map, long src)
{
    long result = src;
    if (!(src < src_des_map.First().Key 
        || src > (src_des_map.Last().Key + src_des_map.Last().Value.Item2)))
    {
        long lb = FindLowerBound(src_des_map.Keys.ToList(), src);
        if (lb == src)
        {
            result = src_des_map[lb].Item1;
        }
        else if (src <= lb + src_des_map[lb].Item2)
        {
            result = src_des_map[lb].Item1 + (src - lb);
        }
    }

    return result;
}

//using System;
//using System.Collections.Generic;

//public class Program
//{
//    public static void Main()
//    {
//        List<long> sortedList = new List<long> { 1, 3, 5, 6, 7, 9 };
//        long value = 6;

//        long lowerBoundIndex = FindLowerBound(sortedList, value);

//        if (lowerBoundIndex >= 0)
//        {
//            Console.WriteLine($"Lower bound of {value} is at index {lowerBoundIndex}: {sortedList[lowerBoundIndex]}");
//        }
//        else
//        {
//            Console.WriteLine($"No lower bound found for {value} in the list.");
//        }
//    }
//}