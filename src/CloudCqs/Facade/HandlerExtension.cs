using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Facade
{
    using Internal;

    public static class HandlerExtension
    {
        public static BuiltHandler<TResponse> Build<TResponse>(this Handler<TResponse, TResponse> execution)
            where TResponse : notnull
            => new(execution.Functions);

        public static BuiltHandler<object> Build<TProps>(this Handler<TProps, object> execution)
            where TProps : notnull
            => new(execution.Functions.Append(new(nameof(Build), _ => Task.FromResult(new object()))).ToArray());

        public static Handler<(TRepositoryResponse? response, TOption option), TResponse> Invoke<TRepositoryRequest, TRepositoryResponse, TOption, TResponse>
            (this Handler<(TRepositoryRequest, TOption), TResponse> execution,
            IRepository<TRepositoryRequest, TRepositoryResponse> repository,
            Func<(TRepositoryRequest request, TOption option), bool> when)
            where TRepositoryRequest : notnull
            where TRepositoryResponse : notnull
        {
            var thisFunction = new Function(
                Description: $"{nameof(Invoke)} {repository.GetType().FullName}",
                Func: async props =>
                {
                    if (props is (TRepositoryRequest request, TOption option))
                    {
                        if (!when((request, option)))
                        {
                            return (default(TRepositoryResponse?), option);
                        }
                        var response = await repository.Invoke(request);
                        if (response is TRepositoryResponse res) return (res, option);
                        throw new TypeGuardException(typeof(TRepositoryResponse), response);
                    }
                    throw new TypeGuardException(typeof((TRepositoryRequest, TOption)), props);
                });

            return new(execution.Functions.Append(thisFunction).ToArray());
        }

        public static Handler<(TRepositoryResponse response, TOption option), TResponse> Invoke<TRepositoryRequest, TRepositoryResponse, TOption, TResponse>
            (this Handler<(TRepositoryRequest request, TOption option), TResponse> execution,
            IRepository<TRepositoryRequest, TRepositoryResponse> repository)
            where TRepositoryRequest : notnull
            where TRepositoryResponse : notnull
        {
            var thisFunction = new Function(
                Description: $"{nameof(Invoke)} {repository.GetType().FullName}",
                Func: async props =>
                {
                    if (props is (TRepositoryRequest request, TOption option))
                    {
                        var response = await repository.Invoke(request);
                        if (response is TRepositoryResponse res) return (res, option);
                        throw new TypeGuardException(typeof(TRepositoryResponse), response);
                    }
                    throw new TypeGuardException(typeof((TRepositoryRequest, TOption)), props);
                });

            return new(execution.Functions.Append(thisFunction).ToArray());
        }

        public static Handler<(TRepositoryResponse response, object? option), TResponse> Invoke<TRepositoryRequest, TRepositoryResponse, TResponse>
            (this Handler<TRepositoryRequest, TResponse> execution,
            IRepository<TRepositoryRequest, TRepositoryResponse> repository)
            where TRepositoryRequest : notnull
            where TRepositoryResponse : notnull
        {
            var thisFunction = new Function(
                Description: $"{nameof(Invoke)} {repository.GetType().FullName}",
                Func: async props =>
                {
                    if (props is TRepositoryRequest request)
                    {
                        var response = await repository.Invoke(request);
                        if (response is TRepositoryResponse res) return (res, default(object));
                        throw new TypeGuardException(typeof(TRepositoryResponse), response);
                    }
                    throw new TypeGuardException(typeof(TRepositoryRequest), props);
                });

            return new(execution.Functions.Append(thisFunction).ToArray());
        }

        public static Handler<(TRepositoryResponse? response, object? option), TResponse> Invoke<TRepositoryRequest, TRepositoryResponse, TResponse>
            (this Handler<TRepositoryRequest, TResponse> execution,
            IRepository<TRepositoryRequest, TRepositoryResponse> repository,
            Func<TRepositoryRequest, bool> when)
            where TRepositoryRequest : notnull
            where TRepositoryResponse : notnull
        {
            var thisFunction = new Function(
                Description: $"{nameof(Invoke)} {repository.GetType().FullName}",
                Func: async props =>
                {
                    if (props is TRepositoryRequest request)
                    {
                        if (!when(request))
                        {
                            return (default(TRepositoryResponse), default(object));
                        }
                        var response = await repository.Invoke(request);
                        if (response is TRepositoryResponse res) return (res, default(object));
                        throw new TypeGuardException(typeof(TRepositoryResponse), response);
                    }
                    throw new TypeGuardException(typeof(TRepositoryRequest), props);
                });

            return new(execution.Functions.Append(thisFunction).ToArray());
        }
    }
}
