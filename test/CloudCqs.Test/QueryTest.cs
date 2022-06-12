using System.Linq;
using System.Threading.Tasks;
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
            .Validate("Filterをチェック、パターン1",
            new()
            {
                ["field1"] = new[] { "error1-1", "error1-2" }

            },
            p => p.filter != "error1")
            .Validate("Filterをチェック、パターン2",
            p => p.filter == "error2"
            ? new()
            {
                ["field1"] = new[] { "error2-1", "error2-2" }

            }
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
                    throw new BadRequestException(new()
                    {
                        ["field1"] = new[] { "error3-1", "error3-2" }

                    });
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
        var e = await Assert.ThrowsExceptionAsync<BadRequestException>(
            () => query.Invoke(new("error1")));
        Assert.AreEqual("error1-1", e.Errors?["field1"][0]);
        Assert.AreEqual("error1-2", e.Errors?["field1"][1]);
    }


    [TestMethod]
    public async Task エラー出力可変のValidationエラーになること()
    {
        var query = new TestQuery(Options.Instance);
        var e = await Assert.ThrowsExceptionAsync<BadRequestException>(
            () => query.Invoke(new("error2")));
        Assert.AreEqual("error2-1", e.Errors?["field1"][0]);
        Assert.AreEqual("error2-2", e.Errors?["field1"][1]);
    }

    [TestMethod]
    public async Task エラーチェックロジックが可変のValidationエラーになること()
    {
        var query = new TestQuery(Options.Instance);
        var e = await Assert.ThrowsExceptionAsync<BadRequestException>(
            () => query.Invoke(new("error3")));
        Assert.AreEqual("error3-1", e.Errors?["field1"][0]);
        Assert.AreEqual("error3-2", e.Errors?["field1"][1]);
    }
}
