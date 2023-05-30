using Dapr.Workflow;
using WebApp.Activities;

namespace WebApp.Workflows.TestWorkflow;

public class TestWorkflow : Workflow<string, string>
{
    
    private readonly WorkflowTaskOptions _defaultActivityRetryOptions = new WorkflowTaskOptions
    {
        // NOTE: Beware that changing the number of retries is a breaking change for existing workflows.
        RetryPolicy = new WorkflowRetryPolicy(
            maxNumberOfAttempts: 3,
            firstRetryInterval: TimeSpan.FromSeconds(5)),
    };
    
    public override async Task<string> RunAsync(WorkflowContext context, string input)
    {
        await context.CallActivityAsync<TransformStringActivity>(
            nameof(TransformStringActivity),
            input,
            _defaultActivityRetryOptions);

        return "Done";
    }
}