namespace CloudCqs.Test;

using System.Linq;
using CloudCqs.CommandFacade;

public class TestCommandFacade : CommandFacade<TestCommandFacade.Request>
{
    public TestCommandFacade(CloudCqsOptions option, Repository repository) : base(option)
    {
        var handler = new Handler()
            .Invoke(
                $"Invoke {nameof(repository.TestQuery)}",
                repository.TestQuery,
                _ => this.UseRequest(),
                param => param.Response
            )
            .Invoke(
                $"Invoke {nameof(repository.TestCommand)}",
                repository.TestCommand,
                param => new Request(param.Name.First())
            );

        this.SetHandler(handler);
    }

    public record Request(string Name);

    public record Response(string[] Name);

    public record Repository(IQuery<Request, Response> TestQuery, ICommand<Request> TestCommand);
}
