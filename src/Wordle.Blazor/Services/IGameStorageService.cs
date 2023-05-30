using Wordle.Blazor.Game;

namespace Wordle.Blazor.Services;

public interface IGameStorageService
{
    ValueTask<bool> ContainsKey(string key);
    ValueTask<GameState> LoadGameState();
    ValueTask<GameStats> LoadStats();
    ValueTask<bool> LoadTheme();
    ValueTask SaveGameState(GameState gameState);
    ValueTask SaveStats(GameStats stats);
    ValueTask UpdateTheme(bool darkMode);
}
