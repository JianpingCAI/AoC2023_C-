public class OutputModule : BaseModule, IModule
{
    public Pulse OutPulse { get; set; } = Pulse.None;

    public Type Type => Type.Output;

    public State State { get; set; } = State.ON;

    public char Label => '@';

    public Pulse Process(IModule prevModule, Pulse inputPulse)
    {
        if (IsPrint)
        {
            Console.WriteLine($"{prevModule.Label}{prevModule.Name} -{prevModule.OutPulse}-> @{this.Name}");
        }

        if (prevModule.OutPulse == Pulse.Low)
        {
            this.State = State.OFF;
        }

        return prevModule.OutPulse;
    }

    public Pulse ProcessInput_DF(IModule prevModule)
    {
        return Pulse.None;
    }
}