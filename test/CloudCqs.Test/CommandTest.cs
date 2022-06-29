namespace CloudCqs.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            () => update.Invoke(new("error")).AsTask()
        );
        var result = e.ValidationResult;
        Assert.AreEqual("error1", result.ErrorMessage);
        var names = result.MemberNames.ToArray();
        Assert.AreEqual("field1", names?[0]);
        Assert.AreEqual("field2", names?[1]);
    }
}
