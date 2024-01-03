using System.Data;
using System.Diagnostics;

internal class Program2
{
    private static readonly int[] _hashCodes = new int[4];
    private static readonly HashSet<int> matSet = [];
    private static int cycles = 0;
    private static readonly List<int> repeatedCycles = [];

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        long result = 0;

        List<DualMatrix<char>> mats = GetInputDataMatrices(lines);

        foreach (DualMatrix<char> mat in mats)
        {
            //mat.Print();
            //Console.Clear();
            //Thread.Sleep(1000);

            int COUNT = 1000000000;
            for (int i = 0; i < COUNT; i++)
            {
                ++cycles;

                RollVertical(mat);
                _hashCodes[0] = (mat.GetMatrixHashCode());

                RollHorizon(mat);
                _hashCodes[1] = (mat.GetMatrixHashCode());

                RollVertical(mat, false);
                _hashCodes[2] = (mat.GetMatrixHashCode());

                RollHorizon(mat, false);
                _hashCodes[3] = (mat.GetMatrixHashCode());

                if (FindRepeatPattern(_hashCodes))
                {
                    break;
                }
            }

            int bigCycle = _repeatIndex2 - _repeatIndex1;
            int remainCycles = (COUNT - (_repeatIndex1 - 1)) % bigCycle;
            remainCycles--;

            for (int i = 1; i <= remainCycles; i++)
            {
                RunOneCycle(mat);
            }

            //Console.WriteLine()

            long load = GetLoad(mat);

            result += load;
        }

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");

        static void RunOneCycle(DualMatrix<char> mat)
        {
            RollVertical(mat);

            RollHorizon(mat);

            RollVertical(mat, false);

            RollHorizon(mat, false);
        }
    }

    private static int _repeatIndex1 = -1;
    private static int _repeatIndex2 = -1;
    private static int? _repeatedCode = null;

    private static bool FindRepeatPattern(int[] hashCodes)
    {
        HashCode hash = new();

        foreach (int hc in hashCodes)
        {
            hash.Add(hc);
        }

        int h = hash.ToHashCode();

        if (h == _repeatedCode)
        {
            _repeatIndex2 = cycles;

            return true;
        }

        if (matSet.Contains(h))
        {
            if (!_repeatedCode.HasValue)
            {
                _repeatIndex1 = cycles;

                _repeatedCode = h;
            }

            //Console.WriteLine("repeated");
        }
        else
        {
            matSet.Add(h);
        }
        return false;
    }

    private static void RollHorizon(DualMatrix<char> mat, bool isWest = true)
    {
        for (int i = 0; i < mat.Rows; i++)
        {
            char[] col = mat.Row(i);
            char[] t = getSortedLoad(col, !isWest);

            mat.SetRow(i, t);
        }

        //Console.Clear();
        //mat.Print();

        //Thread.Sleep(100);
    }

    private static void RollVertical(DualMatrix<char> mat, bool isNorth = true)
    {
        for (int i = 0; i < mat.Columns; i++)
        {
            char[] col = mat.Column(i);
            char[] t = getSortedLoad(col, !isNorth);

            mat.SetColumn(i, t);
        }

        //Console.Clear();
        //mat.Print();

        //if (isNorth)
        //{
        //    Console.WriteLine();
        //    PrintCircleHashCodes(_hashCodes);
        //}
        //Thread.Sleep(100);
    }

    private static char[] getSortedLoad(char[] chars, bool isReverse)
    {
        List<int> splitIndices = getSplitIndices(chars);

        if (splitIndices.Count > 0)
        {
            string colStr = new(chars);

            char[][] subs = colStr.Split('2', StringSplitOptions.RemoveEmptyEntries)
                   .Select(x => sort(x, isReverse))
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

            if (con.Count() != chars.Length)
            {
                throw new ArgumentException();
            }

            return con.ToArray();
        }
        else
        {
            Array.Sort(chars);

            if (isReverse)
            {
                Array.Reverse(chars);
            }

            return chars;
        }

        static char[] sort(string x, bool isReverse)
        {
            char[] y = x.ToCharArray();
            Array.Sort(y);
            if (isReverse)
            {
                Array.Reverse(y);
            }
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

    private static long GetLoad(DualMatrix<char> mat)
    {
        long load = 0;

        for (int i = 0; i < mat.Rows; i++)
        {
            load += mat.Row(i).Count(x => x == '0') * (mat.Rows - i);
        }

        return load;
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
        Console.WriteLine();
    }

    public int GetMatrixHashCode()
    {
        HashCode hash = new();

        foreach (T[] row in _matrix)
        {
            foreach (T? c in row)
            {
                hash.Add(c);
            }
        }

        return hash.ToHashCode();
    }
}