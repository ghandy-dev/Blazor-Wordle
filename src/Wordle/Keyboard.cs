namespace Wordle;

public class Keyboard
{
    public Key[][] Rows { get; } =
    [
        ['Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P'],
        ['A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L' ],
        [ 'Z', 'X', 'C', 'V', 'B', 'N', 'M' ],
    ];

    public Key this[char letter]
    {
        get => Rows.SelectMany(r => r).Single(k => k.Letter == letter);
    }
}