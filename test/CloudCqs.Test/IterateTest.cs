namespace CloudCqs.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class IterateTest
{
    [TestMethod]
    public async Task 正常終了すること()
    {
        var query = new TestIterateQuery(Options.Instance);
        var response = await query.Invoke("test");
        Assert.AreEqual("test: 0", response[0]);
        Assert.AreEqual("test: 9", response[9]);
    }
}
