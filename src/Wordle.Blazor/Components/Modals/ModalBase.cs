using Microsoft.AspNetCore.Components;

namespace Wordle.Blazor.Components;

public class ModalBase : ComponentBase
{
    [Parameter] public EventCallback Closed { get; set; }

    protected async Task OnCloseAsync() => await Closed.InvokeAsync();
}