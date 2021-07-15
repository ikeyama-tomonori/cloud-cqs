using CloudCqs.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CloudCqs.Test
{
    [TestClass]
    public class DataAnnotationTest
    {
        [TestMethod]
        public async Task DataAnnotationValidatorでエラーになる()
        {
            var option = new CloudCqsOptions();
            var request = new DataTestCommand.Request(Name: "123456");
            var command = new DataTestCommand(option);
            var exception = await Assert.ThrowsExceptionAsync<BadRequestException>(
                async () => await command.Invoke(request));
            Assert.IsNotNull(exception.Errors?["Name"][0]);
        }

        [TestMethod]
        public async Task DataAnnotationValidatorでエラーにならない()
        {
            var option = new CloudCqsOptions();
            var request = new DataTestCommand.Request(Name: "12345");
            var command = new DataTestCommand(option);
            var response = await command.Invoke(request);
            Assert.AreEqual(typeof(object), response.GetType());
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
}
