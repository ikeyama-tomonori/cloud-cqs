namespace CloudCqs.Test;

using System.Threading.Tasks;
using CloudCqs.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class NullRepositoryTest
{
    [TestMethod]
    public async Task Invoke_null_repository()
    {
        var repository = new NullRepository(Options.Instance);
        var response = await repository.Invoke(new object());
        Assert.IsNotNull(response);
    }
}
