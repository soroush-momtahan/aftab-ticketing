using Aftab.App.Domain.Ticketing.Enums;
using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Commands.UpdateTicketStatus;

public record UpdateTicketStatusCommand(
    string TicketId,
    TicketStatus NewStatus) : ICommand;
