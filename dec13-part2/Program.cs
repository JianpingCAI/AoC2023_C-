using AocLib.DataTypes;
using System.Diagnostics;

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

            // check horizonal
            long[] rowValues = ConvertToRowValues(mat);
            int r = GetSplitLocation(rowValues);

            if (r > 0)
            {
                result += 100 * r;
            }
            // check vertical
            else
            {
                long[] colValues = ConvertToColValues(mat);
                r = GetSplitLocation(colValues);

                result += r;
            }

            Console.WriteLine($"{r}\n");
        }

        sw.Stop();

        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static int GetSplitLocation(long[] values)
    {
        int splitLoc = 0;
        for (int i = 1; i < values.Length; i++)
        {
            long[] up = values.Take(i).Reverse().ToArray();
            long[] down = values.Skip(i).ToArray();

            int minLen = int.Min(up.Length, down.Length);

            up = up.Take(minLen).ToArray();
            down = down.Take(minLen).ToArray();

            if (IsOneBitDiff(up, down))
            {
                splitLoc = i;
                break;
            }
        }

        return splitLoc;
    }

    private static bool IsOneBitDiff(long[] values1, long[] values2)
    {
        int onebitDiff_Count = 0;
        for (int i = 0; i < values1.Length; i++)
        {
            if (onebitDiff_Count > 1)
            {
                return false;
            }
            long v1 = values1[i];
            long v2 = values2[i];
            if (v1 == v2)
            {
                continue;
            }

            long n = v1 ^ v2;
            bool isOneBitDiff = (n > 0 && (n & (n - 1)) == 0);

            if (isOneBitDiff) //one bit diff
            {
                onebitDiff_Count++;
            }
            else //more than one bit diff
            {
                return false;
            }
        }

        return onebitDiff_Count == 1;
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