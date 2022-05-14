using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CloudCqs.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CloudCqs.Test;

[TestClass]
public class DataAnnotationTest
{
    [TestMethod]
    public async Task DataAnnotationValidatorでエラーになる()
    {
        var request = new DataTestCommand.Request(Name: "123456");
        var command = new DataTestCommand(Options.Instance);
        var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(
            async () => await command.Invoke(request));
        Assert.IsNotNull(exception.Errors?["Name"][0]);
    }

    [TestMethod]
    public async Task DataAnnotationValidatorでエラーにならない()
    {
        var request = new DataTestCommand.Request(Name: "12345");
        var command = new DataTestCommand(Options.Instance);
        var response = await command.Invoke(request);
        Assert.AreEqual(typeof(Void), response.GetType());
    }

    public class DataTestCommand : Command<DataTestCommand.Request>
    {
        public record Request([property: MaxLength(5)] string Name);

        public DataTestCommand(CloudCqsOptions option) : base(option)
        {
            SetHandler(new Handler().Build());
        }
    }
}
