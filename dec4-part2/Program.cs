string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int result = 0;

// step1
int[] winCount_byId = getWinsCount(lines);

// step2
Dictionary<int, int> cardCount_byId = getCardCountById(winCount_byId);

// step3
foreach (KeyValuePair<int, int> item in cardCount_byId)
{
    result += item.Value;
}
Console.WriteLine($"Result = {result}");

int[] getWinsCount(string[] lines)
{
    int[] winCount_byId = new int[lines.Length];
    for (int i = 0; i < lines.Length; i++)
    {
        string line = lines[i];
        (SortedSet<int> WINs, List<int> rawNums) = parseInput(line);

        int count = 0;
        foreach (int num in rawNums)
        {
            if (WINs.Contains(num))
            {
                count++;
            }
        }

        winCount_byId[i] = count;
    }
    return winCount_byId;
}

Dictionary<int, int> getCardCountById(int[] winCount_byId)
{
    Dictionary<int, int> cardCount_byId = [];
    for (int i = 0; i < lines.Length; i++)
    {
        cardCount_byId[i] = 1;
    }

    for (int id = 0; id < lines.Length; id++)
    {
        int winCount = winCount_byId[id];

        for (int k = 1; k <= winCount; k++)
        {
            cardCount_byId[id + k] += cardCount_byId[id];
        }
    }

    return cardCount_byId;
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

    List<int> unsortedWins = winsString.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse).ToList();
    foreach (int num in unsortedWins)
    {
        wins.Add(num);
    }

    List<int> nums = numsString.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse).ToList();

    return (wins, nums);
}