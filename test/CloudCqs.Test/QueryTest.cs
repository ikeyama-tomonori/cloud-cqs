using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using CloudCqs.Query;
using System;

namespace CloudCqs.Test
{
    public class TestQuery : Query<TestQuery.Request, TestQuery.Response>
    {
        public record Request(string Name);
        public record Response(string[] Name);

        public TestQuery(LogContext logContext) : base(logContext)
        {
            var exec = new Execution()
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
                .Validate("Filterをチェック",
                p =>
                {
                    if (p.filter == "error")
                    {
                        return new ValidationError(
                            ("field1", "error1"),
                            ("field1", "error2")
                        );
                    }
                    return null;
                })
                .Then("応答データを作成",
                p =>
                {
                    return
                                        new Response(new[] { p.filter });
                })
                .Then("応答データを作成async",
                async p => await Task.Run(() =>
                {
                    return p;
                }))
                .Build();

            SetExecution(exec);
        }
    }

    [TestClass]
    public class QueryTest
    {
        [TestMethod]
        public async Task 正常終了すること()
        {
            var logContext = new LogContext();
            var query = new TestQuery(logContext);
            var response = await query.Invoke(new("test"));
            Assert.AreEqual("test", response.Name[0]);
        }

        [TestMethod]
        public async Task Validationエラーになること()
        {
            var logContext = new LogContext();
            var query = new TestQuery(logContext);
            var e = await Assert.ThrowsExceptionAsync<ValidationException>(
                () => query.Invoke(new("error")));
            Assert.AreEqual("error1", e.Error.Details[0].Message);
            Assert.AreEqual("error2", e.Error.Details[1].Message);
        }
    }
}
