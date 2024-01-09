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

        DualMatrix<char> mat = GetInputDataMatrices(lines);

        mat.Print();

        RollNorth(mat);

        Console.WriteLine();
        mat.Print();

        long load = CalculateLoad(mat);

        result += load;

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static long CalculateLoad(DualMatrix<char> mat)
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

        // split each column into segments and reorder them
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

    /// <summary>
    /// Do some convertion on the original input data.
    /// </summary>
    /// <param name="lines"></param>
    /// <returns></returns>
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