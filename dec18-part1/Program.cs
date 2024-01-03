using System.Diagnostics;

internal class Program
{
    record Dig(char Dir, int Len, string Color);
    private static bool _isPrint = false;
    private static int _ROWs;
    private static int _COLs;
    private static int _start_i;
    private static int _start_j;

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        if (filePath == "input2.txt")
        {
            _isPrint = true;
        }
        string[] lines = File.ReadAllLines(filePath);

        Stopwatch sw = Stopwatch.StartNew();
        List<Dig> digs = GetInputs(lines);
        int result = DigTrench(digs);

        sw.Stop();
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static int DigTrench(List<Dig> digs)
    {
        int ROWs, COLs;
        GetTrechSize(digs);

        // start from (_start_i, _start_j)
        int[,] mat = FormTrencMatrix(digs);

        //if (_isPrint)
        {
            PrintTrench(mat);
        }

        string[] M = FormShapes(mat);

        //if (_isPrint)
        {
            Console.WriteLine();
            Print(M);
        }

        CorrectByRayCasting(M, mat);

        int result = CountTrenches(mat);
        //if (_isPrint)
        {
            Console.WriteLine();
            PrintTrench(mat);
        }

        return result;
    }

    private static void Print(string[] m)
    {
        foreach (string item in m)
        {
            Console.WriteLine(item);
        }
    }

