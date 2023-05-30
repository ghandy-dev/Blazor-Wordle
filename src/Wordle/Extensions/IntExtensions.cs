namespace Wordle.Extensions;

public static class IntExtensions
{
    public static string ToOrdinalString(this int n)
    {
        return n switch
        {
            <= 0 => n.ToString(),
            _ => (n % 100) switch
            {
                11 or 12 or 13 => n + "th",
                _ => (n % 10) switch
                {
                    1 => n + "st",
                    2 => n + "nd",
                    3 => n + "rd",
                    _ => n + "th",
                },
            }
        };
    }
}