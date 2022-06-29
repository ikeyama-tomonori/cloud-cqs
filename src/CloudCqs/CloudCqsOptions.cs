namespace CloudCqs;

public class CloudCqsOptions
{
    public CloudCqsOptions()
    {
        this.RepositoryExecuted = _ => { };
        this.RepositoryTerminated = _ => { };
        this.FunctionExecuted = _ => { };
        this.FunctionTerminated = _ => { };
    }

    public Action<(Type RepositoryType, object Request, object Response, TimeSpan TimeSpan)> RepositoryExecuted { get; set; }
    public Action<(Type RepositoryType, object Request, Exception Exception, TimeSpan TimeSpan)> RepositoryTerminated { get; set; }
    public Action<(Type RepositoryType, string Description, object Param, object Result, TimeSpan TimeSpan)> FunctionExecuted { get; set; }
    public Action<(Type RepositoryType, string Description, object Param, Exception Exception, TimeSpan TimeSpan)> FunctionTerminated { get; set; }
}
