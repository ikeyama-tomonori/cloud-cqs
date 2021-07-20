using System.Linq;
using System.Threading.Tasks;
using CloudCqs.CommandFacade;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        public TestCommandFacade(CloudCqsOptions option, Repository repository) : base(option)
        {
            var handler = new Handler()
                .Then($"Invoke {nameof(repository.TestQuery)}",
                    props => repository.TestQuery.Invoke(props))
                .Then($"Invoke {nameof(repository.TestCommand)}",
                    props => repository
                        .TestCommand
                        .Invoke(new Request(props.Name.First())))
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
            command.Setup(c => c.Invoke(request)).ReturnsAsync(Void.Value);

            var facase = new TestCommandFacade(Options.Instance, new TestCommandFacade.Repository(
                TestQuery: query.Object,
                TestCommand: command.Object));

            var response = await facase.Invoke(request);
            Assert.AreEqual(typeof(Void), response.GetType());
        }
    }
}
