namespace CloudCqs.Test;

using CloudCqs.Command;

public class TestCommand : Command<TestCommand.Request>
{
    public TestCommand(CloudCqsOptions option) : base(option)
    {
        var handler = new Handler()
            .Then("データ取得", _ => this.UseRequest())
            .Validate(
                "データをチェック",
                p =>
                {
                    if (p.Name == "error")
                    {
                        return new("error1", new[] { "field1", "field2" });
                    }
                    return null;
                }
            )
            .Then("値を返さないために必要", _ => { });

        this.SetHandler(handler);
    }

    public record Request(string Name);
}
