using Microsoft.AspNetCore.Components;

using Wordle.Blazor.Constants;
using Wordle.Blazor.Utilities;

namespace Wordle.Blazor.Components;

public sealed partial class Tile : IDisposable
{
    [Parameter] public bool CurrentTile { get; set; }

    [Parameter] public string? Letter { get; set; }

    [Parameter] public LetterState State { get; set; }

    [Parameter] public double RevealAnimationDelay { get; set; } = TimerConstants.TileRevealDelayFactor;

    [Parameter] public double BounceAnimationDelay { get; set; } = TimerConstants.TileBounceAnimationDelay;

    [Parameter] public bool Reveal { get; set; }

    [Parameter] public bool Pop { get; set; }

    [Parameter] public bool Bounce { get; set; }

    private TimeoutTimer? _timerReveal;
    private TimeoutTimer? _timerFlipOut;
    private TimeoutTimer? _timerPop;
    private TimeoutTimer? _timerBounce;
    private IDisposable? _timerRevealedSubscription;
    private IDisposable? _timerFlipOutSubscription;
    private IDisposable? _timerPopSubscription;
    private IDisposable? _timerBounceSubscription;
    private string _dataState = "empty";
    private string _dataAnimation = "";
    private bool _popped;

    public void Dispose()
    {
        _timerReveal?.Dispose();
        _timerRevealedSubscription?.Dispose();
        _timerFlipOut?.Dispose();
        _timerFlipOutSubscription?.Dispose();
        _timerPop?.Dispose();
        _timerPopSubscription?.Dispose();
        _timerBounce?.Dispose();
        _timerBounceSubscription?.Dispose();
    }

    protected override void OnInitialized()
    {
        _timerReveal = new(RevealAnimationDelay);
        _timerRevealedSubscription = _timerReveal.Elapsed.Subscribe(OnTimerRevealTimerElapsed);

        _timerFlipOut = new(TimerConstants.TileFlipOutDuration);
        _timerFlipOutSubscription = _timerFlipOut.Elapsed.Subscribe(OnTimerFlipOutTimerElapsed);

        _timerPop = new(TimerConstants.TilePopDuration);
        _timerPopSubscription = _timerPop.Elapsed.Subscribe(OnTimerPopTimerElapsed);

        _timerBounce = new(BounceAnimationDelay);
        _timerBounceSubscription = _timerBounce.Elapsed.Subscribe(OnTimerBounceTimerElapsed);

        SetDataState("tbd");

        switch (State)
        {
            case LetterState.Absent:
            case LetterState.Present:
            case LetterState.Correct:
                if (Reveal)
                {
                    _timerReveal.Start();
                    SetDataState("tbd");
                    if (Bounce)
                    {
                        _timerBounce.Start();
                    }
                }

                break;
        }
    }

    protected override Task OnParametersSetAsync()
    {
        if (string.IsNullOrWhiteSpace(Letter))
        {
            _popped = false;
            SetDataState("empty");
        }

        if (State == LetterState.None)
        {
            if (Pop && !_popped)
            {
                _timerPop?.Start();
                SetDataAnimation("pop");
                SetDataState("tbd");
            }
        }

        return base.OnParametersSetAsync();
    }

    private void OnTimerRevealTimerElapsed(EventArgs args)
    {
        _timerFlipOut?.Start();
        SetDataAnimation("flip-in");
    }

    private void OnTimerFlipOutTimerElapsed(EventArgs args)
    {
        SetDataState(
            State switch
            {
                LetterState.Absent => "absent",
                LetterState.Present => "present",
                LetterState.Correct => "correct",
                _ => ""
            });

        SetDataAnimation("flip-out");
    }

    private void OnTimerPopTimerElapsed(EventArgs args)
    {
        SetDataAnimation("");
        _popped = true;
    }

    private void OnTimerBounceTimerElapsed(EventArgs args)
    {
        SetDataAnimation("win");
    }

    private void SetDataAnimation(string animation)
    {
        _dataAnimation = animation;
        StateHasChanged();
    }

    private void SetDataState(string state)
    {
        _dataState = state;
        StateHasChanged();
    }
}