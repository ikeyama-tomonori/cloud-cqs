namespace CloudCqs.Test;

using System.ComponentModel.DataAnnotations;
using System.Net;
using CloudCqs.Query;

public class TestQuery : Query<TestQuery.Request, TestQuery.Response>
{
    public TestQuery(CloudCqsOptions option) : base(option)
    {
        var exec = new Handler()
            .Then(
                "パラメータの取得",
                _ =>
                {
                    var req = this.UseRequest();
                    return new { filter = req.Name };
                }
            )
            .Then(
                "パラメータの取得async",
                async p =>
                    await Task.Run(() =>
                    {
                        return new { p.filter };
                    })
            )
            .Validate(
                "Filterをチェック、パターン1",
                new("error1", new[] { "field1", "field2" }),
                p => p.filter != "error1"
            )
            .Validate(
                "Filterをチェック、パターン2",
                p =>
                    p.filter == "error2"
                        ? new("error1", new[] { "field1", "field2" })
                        : ValidationResult.Success!
            )
            .Then(
                "Validationテスト用データ作成",
                p =>
                {
                    var filters = new[] { p.filter };
                    var newFilter = filters.SingleOrDefault(x => x == "test");
                    return new { filter = newFilter };
                }
            )
            .Validate(
                "filterのnullチェック",
                p =>
                {
                    if (p.filter is null)
                    {
                        throw new StatusCodeException(
                            HttpStatusCode.BadRequest,
                            new("error1", new[] { "field1", "field2" })
                        );
                    }
                    return new { p.filter };
                }
            )
            .Then(
                "応答データを作成",
                p =>
                {
                    return new Response(new[] { p.filter });
                }
            )
            .Then(
                "応答データを作成async",
                async p =>
                    await Task.Run(() =>
                    {
                        return p;
                    })
            );

        this.SetHandler(exec);
    }

    public record Request(string Name);

    public record Response(string[] Name);
}
