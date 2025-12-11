namespace Dispatch;

public interface IRequestExceptionHandler<TRequest, TResult, TException>
    where TException : Exception
{
    Task<TResult> Handle(TRequest request, TException exception, CancellationToken ct);
}
