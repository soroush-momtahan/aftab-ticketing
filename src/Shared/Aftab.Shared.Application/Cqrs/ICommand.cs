namespace Aftab.Shared.Application.Cqrs;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand;

public interface ICommand : IRequest<Result>, IBaseCommand;

public interface IBaseCommand;
