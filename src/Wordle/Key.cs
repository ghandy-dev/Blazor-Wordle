namespace Wordle;

public class Key
{
    public char Letter { get; }

    public LetterState State { get; set; }

    public Key(char letter) 
    {
        Letter = letter;
        State = LetterState.None;
    }

    public static implicit operator Key(char s) => new(s);
}