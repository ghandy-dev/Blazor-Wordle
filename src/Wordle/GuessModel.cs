namespace Wordle;

public class GuessModel
{
    public required string GuessedWord { get; init; }

    public required List<LetterState> LetterStates { get; init; }
}