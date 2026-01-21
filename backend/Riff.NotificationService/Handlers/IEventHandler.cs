namespace Riff.NotificationService.Handlers;

public interface IEventHandler<in TMessage>
{
    Task Handle(TMessage message);
}