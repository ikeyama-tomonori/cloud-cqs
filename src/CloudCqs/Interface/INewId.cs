namespace CloudCqs;

public interface INewId<TRequest, TKey> : IRepository<TRequest, TKey>
    where TRequest : notnull
    where TKey : notnull { }
