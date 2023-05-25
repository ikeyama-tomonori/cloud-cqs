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

    public static Handler<TResult, TResponse> InvokeIterate<
        TResponse,
        TParam,
        TResult,
        TDomainRequest,
        TDomainResponse
    >(
        this Handler<TParam, TResponse> handler,
        string description,
        IRepository<TDomainRequest, TDomainResponse> domainClass,
        Func<TParam, IEnumerable<TDomainRequest>> pre,
        Func<(IEnumerable<TDomainResponse> Response, TParam Param), TResult> post
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
                var requests = pre(param);
                var responses = new List<TDomainResponse>();
                foreach (var request in requests)
                {
                    var response = await domainClass.Invoke(request, cancellationToken);
                    responses.Add(response);
                }
                var result = post((responses.AsEnumerable(), param));

                return result;
            }
        );
}
