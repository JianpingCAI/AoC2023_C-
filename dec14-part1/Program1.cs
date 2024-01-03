using System.Diagnostics;

internal class Program1
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        long result = 0;

        List<DualMatrix<char>> mats = GetInputDataMatrices(lines);

        foreach (DualMatrix<char> mat in mats)
        {
            mat.Print();

            RollNorth(mat);

            Console.WriteLine();
            mat.Print();

            long load = GetLoad(mat);

            result += load;
        }

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static long GetLoad(DualMatrix<char> mat)
    {
        long load = 0;

        for (int i = 0; i < mat.Rows; i++)
        {
            load += mat.Row(i).Count(x => x == '0') * (mat.Rows - i);
        }

        return load;
    }

    private static void RollNorth(DualMatrix<char> mat)
    {
        for (int i = 0; i < mat.Columns; i++)
        {
            char[] col = mat.Column(i);
            char[] t = getSortedLoad(col);

            mat.SetColumn(i, t);
        }

        static char[] getSortedLoad(char[] col)
        {
            List<int> splitIndices = getSplitIndices(col);

            if (splitIndices.Count > 0)
            {
                string colStr = new(col);

                char[][] subs = colStr.Split('2')
                       .Select(sort)
                       .ToArray();

                int count = 0;
                IEnumerable<char> con = [];
                int iSub = 0;
                int s = 0;
                for (s = 0; s < splitIndices.Count;)
                {
                    if (count < splitIndices[s])
                    {
                        con = con.Concat(subs[iSub]);

                        count += subs[iSub].Length;
                        iSub++;
                    }
                    else
                    {
                        con = con.Concat(['2']);

                        count++;
                        ++s;
                    }
                }

                while (iSub < subs.Length)
                {
                    con = con.Concat(subs[iSub]);
                    iSub++;
                }
                while (s < splitIndices.Count)
                {
                    con = con.Concat(['2']);
                    s++;
                }

                if (con.Count() != col.Length)
                {
                }

                return con.ToArray();
            }
            else
            {
                Array.Sort(col);
                return col;
            }

            static char[] sort(string x)
            {
                char[] y = x.ToCharArray();
                Array.Sort(y);
                return y;
            }

            static List<int> getSplitIndices(char[] col)
            {
                List<int> result = [];
                for (int i = 0; i < col.Length; i++)
                {
                    if (col[i] == '2')
                    {
                        result.Add(i);
                    }
                }

                return result;
            }
        }
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
                char[] row = line.ToArray().Select(x => (x == '#') ? '2' : (x == '.' ? '1' : '0')).ToArray();
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

    public void SetColumn(int col, T[] newColValues)
    {
        if (newColValues == null)
        {
            throw new ArgumentNullException(nameof(newColValues));
        }

        if (newColValues.Length != Rows)
        {
            throw new ArgumentException("Length of newRowValues must be equal to the number of columns");
        }

        for (int row = 0; row < Columns; row++)
        {
            _matrix[row][col] = newColValues[row];
            _transposedMatrix[col][row] = newColValues[row];
        }
    }

    internal void Print()
    {
        foreach (T[] row in _matrix)
        {
            foreach (T t in row)
            {
                Console.Write(t);
            }
            Console.WriteLine();
        }
    }
}