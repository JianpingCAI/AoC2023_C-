string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int sum = 0;

string[] DIGITS = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

int getLineValue(string chars)
{
    int firstDigit = 0;
    int lastDigit = 0;

    int firstDigitPos = int.MaxValue;
    int lastDigitPos = int.MinValue;

    // first digit
    for (int d = 0; d < DIGITS.Length; d++)
    {
        int pos = chars.IndexOf(DIGITS[d]);
        if (pos >= 0 && pos < firstDigitPos)
        {
            firstDigitPos = pos;
            firstDigit = d + 1;
        }
    }

    for (int c = 0; c < chars.Length; c++)
    {
        if (firstDigitPos != int.MaxValue && c >= firstDigitPos)
        {
            break;
        }

        if (char.IsDigit(chars[c]))
        {
            firstDigit = chars[c] - '0';
            break;
        }
    }

    // last digit
    for (int d = 0; d < DIGITS.Length; d++)
    {
        int pos = chars.LastIndexOf(DIGITS[d]);
        if (pos >= 0 && pos > lastDigitPos)
        {
            lastDigitPos = pos;
            lastDigit = d + 1;
        }
    }

    for (int c = chars.Length - 1; c >= 0; c--)
    {
        if (lastDigitPos != int.MinValue && c <= lastDigitPos)
        {
            break;
        }

        if (char.IsDigit(chars[c]))
        {
            lastDigit = chars[c] - '0';
            break;
        }
    }
    return firstDigit * 10 + lastDigit;
}

foreach (string line in lines)
{
    int value = getLineValue(line);

    sum += value;
}
Console.WriteLine($"Sum = {sum}");