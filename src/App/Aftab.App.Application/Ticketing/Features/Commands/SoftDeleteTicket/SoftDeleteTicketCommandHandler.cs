using Aftab.App.Application.Abstraction;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Commands.SoftDeleteTicket;

internal sealed class SoftDeleteTicketCommandHandler(
    ITicketingRepository ticketingRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<SoftDeleteTicketCommand>
{
    public async Task<Result> Handle(
        SoftDeleteTicketCommand command, CancellationToken cancellationToken)
    {
        Result<TicketId> ticketIdOrError = PrefixedGuidV3.From<TicketId>(command.TicketId);
        if (ticketIdOrError.IsFailure)
        {
            return Result.Failure(ticketIdOrError.Error);
        }

        Ticket? ticket = await ticketingRepository.GetByIdAsync(ticketIdOrError.Value, cancellationToken);
        if (ticket is null)
        {
            return Result.Failure(TicketingErrors.NotFound(command.TicketId));
        }
        
        ticket.SoftDelete();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
