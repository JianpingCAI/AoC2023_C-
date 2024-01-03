string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int result = 1;

List<int> times = getLineNumbers(lines[0]);
List<int> dists = getLineNumbers(lines[1]);

for (int i = 0; i < times.Count; i++)
{
    int count = 0;

    int T = times[i];
    int D = dists[i];

    for (int t = 1; t < T; t++)
    {
        int dist = (T - t) * t;
        if (dist > D)
        {
            ++count;
        }
    }

    result *= count;
}

static List<int> getLineNumbers(string line)
{
    return line.Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
}

Console.WriteLine($"Result = {result}");