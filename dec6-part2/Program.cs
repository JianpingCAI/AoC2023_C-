string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

long result = 1;

long T = getLineNumber(lines[0]);
long D = getLineNumber(lines[1]);

long count = 0;

for (int t = 1; t < T; t++)
{
    long dist = (T - t) * t;
    if (dist > D)
    {
        ++count;
    }
}

Tuple<double, double> solutions = solveEquation(1, -T, D);

long count2 = (long)solutions.Item1 - (long)solutions.Item2 - 1;
//if (solutions.Item1 - (long)solutions.Item1 > 0.0)
//{
//    ++count2;
//}
if (!isSolution(1, -T, D, (long)solutions.Item1))
{
    ++count2;
}

Tuple<double, double> solveEquation(long a, long b, long c)
{
    double temp = Math.Sqrt(b * b - 4 * a * c);

    double x1 = ((-b + temp) * 0.5);
    double x2 = ((-b - temp) * 0.5);

    return new Tuple<double, double>(x1, x2);
}

bool isSolution(long a, long b, long c, long x)
{
    if (0 == a * x * x + b * x + c)
    {
        return true;
    }

    return false;
}

result = count;

static long getLineNumber(string line)
{
    return long.Parse(line.Split(':').Last().Replace(" ", ""));
}

Console.WriteLine($"Result = {result}");
Console.WriteLine($"Result2 = {count2}");