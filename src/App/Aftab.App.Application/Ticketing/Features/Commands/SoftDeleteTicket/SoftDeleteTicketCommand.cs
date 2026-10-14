using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Commands.SoftDeleteTicket;

public record SoftDeleteTicketCommand(
    string TicketId) : ICommand;
