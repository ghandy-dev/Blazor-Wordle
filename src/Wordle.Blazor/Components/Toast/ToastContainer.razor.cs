using Microsoft.AspNetCore.Components;

using Wordle.Blazor.Services;
using Wordle.Blazor.Utilities;

namespace Wordle.Blazor.Components;

public sealed partial class ToastContainer : IDisposable
{
    [Inject] public required IKeyGenerator KeyGen { get; set; }

    [Inject] public required INotificationService NotificationService { get; set; }

    private readonly List<ToastModel> _messages = new();

    private IDisposable? _notificationSubscription;

    public void AddNewToast(NotificationEventArgs args)
    {
        _messages.Add(new ToastModel
        {
            Key = KeyGen.Generate(),
            Message = args.Message,
            DurationType = args.DurationType
        });

        StateHasChanged();
    }

    public void Dispose()
    {
        _notificationSubscription?.Dispose();
    }

    protected override void OnInitialized()
    {
        _notificationSubscription = NotificationService.NotificationReceived.Subscribe(AddNewToast);
    }

    private async Task OnToastClosedAsync(string key)
    {
        var message = _messages.FirstOrDefault(x => x.Key == key);

        if (message != null)
        {
            _messages.Remove(message);
        }

        await InvokeAsync(StateHasChanged);
    }
}