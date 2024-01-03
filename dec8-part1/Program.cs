string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int result = 0;

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

string curPos = "AAA";
bool isFound = false;
while (!isFound)
{
    foreach (char step in STEPS)
    {
        result++;

        if (step == 'L')
        {
            curPos = maps[curPos].Item1;
        }
        else
        {
            curPos = maps[curPos].Item2;
        }

        if (curPos == "ZZZ")
        {
            isFound = true;
            break;
        }
    }
}

Console.WriteLine($"Result = {result}");