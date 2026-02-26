using System.Reflection;
using TaskTracker.Api.Services;
using TaskTracker.Api.Errors;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var traceId = context.HttpContext.TraceIdentifier;

            var errors = context.ModelState
            ( 
                 .Where(x => x.Value?.Errors.Count > 0)
                 .ToDictionary(
                     k => k.Key,
                     v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

            var payload = new ApiErrorsResponse(
                TraceId: traceId,
                Message: "Validation failed",
                Errors: errors
            );

        };
    }

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
app.UseMiddleware<TaskTracker.Api.Middleware.RequestLoggingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
