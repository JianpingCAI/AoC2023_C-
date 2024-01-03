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

    int predict = getPredictedValue(diffLists);

    results.Add(predict);
}

int getPredictedValue(List<List<int>> diffLists)
{
    int sum = 0;
    foreach (List<int> diffList in diffLists)
    {
        sum += diffList.Last();
    }

    return sum;
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