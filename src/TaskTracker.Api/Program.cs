var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger services (§£§¡§¨§¯§°)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger middleware (§£§¡§¨§¯§°)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// §£§¡§¨§¯§°: §Ö§ã§Ý§Ú §å §ä§Ö§Ò§ñ §ä§à§Ý§î§Ü§à http §Ú §â§å§Ô§Ñ§Ö§ä§ã§ñ §ß§Ñ https ¡ª §Ó§â§Ö§Þ§Ö§ß§ß§à §Ó§í§Ü§Ý§ð§é§Ú §â§Ö§Õ§Ú§â§Ö§Ü§ä
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
