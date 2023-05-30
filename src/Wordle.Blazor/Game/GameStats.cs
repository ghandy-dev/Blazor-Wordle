using System.Linq;

using Wordle.Blazor.Constants;

namespace Wordle.Blazor.Game;

public class GameStats
{
    public int GamesPlayed { get; set; } = 0;

    public int GamesWon { get; set; } = 0;

    public int WinPercentage { get; set; } = 0;

    public int CurrentStreak { get; set; } = 0;

    public int MaxStreak { get; set; } = 0;

    public int AverageGuesses { get; set; } = 0;

    public DateTime LastPlayed { get; set; } = DateTime.Now;

    public Dictionary<string, int> GuessDistribution { get; set; } =
        Enumerable
            .Range(1, GameRulesConstants.MaxGuesses)
            .Select(x => (x.ToString(), 0))
            .Append(("fail", 0))
            .ToDictionary(
                keySelector => keySelector.Item1, 
                valueSelector => valueSelector.Item2);

    public void UpdateStats(int guessCount, bool correctGuess)
    {
        GamesPlayed++;
        if (correctGuess)
        {
            GamesWon++;

            if ((DateTime.Now - LastPlayed).Days > 1)
            {
                CurrentStreak = 1;
            }
            else
            {
                CurrentStreak++;
            }

            GuessDistribution.TryGetValue(guessCount.ToString(), out int currentCount);
            GuessDistribution[guessCount.ToString()] = currentCount + 1;

            if (CurrentStreak > MaxStreak)
            {
                MaxStreak = CurrentStreak;
            }
        }
        else
        {
            CurrentStreak = 0;
            GuessDistribution.TryGetValue("fail", out int currentCount);
            GuessDistribution["fail"] = currentCount + 1;
        }

        WinPercentage = (int)(GamesWon / (double)GamesPlayed * 100);
        AverageGuesses = (int)GuessDistribution.Average(x =>
            int.TryParse(x.Key, out var guessCount)
                ? guessCount * x.Value
                : (GameRulesConstants.MaxGuesses + 1) * x.Value);
    }
}