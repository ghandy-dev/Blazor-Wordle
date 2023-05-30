using System.Reactive.Linq;

using Microsoft.AspNetCore.Components;

using Wordle.Blazor.Constants;
using Wordle.Blazor.Game;
using Wordle.Blazor.Services;
using Wordle.Blazor.Utilities;

namespace Wordle.Blazor.Pages;

public sealed partial class Index : IDisposable
{
    [Inject] public required IGameStorageService StorageService { get; set; }

    [Inject] public required GameService GameService { get; set; }

    [Inject] public required INotificationService NotificationService { get; set; }

    [Inject] public required HttpClient HttpClient { get; set; }

    private TimeoutTimer _displayStatsTimer = null!;
    private bool _displayModal;
    private bool _hardMode = false;
    private bool _darkMode = true;
    private bool _loaded = false;

    private IDisposable? _timerSubscription;
    private IDisposable? _gameEventSubscription;

    private bool GameCompleted => GameService.Game.Status == GameStatus.Win || GameService.Game.Status == GameStatus.Loss;

    public void Dispose()
    {
        _gameEventSubscription?.Dispose();
        _timerSubscription?.Dispose();
        _displayStatsTimer?.Dispose();
    }

    protected override async Task OnInitializedAsync()
    {
        await GameService.InitializeAsync();
        await LoadThemeAsync();

        _gameEventSubscription = GameService.Game.GameEventStream.Where(e => e.GameEventType == GameEventType.GameEnded).Subscribe(OnGameEnded);

        _hardMode = GameService.State.HardMode;

        _displayStatsTimer = new TimeoutTimer(TimerConstants.StatsShortRevealDuration);
        _timerSubscription = _displayStatsTimer.Elapsed.Subscribe(OnTimerElapsed);

        if (GameService.Game.Status == GameStatus.Win || GameService.Game.Status == GameStatus.Loss)
        {
            _displayStatsTimer.Start();
        }

        _loaded = true;
    }

    private async Task LoadThemeAsync()
    {
        if (await StorageService.ContainsKey(LocalStorageConstants.DarkModeKey))
        {
            _darkMode = await StorageService.LoadTheme();
        }
        else
        {
            await StorageService.UpdateTheme(_darkMode);
        }
    }

    private void OnTimerElapsed(EventArgs args)
    {
        DisplayStatsModal();
        StateHasChanged();
    }

    private void OnGameEnded(GameEvent gameEvent)
    {
        _displayStatsTimer.Interval = TimerConstants.StatsRevealDuration;
        _displayStatsTimer.Start();
    }

    private async Task OnHardModeChangedAsync(bool enabled)
    {
        if (GameService.Guesses.Count > 0 && enabled == true)
        {
            await NotificationService.RaiseNotification("Hard mode can only be enabled at the start of a round");
        }
        else
        {
            _hardMode = enabled;
            if (GameService is not null && GameService.State is not null)
            {
                GameService.State.HardMode = enabled;
                GameService.Game!.HardMode = enabled;
                await StorageService.SaveGameState(GameService.State);
            }
        }
    }

    private async Task OnDarkModeChangedAsync(bool enabled)
    {
        _darkMode = enabled;
        await StorageService.UpdateTheme(enabled);
    }
}