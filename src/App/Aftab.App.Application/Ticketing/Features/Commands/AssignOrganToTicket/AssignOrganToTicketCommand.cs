using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Commands.AssignOrganToTicket;

public record AssignOrganToTicketCommand(
    string TicketId,
    string OrganId) : ICommand;
