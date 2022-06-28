namespace CloudCqs;

public interface ICommand<TRequest> : IRepository<TRequest, object> where TRequest : notnull { }
