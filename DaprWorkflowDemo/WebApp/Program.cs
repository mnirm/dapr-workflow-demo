using Dapr.Client;
using Dapr.Workflow;
using Microsoft.AspNetCore.Mvc;
using WebApp.Activities;
using WebApp.Models;
using WebApp.Workflows.TestWorkflow;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<TestWorkflow>();
    
    options.RegisterActivity<NotifyActivity>();
    options.RegisterActivity<DocumentValidationActivity>();
});

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DAPR_GRPC_PORT")))
{
    Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", "4001");
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.MapGet("/approve/{instanceId}", async (string instanceId, [FromQuery] ApprovalResult approvalResult) =>
{
    using var daprClient = new DaprClientBuilder().Build();

    while (!await daprClient.CheckHealthAsync())
    {
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }
    
    /* Line 61 throws an exception
    Dapr.DaprException: Start Workflow operation failed: the Dapr endpoint indicated a failure. See InnerException for details.
    ---> Grpc.Core.RpcException: Status(StatusCode="Unknown", Detail="failed to proxy request: required metadata dapr-app-id not found")
    at Dapr.Client.DaprClientGrpc.RaiseWorkflowEventAsync(String instanceId, String workflowComponent, String eventName, Object eventData, CancellationToken cancellationToken)
    --- End of inner exception stack trace ---
    at Dapr.Client.DaprClientGrpc.RaiseWorkflowEventAsync(String instanceId, String workflowComponent, String eventName, Object eventData, CancellationToken cancellationToken)
     */
    await daprClient.RaiseWorkflowEventAsync(
        instanceId: instanceId,
        workflowComponent: "dapr",
        eventName: Events.ApprovalEvent,
        eventData: approvalResult);

    return new OkResult();
});

app.Run();