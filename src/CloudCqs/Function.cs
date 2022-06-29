namespace CloudCqs;

public record Function(string Description, Func<object, CancellationToken, ValueTask<object>> Func);
