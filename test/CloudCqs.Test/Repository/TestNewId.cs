namespace CloudCqs.Test;

using System.Net;
using CloudCqs.NewId;

public class TestNewId : NewId<TestNewId.Request, Guid>
{
    public TestNewId(CloudCqsOptions option) : base(option)
    {
        var handler = new Handler()
            .Then(
                "データ取得",
                _ =>
                {
                    return new { data = this.UseRequest() };
                }
            )
            .Then(
                "データをチェック",
                p =>
                {
                    if (p.data.Name == "error")
                    {
                        throw new StatusCodeException(
                            HttpStatusCode.BadRequest,
                            new("error1", new[] { "field1", "field2" })
                        );
                    }

                    return p;
                }
            )
            .Then("応答データ作成", p => Guid.Empty);

        this.SetHandler(handler);
    }

    public record Request(string Name);
}
