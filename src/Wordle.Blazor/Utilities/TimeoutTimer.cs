using System.Reactive.Linq;
using System.Timers;

namespace Wordle.Blazor.Utilities;

public sealed class TimeoutTimer : IDisposable
{
    public double Interval
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public IObservable<ElapsedEventArgs> Elapsed => Observable.FromEvent<ElapsedEventHandler, ElapsedEventArgs>(
        handler => (s, e) => handler(e),
        handler => _timer.Elapsed += handler,
        handler => _timer.Elapsed -= handler
    );

    private readonly System.Timers.Timer _timer;

    public TimeoutTimer(double interval)
    {
        _timer = new(interval)
        {
            AutoReset = false
        };
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}