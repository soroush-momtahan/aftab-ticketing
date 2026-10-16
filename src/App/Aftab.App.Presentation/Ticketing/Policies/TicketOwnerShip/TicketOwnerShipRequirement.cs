using Microsoft.AspNetCore.Authorization;

namespace Aftab.App.Presentation.Ticketing.Policies.TicketOwnerShip;

public class TicketOwnershipRequirement(string bypassRole = "Agent") : IAuthorizationRequirement
{
    public string BypassRole { get; } = bypassRole;
}
