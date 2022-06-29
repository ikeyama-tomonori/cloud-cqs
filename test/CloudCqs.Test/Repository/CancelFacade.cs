namespace CloudCqs.Test;

using CloudCqs.Facade;

public class CancelFacade : Facade<string, bool>
{
    public CancelFacade(CloudCqsOptions option, IQuery<string, bool> cancelQuery) : base(option)
    {
        var handler = new Handler().Invoke(
            "キャンセルチェックの呼び出し",
            cancelQuery,
            _ => this.UseRequest(),
            p => p.Response
        );

        this.SetHandler(handler);
    }
}