    private static void PrintTrench(int[,] mat)
    {
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            for (int j = 0; j < mat.GetLength(1); j++)
            {
                Console.Write(mat[i, j] == 1 ? '#' : '.');
            }
            Console.WriteLine();
        }
    }

    private static int CountTrenches(int[,] mat)
    {
        int count = 0;
        for (int i = 0; i < mat.GetLength(0); i++)
        {
            for (int j = 0; j < mat.GetLength(1); j++)
            {
                if (mat[i, j] == 1)
                {
                    ++count;
                }
            }
        }

        return count;
    }

    private static string[] FormShapes(int[,] mat)
    {
        string[] M = new string[_ROWs];

        for (int i = 0; i < _ROWs; i++)
        {
            char[] rowChars = new char[_COLs];
            for (int j = 0; j < _COLs; j++)
            {
                char shape = GetShape(i, j, mat);
                rowChars[j] = shape;
            }

            M[i] = new string(rowChars);
        }

        return M;
    }

    private static char GetShape(int i, int j, int[,] mat)
    {
        if (mat[i, j] == 0)
        {
            return '.';
        }

        //F
        if (i + 1 < _ROWs && j + 1 < _COLs
            && mat[i, j + 1] == 1
            && mat[i + 1, j] == 1)
        {
            return 'F';
        }

        //7
        if (j - 1 >= 0 && i + 1 < _ROWs
            && mat[i, j - 1] == 1
            && mat[i + 1, j] == 1)
        {
            return '7';
        }

        //L
        if (i - 1 >= 0 && j + 1 < _COLs
            && mat[i - 1, j] == 1
            && mat[i, j + 1] == 1)
        {
            return 'L';
        }

        //J
        if (i - 1 >= 0 && j - 1 >= 0
            && mat[i, j - 1] == 1
            && mat[i - 1, j] == 1)
        {
            return 'J';
        }

        //'-'
        if ((j - 1) >= 0 && (j + 1) < _COLs
            && mat[i, j - 1] == 1
            && mat[i, j + 1] == 1)
        {
            return '-';
        }

        //'|'
        if (i - 1 >= 0 && i + 1 < _ROWs
            && mat[i - 1, j] == 1
            && mat[i + 1, j] == 1)
        {
            return '|';
        }

        return '.';
    }

    /**
 mat:   2d matrix, original input
 bound: 2d matrix: 0 - non-boundary; 1 - boundary

 result: check all non-boundary elements, and set them to 2 if outside the boundary loop.
 */

    private static void CorrectByRayCasting(string[] mat, int[,] bound)
    {
        int numRows = bound.GetLength(0);
        for (int i = 0; i < numRows; i++)
        {
            RayCasting(mat, bound, i);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="shapeM"></param>
    /// <param name="bound"></param>
    /// <param name="r">row</param>
    private static void RayCasting(string[] shapeM, int[,] bound, int r)
    {
        bool isInside = false;
        char prevB = ' ';
        char curB;

        int numColums = bound.GetLength(1);
        for (int j = 0; j < numColums; j++)
        {
            // is bounday
            if (bound[r, j] == 1)
            {
                curB = shapeM[r][j];
                switch (curB)
                {
                    case '|':
                    case 'F':
                    case 'L':
                    case 'S':
                        isInside = !isInside;
                        break;

                    case 'J':
                        if (prevB != 'F')
                        {
                            isInside = !isInside;
                        }
                        break;

                    case '7':
                        if (prevB != 'L')
                        {
                            isInside = !isInside;
                        }
                        break;

                    default:
                        break;
                }

                // update previous boundary type
                if (curB != '-')
                {
                    prevB = curB;
                }
            }
            else
            {
                // set outside as 2
                if (!isInside)
                {
                    bound[r, j] = 2;
                }
                else
                {
                    bound[r,j] = 1;
                }
            }
        }
    }

    private static int[,] FormTrencMatrix(List<Dig> digs)
    {
        int[,] mat = new int[_ROWs, _COLs];

        int i = _start_i;
        int j = _start_j;
        mat[i, j] = 1;
        foreach (Dig dig in digs)
        {
            switch (dig.Dir)
            {
                case 'R':
                    {
                        for (int k = 1; k <= dig.Len; k++)
                        {
                            mat[i, j + k] = 1;
                        }

                        j += dig.Len;
                    }
                    break;

                case 'L':
                    {
                        for (int k = 1; k <= dig.Len; k++)
                        {
                            mat[i, j - k] = 1;
                        }
                        j -= dig.Len;
                    }
                    break;

                case 'U':
                    {
                        for (int k = 1; k <= dig.Len; k++)
                        {
                            mat[i - k, j] = 1;
                        }
                        i -= dig.Len;
                    }
                    break;

                case 'D':
                    {
                        for (int k = 1; k <= dig.Len; k++)
                        {
                            mat[i + k, j] = 1;
                        }
                        i += dig.Len;
                    }
                    break;

                default:
                    break;
            }
        }

        return mat;
    }

    private static void GetTrechSize(List<Dig> digs)
    {
        int j = 0;
        int i = 0;
        int colsL = 0;
        int colsR = 0;
        int rowsU = 0;
        int rowsD = 0;
        foreach (Dig dig in digs)
        {
            switch (dig.Dir)
            {
                case 'R':
                    {
                        j += dig.Len;
                        colsR = int.Max(colsR, j);
                    }
                    break;

                case 'L':
                    {
                        j -= dig.Len;
                        colsL = int.Min(colsL, j);
                    }
                    break;

                case 'U':
                    {
                        i -= dig.Len;
                        rowsU = int.Min(rowsU, i);
                    }
                    break;

                case 'D':
                    {
                        i += dig.Len;
                        rowsD = int.Max(rowsD, i);
                    }
                    break;

                default:
                    break;
            }
        }

        _ROWs = rowsD - rowsU + 1;
        _COLs = colsR - colsL + 1;
        _start_i = Math.Abs(rowsU);
        _start_j = Math.Abs(colsL);

        //if (_isPrint)
        {
            Console.WriteLine(i);
            Console.WriteLine(j);

            Console.WriteLine(rowsD - rowsU + 1);

            Console.WriteLine(colsR - colsL + 1);

            Console.WriteLine(_start_i);
            Console.WriteLine(_start_j);
        }
    }

    private static List<Dig> GetInputs(string[] lines)
    {
        List<Dig> digs = [];
        for (int i = 0; i < lines.Length; i++)
        {
            List<string> input = lines[i].Split(' ').ToList();
            digs.Add(new Dig(input[0][0], int.Parse(input[1]), input[2]));
        }
        return digs;
    }
}