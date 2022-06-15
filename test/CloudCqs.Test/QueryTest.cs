using System.Net;
using CloudCqs.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudCqs.Test;

public class TestQuery : Query<TestQuery.Request, TestQuery.Response>
{
    public record Request(string Name);
    public record Response(string[] Name);

    public TestQuery(CloudCqsOptions option) : base(option)
    {
        var exec = new Handler()
            .Then("パラメータの取得",
            _ =>
            {
                var req = UseRequest();
                return new
                {
                    filter = req.Name
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
            .Validate(
                "Filterをチェック、パターン1",
                new("error1", new[] { "field1", "field2" }),
            p => p.filter != "error1")
            .Validate(
                "Filterをチェック、パターン2",
                p => p.filter == "error2"
                    ? new("error1", new[] { "field1", "field2" })
                    : null)
            .Then("Validationテスト用データ作成",
            p =>
            {
                var filters = new[] { p.filter };
                var newFilter = filters.SingleOrDefault(x => x == "test");
                return new { filter = newFilter };
            })
            .Validate("filterのnullチェック",
            p =>
            {
                if (p.filter is null)
                {
                    throw new StatusCodeException(
                        HttpStatusCode.BadRequest,
                        new("error1", new[] { "field1", "field2" }));
                }
                return new { p.filter };
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
            }));

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
    public async Task エラー出力固定のValidationエラーになること()
    {
        var query = new TestQuery(Options.Instance);
        var e = await Assert.ThrowsExceptionAsync<StatusCodeException>(
            () => query.Invoke(new("error1")));
        var result = e.ValidationResult;
        Assert.AreEqual("error1", result?.ErrorMessage);
        var names = result?.MemberNames.ToArray();
        Assert.AreEqual("field1", names?[0]);
        Assert.AreEqual("field2", names?[1]);
    }


    [TestMethod]
    public async Task エラー出力可変のValidationエラーになること()
    {
        var query = new TestQuery(Options.Instance);
        var e = await Assert.ThrowsExceptionAsync<StatusCodeException>(
            () => query.Invoke(new("error2")));
        var result = e.ValidationResult;
        Assert.AreEqual("error1", result?.ErrorMessage);
        var names = result?.MemberNames.ToArray();
        Assert.AreEqual("field1", names?[0]);
        Assert.AreEqual("field2", names?[1]);
    }

    [TestMethod]
    public async Task エラーチェックロジックが可変のValidationエラーになること()
    {
        var query = new TestQuery(Options.Instance);
        var e = await Assert.ThrowsExceptionAsync<StatusCodeException>(
            () => query.Invoke(new("error3")));
        var result = e.ValidationResult;
        Assert.AreEqual("error1", result?.ErrorMessage);
        var names = result?.MemberNames.ToArray();
        Assert.AreEqual("field1", names?[0]);
        Assert.AreEqual("field2", names?[1]);
    }
}
