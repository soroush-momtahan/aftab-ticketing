using Microsoft.AspNetCore.Authorization;

namespace Aftab.App.Presentation.Ticketing.Policies.ReplyTicketOwnerShip;

public class ReplyTicketOwnerShipRequirement(string bypassRole = "Agent") : IAuthorizationRequirement
{
    public string BypassRole { get; } = bypassRole;
}
