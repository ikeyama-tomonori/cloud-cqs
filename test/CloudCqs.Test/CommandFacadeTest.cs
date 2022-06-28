namespace CloudCqs.Test;

using System.Linq;
using System.Threading.Tasks;
using CloudCqs.CommandFacade;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class CommandFacadeTest
{
    [TestMethod]
    public async Task 正常終了すること()
    {
        var request = new TestCommandFacade.Request("test");

        var query = new Mock<IQuery<TestCommandFacade.Request, TestCommandFacade.Response>>();
        query
            .Setup(q => q.Invoke(request, default))
            .ReturnsAsync(new TestCommandFacade.Response(new[] { "test" }));

        var command = new Mock<ICommand<TestCommandFacade.Request>>();
        command.Setup(c => c.Invoke(request, default)).ReturnsAsync(new object());

        var facase = new TestCommandFacade(
            Options.Instance,
            new TestCommandFacade.Repository(TestQuery: query.Object, TestCommand: command.Object)
        );

        var response = await facase.Invoke(request);
        Assert.AreEqual(typeof(object), response.GetType());
    }
}
