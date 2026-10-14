using Aftab.App.Application.Abstraction;
using Aftab.App.Domain.People.Errors;
using Aftab.App.Domain.People.Models;
using Aftab.App.Domain.People.Repository;
using Aftab.App.Domain.People.ValueObjects;
using Aftab.App.Domain.Ticketing.Models;
using Aftab.App.Domain.Ticketing.Repository;
using Aftab.App.Domain.Ticketing.ValueObjects;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.PrefixedGuidTools;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Commands.AddTicket;

internal sealed class AddTicketCommandHandler(
    ITicketingRepository ticketingRepository,
    IPeopleRepository peopleRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddTicketCommand>
{
    public async Task<Result> Handle(AddTicketCommand command, CancellationToken cancellationToken)
    {
        Result<PersonId> personIdOrError = PrefixedGuidV3.From<PersonId>(command.PersonId);
        if (personIdOrError.IsFailure)
        {
            return Result.Failure(personIdOrError.Error);
        }

        Person? person = await peopleRepository.FindByIdAsync(personIdOrError.Value, cancellationToken);
        if (person == null)
        {
            return Result.Failure(PeopleErrors.PersonNotFound);
        }
        
        Result<Title> titleOrError = Title.From(command.Title);
        if (titleOrError.IsFailure)
        {
            return Result.Failure(titleOrError.Error);
        }

        var ticket = Ticket.Create(
            personIdOrError.Value,
            titleOrError.Value,
            command.Content,
            command.Priority,
            command.Department);
        
        await ticketingRepository.CreateAsync(ticket, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
