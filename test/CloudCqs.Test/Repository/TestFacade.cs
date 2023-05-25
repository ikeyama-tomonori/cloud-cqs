namespace CloudCqs.Test;

using System.Linq;
using CloudCqs.Facade;

public class TestFacade : Facade<TestFacade.Request, TestFacade.Response>
{
    public TestFacade(CloudCqsOptions option, Repository repository) : base(option)
    {
        var handler = new Handler()
            .Invoke(
                $"Invoke {nameof(repository.TestQuery)}",
                repository.TestQuery,
                _ => this.UseRequest(),
                p => p.Response
            )
            .Invoke(
                $"Invoke {nameof(repository.TestCommand)}",
                repository.TestCommand,
                p => new Request(p.Name.First()),
                p => p.Param.Name
            )
            .InvokeIterate(
                $"Iterate {nameof(repository.TestQuery)}",
                repository.TestQuery,
                _ =>
                {
                    var requests = new[] { new Request("test"), new Request("test"), };
                    return requests;
                },
                p =>
                {
                    var response = p.Response.SelectMany(x => x.Name).ToArray();
                    return response;
                }
            )
            .Then("応答データ作成", p => new Response(p));

        this.SetHandler(handler);
    }

    public record Request(string Name);

    public record Response(string[] Name);

    public record Repository(IQuery<Request, Response> TestQuery, ICommand<Request> TestCommand);
}
