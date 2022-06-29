namespace CloudCqs.Test;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class FacadeTest
{
    [TestMethod]
    public async Task 正常終了すること()
    {
        var request = new TestFacade.Request("test");

        var query = new Mock<IQuery<TestFacade.Request, TestFacade.Response>>();
        query
            .Setup(q => q.Invoke(request, default))
            .ReturnsAsync(new TestFacade.Response(new[] { "test" }));

        var command = new Mock<ICommand<TestFacade.Request>>();
        command.Setup(c => c.Invoke(request, default)).ReturnsAsync(new object());

        var facase = new TestFacade(
            Options.Instance,
            new TestFacade.Repository(TestQuery: query.Object, TestCommand: command.Object)
        );

        var response = await facase.Invoke(request);
        Assert.AreEqual("test", response.Name.First());
    }
}
