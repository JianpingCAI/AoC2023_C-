using AocLib.DataTypes;
using System.Data;
using System.Diagnostics;

internal class Program2
{
    private static readonly HashSet<int> _cyclePatterns = [];
    private static int cycles = 0;
    private static readonly List<int> repeatedCycles = [];

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        long result = 0;

        DualMatrix<char> mat = GetInputDataMatrices(lines);
        int[] hashCodes = new int[4];

        //mat.Print();
        //Console.Clear();
        //Thread.Sleep(1000);

        const int COUNT_Cycles = 1000000000;
        for (int i = 0; i < COUNT_Cycles; i++)
        {
            ++cycles;

            RollVertical(mat);
            hashCodes[0] = (mat.GetHashCode());

            RollHorizon(mat);
            hashCodes[1] = (mat.GetHashCode());

            RollVertical(mat, false);
            hashCodes[2] = (mat.GetHashCode());

            RollHorizon(mat, false);
            hashCodes[3] = (mat.GetHashCode());

            if (HasRepeatPattern(hashCodes))
            {
                break;
            }
        }

        int bigCycle = _repeatIndex2 - _repeatIndex1;
        int remainCycles = (COUNT_Cycles - (_repeatIndex1 - 1)) % bigCycle;
        remainCycles--;

        for (int i = 1; i <= remainCycles; i++)
        {
            RunOneCycle(mat);
        }

        //Console.WriteLine()

        long load = GetLoad(mat);

        result += load;

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

    private static bool HasRepeatPattern(int[] hashCodes)
    {
        HashCode hash = new();

        foreach (int hc in hashCodes)
        {
            hash.Add(hc);
        }

        int cyclePatternCode = hash.ToHashCode();

        // second time detect the repeat
        if (cyclePatternCode == _repeatedCode)
        {
            _repeatIndex2 = cycles;

            return true;
        }

        // first time detect the repeat
        if (_cyclePatterns.Contains(cyclePatternCode))
        {
            if (!_repeatedCode.HasValue)
            {
                _repeatIndex1 = cycles;

                _repeatedCode = cyclePatternCode;
            }

            //Console.WriteLine("repeated");
        }
        else
        {
            _cyclePatterns.Add(cyclePatternCode);
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

    private static DualMatrix<char> GetInputDataMatrices(string[] lines)
    {
        List<char[]> rows = [];
        for (int i = 0; i < lines.Length; i++)
        {
            char[] row = lines[i].ToArray().Select(x => (x == '#') ? '2' : (x == '.' ? '1' : '0')).ToArray();
            rows.Add(row);
        }

        DualMatrix<char> mat = new(rows.Count, rows[0].Length);
        for (int r = 0; r < rows.Count; r++)
        {
            char[] row = rows[r];
            mat.SetRow(r, row);
        }

        return mat;
    }
}