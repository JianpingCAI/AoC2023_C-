using System.Collections.Concurrent;
using System.Diagnostics;

string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

long result = 0;

string STEPS = lines[0];

Dictionary<string, Tuple<string, string>> maps = [];

for (int i = 2; i < lines.Length; i++)
{
    string input = lines[i];

    string P = input.Substring(0, 3);
    string L = input.Substring(7, 3);
    string R = input.Substring(12, 3);

    maps[P] = new Tuple<string, string>(L, R);
}

Stopwatch stopwatch = new();

string[] currentPositions = maps.Keys.Where(x => x.EndsWith('A')).ToArray();
long[] neededSteps = new long[currentPositions.Length];
int LOOP = neededSteps.Length;

int[] startStepIndices = new int[currentPositions.Length];

ConcurrentDictionary<long, long> value_repeat_pairs = [];

Parallel.For(0, currentPositions.Length, (i) =>
{
    long count = getSteps(ref currentPositions[i], ref startStepIndices[i]);
    neededSteps[i] += count;
});

result = findLeastCommonMultiple(neededSteps);

static long GCD(long a, long b)
{
    while (b != 0)
    {
        long temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

static long LCM(long a, long b)
{
    return Math.Abs(a * b) / GCD(a, b);
}

static long findLeastCommonMultiple(long[] numbers)
{
    if (numbers.Length < 2)
    {
        throw new ArgumentException("List must contain at least two numbers.");
    }

    long lcm = numbers[0];
    for (int i = 1; i < numbers.Length; i++)
    {
        lcm = LCM(lcm, numbers[i]);
    }

    return lcm;
}

long getSteps(ref string curPos, ref int startIndex)
{
    if (startIndex == STEPS.Length)
    {
        startIndex = 0;
    }

    int i = startIndex;

    bool isFound = false;
    long count = 0;

    while (!isFound)
    {
        for (; i < STEPS.Length; i++)
        {
            count++;

            curPos = (STEPS[i] == 'L') ? maps[curPos].Item1 : maps[curPos].Item2;

            if (curPos[2] == 'Z')
            {
                isFound = true;
                startIndex = i + 1;

                if (startIndex != STEPS.Length)
                {
                    Console.WriteLine($"{startIndex}");
                }
                break;
            }
        }
        i = 0;
    }

    return count;
}

Console.WriteLine($"Result = {result}");
stopwatch.Stop();
Console.WriteLine($"Time = {stopwatch.Elapsed.TotalNanoseconds}");