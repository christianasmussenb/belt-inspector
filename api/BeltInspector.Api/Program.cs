using Amazon.S3;
using BeltInspector.Api.Data;
using BeltInspector.Api.Services;
using BeltInspector.Api.Settings;
using Amazon.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? builder.Configuration["DB_CONNECTION_STRING"]
                      ?? "Host=localhost;Database=beltinspector;Username=postgres;Password=postgres";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.Configure<R2Options>(options =>
{
    builder.Configuration.GetSection("R2").Bind(options);
    options.Endpoint = builder.Configuration["R2_ENDPOINT"] ?? options.Endpoint;
    options.Bucket = builder.Configuration["R2_BUCKET"] ?? options.Bucket;
    options.AccessKey = builder.Configuration["R2_ACCESS_KEY"] ?? options.AccessKey;
    options.SecretKey = builder.Configuration["R2_SECRET_KEY"] ?? options.SecretKey;
});
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var options = sp.GetRequiredService<IOptions<R2Options>>().Value;

    var config = new AmazonS3Config
    {
        ServiceURL = options.Endpoint,
        ForcePathStyle = true,
    };

    return string.IsNullOrWhiteSpace(options.AccessKey) || string.IsNullOrWhiteSpace(options.SecretKey)
        ? new AmazonS3Client(new AnonymousAWSCredentials(), config)
        : new AmazonS3Client(options.AccessKey, options.SecretKey, config);
});
builder.Services.AddScoped<IFileStorageService, R2FileStorageService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(policy =>
{
    policy.AddDefaultPolicy(p =>
        p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.Run();

namespace BeltInspector.Api.Settings
{
    public class R2Options
    {
        public string Endpoint { get; set; } = "https://8d2ac30c8aa0d8419833f740e39e61c6.r2.cloudflarestorage.com";
        public string Bucket { get; set; } = "belt-inspector";
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
}
