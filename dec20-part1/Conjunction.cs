public class Conjunction : BaseModule, IModule
{
    public Pulse OutPulse { get; set; } = Pulse.None;

    public Type Type => Type.Conjunction;

    public State State { get; set; } = State.OFF; //not all high inputs

    private Dictionary<string, Pulse> _inputModulePulses = [];

    public char Label => '&';

    private void CheckState()
    {
        bool isAllHighInputs = _inputModulePulses.Values.All(x => x != Pulse.Low);

        if (_inputModulePulses.Values.Any(x => x == Pulse.None))
        {
            throw new Exception();
        }

        if (isAllHighInputs)
        {
            // all high inputs
            State = State.ON;
        }
        else
        {
            State = State.OFF;
        }
    }

    public Pulse Process(IModule inputModule, Pulse inputPulse)
    {
        if (_inputModulePulses.Count == 0)
        {
            foreach (var inModule in InputModules.Values)
            {
                _inputModulePulses.Add(inModule.Name, Pulse.Low);
            }
        }

        _inputModulePulses[inputModule.Name] = inputPulse;

        GenerateOutput();

        if (IsPrint)
        {
            Console.WriteLine($"{inputModule.Label}{inputModule.Name} -{inputPulse}-> {this.Label}{this.Name}");
        }
        return OutPulse;
    }

    private void GenerateOutput()
    {
        CheckState();

        switch (State)
        {
            case State.ON:
                OutPulse = Pulse.Low;
                break;

            case State.OFF:
                OutPulse = Pulse.High;
                break;
        }
    }
}