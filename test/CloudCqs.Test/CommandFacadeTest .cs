using CloudCqs.Facade;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Test
{
    public class TestCommandFacade : CommandFacade<TestCommandFacade.Request>
    {
        public record Request(string Name);
        public record Response(string[] Name);
        public record Repository(
            IQuery<Request, Response> TestQuery,
            ICommand<Request> TestCommand);

        public TestCommandFacade(CloudCqsOption option, Repository repository) : base(option)
        {
            var handler = new Handler()
                .Then("Query呼び出しパラメータの作成", p => p)
                .Invoke(repository.TestQuery)
                .Then("Command呼び出しパラメータの作成", p => new Request(p.response.Name.First()))
                .Invoke(repository.TestCommand)
                .Build();

            SetHandler(handler);
        }
    }

    [TestClass]
    public class CommandFacadeTest
    {
        [TestMethod]
        public async Task 正常終了すること()
        {
            var request = new TestCommandFacade.Request("test");

            var query = new Mock<IQuery<TestCommandFacade.Request, TestCommandFacade.Response>>();
            query.Setup(q => q.Invoke(request)).ReturnsAsync(new TestCommandFacade.Response(new[] { "test" }));

            var command = new Mock<ICommand<TestCommandFacade.Request>>();
            command.Setup(c => c.Invoke(request)).ReturnsAsync(new object());

            var option = new CloudCqsOption();
            var facase = new TestCommandFacade(option, new TestCommandFacade.Repository(
                TestQuery: query.Object,
                TestCommand: command.Object));

            var response = await facase.Invoke(request);
            Assert.AreEqual(typeof(object), response.GetType());
        }
    }
}
