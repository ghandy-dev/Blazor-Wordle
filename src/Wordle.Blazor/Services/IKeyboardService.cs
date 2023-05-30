using Microsoft.AspNetCore.Components.Web;

namespace Wordle.Blazor.Services;

public interface IKeyboardService
{
    IObservable<KeyboardEventArgs> OnKeyDown { get; }

    void RaiseEvent(KeyboardEventArgs args);
}