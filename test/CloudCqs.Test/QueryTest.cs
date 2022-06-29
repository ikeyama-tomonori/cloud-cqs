namespace CloudCqs.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            () => query.Invoke(new("error1"))
        );
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
            () => query.Invoke(new("error2"))
        );
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
            () => query.Invoke(new("error3"))
        );
        var result = e.ValidationResult;
        Assert.AreEqual("error1", result?.ErrorMessage);
        var names = result?.MemberNames.ToArray();
        Assert.AreEqual("field1", names?[0]);
        Assert.AreEqual("field2", names?[1]);
    }
}
