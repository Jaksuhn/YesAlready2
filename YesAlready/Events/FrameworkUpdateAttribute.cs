namespace YesAlready.Events;

public class FrameworkUpdateAttribute : EventAttribute
{
    public uint NthTick { get; init; } = 0;
}
