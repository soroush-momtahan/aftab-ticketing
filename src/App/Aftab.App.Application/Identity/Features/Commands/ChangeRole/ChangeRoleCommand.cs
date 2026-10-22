using Aftab.App.Domain.Identity.Enums;
using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Identity.Features.Commands.ChangeRole;

public record ChangeRoleCommand(
    string PersonId,
    RoleType RoleType) : ICommand;
