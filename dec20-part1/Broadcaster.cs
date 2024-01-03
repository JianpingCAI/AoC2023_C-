public class Broadcaster : BaseModule, IModule
{
    public Pulse OutPulse { get; set; } = Pulse.Low;

    public State State { get; set; }

    public Type Type => Type.Broadcaster;

    public char Label => '.';

    public Pulse Process(IModule prevModule, Pulse inputPulse)
    {
        if (IsPrint)
        {
            Console.WriteLine($"button {inputPulse} -> {this.Name}");
        }

        return Pulse.Low;
    }
}