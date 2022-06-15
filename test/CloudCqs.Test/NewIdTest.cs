using System.Net;
using CloudCqs.NewId;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudCqs.Test;

public class TestNewId : NewId<TestNewId.Request, Guid>
{
    public record Request(string Name);

    public TestNewId(CloudCqsOptions option) : base(option)
    {
        var handler = new Handler()
            .Then("データ取得", _ =>
            {
                return new { data = UseRequest() };
            })
            .Then("データをチェック",
            p =>
            {
                if (p.data.Name == "error")
                {
                    throw new StatusCodeException(
                        HttpStatusCode.BadRequest,
                        new("error1", new[] { "field1", "field2" }));
                }

                return p;
            })
            .Then("応答データ作成", p => Guid.Empty);

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
        var e = await Assert.ThrowsExceptionAsync<StatusCodeException>(
            () => create.Invoke(new("error")));
        var result = e.ValidationResult;
        Assert.AreEqual("error1", result?.ErrorMessage);
        var names = result?.MemberNames.ToArray();
        Assert.AreEqual("field1", names?[0]);
        Assert.AreEqual("field2", names?[1]);
    }
}
