using CloudCqs.Command;
using Microsoft.Extensions.Logging;
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
            var logContext = new LogContext();
            var request = new DataTestCommand.Request("123456");
            var command = new DataTestCommand(logContext);
            await Assert.ThrowsExceptionAsync<ValidationException>(
                async () => await command.Invoke(request));
        }

        [TestMethod]
        public async Task DataAnnotationValidatorでエラーにならない()
        {
            var logContext = new LogContext();
            var request = new DataTestCommand.Request("12345");
            var command = new DataTestCommand(logContext);
            await command.Invoke(request);
        }

        public class DataTestCommand : Command<DataTestCommand.Request>
        {
            public record Request([property: MaxLength(5)] string Name);

            public DataTestCommand(LogContext logContext) : base(logContext)
            {
                SetExecution(new Execution().Build());
            }
        }
    }
}
