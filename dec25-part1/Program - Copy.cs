using System.Diagnostics;
using System.Linq.Expressions;

internal class Node
{
    public string Name { get; set; }

    public List<string> Connects { get; set; } = [];
}

internal class Graph
{
    private record EdgeNodeNamePair(string NodeName1, string NodeName2);

    public int NodeCount { get => Dict_Vert_LinkedVerts.Count; }
    public int EdgeCount { get => Dict_Vert_LinkedVerts.Select(x => x.Value.Count).Sum() / 2; }

    private readonly Random _Random = new();

    public Dictionary<string, List<string>> Dict_Vert_LinkedVerts = [];

    public void CreateGraph(string[] lines)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            var str_name_connections = line.Split(':', StringSplitOptions.TrimEntries).ToArray();
            var curLinkedVerts = str_name_connections[1].Split(' ').ToArray();

            string curName = str_name_connections[0];

            if (!Dict_Vert_LinkedVerts.ContainsKey(curName))
            {
                Dict_Vert_LinkedVerts[curName] = new List<string>();
            }

            foreach (var linkedVert in curLinkedVerts)
            {
                if (!Dict_Vert_LinkedVerts.ContainsKey(linkedVert))
                {
                    Dict_Vert_LinkedVerts[linkedVert] = new List<string>();
                }

                Dict_Vert_LinkedVerts[curName].Add(linkedVert);
                Dict_Vert_LinkedVerts[linkedVert].Add(curName);
            }
        }
    }

    public int MinCut()
    {
        int nodeCount = NodeCount;

        while (nodeCount > 2)
        {
            int edgeCount = Dict_Vert_LinkedVerts.Select(x => x.Value.Count).Sum();

            // pick a random edge
            int edgeIndex = _Random.Next(edgeCount);

            int count = 0;
            foreach (var vert_linkedVerts in Dict_Vert_LinkedVerts)
            {
                int curStartINdex = count;

                count += vert_linkedVerts.Value.Count;
                if (edgeIndex >= curStartINdex && edgeIndex < count)
                {
                    string linkedVert = Dict_Vert_LinkedVerts[vert_linkedVerts.Key][edgeIndex - curStartINdex];

                    ContractEdge(vert_linkedVerts.Key, linkedVert);

                    break;
                }
            }
            // contract the edge

            nodeCount--;
        }

        return 0;
    }

    // merge nodeName2 to nodeName1
    private void ContractEdge(string nodeName1, string nodeName2)
    {
        // merger node connections to node1's
        //List<string> newLinkVerts = Dict_Vert_LinkedVerts[nodeName2];

        List<string> newLinkVerts = Dict_Vert_LinkedVerts.Where(x => x.Value.Contains(nodeName2)).Select(x => x.Key).ToList();
        //if (t.Count != newLinkVerts.Count)
        //{
        //    throw new Exception();
        //}

        foreach (string newLinkVert in newLinkVerts)
        {
            // remove
            Dict_Vert_LinkedVerts[newLinkVert].RemoveAll(x => x == nodeName2);

            // do not include self loop
            if (newLinkVert != nodeName1)
            {
                Dict_Vert_LinkedVerts[nodeName1].Add(newLinkVert);
                Dict_Vert_LinkedVerts[newLinkVert].Add(nodeName1);
            }
        }

        // remove edges
        Dict_Vert_LinkedVerts.Remove(nodeName2);

        Console.WriteLine($"Merge {nodeName1}-{nodeName2}");
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input2.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        int result = 0;
        for (int i = 0; i < 10; i++)
        {
            Graph graph = new();
            graph.CreateGraph(lines);

            Console.WriteLine($"Node Count = {graph.NodeCount}");
            Console.WriteLine($"Edge Count = {graph.EdgeCount}");
            //foreach (var item in dict_name_node)
            //{
            //    Console.WriteLine($"{item.Key}, conn = {item.Value.Connects.Count}");
            //}

            result = graph.MinCut();

            Console.WriteLine($"Node Count = {graph.NodeCount}");
            Console.WriteLine($"Edge Count = {graph.EdgeCount}");
            //foreach (var item in dict_name_node)
            //{
            //    Console.WriteLine($"{item.Key}, conn = {item.Value.Connects.Count}");
            //}

            Console.WriteLine($"Result = {result}");
        }

        sw.Stop();

        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
    }

    private static List<string> FindCommonConnections(Node node1, Node node2)
    {
        HashSet<string> nodes = [];
        foreach (string node in node1.Connects)
        {
            nodes.Add(node);
        }
        List<string> common = [];

        foreach (var item in node2.Connects)
        {
            if (nodes.Contains(item))
            {
                common.Add(item);
            }
        }
        return common;
    }

    private static void Remove(Dictionary<string, Node> dict_name_node, string v1, string v2)
    {
        dict_name_node[v1].Connects.Remove(v2);
        dict_name_node[v2].Connects.Remove(v1);
    }
}