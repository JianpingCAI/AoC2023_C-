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
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);

        Stopwatch sw = Stopwatch.StartNew();
        List<DigInstruction> digs = GetInputs(lines);

        long result = DigTrench(digs);

        sw.Stop();
        Console.WriteLine($"Result = {result}");

        // not an optimized solution. takes around 20 seconds to complete
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static long DigTrench(List<DigInstruction> digs)
    {
        GetTrenchSize(digs);

        // find the trench step nodes (which are all the corners), which forms the trench loop
        List<Node> trenchStepNodes = GetTrenchStepNodes(digs);

        // ray casting with the trench
        long result = RayCasting(trenchStepNodes);

        return result;
    }

    private static long RayCasting(List<Node> trenchNodes)
    {
        long area = 0;

        // find the first and last row of the boundary
        int top_Boundary_i = int.MaxValue;
        int bottom_Boundary_i = int.MinValue;

        foreach (Node node in trenchNodes)
        {
            top_Boundary_i = int.Min(node.i, top_Boundary_i);
            bottom_Boundary_i = int.Max(node.i, bottom_Boundary_i);
        }

        for (int row = top_Boundary_i; row <= bottom_Boundary_i; row++)
        {
            long lineArea = GetValidRowArea(trenchNodes, row);

            //Console.WriteLine($"row={row}: {lineArea}");
            area += lineArea;
        }

        return area;
    }

    private static List<Node> _prevIntersectNodes = [];
    private static int _prevRowIndex = -1;
    private static long _prevLineArea = 0;

    /// <summary>
    /// Ray casting on the given curRow
    /// </summary>
    /// <param name="trenchNodes"></param>
    /// <param name="curRow"></param>
    /// <returns></returns>
    private static long GetValidRowArea(List<Node> trenchNodes, int curRow)
    {
        long lineArea = 0;

        // get the nodes on the given curRow
        List<Node> intersectNodes = [];
        //HashSet<Node> visited = [];

        for (int i = 0; i < trenchNodes.Count; i++)
        {
            // get a line segment: connect two neighboring two trench corner nodes to form a line
            Node lineStart;
            Node lineEnd;
            if (i != trenchNodes.Count - 1)
            {
                lineStart = trenchNodes[i];
                lineEnd = trenchNodes[i + 1];
            }
            else
            {
                lineStart = trenchNodes.Last();
                lineEnd = trenchNodes.First();
            }

            int minI = int.Min(lineStart.i, lineEnd.i);
            int maxI = int.Max(lineStart.i, lineEnd.i);

            // intersect with the line
            if (curRow >= minI && curRow <= maxI)
            {
                // horizontal line
                if (minI == maxI)
                {
                    intersectNodes.Add(lineStart);
                    intersectNodes.Add(lineEnd);
                }
                //vertical line
                else //if (minI != maxI)
                {
                    if (curRow != minI && curRow != maxI)
                    {
                        Node newNode = new(curRow, lineStart.j, NodeType.I);
                        intersectNodes.Add(newNode);
                    }
                }
            }
        }

        // sort
        intersectNodes.Sort((x, y) => x.j - y.j);

        // why this cache does not work???
        if (!IsSameIntersectionPattern(intersectNodes, _prevIntersectNodes))
        {
            lineArea = GetOverlappedAreaLength(intersectNodes);
        }
        else
        {
            lineArea = _prevLineArea;
        }

        // cache the result
        _prevLineArea = lineArea;
        _prevRowIndex = curRow;
        _prevIntersectNodes = intersectNodes;

        return lineArea;
    }

    private static bool IsSameIntersectionPattern(List<Node> list1, List<Node> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i].j != list2[i].j || list1[i].Type != list2[i].Type)
            {
                return false;
            }
        }

        return true;
    }

    record Range(int j1, int j2);

    // same as day 10
    private static long GetOverlappedAreaLength(List<Node> intersectNodes)
    {
        long length = 0;

        bool isInside = false;
        int boundStart_j = intersectNodes[0].j;
        NodeType prev_Type = intersectNodes[0].Type;

        List<Range> segments = [];

        for (int n = 0; n < intersectNodes.Count; n++)
        {
            Node node = intersectNodes[n];
            switch (node.Type)
            {
                // vertical line
                case NodeType.I:
                    {
                        isInside = !isInside;

                        // get inside
                        if (isInside)
                        {
                            boundStart_j = node.j;
                        }
                        // get outside
                        else
                        {
                            segments.Add(new Range(boundStart_j, node.j));

                            boundStart_j = -1;
                        }
                    }
                    break;
                // corner line
                case NodeType.F:
                case NodeType.L:
                    {
                        isInside = !isInside;

                        // get inside
                        if (isInside)
                        {
                            boundStart_j = node.j;
                        }
                        // get outside
                        else
                        {
                            segments.Add(new Range(boundStart_j, node.j));

                            boundStart_j = node.j;
                        }
                    }
                    break;
                // corner line
                case NodeType.J:
                    if (prev_Type != NodeType.F)
                    {
                        isInside = !isInside;
                    }

                    // still  inside
                    if (isInside)
                    {
                        segments.Add(new Range(boundStart_j, node.j));

                        boundStart_j = node.j;
                    }
                    // get outside
                    else
                    {
                        segments.Add(new Range(boundStart_j, node.j));

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
                        segments.Add(new Range(boundStart_j, node.j));

                        boundStart_j = node.j;
                    }
                    // get outside
                    else
                    {
                        segments.Add(new Range(boundStart_j, node.j));

                        boundStart_j = -1;
                    }
                    break;

                default:
                    break;
            }

            prev_Type = node.Type;
        }

        length = GetLengthFromSegments(segments);

        return length;
    }

    private static long GetLengthFromSegments(List<Range> segments)
    {
        long length = 0;
        HashSet<int> values = [];

        foreach (Range range in segments)
        {
            length += range.j2 - range.j1 + 1;
            values.Add(range.j1);
            values.Add(range.j2);
        }

        int duplicates = segments.Count * 2 - values.Count;

        return length - duplicates;
    }

    record Pos(int I, int J, int LenI, int LenJ);

    private enum NodeType
    {
        L,
        J,
        F,
        T,
        I //verical
    }

    record Node(int i, int j, NodeType Type);

    private static List<Node> GetTrenchStepNodes(List<DigInstruction> digInstructions)
    {
        int i = _start_i;
        int j = _start_j;

        List<Node> nodes = [];

        for (int n = 0; n < digInstructions.Count; n++)
        {
            DigInstruction inst = digInstructions[n];
            char curDir = inst.Dir;
            char nextDir;
            if (n < digInstructions.Count - 1)
            {
                nextDir = digInstructions[n + 1].Dir;
            }
            else
            {
                nextDir = digInstructions[0].Dir;
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
        //if (startNode.i != _start_i || startNode.j != _start_j)
        //{
        //    throw new Exception();
        //}

        nodes.RemoveAt(nodes.Count - 1);
        nodes.Insert(0, startNode);

        return nodes;
    }

    private static void GetTrenchSize(List<DigInstruction> digs)
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