namespace CloudCqs;

public record Function(string Description, Func<object, CancellationToken, Task<object>> Func);
