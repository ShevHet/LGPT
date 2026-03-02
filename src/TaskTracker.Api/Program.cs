using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Services;
using TaskTracker.Api.Errors;
using TaskTracker.Api.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options => options.SuppressModelStateInvalidFilter = true);
    

builder.Services.Configure<TaskTrackerOptions>(
    builder.Configuration.GetSection(TaskTrackerOptions.SectionName));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<ITaskService, InMemoryTaskService>();

var app = builder.Build();
app.UseMiddleware<TaskTracker.Api.Middleware.ExceptionHandlingMiddleware>();
app.UseMiddleware<TaskTracker.Api.Middleware.RequestLoggingMiddleware>();

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (OperationCanceledException) when (context.RequestAborted
    .IsCancellationRequested)
    {
        context.Response.StatusCode = 499;
    }
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
