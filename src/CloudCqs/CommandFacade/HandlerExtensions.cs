namespace CloudCqs.CommandFacade;

public static class HandlerExtensions
{
    public static Handler<TResult, TResponse> Invoke<TResponse, TParam, TResult, TDomainRequest, TDomainResponse>(
    this Handler<TParam, TResponse> handler,
    string description,
    IRepository<TDomainRequest, TDomainResponse> domainClass,
    Func<TParam, TDomainRequest> pre,
    Func<(TDomainResponse response, TParam param), TResult> post)
    where TParam : notnull
    where TResult : notnull
    where TResponse : notnull
    where TDomainResponse : notnull
    where TDomainRequest : notnull
    => Facade.HandlerExtensions.Invoke(handler, description, domainClass, pre, post);

    public static Handler<object, TResponse> Invoke<TResponse, TParam, TDomainRequest, TDomainResponse>(
        this Handler<TParam, TResponse> handler,
        string description,
        IRepository<TDomainRequest, TDomainResponse> domainClass,
        Func<TParam, TDomainRequest> pre)
        where TParam : notnull
        where TResponse : notnull
        where TDomainResponse : notnull
        where TDomainRequest : notnull
    => Facade.HandlerExtensions.Invoke(handler, description, domainClass, pre, p => new object());
}
