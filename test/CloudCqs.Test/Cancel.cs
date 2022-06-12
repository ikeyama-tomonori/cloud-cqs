using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudCqs.Test.CancelQuery
{
    using CloudCqs.Query;

    public class CancelQuery : Query<string, bool>
    {
        public CancelQuery(CloudCqsOptions option) : base(option)
        {
            var handler = new Handler()
                .Then("キャンセルのチェック", _ =>
                {
                    var cancel = UseCancellationToken();
                    return Task.FromResult(cancel.IsCancellationRequested);
                })
                .Build();

            SetHandler(handler);
        }
    }
}

namespace CloudCqs.Test.CancelFacade
{
    using CloudCqs.Facade;

    public class CancelFacade : Facade<string, bool>
    {
        public CancelFacade(CloudCqsOptions option, IQuery<string, bool> cancelQuery) : base(option)
        {
            var handler = new Handler()
                .Invoke("キャンセルチェックの呼び出し",
                cancelQuery,
                _ => UseRequest(),
                p => p.response)
                .Build();

            SetHandler(handler);
        }
    }
}

namespace CloudCqs.Test
{
    [TestClass]
    public class CancelTest
    {
        [TestMethod]
        public async Task キャンセルを検知できること()
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var query = new CancelQuery.CancelQuery(Options.Instance);

            source.Cancel();
            var response = await query.Invoke(new("test"), token);
            Assert.AreEqual(true, response);
        }

        [TestMethod]
        public async Task FacadeからCancellationTokenが渡ること()
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var query = new CancelFacade.CancelFacade(Options.Instance,
                new CancelQuery.CancelQuery(Options.Instance));

            source.Cancel();
            var response = await query.Invoke(new("test"), token);
            Assert.AreEqual(true, response);
        }

    }
}
