using AocLib.DataTypes;
using System.Diagnostics;

/// <summary>
/// Dynamic programming
/// </summary>
internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        long result = 0;
        List<DualMatrix<char>> dataMats = GetInputDataMatrices(lines);

        foreach (DualMatrix<char> mat in dataMats)
        {
            Console.WriteLine($"({mat.Rows}, {mat.Columns})");

            // convert each row into a long value
            long[] rowValues = ConvertToRowValues(mat);
            (int hStart, int hMaxLength) = FineLongestPalindromeByMirror(rowValues);

            Console.WriteLine($"h={hStart}, len={hMaxLength}");

            long[] colValues = ConvertToColValues(mat);
            (int vStart, int vMaxLength) = FineLongestPalindromeByMirror(colValues);
            Console.WriteLine($"v={vStart}, len={vMaxLength}");

            if (hMaxLength > 1
                && (hStart == 0 || hStart + hMaxLength == mat.Rows))
            {
                int hIndex = hStart + hMaxLength / 2;
                result += 100 * hIndex;

                if (hIndex == 0)
                {
                }

                Console.WriteLine($"index = {hIndex}");
            }
            else
            {
                int vIndex = vStart + vMaxLength / 2;
                result += vIndex;

                if (vIndex == 0)
                {
                }
                Console.WriteLine($"index = {vIndex}");
            }

            Console.WriteLine();
        }

        sw.Stop();

        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    // find max matching pattern
    private static (int start, int maxLength) FineLongestPalindromeByMirror(long[] values)
    {
        int N = values.Length;
        int maxLength = 1;
        int startIndex = 0;

        // dp[i][j] is true if values[i,j] is a Palindrome
        bool[,] dp = new bool[N, N];

        // length = 1
        for (int i = 0; i < N; i++)
        {
            dp[i, i] = true;
        }

        // length = 2
        for (int i = 0; i < N - 1; i++)
        {
            dp[i, i + 1] = (values[i] == values[i + 1]);

            if (dp[i, i + 1]
                && (i + 2 == N || i == 0))
            {
                startIndex = i;
                maxLength = 2;
            }
        }

        // length >= 3
        for (int L = 3; L <= N; L++)
        {
            for (int i = 0; i + L - 1 < N; i++)
            {
                int j = i + L - 1;
                dp[i, j] = dp[i + 1, j - 1] && (values[i] == values[j]);

                if (dp[i, j] && L % 2 == 0
                    && (i + L == N || i == 0))
                {
                    startIndex = i;
                    maxLength = L;
                }
            }
        }

        return (startIndex, maxLength);
    }

    private static long[] ConvertToColValues(DualMatrix<char> mat)
    {
        long[] values = new long[mat.Columns];

        for (int i = 0; i < mat.Columns; i++)
        {
            char[] col = mat.Column(i);
            long value = Convert.ToInt64(new string(col), 2);
            values[i] = value;
        }

        return values;
    }

    private static long[] ConvertToRowValues(DualMatrix<char> mat)
    {
        long[] values = new long[mat.Rows];

        for (int i = 0; i < mat.Rows; i++)
        {
            char[] row = mat.Row(i);
            long value = Convert.ToInt64(new string(row), 2);
            values[i] = value;
        }

        return values;
    }

    private static List<DualMatrix<char>> GetInputDataMatrices(string[] lines)
    {
        List<DualMatrix<char>> dataMats = [];

        List<char[]> rows = [];
        for (int i = 0; i <= lines.Length; i++)
        {
            string line = i == lines.Length ? string.Empty : lines[i];
            if (!string.IsNullOrEmpty(line))
            {
                char[] row = line.Select(x => x == '#' ? '1' : '0').ToArray();
                rows.Add(row);
            }
            else
            {
                DualMatrix<char> mat = new(rows.Count, rows[0].Length);
                for (int r = 0; r < rows.Count; r++)
                {
                    char[] row = rows[r];
                    mat.SetRow(r, row);
                }

                dataMats.Add(mat);

                rows.Clear();
            }
        }

        return dataMats;
    }
}