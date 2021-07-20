using System.Threading.Tasks;
using CloudCqs.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudCqs.Test
{
    public class TestQuery : Query<TestQuery.Request, TestQuery.Response>
    {
        public record Request(string Name);
        public record Response(string[] Name);

        public TestQuery(CloudCqsOptions option) : base(option)
        {
            var exec = new Handler()
                .Then("パラメータの取得",
                p =>
                {
                    return new
                    {
                        filter = p.Name
                    };
                })
                .Then("パラメータの取得async",
                async p => await Task.Run(() =>
                {
                    return new
                    {
                        p.filter
                    };
                }))
                .Then("Filterをチェック",
                p =>
                {
                    if (p.filter == "error")
                    {
                        throw new BadRequestException(
                            new()
                            {
                                {
                                    "field1",
                                    new[] { "error1", "error2" }
                                }
                            });
                    }
                    return p;
                })
                .Then("応答データを作成",
                p =>
                {
                    return new Response(new[] { p.filter });
                })
                .Then("応答データを作成async",
                async p => await Task.Run(() =>
                {
                    return p;
                }))
                .Build();

            SetHandler(exec);
        }
    }

    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public async Task 正常終了すること()
        {
            var query = new TestQuery(Options.Instance);
            var response = await query.Invoke(new("test"));
            Assert.AreEqual("test", response.Name[0]);
        }

        [TestMethod]
        public async Task Validationエラーになること()
        {
            var query = new TestQuery(Options.Instance);
            var e = await Assert.ThrowsExceptionAsync<BadRequestException>(
                () => query.Invoke(new("error")));
            Assert.AreEqual("error1", e.Errors?["field1"][0]);
            Assert.AreEqual("error2", e.Errors?["field1"][1]);
        }
    }
}
