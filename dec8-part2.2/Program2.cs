string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

long result = 0;

string STEPS = lines[0];

Dictionary<string, Tuple<string, string>> maps = [];

for (int i = 2; i < lines.Length; i++)
{
    string input = lines[i];

    var P = input.Substring(0, 3);
    var L = input.Substring(7, 3);
    var R = input.Substring(12, 3);

    maps[P] = new Tuple<string, string>(L, R);
}

//
string[] curPoss = maps.Keys.Where(x => x.EndsWith('A')).ToArray();

bool isFound = false;
while (!isFound)
{
    foreach (char step in STEPS)
    {
        result++;

        //int[] founds = new int[curPoss.Length];

        //for (int i = 0; i < curPoss.Length; i++)
        bool success = true;
        Parallel.For(0, curPoss.Length, (i) =>
        {
            curPoss[i] = (step == 'L') ? maps[curPoss[i]].Item1 : maps[curPoss[i]].Item2;

            if (curPoss[i][2] != 'Z')
            {
                success = false;
            }
        });

        if (success)
        {
            isFound = true;
            break;
        }
        Console.WriteLine($"{result}");
    }
}

//if (curPos.EndsWith('Z'))
//{
//    isFound = true;
//    break;
//}
Console.WriteLine($"Result = {result}");