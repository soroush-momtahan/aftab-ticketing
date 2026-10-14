using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Commands.UpdateTicketContent;

public record UpdateTicketContentCommand(
    string TicketId,
    string NewContent) : ICommand;
