using System.Reactive.Linq;

using Wordle.Blazor.Constants;
using Wordle.Blazor.Enums;
using Wordle.Blazor.Services;
using Wordle.Providers;

namespace Wordle.Blazor.Game;

public sealed class GameService : IDisposable
{
    public WordleGame Game { get; set; }

    public GameState State { get; private set; } = new();

    public GameStats Stats { get; private set; } = new();

    public int CurrentRowIndex { get; private set; } = 0;

    public List<GuessModel> Guesses => Game.Guesses;

    private readonly INotificationService _notificationService;
    private readonly IGameStorageService _storageService;
    private readonly IDisposable? _gameStateSubscription;

    public GameService(
        INotificationService notificationService,
        IGameStorageService localStorage,
        IWordProvider  wordProvider)
    {
        _notificationService = notificationService;
        _storageService = localStorage;
        Game = new WordleGame(GameRulesConstants.MaxGuesses, wordProvider);
        _gameStateSubscription = Game.GameEventStream.Subscribe(async e => await OnGameEventAsync(e));
    }

    public void Dispose()
    {
        _gameStateSubscription?.Dispose();
    }

    public async Task InitializeAsync()
    {
        await LoadGameStatsAsync();
        await LoadGameStateAsync();
    }

    private async Task LoadGameStatsAsync()
    {
        if (await _storageService.ContainsKey(LocalStorageConstants.GameStatsKey))
        {
            Stats = await _storageService.LoadStats() ?? new GameStats();
        }
        else
        {
            await _storageService.SaveStats(Stats);
        }
    }

    private async Task LoadGameStateAsync()
    {
        if (await _storageService.ContainsKey(LocalStorageConstants.GameStateKey))
        {
            State = await _storageService.LoadGameState();
        }

        if (DateOnly.TryParse(State.LastPlayed, out var lastPlayed)
            && lastPlayed.CompareTo(DateOnly.FromDateTime(DateTime.Now)) != 0)
        {
            State = new GameState();
        }
        else
        {
            Game.LoadState(State.Guesses);
            Game.HardMode = State.HardMode;
            CurrentRowIndex = State.RowIndex;
        }
    }

    private async Task OnGameEventAsync(GameEvent gameEvent)
    {
        switch (gameEvent.GameEventType)
        {
            case GameEventType.GameEnded:
                if (Game.Status == GameStatus.Win)
                {
                    await UpdateStatsAsync(true);
                    await _notificationService.RaiseNotification(MessageConstants.GetSuccessMessage(Game.GuessCount));
                }
                else if (Game.Status == GameStatus.Loss)
                {
                    await UpdateStatsAsync(false);
                    await _notificationService.RaiseNotification(Game.Word, DurationType.Infinite);
                }
                break;
            case GameEventType.InvalidGuess:
                await _notificationService.RaiseNotification((string)gameEvent.Data!);
                break;
            case GameEventType.GuessMade:
                CurrentRowIndex++;
                break;
        }

        await UpdateStateAsync();
    }

    private async Task UpdateStateAsync()
    {
        State.Guesses = Game!.Guesses;
        State.RowIndex = CurrentRowIndex;
        State.LastPlayed = DateOnly.FromDateTime(DateTime.Now).ToShortDateString();
        State.Status = Game.Status;
        await _storageService.SaveGameState(State);
    }

    private async Task UpdateStatsAsync(bool correctGuess)
    {
        Stats.UpdateStats(Game!.GuessCount, correctGuess);
        await _storageService.SaveStats(Stats);
    }
}