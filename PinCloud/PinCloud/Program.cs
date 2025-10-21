using APinI.BE;
using APinI.Repository;
using APinI.Schedular;
using APinI.Schedular.Jobs;
using APinI.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure file upload size limits
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB
});
builder.Services.AddSingleton<ICicdService, CicdService>();
builder.Services.AddSingleton<IPowerShellService, PowerShellService>();
builder.Services.AddSingleton<UpdateLocalWebsiteIpAddress>();
builder.Services.AddHostedService<UpdateLocalWebsiteIpAddress>();
builder.Services.AddSingleton<IHttpClientService, HttpClientService>();
builder.Services.AddSingleton<IIQOptionService, IQOptionService>();
builder.Services.AddSingleton<SchedulerHealthCheck>();
builder.Services.AddSingleton<IIQOptionService, IQOptionService>();
builder.Services.AddSingleton<PinDataRepository>();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      });
});

var app = builder.Build();

new DeviceSecheduler().Run();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Enable default static file serving from wwwroot
app.UseStaticFiles();

// Enable static file serving from persistent uploads directory
var uploadsBasePath = app.Configuration["FileUpload:BasePath"] ?? 
                     Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
var uploadsPath = Path.Combine(uploadsBasePath, "APinI_Uploads");

if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();
app.MapRazorPages();
app.UseWebSockets();

app.Run();
