using System.Reactive.Subjects;

using Microsoft.AspNetCore.Components.Web;

namespace Wordle.Blazor.Services;

public class KeyboardService : IKeyboardService
{
    public IObservable<KeyboardEventArgs> OnKeyDown => _keyboardSubject;

    public static KeyboardService Instance { get; } = new KeyboardService();

    private readonly Subject<KeyboardEventArgs> _keyboardSubject = new();

    [Microsoft.JSInterop.JSInvokable]
    public static void OnKeyDownEvent(KeyboardEventArgs args)
    {
        Instance.RaiseEvent(args);
    }

    public void RaiseEvent(KeyboardEventArgs args)
    {
        _keyboardSubject.OnNext(args);
    }
}