using System.Reactive.Linq;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using Wordle.Blazor.Constants;
using Wordle.Blazor.Game;
using Wordle.Blazor.Services;
using Wordle.Blazor.Utilities;

namespace Wordle.Blazor.Components;

public sealed partial class CurrentRow : IDisposable
{
    [Inject] public required IKeyboardService KeyboardService { get; set; }

    [Inject] public required GameService GameService { get; set; }

    private readonly string[] _guess = new string[GameRulesConstants.WordLength];
    private string _animation = "";
    private int _currentIndex;
    private TimeoutTimer? _timer;

    private IDisposable? _timerSubscription;
    private IDisposable? _keyboardEnterSubscription;
    private IDisposable? _keyboardDeleteLetterSubscription;
    private IDisposable? _keyboardAddLetterSubscription;
    private IDisposable? _gameEventSubscription;

    public void Dispose()
    {
        _timerSubscription?.Dispose();
        _keyboardEnterSubscription?.Dispose();
        _keyboardDeleteLetterSubscription?.Dispose();
        _keyboardEnterSubscription?.Dispose();
        _gameEventSubscription?.Dispose();
        _timer?.Dispose();
    }

    protected override void OnInitialized()
    {
        _timer = new(TimerConstants.RowShakeDuration);
        _timerSubscription = _timer.Elapsed.Subscribe(OnTimeoutTimerElapsed);

        _keyboardEnterSubscription = KeyboardService.OnKeyDown
            .Where(e => string.Compare(e.Key, "enter", StringComparison.OrdinalIgnoreCase) == 0 && !e.Repeat)
            .Subscribe(_ => GameService.Game.MakeGuess(string.Join("", _guess)));

        _keyboardDeleteLetterSubscription = KeyboardService.OnKeyDown
            .Where(e => string.Compare(e.Key, "backspace", StringComparison.OrdinalIgnoreCase) == 0 && !e.Repeat)
            .Subscribe(_ => DeleteLetter());

        _keyboardAddLetterSubscription = KeyboardService.OnKeyDown.Subscribe(OnKeyDown);

        _gameEventSubscription = GameService.Game.GameEventStream
            .Where(e => e.GameEventType == GameEventType.InvalidGuess)
            .Subscribe(OnInvalidGuess);
    }

    private void OnKeyDown(KeyboardEventArgs args)
    {
        if (args.Repeat)
        {
            return;
        }

        var pattern = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var key = args.Key.ToUpper();

        if (pattern.Contains(key))
        {
            AddLetter(key);
        }
    }

    private void DeleteLetter()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            _guess[_currentIndex] = "";
            StateHasChanged();
        }
    }

    private void AddLetter(string letter)
    {
        if (_currentIndex < GameRulesConstants.WordLength)
        {
            _guess[_currentIndex] = letter;
            _currentIndex++;
            StateHasChanged();
        }
    }

    private void OnTimeoutTimerElapsed(EventArgs args)
    {
        _animation = "";
        StateHasChanged();
    }

    private void OnInvalidGuess(GameEvent gameEvent)
    {
        _animation = "shake";
        _timer?.Start();
        StateHasChanged();
    }
}