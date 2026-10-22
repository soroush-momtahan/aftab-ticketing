using Aftab.App.Application.Abstraction;
using Aftab.App.Domain.Organization.Errors;
using Aftab.App.Domain.Organization.Models;
using Aftab.App.Domain.Organization.Repository;
using Aftab.App.Domain.Organization.ValueObjects;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Commands.AssignOrganToTicket;

internal sealed class AssignOrganToTicketCommandHandler(
    ITicketingRepository ticketingRepository,
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AssignOrganToTicketCommand>
{
    public async Task<Result> Handle(
        AssignOrganToTicketCommand command, 
        CancellationToken cancellationToken)
    {
        Result<OrganId> organIdOrError = PrefixedGuidV3.From<OrganId>(command.OrganId);
        if (organIdOrError.IsFailure)
        {
            return Result.Failure(organIdOrError.Error);
        }

        Result<TicketId> ticketIdOrError = PrefixedGuidV3.From<TicketId>(command.TicketId);
        if (ticketIdOrError.IsFailure)
        {
            return Result.Failure(ticketIdOrError.Error);
        }

        Organ? organ = await organizationRepository.GetByIdAsync(organIdOrError.Value, cancellationToken);
        if (organ is null)
        {
            return Result.Failure(OrganizationErrors.NotFound);
        }
        
        Ticket? ticket = await ticketingRepository.GetByIdAsync(ticketIdOrError.Value, cancellationToken);
        if (ticket is null)
        {
            return Result.Failure(TicketingErrors.NotFound(command.TicketId));
        }
        
        ticket.AssignOrgan(organIdOrError.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
