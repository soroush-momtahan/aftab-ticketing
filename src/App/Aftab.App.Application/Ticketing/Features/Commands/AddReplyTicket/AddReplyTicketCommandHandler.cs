using Aftab.App.Application.Abstraction;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Domain.Ticketing.Errors;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Commands.AddReplyTicket;

internal sealed class AddReplyTicketCommandHandler(
    IPeopleRepository peopleRepository,
    ITicketingRepository ticketingRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddReplyTicketCommand>
{
    public async Task<Result> Handle(
        AddReplyTicketCommand command, CancellationToken cancellationToken)
    {
        Result<TicketId> ticketIdOrError = PrefixedGuidV3.From<TicketId>(command.TicketId);
        if (ticketIdOrError.IsFailure)
        {
            return Result.Failure(ticketIdOrError.Error);
        }

        Result<PersonId> personIdOrError = PrefixedGuidV3.From<PersonId>(command.ReplierId);
        if (personIdOrError.IsFailure)
        {
            return Result.Failure(personIdOrError.Error);
        }

        Person? person = await peopleRepository.FindByIdAsync(personIdOrError.Value, cancellationToken);
        if (person is null)
        {
            return Result.Failure(PeopleErrors.PersonNotFound);
        }

        Ticket? ticket = await ticketingRepository.GetByIdAsync(ticketIdOrError.Value, cancellationToken);
        if (ticket is null)
        {
            return Result.Failure(TicketingErrors.NotFound(command.TicketId));
        }
        
        ticket.AddReply(command.Content, personIdOrError.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
