using System.Diagnostics;

/**
 * https://www.tutorialspoint.com/design_and_analysis_of_algorithms/design_and_analysis_of_algorithms_kargers_minimum_cut.htm
 *
 */
record Edge(int u, int v);

internal class Graph
{
    private readonly int V;

    // one direction
    private readonly List<Edge> _edges = [];

    internal List<Edge> Edges => _edges;

    private int find(int[] parent, int i)
    {
        if (parent[i] == i)
            return i;
        return find(parent, parent[i]);
    }

    private void unionSets(int[] parent, int[] rank, int x, int y)
    {
        int xroot = find(parent, x);
        int yroot = find(parent, y);

        if (rank[xroot] < rank[yroot])
            parent[xroot] = yroot;
        else if (rank[xroot] > rank[yroot])
            parent[yroot] = xroot;
        else
        {
            parent[yroot] = xroot;
            rank[xroot]++;
        }
    }

    public Graph(int vertices)
    {
        V = vertices;
    }

    public void addEdge(int u, int v)
    {
        _edges.Add(new Edge(u, v));
    }

    public List<Edge> kargerMinCut()
    {
        int[] parent = new int[V];
        int[] rank = new int[V];
        for (int i = 0; i < V; i++)
        {
            parent[i] = i;
            rank[i] = 0;
        }
        int v = V;
        while (v > 2)
        {
            int randomIndex = new Random().Next(_edges.Count) % _edges.Count;
            int u = _edges[randomIndex].u;
            int w = _edges[randomIndex].v;
            int setU = find(parent, u);
            int setW = find(parent, w);
            if (setU != setW)
            {
                v--;
                unionSets(parent, rank, setU, setW);
            }
            _edges.RemoveAt(randomIndex);
        }

        int minCut = 0;
        List<Edge> minEdges = [];
        foreach (var edge in _edges)
        {
            int setU = find(parent, edge.u);
            int setW = find(parent, edge.v);
            if (setU != setW)
            {
                minCut++;
                minEdges.Add(edge);
            }
        }

        return minEdges;
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
                graph.addEdge(dict_name_index[item.NodeName1], dict_name_index[item.NodeName2]);
            }

            // Find the edges in the minimum cut
            var minCutEdges = graph.kargerMinCut();
            if (minCutEdges.Count < minCut)
            {
                minEdges = minCutEdges;
                minCut = minCutEdges.Count;
                minGraph = graph;
            }
        }

        Console.WriteLine($"Min cut = {minCut}");
        foreach (var item in minEdges)
        {
            Console.WriteLine($"Cut {dict_index_name[item.u]}-{dict_index_name[item.v]}");
        }

        foreach (Edge minEdge in minEdges)
        {
            var v1 = dict_index_name[minEdge.u];
            var v2 = dict_index_name[minEdge.v];

            dict_vert_linkedVerts[v1].Remove(v2);
            dict_vert_linkedVerts[v2].Remove(v1);
        }

        string group_vert = dict_index_name[minEdges[0].u];

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