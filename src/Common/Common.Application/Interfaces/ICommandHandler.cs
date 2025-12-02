using Common.Application.Result;

namespace Common.Application.Interfaces;


public interface ICommandHandler<TCommand, TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}
