using AocMath;

public class ModuleSystem
{
    private Dictionary<string, IModule> _dict_name_module = [];

    public IModule Broadcaster { get; set; } = null;

    public int GetSystemState()
    {
        HashSet<string> moduleNames = [];

        HashCode hash = new();

        Queue<IModule> que = new();
        foreach (IModule item in Broadcaster.NextModules.Values)
        {
            moduleNames.Add(item.Name);
            que.Enqueue(item);
        }

        while (que.Count > 0)
        {
            IModule curModule = que.Dequeue();
            if (curModule.Type == Type.Output)
            {
                continue;
            }

            if (curModule.Type == Type.FlipFlop)
            {
                hash.Add(curModule.State);
            }
            //else if (curModule.Type == Type.Conjunction)
            //{
            //    //hash.Add((curModule as Conjunction).LastRemember);
            //}

            foreach (IModule nextModule in curModule.NextModules.Values)
            {
                if (!moduleNames.Contains(nextModule.Name))
                {
                    moduleNames.Add(nextModule.Name);
                    que.Enqueue(nextModule);
                }
            }
        }

        return hash.ToHashCode();
    }

    public SystemPulseCount RunWarmup()
    {
        int countLowPulse = 1; //botton-low
        int countHighPulse = 0;

        Queue<Tuple<IModule?, Pulse, IModule>> que = new();
        que.Enqueue(new Tuple<IModule?, Pulse, IModule>(null, Pulse.Low, this.Broadcaster));

        // breadth first
        while (que.Count > 0)
        {
            Tuple<IModule?, Pulse, IModule> queItem = que.Dequeue();
            IModule? curInputModule = queItem.Item1;
            Pulse curInputPulse = queItem.Item2;
            IModule curModule = queItem.Item3;

            // process the input module
            Pulse curOutPulse = curModule.Process(curInputModule, curInputPulse);

            if (curOutPulse == Pulse.None
                && curModule.Type == Type.FlipFlop)
            {
                continue;
            }

            if (curOutPulse == Pulse.None
                && curModule.Type != Type.FlipFlop)
            {
                throw new Exception();
            }

            // next modules
            foreach (IModule nextModule in curModule.NextModules.Values)
            {
                switch (curOutPulse)
                {
                    case Pulse.Low:
                        ++countLowPulse;
                        break;

                    case Pulse.High:
                        ++countHighPulse;
                        break;
                }

                que.Enqueue(new Tuple<IModule?, Pulse, IModule>(curModule, curOutPulse, nextModule));
            }
        }

        return new SystemPulseCount(countLowPulse, countHighPulse);
    }

    internal long RunWarmups(Dictionary<string, long> dict_name_outLow)
    {
        long countWarmups = 0;
        while (true)
        {
            ++countWarmups;
            Console.WriteLine($"Warmup #{countWarmups}");

            Queue<Tuple<IModule?, Pulse, IModule>> que = new();
            que.Enqueue(new Tuple<IModule?, Pulse, IModule>(null, Pulse.Low, this.Broadcaster));

            // breadth first
            while (que.Count > 0)
            {
                Tuple<IModule?, Pulse, IModule> queItem = que.Dequeue();
                IModule? curInputModule = queItem.Item1;
                Pulse curInputPulse = queItem.Item2;
                IModule curModule = queItem.Item3;

                // process the input module
                Pulse curOutPulse = curModule.Process(curInputModule, curInputPulse);

                if (curOutPulse == Pulse.None
                    && curModule.Type == Type.FlipFlop)
                {
                    continue;
                }

                // part2 - data specific solution
                if (dict_name_outLow.ContainsKey(curModule.Name))
                {
                    if (curOutPulse == Pulse.High)
                    {
                        dict_name_outLow[curModule.Name] = countWarmups;
                    }
                }

                // next modules
                foreach (IModule nextModule in curModule.NextModules.Values)
                {
                    que.Enqueue(new Tuple<IModule?, Pulse, IModule>(curModule, curOutPulse, nextModule));
                }
            }

            // part2 - data specific solution
            if (dict_name_outLow.Values.All(x => x > 0))
            {
                break;
            }
        }

        long[] values = dict_name_outLow.Values.ToArray();

        // part2 - data specific solution
        long lcmValue = new LCMSolver<long>().LCM(values);

        return lcmValue;
    }

    public void Print()
    {
        HashSet<string> startNames = [];

        Queue<IModule> que = new();

        que.Enqueue(this.Broadcaster);
        startNames.Add(this.Broadcaster.Name);

        while (que.Count > 0)
        {
            IModule curModule = que.Dequeue();
            if (curModule.Type == Type.Output)
            {
                continue;
            }

            string curTypeStr = string.Empty;
            switch (curModule.Type)
            {
                case Type.Broadcaster:
                    break;

                case Type.FlipFlop:
                    curTypeStr = "%";
                    break;

                case Type.Conjunction:
                    curTypeStr = "&";
                    break;

                case Type.Output:
                    break;

                default:
                    break;
            }

            Console.Write($"{curTypeStr}{curModule.Name} -> ");

            foreach (IModule next in curModule.NextModules.Values)
            {
                Console.Write($"{next.Name}, ");

                if (!startNames.Contains(next.Name))
                {
                    que.Enqueue(next);
                    startNames.Add(next.Name);
                }
            }

            Console.WriteLine();
        }
    }

    internal void BuildSystem(string[] lines)
    {
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

                _dict_name_module.Add(name, module);
            }
            else if (str_module.StartsWith('&'))
            {
                string name = str_module.Substring(1);

                IModule module = new Conjunction
                {
                    Name = name
                };
                _dict_name_module.Add(name, module);
            }
            else if (str_module == "broadcaster")
            {
                //broadcaster

                string name = "broadcaster";
                IModule module = new Broadcaster
                {
                    Name = name
                };

                _dict_name_module.Add(name, module);
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

                curModule = _dict_name_module[name];
            }
            else if (str_module == "broadcaster")
            {
                //broadcaster
                string name = "broadcaster";
                curModule = _dict_name_module[name];
            }
            else
            {
                curModule = _dict_name_module[str_module];
            }

            // add the connection
            foreach (string nextName in nextNames)
            {
                if (!_dict_name_module.TryGetValue(nextName, out IModule? value))
                {
                    value = new OutputModule
                    {
                        Name = nextName
                    };
                    // an output module found
                    _dict_name_module[nextName] = value;

                    ++test_outputModuleCount;
                }

                curModule.NextModules.Add(nextName, value);
                value.InputModules.Add(curModule.Name, curModule);
            }
        }

        Console.WriteLine($"Output module count: {test_outputModuleCount}");
        Broadcaster = _dict_name_module["broadcaster"];
    }

    internal IModule GetModuleByName(string name)
    {
        if (!_dict_name_module.TryGetValue(name, out IModule module))
        {
            throw new Exception($"{name} does not exist");
        }

        return module;
    }
}