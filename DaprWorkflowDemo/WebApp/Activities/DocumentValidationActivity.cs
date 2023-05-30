using Dapr.Workflow;
using WebApp.Models;

namespace WebApp.Activities;

public class DocumentValidationActivity : WorkflowActivity<DocumentToValidate, ValidatedDocumentResult>
{
    private readonly ILogger _logger;
    
    public DocumentValidationActivity(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<DocumentValidationActivity>();
    }

    public override Task<ValidatedDocumentResult> RunAsync(WorkflowActivityContext context, DocumentToValidate input)
    {
        return Task.FromResult(new ValidatedDocumentResult(input.Text, input.Pages, Guid.NewGuid()));
    }
}