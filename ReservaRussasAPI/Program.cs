using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservaRussasAPI.Extensions;
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

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
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

// APLICAR MIGRATIONS AUTOMATICAMENTE
try
{
    await app.Services.EnsureDatabaseMigratedAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Falha crítica ao aplicar migrations. A aplicação será encerrada.");

    // Em produção, você pode querer continuar sem o banco ou implementar retry logic
    if (app.Environment.IsProduction())
    {
        logger.LogError("Aplicação continuará sem conexão com banco de dados.");
    }
    else
    {
        throw; // Em desenvolvimento, pare a aplicação
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfigurationReservaRussas();
}
else
{
    app.UseExceptionHandler("/Home/Error");
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