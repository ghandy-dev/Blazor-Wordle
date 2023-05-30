namespace Wordle;

public class GameEvent
{
    public required GameEventType GameEventType { get; init; }

    public required object? Data { get; init; }
}