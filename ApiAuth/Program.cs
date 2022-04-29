using ApiAuth.Auth;
using ApiAuth.Auth.Data;
using ApiAuth.Auth.Endpoints;
using ApiAuth.Common;
using ApiAuth.Services;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

ApplicationLoggerFactory.CreateLogger(builder.Configuration, builder.Environment);

var services = builder.Services;
//var env = builder.Environment;

services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSnakeCaseNamingConvention();
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName));
});
services.AddCors();
services.AddControllers(options => { options.UseNamespaceRouteToken(); })
    .AddFluentValidation(s => { s.RegisterValidatorsFromAssemblyContaining<RegisterAcoountRequestValidator>(); });

services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = ".NET API Auth", Version = "v1" });
    //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "SampleEndpointApp.xml"));
    c.UseApiEndpoints();
});

services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

services.AddScoped<ITokensService, TokensService>();
services.AddScoped<IMailService, MailService>();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", ".NET API Auth"));
app.UseRouting();

app.UseMiddleware<JwtMiddleware>();

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
app.MapGet("/", () => "Hello World!");

app.Run();