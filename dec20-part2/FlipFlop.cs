public class FlipFlop : BaseModule, IModule
{
    public Pulse OutPulse { get; set; } = Pulse.Low;

    public State State { get; set; } = State.OFF;

    public Type Type => Type.FlipFlop;

    public char Label => '%';

    public Pulse Process(IModule prevModule, Pulse inputPulse)
    {
        GenerateOutput(inputPulse);

        if (IsPrint)
        {
            Console.WriteLine($"{prevModule.Label}{prevModule.Name} -{inputPulse}-> {this.Label}{this.Name}");
        }

        return OutPulse;
    }

    private void GenerateOutput(Pulse inputPulse)
    {
        switch (inputPulse)
        {
            case Pulse.High:
                {
                    OutPulse = Pulse.None;
                }
                break;

            case Pulse.Low:
                {
                    State = (State == State.ON) ? State.OFF : State.ON;

                    // ON
                    switch (State)
                    {
                        case State.ON:
                            OutPulse = Pulse.High;
                            break;

                        case State.OFF:
                            OutPulse = Pulse.Low;
                            break;
                    }
                }
                break;

            case Pulse.None:
                throw new Exception($"Should not receive {Pulse.None}");
        }
    }
}