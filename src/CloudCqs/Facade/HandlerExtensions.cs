namespace CloudCqs.Facade;

public static class HandlerExtensions
{
    public static Handler<TResult, TResponse> Invoke<
        TResponse,
        TParam,
        TResult,
        TDomainRequest,
        TDomainResponse
    >(
        this Handler<TParam, TResponse> handler,
        string description,
        IRepository<TDomainRequest, TDomainResponse> domainClass,
        Func<TParam, TDomainRequest> pre,
        Func<(TDomainResponse Response, TParam Param), TResult> post
    )
        where TParam : notnull
        where TResult : notnull
        where TResponse : notnull
        where TDomainResponse : notnull
        where TDomainRequest : notnull =>
        handler.Then(
            description,
            async (param, cancellationToken) =>
            {
                var request = pre(param);
                var response = await domainClass.Invoke(request, cancellationToken);
                var result = post((response, param));
                return result;
            }
        );
}
