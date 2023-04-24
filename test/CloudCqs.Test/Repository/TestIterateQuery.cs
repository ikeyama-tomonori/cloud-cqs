namespace CloudCqs.Test;

using CloudCqs.Query;

public class TestIterateQuery : Query<string, string[]>
{
    public TestIterateQuery(CloudCqsOptions option) : base(option)
    {
        var exec = new Handler()
            .Then("配列の作成1", _ => Enumerable.Range(0, 10))
            .ThenIterate(
                "非同期処理で値を返さない",
                async p =>
                {
                    await Task.Run(() => { });
                }
            )
            .Then("配列の作成2", _ => Enumerable.Range(0, 10))
            .ThenIterate("同期処理で値を返さない", _ => { })
            .Then("配列の作成3", _ => Enumerable.Range(0, 10))
            .ThenIterate(
                "非同期処理で値を返す",
                async p =>
                {
                    var req = this.UseRequest();
                    return await Task.Run(() => $"{req}: {p}");
                }
            )
            .ThenIterate("同期処理で値を返す", p => p)
            .Then("応答データ作成", p => p.ToArray());

        this.SetHandler(exec);
    }
}
