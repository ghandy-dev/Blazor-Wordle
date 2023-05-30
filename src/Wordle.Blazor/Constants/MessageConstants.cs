namespace Wordle.Blazor.Constants;

public static class MessageConstants
{
    public static string GetSuccessMessage(int guessCount)
        => SuccessMessages[guessCount];

    private static readonly Dictionary<int, string> SuccessMessages = new()
    {
        { 1, "Genius" },
        { 2, "Magnificent" },
        { 3, "Impressive" },
        { 4, "Splendid" },
        { 5, "Great" },
        { 6, "Phew" },
    };
}