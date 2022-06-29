namespace CloudCqs.Test;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CancelTest
{
    [TestMethod]
    public async Task キャンセルを検知できること()
    {
        var source = new CancellationTokenSource();
        var token = source.Token;

        var query = new CancelQuery(Options.Instance);

        source.Cancel();
        var response = await query.Invoke(new("test"), token);
        Assert.AreEqual(true, response);
    }

    [TestMethod]
    public async Task FacadeからCancellationTokenが渡ること()
    {
        var source = new CancellationTokenSource();
        var token = source.Token;

        var query = new CancelFacade(Options.Instance, new CancelQuery(Options.Instance));

        source.Cancel();
        var response = await query.Invoke(new("test"), token);
        Assert.AreEqual(true, response);
    }
}
