using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.People.Commands.AssignPersonToOrgan;

public record AssignPersonToOrganCommand(
    string PersonId,
    string OrganId) : ICommand;
