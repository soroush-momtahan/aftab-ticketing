using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Commands.SoftDeleteReplyTicket;

public record SoftDeleteReplyTicketCommand(
    string TicketId,
    string ReplyTicketId) : ICommand;
