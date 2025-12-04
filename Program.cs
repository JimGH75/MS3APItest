using MonS3ApiLight.Services;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// load json config (appsettings.json)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.Configure<MonS3ApiLight.Config.S3Config>(builder.Configuration.GetSection("MoneyS3"));
builder.Services.AddSingleton<MonS3ReaderService>();
builder.Services.AddSingleton<AddressService>();
builder.Services.AddSingleton<OrderService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowAll");
app.UseStaticFiles(); // serve wwwroot

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
