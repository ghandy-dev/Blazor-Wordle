using Microsoft.AspNetCore.Components;

using Wordle.Blazor.Constants;
using Wordle.Blazor.Enums;
using Wordle.Blazor.Utilities;

namespace Wordle.Blazor.Components;

public sealed partial class Toast : ComponentBase, IDisposable
{
    private readonly string _animation = "fade";

    [Parameter] public required string Key { get; set; }

    [Parameter] public required string Message { get; set; }

    [Parameter] public EventCallback<string> Closed { get; set; }

    [Parameter] public DurationType DurationType { get; set; }

    private TimeoutTimer? _timer;
    private IDisposable? _timerSubscription;

    public void Dispose()
    {
        _timerSubscription?.Dispose();
        _timer?.Dispose();
    }

    protected override void OnInitialized()
    {
        if (DurationType == DurationType.Default)
        {
            if (_timer == null)
            {
                _timer = new(TimerConstants.ToastShowDuration);
                _timerSubscription = _timer.Elapsed.Subscribe(async (e) => await Closed.InvokeAsync(Key));
                _timer.Start();
            }
        }
    }
}