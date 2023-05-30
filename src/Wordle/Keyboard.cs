namespace Wordle;

public class Keyboard
{
    public Key[][] Rows { get; } = new Key[][]
    {
        new Key[] { 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P' },
        new Key[] { 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L' },
        new Key[] { 'Z', 'X', 'C', 'V', 'B', 'N', 'M' },
    };

    public Key this[char letter]
    {
        get => Rows.SelectMany(r => r).Single(k => k.Letter == letter);
    }
}