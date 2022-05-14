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
            .Then($"Invoke {nameof(repository.TestQuery)}",
                p => repository.TestQuery.Invoke(p))
            .Then($"Invoke {nameof(repository.TestCommand)}",
                async p =>
                {
                    await repository
                    .TestCommand
                    .Invoke(new Request(p.Name.First()));
                    return p;
                })
            .Then("応答データ作成", p => new Response(p.Name))
            .Build();

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
        query.Setup(q => q.Invoke(request)).ReturnsAsync(new TestFacade.Response(new[] { "test" }));

        var command = new Mock<ICommand<TestFacade.Request>>();
        command.Setup(c => c.Invoke(request)).ReturnsAsync(Void.Value);

        var facase = new TestFacade(Options.Instance, new TestFacade.Repository(
            TestQuery: query.Object,
            TestCommand: command.Object));

        var response = await facase.Invoke(request);
        Assert.AreEqual("test", response.Name.First());
    }
}
