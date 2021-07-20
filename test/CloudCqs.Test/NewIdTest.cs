using System;
using System.Threading.Tasks;
using CloudCqs.NewId;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudCqs.Test
{
    public class TestNewId : NewId<TestNewId.Request, Guid>
    {
        public record Request(string Name);

        public TestNewId(CloudCqsOptions option) : base(option)
        {
            var handler = new Handler()
                .Then("データ取得", p =>
                {
                    return new { data = p };
                })
                .Then("データをチェック",
                p =>
                {
                    if (p.data.Name == "error")
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
            var create = new TestNewId(Options.Instance);
            var response = await create.Invoke(new("test"));
            Assert.AreEqual(Guid.Empty, response);
        }

        [TestMethod]
        public async Task Validationエラーになること()
        {
            var create = new TestNewId(Options.Instance);
            var e = await Assert.ThrowsExceptionAsync<BadRequestException>(
                () => create.Invoke(new("error")));
            Assert.AreEqual("error1", e.Errors?["field1"][0]);
            Assert.AreEqual("error2", e.Errors?["field1"][1]);
        }
    }
}
