string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int result = 0;

List<List<int>> inputLists = [];
for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    List<int> t = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList().Select(int.Parse).ToList();
    inputLists.Add(t);
}

List<int> results = new(inputLists.Count);
foreach (List<int> inputList in inputLists)
{
    List<List<int>> diffLists = getAllDiffLists(inputList);

    int history = getHistoryValue(diffLists);

    results.Add(history);
}

int getHistoryValue(List<List<int>> diffLists)
{
    int value = 0;
    bool add = false;

    foreach (List<int> list in diffLists)
    {
        add = !add;
        value += (add) ? list.First() : (-list.First());
    }

    return value;
}

List<List<int>> getAllDiffLists(List<int> inputList)
{
    List<List<int>> diffLists = [];
    diffLists.Add(inputList);

    List<int> currInput = inputList;
    while (true)
    {
        List<int> diffList = getDiffList(currInput);

        currInput = diffList;
        if (currInput.All(x => x == 0))
        {
            break;
        }
        diffLists.Add(diffList);
    }
    return diffLists;
}

List<int> getDiffList(List<int> inputList)
{
    List<int> diffs = new(inputList.Count - 1);

    for (int i = 1; i < inputList.Count; i++)
    {
        diffs.Add(inputList[i] - inputList[i - 1]);
    }

    return diffs;
}

result = results.Sum();
Console.WriteLine($"Result = {result}");