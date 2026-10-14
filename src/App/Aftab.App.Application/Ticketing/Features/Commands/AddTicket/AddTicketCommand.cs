using Aftab.App.Domain.Ticketing.Enums;
using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Commands.AddTicket;

public record AddTicketCommand(
    string PersonId,
    string Title,
    string Content,
    TicketPriority Priority,
    TicketDepartment Department) : ICommand;
