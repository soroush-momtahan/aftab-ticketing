using Aftab.App.Application.Ticketing.Services;
using Aftab.Shared.Application.Cqrs;
using Aftab.Shared.Domain.Results;

namespace Aftab.App.Application.Ticketing.Features.Queries.GetAllTicketReports;

internal sealed class GetAllTicketReportsQueryHandler(
    ITicketingReporterQueryService ticketingReporterQueryService) : IQueryHandler<GetAllTicketReportsQuery, GetAllTicketReportsQuery.Result>
{
    public async Task<Result<GetAllTicketReportsQuery.Result>> Handle(
        GetAllTicketReportsQuery request, CancellationToken cancellationToken)
    {
        List<TicketReporterView> ticketReporterViews = await ticketingReporterQueryService.GetAllAsync(cancellationToken);
        var ticketReportsDtos =
            ticketReporterViews.Select(GetAllTicketReportsQuery.ToTicketReports).ToList();

        return new GetAllTicketReportsQuery.Result(ticketReportsDtos);
    }
}
