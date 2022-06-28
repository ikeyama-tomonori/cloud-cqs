namespace CloudCqs;

public interface ICommandFacade<TRequest> : IRepository<TRequest, object>
    where TRequest : notnull { }
