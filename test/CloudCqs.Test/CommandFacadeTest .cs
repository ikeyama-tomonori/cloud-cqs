using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading.Tasks;
using CloudCqs.Facade;
using System.Linq;
using Moq;

namespace CloudCqs.Test
{
    public class TestCommandFacade : CommandFacade<TestCommandFacade.Request>
    {
        public record Request(string Name);
        public record Response(string[] Name);
        public record Repository(
            IQuery<Request, Response> TestQuery,
            ICommand<Request> TestCommand);

        public TestCommandFacade(LogContext logContext, Repository repository) : base(logContext)
        {
            var exec = new Execution()
                .Then("Query呼び出しパラメータの作成", p => p)
                .Invoke(repository.TestQuery)
                .Then("Command呼び出しパラメータの作成", p => new Request(p.response.Name.First()))
                .Invoke(repository.TestCommand)
                .Build();

            SetExecution(exec);
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

            var logContext = new LogContext();
            var facase = new TestCommandFacade(logContext, new TestCommandFacade.Repository(
                TestQuery: query.Object,
                TestCommand: command.Object));

            var response = await facase.Invoke(request);
            Assert.AreEqual(typeof(object), response.GetType());
        }
    }
}
