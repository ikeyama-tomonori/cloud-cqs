namespace CloudCqs.Test;

using System.Linq;
using System.Threading.Tasks;
using CloudCqs.Facade;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            .Then("応答データ作成", p => new Response(p));

        this.SetHandler(handler);
    }

    public record Request(string Name);

    public record Response(string[] Name);

    public record Repository(IQuery<Request, Response> TestQuery, ICommand<Request> TestCommand);
}
