using System.Reflection;
using TaskTracker.Application.Options;
using TaskTracker.Application;
using TaskTracker.Infrastructure;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = false;
    });

builder.Services.Configure<TaskTrackerOptions>(
    builder.Configuration.GetSection(TaskTrackerOptions.SectionName));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskTracker API",
        Version = "v1",
        Description = "API for tasks and projects."
    });

    options.SupportNonNullableReferenceTypes();

    var xmlFiles = new[]
    {
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",
        "TaskTracker.Application.xml"
    };

    foreach(var xmlFile in xmlFiles)
    {
        var xmLPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if(File.Exists(xmLPath))
        {
            options.IncludeXmlComments(xmLPath);
        }
    }
});

var app = builder.Build();
app.UseMiddleware<TaskTracker.Api.Middleware.ExceptionHandlingMiddleware>();
app.UseMiddleware<TaskTracker.Api.Middleware.RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }