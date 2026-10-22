using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Organization.Features.Commands.AddOrgan;

public record AddOrganCommand(
    string Name,
    string Description) : ICommand;
