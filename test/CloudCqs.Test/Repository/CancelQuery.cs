namespace CloudCqs.Test;

using System.Threading.Tasks;

using CloudCqs.Query;

public class CancelQuery : Query<string, bool>
{
    public CancelQuery(CloudCqsOptions option) : base(option)
    {
        var handler = new Handler().Then(
            "キャンセルのチェック",
            _ =>
            {
                var cancel = this.UseCancellationToken();
                return ValueTask.FromResult(cancel.IsCancellationRequested);
            }
        );

        this.SetHandler(handler);
    }
}
