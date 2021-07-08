using CloudCqs.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace CloudCqs.Test
{
    public class TestCommand : Command<TestCommand.Request>
    {
        public record Request(string Name);

        public TestCommand(CloudCqsOption option) : base(option)
        {
            var handler = new Handler()
                .Then("データ取得", p =>
                {
                    return p;
                })
                .Validate("データをチェック",
                p =>
                {
                    if (p.Name == "error")
                    {
                        return new ValidationError(
                            ("field1", "error1"),
                            ("field1", "error2"));
                    }
                    return null;
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
            var option = new CloudCqsOption();
            var update = new TestCommand(option);
            var response = await update.Invoke(new("test"));
            Assert.AreEqual(typeof(object), response.GetType());
        }

        [TestMethod]
        public async Task Validationエラーになること()
        {
            var option = new CloudCqsOption();
            var update = new TestCommand(option);
            var e = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => update.Invoke(new("error")));
            Assert.AreEqual("error1", e.Error.Details[0].Message);
            Assert.AreEqual("error2", e.Error.Details[1].Message);
        }
    }
}
