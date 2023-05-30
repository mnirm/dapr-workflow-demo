using Dapr.Workflow;
using WebApp.Activities;
using WebApp.Models;

namespace WebApp.Workflows.TestWorkflow;

public class TestWorkflow : Workflow<string, DocumentProcessedResult>
{
    public override async Task<DocumentProcessedResult> RunAsync(WorkflowContext context, string input)
    {
        await context.CallActivityAsync(
            nameof(NotifyActivity),
            new Notification($"Started workflow with instance ID: {context.InstanceId}"));

        var validatedDocument = await context.CallActivityAsync<ValidatedDocumentResult>(
            nameof(DocumentValidationActivity),
            new DocumentToValidate("Test String", 2));

        await context.CallActivityAsync(
            nameof(NotifyActivity),
            new Notification($"Processed Document with name {validatedDocument.DocumentName}"));

        ApprovalResult approvalResult = await context.WaitForExternalEventAsync<ApprovalResult>(
            eventName: Events.ApprovalEvent);
        
        context.SetCustomStatus($"Approval result: {approvalResult}");

        return approvalResult == ApprovalResult.Rejected ? new DocumentProcessedResult(false) : new DocumentProcessedResult(true);
    }
}