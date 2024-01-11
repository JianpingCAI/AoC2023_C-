using System.Diagnostics;

string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);
Stopwatch sw = Stopwatch.StartNew();

SortedSet<int> valueSet = [];

int currentTotal = 0;
for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    if (string.IsNullOrEmpty(line))
    {
        valueSet.Add(currentTotal);
        currentTotal = 0;

        continue;
    }

    currentTotal += int.Parse(line);
}

if (currentTotal > 0)
{
    valueSet.Add(currentTotal);
}

int result = valueSet.TakeLast(3).Sum();

sw.Stop();
Console.WriteLine($"Result = {result}");
Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");