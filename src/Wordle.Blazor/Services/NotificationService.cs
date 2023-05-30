using System.Reactive.Subjects;

using Wordle.Blazor.Enums;

namespace Wordle.Blazor.Services;

public class NotificationService : INotificationService
{
    public IObservable<NotificationEventArgs> NotificationReceived => _notificationSubject;

    private readonly Subject<NotificationEventArgs> _notificationSubject = new();

    public ValueTask RaiseNotification(string message)
    {
        _notificationSubject.OnNext(new NotificationEventArgs
        {
            Message = message,
            DurationType = DurationType.Default
        });

        return ValueTask.CompletedTask;
    }

    public ValueTask RaiseNotification(string message, DurationType durationType)
    {
        _notificationSubject.OnNext(new NotificationEventArgs
        {
            Message = message,
            DurationType = durationType,
        });

        return ValueTask.CompletedTask;
    }
}