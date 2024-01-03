public interface IModule
{
    public Type Type { get; }
    public State State { get; set; }
    public Pulse OutPulse { get; set; }

    public Dictionary<string, IModule> InputModules { get; set; }
    public Dictionary<string, IModule> NextModules { get; set; }
    string Name { get; set; }

    Pulse Process(IModule prevModule, Pulse inputPulse);

    //public Queue<Pulse> OutQueue { get; set; }

    public char Label { get; }
}