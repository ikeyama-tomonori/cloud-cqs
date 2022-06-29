namespace CloudCqs;

public interface IQuery<TRequest, TResponse> : IRepository<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull { }
