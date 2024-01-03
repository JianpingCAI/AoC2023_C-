using System.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        string filePath = "input.txt";
        string[] lines = File.ReadAllLines(filePath);
        Stopwatch sw = Stopwatch.StartNew();

        long result = 0;
        bool isPrint = true;
        if (filePath == "input.txt")
        {
            isPrint = false;
        }

        // ** build
        ModuleSystem moduleSystem = new();
        moduleSystem.BuildSystem(lines);

        if (isPrint)
        {
            moduleSystem.Print();
            Console.WriteLine();
        }
        //moduleSystem.Print();

        //CheckPath(rxModule);

        HashSet<int> systemCodes = [];
        int cycle_lowPulses = 0;
        int cycle_highPulses = 0;
        int cycleCount = 0;

        #region Part2 Data specific solution

        // rx
        IModule rxModule = moduleSystem.GetModuleByName("rx");

        IModule rxInputModule = rxModule.InputModules.FirstOrDefault().Value;

        var targetModuleNames = rxInputModule.InputModules.Keys;

        Dictionary<string, long> dict_name_outLow = [];
        foreach (string name in targetModuleNames)
        {
            dict_name_outLow.Add(name, 0);
        }

        #endregion Part2 Data specific solution

        // ** warm up

        long counts = moduleSystem.RunWarmups(dict_name_outLow);

        result = counts;

        sw.Stop();
        // right answer - 791120136
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
        return;
    }

    private static void CheckPath(IModule rxModule)
    {
        List<IModule> modules = [];
        HashSet<string> namesSet = [];

        Queue<IModule> queue = new();
        queue.Enqueue(rxModule);
        while (queue.Count > 0)
        {
            IModule curM = queue.Dequeue();
            if (namesSet.Contains(curM.Name))
            {
                if (curM.Type == Type.Broadcaster)
                {
                    Console.WriteLine("Broadcaster found.");
                }
                continue;
            }
            namesSet.Add(curM.Name);

            modules.Add(curM);

            foreach (IModule prevM in curM.InputModules.Values)
            {
                modules.Insert(0, prevM);
                queue.Enqueue(prevM);
            }
        }

        IModule firstModule = modules.First();

        Console.WriteLine(firstModule.Name);
        Console.WriteLine($"Related Modules {namesSet.Count}");
    }
}