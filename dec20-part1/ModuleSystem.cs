public class ModuleSystem
{
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
            else if (curModule.Type == Type.Conjunction)
            {
                //hash.Add((curModule as Conjunction).LastRemember);
            }

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
}