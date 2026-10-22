using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Commands.ChangeReplyTicketVisibility;

public record ChangeReplyTicketVisibilityCommand(
    string TicketId,
    string ReplyTicketId,
    bool Visibility) : ICommand;
