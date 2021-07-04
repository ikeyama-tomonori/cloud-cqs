﻿using System.Threading.Tasks;

namespace CloudCqs
{
    public interface IRepository<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        Task<TResponse> Invoke(TRequest request);
    }

    public interface IFacade<TRequest, TResponse> : IRepository<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
    }

    public interface ICommandFacade<TRequest> : IRepository<TRequest, object>
        where TRequest : notnull
    {
    }

    public interface IQuery<TRequest, TResponse> : IRepository<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
    }

    public interface INewId<TRequest, TKey> : IRepository<TRequest, TKey>
        where TRequest : notnull
        where TKey : notnull
    {
    }

    public interface ICommand<TRequest> : IRepository<TRequest, object>
        where TRequest : notnull
    {
    }
}
