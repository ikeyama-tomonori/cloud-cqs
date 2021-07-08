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

        public TestFacade(CloudCqsOption option, Repository repository) : base(option)
        {
            var handler = new Handler()
                .Then("Query呼び出しパラメータの作成", p => p)
                .Invoke(repository.TestQuery)
                .Then("Command呼び出しパラメータの作成", p =>
                    (new Request(p.response.Name.First()), p.response))
                .Invoke(repository.TestCommand)
                .Then("応答データ作成", p => new Response(p.option.Name))
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
            command.Setup(c => c.Invoke(request)).ReturnsAsync(new object());

            var option = new CloudCqsOption();
            var facase = new TestFacade(option, new TestFacade.Repository(
                TestQuery: query.Object,
                TestCommand: command.Object));

            var response = await facase.Invoke(request);
            Assert.AreEqual("test", response.Name.First());
        }
    }
}
