using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservaRussasAPI.Extensions;
using RR.Core.Entities;
using RR.Infraestructure.DataContext;
using RR.ReservaRussasAPI.Docs;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerDocumentation();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddApplicationServices();

ReservaRussasConnectString reservaRussasConnectString = new();
reservaRussasConnectString = builder.Configuration.GetSection("Connection").Get<ReservaRussasConnectString>();
string str_conexao = $"Host={reservaRussasConnectString.Host};Port={reservaRussasConnectString.Port};Database={reservaRussasConnectString.DataBase};Username={reservaRussasConnectString.UserName};Password={reservaRussasConnectString.Password}";

// CORREÇÃO: Configuração do DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(str_conexao, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("RR.Infraestructure");
        npgsqlOptions.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);
    });

    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
});

// Configuração JWT apenas para autenticação (sem autorização por roles)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
            ClockSkew = TimeSpan.Zero // Remove tolerância de tempo padrão
        };
    });

// Removidas as políticas de autorização baseadas em roles
// Agora só precisamos verificar se o usuário está autenticado
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfigurationReservaRussas();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

// Executar migrações
using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
    {
        context.Database.SetCommandTimeout((int)TimeSpan.FromMinutes(20).TotalSeconds);
        context.Database.Migrate();
    }
}

app.UseCors("AllowAllOrigins"); // Especificar a política CORS
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// IMPORTANTE: A ordem é crucial - Authentication antes de Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync($"API FUNCIONANDO E INICIADA EM {DateTime.Now}", default);
    });
});

app.Run();