using CloudCqs.Facade;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Threading.Tasks;

namespace CloudCqs.Test
{
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
                    props => repository.TestQuery.Invoke(props))
                .Then($"Invoke {nameof(repository.TestCommand)}",
                    async props =>
                    {
                        await repository
                        .TestCommand
                        .Invoke(new Request(props.Name.First()));
                        return props;
                    })
                .Then("応答データ作成", props => new Response(props.Name))
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

            var option = new CloudCqsOptions();
            var facase = new TestFacade(option, new TestFacade.Repository(
                TestQuery: query.Object,
                TestCommand: command.Object));

            var response = await facase.Invoke(request);
            Assert.AreEqual("test", response.Name.First());
        }
    }
}
