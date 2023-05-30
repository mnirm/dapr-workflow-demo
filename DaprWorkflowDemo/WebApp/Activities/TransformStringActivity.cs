using Dapr.Workflow;

namespace WebApp.Activities;

public class TransformStringActivity : WorkflowActivity<string, string>
{
    private readonly ILogger _logger;
    
    public TransformStringActivity(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<TransformStringActivity>();
    }
    
    public override async Task<string> RunAsync(WorkflowActivityContext context, string input)
    {
        _logger.LogInformation($"Instance ID: {context.InstanceId}, inputed {input} into {this.GetType().FullName}");
        
        await Task.Delay(1000);
        var random = new Random();
        var number = random.Next(1, 100);

        return input + "addedTextToString" + number;
    }
}