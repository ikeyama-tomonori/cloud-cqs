using CloudCqs.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace CloudCqs.Test
{
    public class TestCommand : Command<TestCommand.Request>
    {
        public record Request(string Name);

        public TestCommand(CloudCqsOptions option) : base(option)
        {
            var handler = new Handler()
                .Then("データ取得", p =>
                {
                    return p;
                })
                .Then("データをチェック",
                p =>
                {
                    if (p.Name == "error")
                    {
                        throw new BadRequestException(new()
                        {
                            {
                                "field1",
                                new[] { "error1", "error2" }
                            }
                        });
                    }
                    return p;
                })
                .Build();

            SetHandler(handler);
        }
    }

    [TestClass]
    public class CommandTest
    {
        [TestMethod]
        public async Task 正常終了すること()
        {
            var option = new CloudCqsOptions();
            var update = new TestCommand(option);
            var response = await update.Invoke(new("test"));
            Assert.AreEqual(typeof(Void), response.GetType());
        }

        [TestMethod]
        public async Task Validationエラーになること()
        {
            var option = new CloudCqsOptions();
            var update = new TestCommand(option);
            var e = await Assert.ThrowsExceptionAsync<BadRequestException>(
                () => update.Invoke(new("error")));
            Assert.AreEqual("error1", e.Errors?["field1"][0]);
            Assert.AreEqual("error2", e.Errors?["field1"][1]);
        }
    }
}
