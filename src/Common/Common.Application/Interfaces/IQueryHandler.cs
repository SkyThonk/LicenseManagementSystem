using Common.Application.Result;

namespace Common.Application.Interfaces;


public interface IQueryHandler<TQuery, TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
