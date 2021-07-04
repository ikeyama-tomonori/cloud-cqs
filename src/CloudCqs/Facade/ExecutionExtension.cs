using System;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Facade
{
    using Internal;

    public static class ExecutionExtension
    {
        public static ExecutionCompleted<TResponse> Build<TResponse>(this Execution<TResponse, TResponse> execution)
            where TResponse : notnull
            => new(execution.Functions);

        public static ExecutionCompleted<object> Build<TProps>(this Execution<TProps, object> execution)
            where TProps : notnull
            => new(execution.Functions.Append(new(nameof(Build), async _ => await Task.Run(() => new object()))).ToArray());

        public static Execution<(TRepositoryResponse response, TOption option), TResponse> Invoke<TRepositoryRequest, TRepositoryResponse, TOption, TResponse>
            (this Execution<(TRepositoryRequest, TOption), TResponse> execution,
            IRepository<TRepositoryRequest, TRepositoryResponse> repository)
            where TRepositoryRequest : notnull
            where TRepositoryResponse : notnull
        {
            var thisFunction = new FunctionBlock(
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

        public static Execution<(TRepositoryResponse? response, TOption option), TResponse> Invoke<TRepositoryRequest, TRepositoryResponse, TOption, TResponse>
            (this Execution<(TRepositoryRequest, TOption), TResponse> execution,
            IRepository<TRepositoryRequest, TRepositoryResponse> repository,
            Func<(TRepositoryRequest, TOption), bool> when)
            where TRepositoryRequest : notnull
            where TRepositoryResponse : notnull
        {
            var thisFunction = new FunctionBlock(
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

        public static Execution<(TRepositoryResponse response, object? option), TResponse> Invoke<TRepositoryRequest, TRepositoryResponse, TResponse>
            (this Execution<TRepositoryRequest, TResponse> execution,
            IRepository<TRepositoryRequest, TRepositoryResponse> repository)
            where TRepositoryRequest : notnull
            where TRepositoryResponse : notnull
        {
            var thisFunction = new FunctionBlock(
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

        public static Execution<(TRepositoryResponse? response, object? option), TResponse> Invoke<TRepositoryRequest, TRepositoryResponse, TResponse>
            (this Execution<TRepositoryRequest, TResponse> execution,
            IRepository<TRepositoryRequest, TRepositoryResponse> repository,
            Func<TRepositoryRequest, bool> when)
            where TRepositoryRequest : notnull
            where TRepositoryResponse : notnull
        {
            var thisFunction = new FunctionBlock(
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
