using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // UserOnlyStore<>
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReservaRussasAPI.Extensions;
using RR.Core.Entities;                    
using RR.Infraestructure.DataContext;
using RR.ReservaRussasAPI.Docs;
using RR.Util.Criptography;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

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
string str_conexao =
    $"Host={reservaRussasConnectString.Host};Port={reservaRussasConnectString.Port};Database={reservaRussasConnectString.DataBase};Username={reservaRussasConnectString.UserName};Password={reservaRussasConnectString.Password}";

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

// >>> Identity Core (sem UI/cookies) + UserOnlyStore (sem roles por enquanto)
builder.Services
    .AddIdentityCore<AppUser>(o =>
    {
        o.User.RequireUniqueEmail = true;
        o.Password.RequiredLength = 8;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireDigit = false;
    })
    .AddSignInManager(); 

builder.Services.AddScoped<IUserStore<AppUser>, UserOnlyStore<AppUser, ApplicationDbContext, int>>();

builder.Services.AddSingleton<IPasswordHasher<AppUser>, Pbkdf2PasswordHasherAdapter>();

builder.Services.AddHttpContextAccessor();

// >>> Autenticação JWT (mantida como está)
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
            ClockSkew = TimeSpan.Zero
        };
    });

//Versionamento das APIs
builder.Services.AddApiVersioning(p =>
{
    p.DefaultApiVersion = new Asp.Versioning.ApiVersion(1.0);
    p.ReportApiVersions = true;
    p.AssumeDefaultVersionWhenUnspecified = true;
})
    .AddApiExplorer(p =>
    {
        p.GroupNameFormat = "'v'VVV";
        p.SubstituteApiVersionInUrl = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfigurationReservaRussas();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
    {
        context.Database.SetCommandTimeout((int)TimeSpan.FromMinutes(20).TotalSeconds);
        context.Database.Migrate();
    }
}

app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

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
