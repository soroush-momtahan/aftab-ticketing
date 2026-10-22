using Aftab.App.Application.Ticketing.Services;
using Aftab.Shared.Application.Cqrs;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetAllTicketReports;

public sealed record GetAllTicketReportsQuery : IQuery<GetAllTicketReportsQuery.Result>
{
    public sealed record Result(
        List<TicketReportsDto>  TicketReports);

    public sealed record TicketReportsDto(
        string Id,
        string? OrganId,
        string OrganName,
        int TotalStatusCount,
        int OpenTickets,
        int InprogressTickets,
        int AnsweredTickets,
        int ClosedTickets);

    public static TicketReportsDto ToTicketReports(TicketReporterView reporter)
    {
        return new TicketReportsDto(
            reporter.Id,
            reporter.OrganId,
            reporter.OrganName,
            reporter.TotalStatusCount,
            reporter.OpenTickets,
            reporter.InprogressTickets,
            reporter.AnsweredTickets,
            reporter.ClosedTickets);
    }
}
