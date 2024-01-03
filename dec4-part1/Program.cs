string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int result = 0;

for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    (SortedSet<int> wins, List<int> nums) = parseInput(line);

    int count = 0;
    foreach (int num in nums)
    {
        if (wins.Contains(num))
        {
            count++;
        }
    }

    if (count > 0)
    {
        result += (int)Math.Pow(2, count - 1);
    }
}

(SortedSet<int> wins, List<int> nums) parseInput(string line)
{
    int winStart = line.IndexOf(':') + 2;
    int numStart = line.IndexOf('|') + 2;

    string winsString = line.Substring(winStart, numStart - 3 - winStart);
    string numsString = line.Substring(numStart, line.Length - numStart);
    //Console.WriteLine($"win: {winsString}");
    //Console.WriteLine($"nums: {numsString}");

    SortedSet<int> wins = [];

    List<int> unsortedWins = winsString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse).ToList();
    foreach (int num in unsortedWins)
    {
        wins.Add(num);
    }

    List<int> nums = numsString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse).ToList();

    return (wins, nums);
}

//List<int> integerList = numbers.Split(',')
//                                       .Select(int.Parse)
//                                       .ToList();
Console.WriteLine($"Result = {result}");