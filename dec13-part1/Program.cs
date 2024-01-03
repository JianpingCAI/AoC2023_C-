using System.Diagnostics;

public class DualMatrix<T>
{
    private readonly T[][] _matrix;
    private readonly T[][] _transposedMatrix;
    private readonly int _rows;
    private readonly int _columns;

    public int Rows => _rows;

    public int Columns => _columns;

    public DualMatrix(int rows, int columns)
    {
        this._rows = rows;
        this._columns = columns;

        _matrix = new T[rows][];
        _transposedMatrix = new T[columns][];

        for (int i = 0; i < rows; i++)
        {
            _matrix[i] = new T[columns];
        }

        for (int j = 0; j < columns; j++)
        {
            _transposedMatrix[j] = new T[rows];
        }
    }

    public void Set(int row, int column, T value)
    {
        _matrix[row][column] = value;
        _transposedMatrix[column][row] = value;
    }

    public T[] Row(int row)
    {
        return _matrix[row];
    }

    public T[] Column(int column)
    {
        return _transposedMatrix[column];
    }

    public void SetRow(int row, T[] newRowValues)
    {
        if (newRowValues == null)
        {
            throw new ArgumentNullException(nameof(newRowValues));
        }

        if (newRowValues.Length != Columns)
        {
            throw new ArgumentException("Length of newRowValues must be equal to the number of columns");
        }

        for (int col = 0; col < Columns; col++)
        {
            _matrix[row][col] = newRowValues[col];
            _transposedMatrix[col][row] = newRowValues[col];
        }
    }
}

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

        //40934, 49065, 53413, 39911,40006
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    // find max matching pattern
    private static (int start, int maxLength) FineLongestPalindromeByMirror(long[] values)
    {
        int N = values.Length;
        int maxLength = 1;
        int start = 0;

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
                start = i;
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
                    start = i;
                    maxLength = L;
                }
            }
        }

        return (start, maxLength);
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