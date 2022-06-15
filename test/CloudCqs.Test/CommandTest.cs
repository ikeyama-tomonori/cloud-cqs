using CloudCqs.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudCqs.Test;

public class TestCommand : Command<TestCommand.Request>
{
    public record Request(string Name);

    public TestCommand(CloudCqsOptions option) : base(option)
    {
        var handler = new Handler()
            .Then("データ取得", _ => UseRequest())
            .Validate("データをチェック",
            p =>
            {
                if (p.Name == "error")
                {
                    return new("error1", new[] { "field1", "field2" });
                }
                return null;
            })
            .Then("値を返さないために必要", _ => { });

        SetHandler(handler);
    }
}

[TestClass]
public class CommandTest
{
    [TestMethod]
    public async Task 正常終了すること()
    {
        var update = new TestCommand(Options.Instance);
        var response = await update.Invoke(new("test"));
        Assert.AreEqual(typeof(object), response.GetType());
    }

    [TestMethod]
    public async Task Validationエラーになること()
    {
        var update = new TestCommand(Options.Instance);
        var e = await Assert.ThrowsExceptionAsync<StatusCodeException>(
            () => update.Invoke(new("error")));
        var result = e.ValidationResult;
        Assert.AreEqual("error1", result.ErrorMessage);
        var names = result.MemberNames.ToArray();
        Assert.AreEqual("field1", names?[0]);
        Assert.AreEqual("field2", names?[1]);
    }
}
