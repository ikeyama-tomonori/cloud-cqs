namespace CloudCqs;

public interface IRepository<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    Task<TResponse> Invoke(TRequest request, CancellationToken cancellationToken = default);
}
