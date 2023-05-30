using Dapr.Workflow;
using WebApp.Activities;
using WebApp.Workflows.TestWorkflow;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDaprWorkflow(options =>
{
    options.RegisterWorkflow<TestWorkflow>();
    options.RegisterActivity<TransformStringActivity>();
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

app.Run();