using Aftab.App.Application.Abstraction;
using Aftab.App.Domain.Organization.Models;
using Aftab.App.Domain.Organization.Repository;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Organization.Features.Commands.AddOrgan;

internal sealed class AddOrganCommandHandler(
    IOrganizationRepository organizationRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AddOrganCommand>
{
    public async Task<Result> Handle(
        AddOrganCommand command, 
        CancellationToken cancellationToken)
    {
        var organ = Organ.Create(
            command.Name,
            command.Description);

        await organizationRepository.AddAsync(organ, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
