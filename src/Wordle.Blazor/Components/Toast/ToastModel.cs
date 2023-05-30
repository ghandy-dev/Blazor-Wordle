using Wordle.Blazor.Enums;

namespace Wordle.Blazor.Components;

public class ToastModel
{
    public required string Key { get; init; }
    public required string Message { get; init; }
    public required DurationType DurationType { get; init; }
}