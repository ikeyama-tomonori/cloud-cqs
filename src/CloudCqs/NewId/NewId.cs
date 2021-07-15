namespace CloudCqs.NewId
{
    public abstract class NewId<TRequest, TKey> : Repository<TRequest, TKey>, INewId<TRequest, TKey>
        where TRequest : notnull
        where TKey : notnull
    {
        protected class Handler : Handler<TRequest, TKey>
        {
        }

        protected NewId(CloudCqsOptions option) : base(option)
        {
        }
    }
}
