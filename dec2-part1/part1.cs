string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

const int MAX_R = 12;
const int MAX_G = 13;
const int MAX_B = 14;

const string R = "red";
const string G = "green";
const string B = "blue";

int result = 0;

// parse each line to get the number of each color
int getColorCount(string gameLine, string color)
{
    int maxCount = 0;

    int colorPos = 0;
    while (colorPos != -1 && colorPos < gameLine.Length)
    {
        colorPos = gameLine.IndexOf(color, colorPos, StringComparison.OrdinalIgnoreCase);

        if (colorPos != -1)
        {
            //Console.WriteLine($"'{color}' found at index: {startIndex}");
            //find digit pos
            int frontPos = gameLine.LastIndexOf(" ", colorPos - 2, StringComparison.OrdinalIgnoreCase);

            string digitString = gameLine.Substring(frontPos + 1, colorPos - frontPos - 1);
            int digit = int.Parse(digitString);

            if (digit > maxCount)
            {
                maxCount = digit;
            }

            colorPos += color.Length; // Move past the last found word
        }
    }

    return maxCount;
}

// count
for (int i = 0; i < lines.Length; i++)
{
    Console.WriteLine($"{lines[i]}");
    int r = getColorCount(lines[i], R);
    Console.WriteLine($"R={r}");
    if (r > MAX_R)
    {
        continue;
    }

    int g = getColorCount(lines[i], G);
    Console.WriteLine($"G={g}");
    if (g > MAX_G)
    {
        continue;
    }

    int b = getColorCount(lines[i], B);
    Console.WriteLine($"B={b}");
    if (b > MAX_B)
    {
        continue;
    }

    result += (i + 1);
}

Console.WriteLine($"Result = {result}");