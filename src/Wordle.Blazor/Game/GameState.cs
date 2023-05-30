namespace Wordle.Blazor.Game;

public class GameState
{
    public List<GuessModel> Guesses { get; set; } = new();

    public int RowIndex { get; set; } = 0;

    public GameStatus Status { get; set; } = GameStatus.InProgress;

    public bool HardMode { get; set; } = false;

    public string LastPlayed { get; set; } = "";
}