using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CloudCqs.NewId;
using System;

namespace CloudCqs.Test
{
    public class TestNewId : NewId<TestNewId.Request, Guid>
    {
        public record Request(string Name);

        public TestNewId(LogContext logContext) : base(logContext)
        {
            var exec = new Execution()
                .Then("データ取得", p =>
                {
                    return new { data = p };
                })
                .Validate("データをチェック",
                p =>
                {
                    if (p.data.Name == "error")
                    {
                        return new ValidationError(
                            ("field1", "error1"),
                            ("field1", "error2"));
                    }

                    return null;
                })
                .Then("応答データ作成", p => Guid.Empty)
                .Build();

            SetExecution(exec);
        }
    }

    [TestClass]
    public class NewIdTest
    {
        [TestMethod]
        public async Task 正常終了すること()
        {
            var logContext = new LogContext();
            var create = new TestNewId(logContext);
            var response = await create.Invoke(new("test"));
            Assert.AreEqual(Guid.Empty, response);
        }

        [TestMethod]
        public async Task Validationエラーになること()
        {
            var logContext = new LogContext();
            var create = new TestNewId(logContext);
            var e = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => create.Invoke(new("error")));
            Assert.AreEqual("error1", e.Error.Details[0].Message);
            Assert.AreEqual("error2", e.Error.Details[1].Message);
        }
    }
}
