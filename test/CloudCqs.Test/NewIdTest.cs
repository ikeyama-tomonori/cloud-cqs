using CloudCqs.NewId;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace CloudCqs.Test
{
    public class TestNewId : NewId<TestNewId.Request, Guid>
    {
        public record Request(string Name);

        public TestNewId(CloudCqsOption option) : base(option)
        {
            var handler = new Handler()
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

            SetHandler(handler);
        }
    }

    [TestClass]
    public class NewIdTest
    {
        [TestMethod]
        public async Task 正常終了すること()
        {
            var option = new CloudCqsOption();
            var create = new TestNewId(option);
            var response = await create.Invoke(new("test"));
            Assert.AreEqual(Guid.Empty, response);
        }

        [TestMethod]
        public async Task Validationエラーになること()
        {
            var option = new CloudCqsOption();
            var create = new TestNewId(option);
            var e = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => create.Invoke(new("error")));
            Assert.AreEqual("error1", e.Error.Details[0].Message);
            Assert.AreEqual("error2", e.Error.Details[1].Message);
        }
    }
}
