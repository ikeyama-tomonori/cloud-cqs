namespace CloudCqs
{
    public record ValidationError
    {
        public (string Field, string Message)[] Details { get; }

        public ValidationError(params (string Field, string Message)[] details)
        {
            Details = details;
        }
    }
}
