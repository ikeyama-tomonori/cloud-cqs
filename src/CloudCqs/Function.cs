namespace CloudCqs;

public record Function(string Description, Func<object, Task<object>> Func);
