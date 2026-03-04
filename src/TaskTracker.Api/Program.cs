using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Services;
using TaskTracker.Api.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

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

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
