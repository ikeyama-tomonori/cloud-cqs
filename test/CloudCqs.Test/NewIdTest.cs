namespace CloudCqs.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            () => create.Invoke(new("error"))
        );
        var result = e.ValidationResult;
        Assert.AreEqual("error1", result?.ErrorMessage);
        var names = result?.MemberNames.ToArray();
        Assert.AreEqual("field1", names?[0]);
        Assert.AreEqual("field2", names?[1]);
    }
}
