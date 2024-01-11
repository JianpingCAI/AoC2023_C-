using System.Diagnostics;

string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);
Stopwatch sw = Stopwatch.StartNew();

int maxValue = int.MinValue;

int currentTotal = 0;
for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
    if (string.IsNullOrEmpty(line))
    {
        maxValue = int.Max(maxValue, currentTotal);
        currentTotal = 0;
        continue;
    }

    currentTotal += int.Parse(line);
}
maxValue = int.Max(maxValue, currentTotal);

int result = maxValue;

sw.Stop();
Console.WriteLine($"Result = {result}");
Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");