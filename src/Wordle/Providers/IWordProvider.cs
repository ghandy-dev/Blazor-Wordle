namespace Wordle.Providers;

public interface IWordProvider
{
    bool CheckWord(string word);

    string GetTodaysWordle();

    string GetPreviousWordle(DateOnly date);
}