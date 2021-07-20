using System.Threading.Tasks;
using CloudCqs.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudCqs.Test
{
    public class NullRepository : Query<object, object>
    {
        public NullRepository(CloudCqsOptions option) : base(option)
        {
            var handler = new Handler()
                .Then("Do nothing", () => { })
                .Then("Do nothing async", async () => { await Task.Run(() => { }); })
                .Then("Return", () => new object())
                .Build();
            SetHandler(handler);
        }
    }

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
}
