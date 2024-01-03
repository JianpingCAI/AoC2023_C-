using System.Diagnostics;

public class Graph
{
    private Random random = new();

    public int VerticesCount;
    public List<int>[] AdjacencyList;

    // Each vertex initially represents itself
    private int[] _vertexLabels;

    public Graph(int verticesCount) : this(verticesCount, true)
    {
    }

    private Graph(int verticesCount, bool initializeLabels)
    {
        VerticesCount = verticesCount;
        AdjacencyList = new List<int>[verticesCount];
        if (initializeLabels)
        {
            _vertexLabels = Enumerable.Range(0, verticesCount).ToArray();
        }

        for (int i = 0; i < verticesCount; ++i)
        {
            AdjacencyList[i] = new List<int>();
        }
    }

    public void AddEdge(int u, int v)
    {
        if (u < 0 || u >= VerticesCount || v < 0 || v >= VerticesCount)
            throw new ArgumentOutOfRangeException("Vertex index out of range");

        AdjacencyList[u].Add(v);
        AdjacencyList[v].Add(u); // Since the graph is undirected
    }

    // Method to contract an edge
    private void ContractEdge(int u, int v)
    {
        // Merging the edges from vertex v to u
        foreach (int vertex in AdjacencyList[v])
        {
            if (vertex != u)
            {
                AdjacencyList[u].Add(vertex);
                AdjacencyList[vertex].Remove(v);
                AdjacencyList[vertex].Add(u);
            }
        }

        // Remove self loops
        AdjacencyList[u].RemoveAll(vertex => vertex == u);

        // Correctly update labels for all vertices originally connected to v
        for (int i = 0; i < _vertexLabels.Length; i++)
        {
            if (_vertexLabels[i] == v)
                _vertexLabels[i] = u;
        }
    }

    private void ContractEdge(int u, int v, List<int>[] localAdjList, int[] localVertexLabels)
    {
        foreach (int vertex in localAdjList[v])
        {
            if (vertex != u)
            {
                localAdjList[u].Add(vertex);
                localAdjList[vertex].Remove(v);
                localAdjList[vertex].Add(u);
            }
        }

        localAdjList[u].RemoveAll(vertex => vertex == u);

        for (int i = 0; i < localVertexLabels.Length; i++)
        {
            if (localVertexLabels[i] == v)
                localVertexLabels[i] = u;
        }
    }

    // Method to find the minimum cut
    public int KargerMinCut()
    {
        int vertices = VerticesCount;
        int edges = 0;

        // Count total edges in the graph
        foreach (List<int> list in AdjacencyList)
        {
            edges += list.Count;
        }

        edges /= 2; // Each edge counted twice

        while (vertices > 2)
        {
            // Pick a random edge
            int edgeIndex = random.Next(edges);
            int u = 0, v = 0;

            // Find vertices of the randomly selected edge
            for (int i = 0; i < AdjacencyList.Length; i++)
            {
                if (edgeIndex < AdjacencyList[i].Count)
                {
                    u = i;
                    v = AdjacencyList[i][edgeIndex];
                    break;
                }
                edgeIndex -= AdjacencyList[i].Count;
            }

            // Contract the edge
            ContractEdge(u, v);

            vertices--;
        }

        // Count and return remaining edges
        return AdjacencyList[0].Count;
    }

    public List<(int, int)> KargerMinCutEdges()
    {
        int localVerticesCount = VerticesCount;

        while (localVerticesCount > 2)
        {
            var flattenedEdges = AdjacencyList.SelectMany((edges, index) => edges.Select(v => (index, v))).ToList();
            var (u, v) = flattenedEdges[random.Next(flattenedEdges.Count)];

            ContractEdge(u, v);
            localVerticesCount--;
        }

        List<(int, int)> minCutEdges = new List<(int, int)>();
        foreach (int vertex in AdjacencyList[0])
        {
            if (_vertexLabels[vertex] != _vertexLabels[0])
            {
                minCutEdges.Add((_vertexLabels[0], _vertexLabels[vertex]));
            }
        }

        return minCutEdges.Distinct().ToList();
    }

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
            string filePath = "input2.txt";
            string[] lines = File.ReadAllLines(filePath);
            Stopwatch sw = Stopwatch.StartNew();

            int result = 0;

            List<EdgeNodeNamePair> diEdge_Name_Pairs = [];
            Dictionary<string, int> dict_name_index = [];

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
                }

                foreach (string? connectNodeName in connection_names)
                {
                    if (!dict_name_index.ContainsKey(connectNodeName))
                    {
                        dict_name_index[connectNodeName] = vertexIndex++;
                    }

                    diEdge_Name_Pairs.Add(new EdgeNodeNamePair(curName, connectNodeName));
                }
            }

            Dictionary<int, string> dict_index_name = [];
            foreach (KeyValuePair<string, int> item in dict_name_index)
            {
                dict_index_name.Add(item.Value, item.Key);
            }

            int maxIters = 50;
            List<(int, int)> minCutEdgesResult = [];
            int minCut = int.MaxValue;
            for (int i = 0; i < maxIters; i++)
            {
                Graph graph = new(dict_name_index.Count); // Create a graph

                // Add the edges to the graph
                foreach (EdgeNodeNamePair item in diEdge_Name_Pairs)
                {
                    graph.AddEdge(dict_name_index[item.NodeName1], dict_name_index[item.NodeName2]);
                }

                // Find the edges in the minimum cut
                List<(int, int)> minCutEdges = graph.KargerMinCutEdges();
                if (minCutEdges.Count < minCut)
                {
                    minCut = minCutEdges.Count;
                    minCutEdgesResult = minCutEdges;
                }
            }

            foreach ((int, int) edgeIndex in minCutEdgesResult)
            {
                Console.WriteLine($"Edge in minimum cut: ({dict_index_name[edgeIndex.Item1]}, {dict_index_name[edgeIndex.Item2]})");
            }

            foreach (var edge in minCutEdgesResult)
            {
                Console.WriteLine($"Edge in minimum cut: ({edge.Item1}, {edge.Item2})");
            }

            sw.Stop();

            result = minCut;
            Console.WriteLine($"Result = {result}");
            Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
        }
    }
}