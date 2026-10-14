using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Commands.UpdateReplyTicketContent;

public record UpdateReplyTicketContentCommand(
    string TicketId,
    string ReplyTicketId,
    string NewContent) : ICommand;
