using System.Linq;
using System.Threading.Tasks;
using CloudCqs.Facade;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CloudCqs.Test;

public class TestFacade : Facade<TestFacade.Request, TestFacade.Response>
{
    public record Request(string Name);
    public record Response(string[] Name);
    public record Repository(
        IQuery<Request, Response> TestQuery,
        ICommand<Request> TestCommand);

    public TestFacade(CloudCqsOptions option, Repository repository) : base(option)
    {
        var handler = new Handler()
            .Invoke($"Invoke {nameof(repository.TestQuery)}",
                repository.TestQuery,
                _ => UseRequest(),
                p => p.response)
            .Invoke($"Invoke {nameof(repository.TestCommand)}",
                repository.TestCommand,
                p => new Request(p.Name.First()),
                p => p.param.Name)
            .Then("応答データ作成", p => new Response(p));

        SetHandler(handler);
    }
}

[TestClass]
public class FacadeTest
{
    [TestMethod]
    public async Task 正常終了すること()
    {
        var request = new TestFacade.Request("test");

        var query = new Mock<IQuery<TestFacade.Request, TestFacade.Response>>();
        query.Setup(q => q.Invoke(request, default)).ReturnsAsync(new TestFacade.Response(new[] { "test" }));

        var command = new Mock<ICommand<TestFacade.Request>>();
        command.Setup(c => c.Invoke(request, default)).ReturnsAsync(new object());

        var facase = new TestFacade(Options.Instance, new TestFacade.Repository(
            TestQuery: query.Object,
            TestCommand: command.Object));

        var response = await facase.Invoke(request);
        Assert.AreEqual("test", response.Name.First());
    }
}
