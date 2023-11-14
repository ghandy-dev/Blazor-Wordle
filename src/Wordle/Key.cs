namespace Wordle;

public class Key(char letter)
{
    public char Letter { get; } = letter;

    public LetterState State { get; set; } = LetterState.None;

    public static implicit operator Key(char s) => new(s);
}