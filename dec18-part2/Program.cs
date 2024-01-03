using System.Diagnostics;

internal class Program
{
    record DigInstruction(char Dir, int Len);
    private static readonly bool _isPrint = false;
    private static int _ROWs;
    private static int _COLs;
    private static int _start_i;
    private static int _start_j;

    private static void Main(string[] args)
    {
        //Rows = 13434340
        //Columns = 11775677
        string filePath = "input.txt";
        //if (filePath == "input2.txt")
        //{
        //    _isPrint = true;
        //}
        string[] lines = File.ReadAllLines(filePath);

        Stopwatch sw = Stopwatch.StartNew();
        List<DigInstruction> digs = GetInputs(lines);

        //List<DigInstruction> digs = GetInputs2(lines);
        long result = DigTrench(digs);

        sw.Stop();
        //54732849558210 (too high)
        //54662804037719
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static long DigTrench(List<DigInstruction> digs)
    {
        GetTrechSize(digs);

        // start from (_start_i, _start_j)
        List<Node> boundaryNodes = GetBoundaryNodes(digs);

        if (_isPrint)
        {
            //PrintTrench(mat);
        }

        //string[] shapeM = FormShapes(mat);

        //if (_isPrint)
        //{
        //    Console.WriteLine();
        //    Print(shapeM);
        //}

        //CorrectByRayCasting(shapeM, mat);

        long result = RayCasting(boundaryNodes);
        //long result = CountTrenches(mat);
        //if (_isPrint)
        //{
        //    Console.WriteLine();
        //    PrintTrench(mat);
        //}

        return result;
    }

    private static long RayCasting(List<Node> boundaryNodes)
    {
        long area = 0;
        int top_Boundary_Row = int.MaxValue;
        int bottom_Boundary_Row = int.MinValue;

        foreach (Node node in boundaryNodes)
        {
            top_Boundary_Row = int.Min(node.I, top_Boundary_Row);
            bottom_Boundary_Row = int.Max(node.I, bottom_Boundary_Row);
        }

        for (int row = top_Boundary_Row; row <= bottom_Boundary_Row; row++)
        {
            long lineArea = GetValidRowArea(boundaryNodes, row);

            //Console.WriteLine($"row={row}: {lineArea}");
            area += lineArea;
        }

        return area;
    }

    private static readonly List<Node> prevIntersectNodes = [];
    private static readonly long prevLineArea = 0;

    private static long GetValidRowArea(List<Node> nodes, int curRow)
    {
        long lineArea = 0;

        List<Node> intersectNodes = [];
        HashSet<Node> visited = [];

        for (int i = 0; i < nodes.Count; i++)
        {
            // get a line
            Node lineStart;
            Node lineEnd;
            if (i != nodes.Count - 1)
            {
                lineStart = nodes[i];
                lineEnd = nodes[i + 1];
            }
            else
            {
                lineStart = nodes.Last();
                lineEnd = nodes.First();
            }

            int minI = int.Min(lineStart.I, lineEnd.I);
            int maxI = int.Max(lineStart.I, lineEnd.I);

            // intersect with the line
            if (curRow >= minI && curRow <= maxI)
            {
                // horizontal line
                if (minI == maxI && curRow == minI)
                {
                    if (!visited.Contains(lineStart))
                    {
                        intersectNodes.Add(lineStart);
                        visited.Add(lineStart);
                    }
                    if (!visited.Contains(lineEnd))
                    {
                        intersectNodes.Add(lineEnd);
                        visited.Add(lineEnd);
                    }
                }
                //vertical line
                else if (minI != maxI)
                {
                    if (curRow != minI && curRow != maxI)
                    {
                        Node newNode = new(curRow, lineStart.J, NodeType.I);
                        if (!visited.Contains(newNode))
                        {
                            intersectNodes.Add(newNode);
                            visited.Add(newNode);
                        }
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        if (intersectNodes.Count == 0)
        {
            throw new Exception();
        }

        // sort
        intersectNodes.Sort((x, y) => x.J - y.J);

        // why this cache does not work???
        //if (!IsEqualColumns(intersectNodes, prevIntersectNodes))
        //{
        lineArea = GetOverlappedAreaLength(intersectNodes);

        //    prevIntersectNodes = intersectNodes;
        //    prevLineArea = lineArea;
        //}
        //// cache the result
        //else
        //{
        //    lineArea = prevLineArea;
        //}

        return lineArea;
    }

    private static bool IsEqualColumns(List<Node> list1, List<Node> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i].J != list2[i].J)
            {
                return false;
            }
        }

        return true;
    }

    record Range(int Col1, int Col2);

    private static long GetOverlappedAreaLength(List<Node> intersectNodes)
    {
        long length = 0;

        bool isInside = false;
        int boundStart_j = intersectNodes[0].J;
        NodeType prev_Type = intersectNodes[0].Type;

        List<Range> ranges = [];

        for (int n = 0; n < intersectNodes.Count; n++)
        {
            Node? node = intersectNodes[n];
            switch (node.Type)
            {
                case NodeType.X:
                    {
                        throw new Exception();
                    }

                case NodeType.I:
                    {
                        isInside = !isInside;

                        // get inside
                        if (isInside)
                        {
                            boundStart_j = node.J;
                        }
                        // get outside
                        else
                        {
                            if (boundStart_j == -1)
                            {
                                throw new Exception();
                            }

                            ranges.Add(new Range(boundStart_j, node.J));

                            boundStart_j = -1;
                        }
                    }
                    break;

                case NodeType.F:
                case NodeType.L:
                    {
                        isInside = !isInside;

                        // get inside
                        if (isInside)
                        {
                            boundStart_j = node.J;
                        }
                        // get outside
                        else
                        {
                            if (boundStart_j == -1)
                            {
                                throw new Exception();
                            }
                            ranges.Add(new Range(boundStart_j, node.J));

                            boundStart_j = node.J;
                        }
                    }
                    break;

                case NodeType.J:
                    if (prev_Type != NodeType.F)
                    {
                        isInside = !isInside;
                    }

                    // still  inside
                    if (isInside)
                    {
                        if (boundStart_j == -1)
                        {
                            throw new Exception();
                        }

                        ranges.Add(new Range(boundStart_j, node.J));

                        boundStart_j = node.J;
                    }
                    // get outside
                    else
                    {
                        if (boundStart_j == -1)
                        {
                            throw new Exception();
                        }
                        ranges.Add(new Range(boundStart_j, node.J));

                        boundStart_j = -1;
                    }

                    break;

                case NodeType.T:
                    if (prev_Type != NodeType.L)
                    {
                        isInside = !isInside;
                    }

                    // still  inside
                    if (isInside)
                    {
                        if (boundStart_j == -1)
                        {
                            throw new Exception();
                        }

                        ranges.Add(new Range(boundStart_j, node.J));

                        boundStart_j = node.J;
                    }
                    // get outside
                    else
                    {
                        if (boundStart_j == -1)
                        {
                            throw new Exception();
                        }
                        ranges.Add(new Range(boundStart_j, node.J));

                        boundStart_j = -1;
                    }
                    break;

                default:
                    break;
            }

            prev_Type = node.Type;
        }

        length = GetLengthFromSegments(ranges);

        return length;
    }

    private static long GetLengthFromSegments(List<Range> ranges)
    {
        long length = 0;
        HashSet<int> values = [];

        foreach (Range range in ranges)
        {
            length += range.Col2 - range.Col1 + 1;
            values.Add(range.Col1);
            values.Add(range.Col2);
        }

        int duplicates = ranges.Count * 2 - values.Count;

        return length - duplicates;
    }

    record Pos(int I, int J, int LenI, int LenJ);

    private enum NodeType
    {
        X, //unknown
        L,
        J,
        F,
        T,
        I //verical
    }

    record Node(int I, int J, NodeType Type);

    private static List<Node> GetBoundaryNodes(List<DigInstruction> digs)
    {
        int i = _start_i;
        int j = _start_j;

        List<Node> nodes = [];

        for (int n = 0; n < digs.Count; n++)
        {
            DigInstruction inst = digs[n];
            char curDir = inst.Dir;
            char nextDir;
            if (n < digs.Count - 1)
            {
                nextDir = digs[n + 1].Dir;
            }
            else
            {
                nextDir = digs[0].Dir;
            }

            switch (curDir)
            {
                case 'R':
                    {
                        //7
                        if (nextDir == 'D')
                        {
                            nodes.Add(new Node(i, j + inst.Len, NodeType.T));
                        }
                        //J
                        else if (nextDir == 'U')
                        {
                            nodes.Add(new Node(i, j + inst.Len, NodeType.J));
                        }

                        j += inst.Len;
                    }
                    break;

                case 'L':
                    {
                        //F
                        if (nextDir == 'D')
                        {
                            nodes.Add(new Node(i, j - inst.Len, NodeType.F));
                        }
                        //L
                        else if (nextDir == 'U')
                        {
                            nodes.Add(new Node(i, j - inst.Len, NodeType.L));
                        }

                        j -= inst.Len;
                    }
                    break;

                case 'U':
                    {
                        //F
                        if (nextDir == 'R')
                        {
                            nodes.Add(new Node(i - inst.Len, j, NodeType.F));
                        }
                        //7
                        else if (nextDir == 'L')
                        {
                            nodes.Add(new Node(i - inst.Len, j, NodeType.T));
                        }

                        i -= inst.Len;
                    }
                    break;

                case 'D':
                    {
                        //L
                        if (nextDir == 'R')
                        {
                            nodes.Add(new Node(i + inst.Len, j, NodeType.L));
                        }
                        //J
                        else if (nextDir == 'L')
                        {
                            nodes.Add(new Node(i + inst.Len, j, NodeType.J));
                        }
                        i += inst.Len;
                    }
                    break;

                default:
                    break;
            }
        }

        Node startNode = nodes.Last();
        if (startNode.I != _start_i || startNode.J != _start_j)
        {
            throw new Exception();
        }
        if (startNode.Type == NodeType.X)
        {
            throw new Exception();
        }

        nodes.RemoveAt(nodes.Count - 1);
        nodes.Insert(0, startNode);

        return nodes;
    }

    private static void GetTrechSize(List<DigInstruction> digs)
    {
        int j = 0;
        int i = 0;
        int colsL = 0;
        int colsR = 0;
        int rowsU = 0;
        int rowsD = 0;
        foreach (DigInstruction dig in digs)
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

        if (i != 0 || j != 0)
        {
            throw new Exception();
        }

        //if (_isPrint)
        {
            Console.WriteLine(i);
            Console.WriteLine(j);

            Console.WriteLine($"Rows = {rowsD - rowsU + 1}");

            Console.WriteLine($"Columns = {colsR - colsL + 1}");

            Console.WriteLine($"_start_i = {_start_i}");
            Console.WriteLine($"_start_j = {_start_j}");
        }
    }

    private static List<DigInstruction> GetInputs2(string[] lines)
    {
        List<DigInstruction> digs = [];
        for (int i = 0; i < lines.Length; i++)
        {
            List<string> input = lines[i].Split(' ').ToList();
            digs.Add(new DigInstruction(input[0][0], int.Parse(input[1])));
        }
        return digs;
    }

    private static List<DigInstruction> GetInputs(string[] lines)
    {
        List<DigInstruction> digs = [];
        for (int i = 0; i < lines.Length; i++)
        {
            List<string> input = lines[i].Split(' ').ToList();
            string ins = input.Last();
            char dir = GetDirection(ins[ins.Length - 2]);
            string hexString = ins.Substring(2, ins.Length - 4);
            int intValue = Convert.ToInt32(hexString, 16);

            digs.Add(new DigInstruction(dir, intValue));
        }

        if (_isPrint)
        {
            PrintInstructions(digs);
        }
        return digs;
    }

    private static char GetDirection(char v)
    {
        switch (v)
        {
            case '0':
                return 'R';

            case '1':
                return 'D';

            case '2':
                return 'L';

            case '3':
                return 'U';

            default:
                break;
        }
        throw new ArgumentException();
    }

    private static void PrintInstructions(List<DigInstruction> digs)
    {
        foreach (DigInstruction dig in digs)
        {
            Console.Write(dig.Dir + " ");
            Console.Write(dig.Len);
            Console.WriteLine();
        }
    }
}