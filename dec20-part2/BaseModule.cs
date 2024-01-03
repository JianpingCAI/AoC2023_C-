public enum State
{
    ON,
    OFF
};

public enum Pulse
{
    High,
    Low,
    None
};

public enum Type
{
    Broadcaster,
    FlipFlop,
    Conjunction,
    Output
}

public record SystemPulseCount(int Low, int High);

public class BaseModule
{
    public bool IsPrint { get; set; } = false;

    public string Name { get; set; } = string.Empty;

    public Dictionary<string, IModule> InputModules { get; set; } = [];
    public Dictionary<string, IModule> NextModules { get; set; } = [];
}