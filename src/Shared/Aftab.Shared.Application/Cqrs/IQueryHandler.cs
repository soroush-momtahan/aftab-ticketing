namespace Aftab.Shared.Application.Cqrs;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IRequest<Result<TResponse>>;
