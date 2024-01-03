string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);
Stopwatch sw = Stopwatch.StartNew();

int result = 0;
bool isPrint = false;

for (int i = 0; i < lines.Length; i++)
{
    string line = lines[i];
}

sw.Stop();
Console.WriteLine($"Result = {result}");
Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");