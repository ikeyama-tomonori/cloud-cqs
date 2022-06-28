namespace CloudCqs.Test;

using System.Threading.Tasks;
using CloudCqs.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class NullRepository : Query<object, object>
{
    public NullRepository(CloudCqsOptions option) : base(option)
    {
        var handler = new Handler()
            .Then("Do nothing", _ => { })
            .Then(
                "Do nothing async",
                async _ =>
                {
                    await Task.Run(() => { });
                }
            )
            .Then("Return", _ => new object());
        this.SetHandler(handler);
    }
}
