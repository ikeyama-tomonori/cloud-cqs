using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CloudCqs.Command;

namespace CloudCqs.Test
{
    public class TestCommand : Command<TestCommand.Request>
    {
        public record Request(string Name);

        public TestCommand(LogContext logContext) : base(logContext)
        {
            var exec = new Execution()
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

            SetExecution(exec);
        }
    }

    [TestClass]
    public class CommandTest
    {
        [TestMethod]
        public async Task 正常終了すること()
        {
            var logContext = new LogContext();
            var update = new TestCommand(logContext);
            var response = await update.Invoke(new("test"));
            Assert.AreEqual(typeof(object), response.GetType());
        }

        [TestMethod]
        public async Task Validationエラーになること()
        {
            var logContext = new LogContext();
            var update = new TestCommand(logContext);
            var e = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => update.Invoke(new("error")));
            Assert.AreEqual("error1", e.Error.Details[0].Message);
            Assert.AreEqual("error2", e.Error.Details[1].Message);
        }
    }
}
