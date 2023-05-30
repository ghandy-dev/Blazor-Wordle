using Blazored.LocalStorage;

using Wordle.Blazor.Constants;
using Wordle.Blazor.Game;

namespace Wordle.Blazor.Services;

public class GameStorageService : IGameStorageService
{
    private readonly ILocalStorageService _localStorage;

    public GameStorageService(ILocalStorageService localStorageService)
    {
        _localStorage = localStorageService;
    }

    public async ValueTask<bool> ContainsKey(string key) =>
        await _localStorage.ContainKeyAsync(key);

    public async ValueTask SaveStats(GameStats stats) =>
        await _localStorage.SetItemAsync(LocalStorageConstants.GameStatsKey, stats);

    public async ValueTask<GameStats> LoadStats() =>
        await _localStorage.GetItemAsync<GameStats>(LocalStorageConstants.GameStatsKey);

    public async ValueTask<GameState> LoadGameState() =>
        await _localStorage.GetItemAsync<GameState>(LocalStorageConstants.GameStateKey);

    public async ValueTask SaveGameState(GameState gameState) =>
        await _localStorage.SetItemAsync(LocalStorageConstants.GameStateKey, gameState);

    public async ValueTask<bool> LoadTheme() =>
        await _localStorage.GetItemAsync<bool>(LocalStorageConstants.DarkModeKey);

    public async ValueTask UpdateTheme(bool darkMode) =>
        await _localStorage.SetItemAsync(LocalStorageConstants.DarkModeKey, darkMode);
}