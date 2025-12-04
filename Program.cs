using Microsoft.AspNetCore.Builder;
using MonS3ApiLight.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Services
builder.Services.Configure<MonS3ApiLight.Config.S3Config>(
    builder.Configuration.GetSection("MoneyS3")
);
builder.Services.AddSingleton<MonS3ReaderService>();
builder.Services.AddSingleton<AddressService>();
builder.Services.AddSingleton<OrderService>();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new() { Title = "MonS3ApiLight API", Version = "v1" });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MonS3ApiLight API V1");
        c.RoutePrefix = string.Empty; // Swagger UI na root
    });
}

app.MapControllers();
app.Run();
