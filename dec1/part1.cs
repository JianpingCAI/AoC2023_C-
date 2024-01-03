string filePath = "input.txt";
string[] lines = File.ReadAllLines(filePath);

int sum = 0;
char? firstDigit = null;
char? lastDigit = null;

foreach (string line in lines)
{
    // Find the first digit from the front
    foreach (char c in line)
    {
        if (char.IsDigit(c))
        {
            firstDigit = c;
            break; // Stop the loop once the first digit is found
        }
    }

    // Find the first digit from the end
    for (int i = line.Length - 1; i >= 0; i--)
    {
        if (char.IsDigit(line[i]))
        {
            lastDigit = line[i];
            break; // Stop the loop once the last digit is found
        }
    }

    // get the number and sum it
    if (firstDigit != null)
    {
        int number = (firstDigit.Value - '0') * 10 + (lastDigit!.Value - '0');

        sum += number;
    }
}

Console.WriteLine($"Sum = {sum}");