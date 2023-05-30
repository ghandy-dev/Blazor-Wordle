using Wordle.Blazor.Enums;

namespace Wordle.Blazor.Services;

public class NotificationEventArgs
{
    public required string Message { get; init; }

    public required DurationType DurationType { get; init; }
}
