using System.Diagnostics;

/**
 * https://www.tutorialspoint.com/design_and_analysis_of_algorithms/design_and_analysis_of_algorithms_kargers_minimum_cut.htm
 *
 *Algorithm
    Step 1 − Choose any random edge [u, v] from the graph G to be contracted.

    Step 2 − Merge the vertices to form a supernode and connect the edges of the other adjacent nodes of the vertices to the supernode formed. Remove the self nodes, if any.

    Step 3 − Repeat the process until there’s only two nodes left in the contracted graph.

    Step 4 − The edges connecting these two nodes are the minimum cut edges.

    The algorithm does not always the give the optimal output so the process is repeated multiple times to decrease the probability of error.
 */
record Edge(int V1, int V2);

internal class Graph
{
    private readonly int _vertCount;

    // one direction
    private readonly List<Edge> _edges = [];

    internal List<Edge> Edges => _edges;

    public Graph(int vertices)
    {
        _vertCount = vertices;
    }

    private int FindParentGroupIndex(int[] parent, int i)
    {
        if (parent[i] == i)
        {
            return i;
        }

        return FindParentGroupIndex(parent, parent[i]);
    }

    private void MergeTwoGroups(int[] parent, int[] rank, int v1, int v2)
    {
        int v1_parent = FindParentGroupIndex(parent, v1);
        int v2_parent = FindParentGroupIndex(parent, v2);

        if (rank[v1_parent] < rank[v2_parent])
        {
            parent[v1_parent] = v2_parent;
        }
        else if (rank[v1_parent] > rank[v2_parent])
        {
            parent[v2_parent] = v1_parent;
        }
        else
        {
            parent[v2_parent] = v1_parent;
            rank[v1_parent]++;
        }
    }

    public void AddEdge(int v1, int v2)
    {
        _edges.Add(new Edge(v1, v2));
    }

    public List<Edge> RunKargerMinCut()
    {
        // parent group denoted by the leading vertex index
        int[] parents = new int[_vertCount];
        // rank of a group
        int[] ranks = new int[_vertCount];
        for (int i = 0; i < _vertCount; i++)
        {
            parents[i] = i;
            ranks[i] = 0;
        }

        int curGroupCount = _vertCount;
        while (curGroupCount > 2)
        {
            int randomIndex = new Random().Next(_edges.Count) % _edges.Count;

            int v1 = _edges[randomIndex].V1;
            int v2 = _edges[randomIndex].V2;
            int v1_parent = FindParentGroupIndex(parents, v1);
            int v2_parent = FindParentGroupIndex(parents, v2);

            if (v1_parent != v2_parent)
            {
                MergeTwoGroups(parents, ranks, v1_parent, v2_parent);
                curGroupCount--;
            }
            _edges.RemoveAt(randomIndex);
        }

        int minCut = 0;
        List<Edge> minEdges = [];
        foreach (Edge edge in _edges)
        {
            int v1_parent = FindParentGroupIndex(parents, edge.V1);
            int v2_parent = FindParentGroupIndex(parents, edge.V2);

            // can cut this edge
            if (v1_parent != v2_parent)
            {
                minCut++;
                minEdges.Add(edge);
            }
        }

        return minEdges;
    }

    internal void GetOneGroupVerices(List<Edge> minEdges)
    {
        List<Edge> remainEdges = _edges.ToList();
        foreach (Edge edge in minEdges)
        {
            remainEdges.Remove(new Edge(edge.V1, edge.V2));
            remainEdges.Remove(new Edge(edge.V2, edge.V1));
        }
    }
};

internal class Program
{
    internal class Node
    {
        public string Name { get; set; }

        public HashSet<string> Connects { get; set; } = [];
    }

    record EdgeNodeNamePair(string NodeName1, string NodeName2);

    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        int result = 0;

        // non-directional
        List<EdgeNodeNamePair> edge_name_pairs = [];
        Dictionary<string, int> dict_name_index = [];
        Dictionary<string, List<string>> dict_vert_linkedVerts = [];

        int vertexIndex = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            string[] str_name_connections = line.Split(':', StringSplitOptions.TrimEntries).ToArray();
            string[] connection_names = str_name_connections[1].Split(' ').ToArray();

            string curName = str_name_connections[0];
            if (!dict_name_index.ContainsKey(curName))
            {
                dict_name_index[curName] = vertexIndex++;
                dict_vert_linkedVerts[curName] = [];
            }

            foreach (string? linkedVertName in connection_names)
            {
                if (!dict_name_index.ContainsKey(linkedVertName))
                {
                    dict_name_index[linkedVertName] = vertexIndex++;
                    dict_vert_linkedVerts[linkedVertName] = [];
                }

                edge_name_pairs.Add(new EdgeNodeNamePair(curName, linkedVertName));

                dict_vert_linkedVerts[curName].Add(linkedVertName);
                dict_vert_linkedVerts[linkedVertName].Add(curName);
            }
        }

        Dictionary<int, string> dict_index_name = [];
        foreach (KeyValuePair<string, int> item in dict_name_index)
        {
            dict_index_name.Add(item.Value, item.Key);
        }

        int maxIters = 500;
        List<Edge> minEdges = [];
        int minCut = int.MaxValue;
        Graph minGraph = null;
        for (int i = 0; i < maxIters; i++)
        {
            Graph graph = new(dict_name_index.Count); // Create a graph

            // Add the edges to the graph
            foreach (EdgeNodeNamePair item in edge_name_pairs)
            {
                graph.AddEdge(dict_name_index[item.NodeName1], dict_name_index[item.NodeName2]);
            }

            // Find the edges in the minimum cut
            List<Edge> minCutEdges = graph.RunKargerMinCut();
            if (minCutEdges.Count < minCut)
            {
                minEdges = minCutEdges;
                minCut = minCutEdges.Count;
                minGraph = graph;
            }
        }

        Console.WriteLine($"Min cut = {minCut}");
        foreach (Edge item in minEdges)
        {
            Console.WriteLine($"Cut {dict_index_name[item.V1]}-{dict_index_name[item.V2]}");
        }

        foreach (Edge minEdge in minEdges)
        {
            string v1 = dict_index_name[minEdge.V1];
            string v2 = dict_index_name[minEdge.V2];

            dict_vert_linkedVerts[v1].Remove(v2);
            dict_vert_linkedVerts[v2].Remove(v1);
        }

        string group_vert = dict_index_name[minEdges[0].V1];

        List<string> groupOne = GetVertGroup(dict_vert_linkedVerts, group_vert);

        int groupOne_Count = groupOne.Count;
        int groutTwo_Count = dict_index_name.Count - groupOne_Count;

        result = groupOne_Count * groutTwo_Count;

        sw.Stop();

        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static List<string> GetVertGroup(Dictionary<string, List<string>> dict_vert_linkedVerts, string startVertName)
    {
        List<string> verts = [];
        HashSet<string> visited = [];

        Queue<string> que = [];
        que.Enqueue(startVertName);

        while (que.Count > 0)
        {
            string curVert = que.Dequeue();
            if (visited.Contains(curVert))
            {
                continue;
            }
            visited.Add(curVert);

            verts.Add(curVert);

            foreach (string nextVert in dict_vert_linkedVerts[curVert])
            {
                que.Enqueue(nextVert);
            }
        }

        return verts;
    }
}