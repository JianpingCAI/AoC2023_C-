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
        ModuleSystem moduleSystem = BuildSystem(lines);
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

        // ** warm up
        for (int i = 0; i < 1000; i++)
        {
            Console.WriteLine($"Warm-up #{i + 1}");

            SystemPulseCount counts = moduleSystem.RunWarmup();

            int systemCode = moduleSystem.GetSystemState();

            // check if find a duplicated state
            if (!systemCodes.Contains(systemCode))
            {
                ++cycleCount;
                cycle_lowPulses += counts.Low;
                cycle_highPulses += counts.High;

                systemCodes.Add(systemCode);
            }
            else
            {
                break;
            }

            Console.WriteLine($"Low = {counts.Low}, High = {counts.High}");
            Console.WriteLine();
        }

        Console.WriteLine($"Cycle = {cycleCount}: Low = {cycle_lowPulses}, High ={cycle_highPulses}");

        int totalCycles = 1000 / cycleCount;
        int remainCycles = 1000 % cycleCount;

        result += (cycle_lowPulses * cycle_highPulses) * (totalCycles) * (totalCycles);

        if (remainCycles > 0)
        {
            Console.WriteLine("Some more");
        }

        sw.Stop();
        // right answer - 791120136
        Console.WriteLine($"Result = {result}");
        Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
        return;

        //HashSet<int> systemCodes = [];
        //int countLow = 0;
        //int countHigh = 0;
        //int cycleCount = 0;
        //// ** warm up
        //int i = 0;
        ////for (int i = 0; i < 1000; i++)
        //while (true)
        //{
        //    ++i;

        //    if (i % 10000 == 0)
        //    {
        //        Console.WriteLine($"Warm-up #{i + 1}");
        //    }

        //    SystemPulseCount counts = moduleSystem.RunWarmup(Pulse.Low);

        //    //int systemCode = moduleSystem.GetSystemState();
        //    //if (!systemCodes.Contains(systemCode))
        //    //{
        //    //    ++cycleCount;
        //    //    countLow += counts.Low;
        //    //    countHigh += counts.High;

        //    //    systemCodes.Add(systemCode);
        //    //}
        //    //else
        //    //{
        //    //    break;
        //    //}

        //    if (rxModule.State == State.OFF)
        //    {
        //        break;
        //    }
        //    //Console.WriteLine($"System State = {systemCode}");
        //    //Console.WriteLine($"Low = {counts.Low}, High = {counts.High}");
        //    //Console.WriteLine();
        //}

        //Console.WriteLine($"Cycle = {cycleCount}: Low = {countLow}, High ={countHigh}");

        //result = i;

        //sw.Stop();
        //// too low - 33820
        ////63001
        //Console.WriteLine($"Result = {result}");
        //Console.WriteLine($"Time = {sw.Elapsed.TotalSeconds} seconds");
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

    private static ModuleSystem BuildSystem(string[] lines)
    {
        Dictionary<string, IModule> dict_name_module = [];

        // create all the modules (exclude Output modules )
        foreach (string line in lines)
        {
            string[] tmp1 = line.Split("->", StringSplitOptions.TrimEntries).ToArray();

            string str_module = tmp1[0];

            if (str_module.StartsWith('%'))
            {
                string name = str_module.Substring(1);

                IModule module = new FlipFlop
                {
                    Name = name
                };

                dict_name_module.Add(name, module);
            }
            else if (str_module.StartsWith('&'))
            {
                string name = str_module.Substring(1);

                IModule module = new Conjunction
                {
                    Name = name
                };
                dict_name_module.Add(name, module);
            }
            else if (str_module == "broadcaster")
            {
                //broadcaster

                string name = "broadcaster";
                IModule module = new Broadcaster
                {
                    Name = name
                };

                dict_name_module.Add(name, module);
            }
        }

        // build the connections
        int test_outputModuleCount = 0;
        foreach (string line in lines)
        {
            string[] tmp1 = line.Split("->", StringSplitOptions.TrimEntries).ToArray();

            string str_module = tmp1[0];
            string str_connections = tmp1[1];

            string[] nextNames = str_connections.Split(',', StringSplitOptions.TrimEntries).ToArray();

            // ** get the current module
            IModule curModule;
            if (str_module.StartsWith('%') || str_module.StartsWith('&'))
            {
                string name = str_module.Substring(1);

                curModule = dict_name_module[name];
            }
            else if (str_module == "broadcaster")
            {
                //broadcaster
                string name = "broadcaster";
                curModule = dict_name_module[name];
            }
            else
            {
                curModule = dict_name_module[str_module];
            }

            // add the connection
            foreach (string nextName in nextNames)
            {
                if (!dict_name_module.TryGetValue(nextName, out IModule? value))
                {
                    value = new OutputModule
                    {
                        Name = nextName
                    };
                    // an output module found
                    dict_name_module[nextName] = value;

                    ++test_outputModuleCount;
                }

                curModule.NextModules.Add(nextName, value);
                value.InputModules.Add(curModule.Name, curModule);
            }
        }

        Console.WriteLine($"Output module count: {test_outputModuleCount}");
        return new ModuleSystem
        {
            Broadcaster = dict_name_module["broadcaster"]
        };
    }
}