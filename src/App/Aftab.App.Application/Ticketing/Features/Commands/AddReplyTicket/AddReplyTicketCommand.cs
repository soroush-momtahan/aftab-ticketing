using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Commands.AddReplyTicket;

public record AddReplyTicketCommand(
    string TicketId,
    string ReplierId,
    string Content) : ICommand;
