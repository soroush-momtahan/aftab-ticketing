namespace Aftab.Shared.Application.Messaging;

public interface IApplicationEvent : INotification
{
    Guid Id { get; }
    DateTime OccuredOn { get; }
};
