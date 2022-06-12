namespace CloudCqs;

public class CloudCqsOptions
{
    public Action<(Type repositoryType, object request, object response, TimeSpan timeSpan)> RepositoryExecuted { get; set; }
    public Action<(Type repositoryType, object request, Exception exception, TimeSpan timeSpan)> RepositoryTerminated { get; set; }
    public Action<(Type repositoryType, string description, object param, object result, TimeSpan timeSpan)> FunctionExecuted { get; set; }
    public Action<(Type repositoryType, string description, object param, Exception exception, TimeSpan timeSpan)> FunctionTerminated { get; set; }

    public CloudCqsOptions()
    {
        RepositoryExecuted = _ => { };
        RepositoryTerminated = _ => { };
        FunctionExecuted = _ => { };
        FunctionTerminated = _ => { };
    }
}
