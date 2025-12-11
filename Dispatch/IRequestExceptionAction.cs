namespace Dispatch;

public interface IRequestExceptionAction<TRequest, TException>
    where TException : Exception
{
    Task Execute(TRequest request, TException exception, CancellationToken ct);
}
