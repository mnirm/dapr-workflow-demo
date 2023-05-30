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
    
    await daprClient.RaiseWorkflowEventAsync(
        instanceId: instanceId,
        workflowComponent: "dapr",
        eventName: Events.ApprovalEvent,
        eventData: approvalResult);

    return new OkResult();
});

app.Run();