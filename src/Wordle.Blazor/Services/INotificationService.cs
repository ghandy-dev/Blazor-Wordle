using Wordle.Blazor.Enums;

namespace Wordle.Blazor.Services;

public interface INotificationService
{
    IObservable<NotificationEventArgs> NotificationReceived { get; }

    ValueTask RaiseNotification(string message);

    ValueTask RaiseNotification(string message, DurationType durationType);
}