namespace CloudCqs;

public interface IFacade<TRequest, TResponse> : IRepository<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull { }
