using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ReservaRussasAPI.Controllers.Base;
using ReservaRussasAPI.Extensions;
using RR.Core.Entities;
using RR.Infraestructure.DataContext;
using RR.ReservaRussasAPI.Docs;
using RR.Util.Criptography;
using RR.Core.Enums;
using ReservaRussasAPI.Attributes;
using ReservaRussasAPI.Handler;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver() { };
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerDocumentation();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCredentials", policy =>
    {
        policy.WithOrigins(
#if DEBUG
            "http://localhost:4200", "https://localhost:4200"
#else 
            "https://*.ufc.br", "http://*.ufc.br"
#endif
            )
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
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

// Configuração Identity completa com Entity Framework - IDs string
builder.Services
    .AddIdentity<AppUser, Microsoft.AspNetCore.Identity.IdentityRole>(o =>
    {
        o.User.RequireUniqueEmail = true;
        o.Password.RequiredLength = 8;
        o.Password.RequireNonAlphanumeric = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireDigit = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IPasswordHasher<AppUser>, Pbkdf2PasswordHasherAdapter>();

builder.Services.AddHttpContextAccessor();

// Configuração de Cookie Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.AccessDeniedPath = "/auth/access-denied";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None; // Necessário para CORS
    options.Cookie.Name = "ReservaRussas.Auth";

    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
            context.Response.Headers.Append("Content-Type", "application/json");
            return context.Response.WriteAsync(JsonConvert.SerializeObject(new CustomResult(System.Net.HttpStatusCode.Unauthorized, false, $"Você não está autorizado a usar o endpoint [ {context.Request.Path} ].")));
        }
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 403;
            context.Response.Headers.Append("Content-Type", "application/json");
            return context.Response.WriteAsync(JsonConvert.SerializeObject(new CustomResult(System.Net.HttpStatusCode.Forbidden, false, "Acesso negado.")));
        }
        return Task.CompletedTask;
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

builder.Services.AddScoped<IAuthorizationHandler, MinPermissionHandler>();

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("AdminOrAbove",
    p => p.Requirements.Add(new MinPermissionRequirement(EAccountPermission.Admin)));

  options.AddPolicy("ManagerOrAbove",
    p => p.Requirements.Add(new MinPermissionRequirement(EAccountPermission.Manager)));

  options.AddPolicy("ServantOrAbove",
    p => p.Requirements.Add(new MinPermissionRequirement(EAccountPermission.Servant)));

  options.AddPolicy("StudentOrAbove",
    p => p.Requirements.Add(new MinPermissionRequirement(EAccountPermission.Student)));
});

var app = builder.Build();

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    using (var context = scope.ServiceProvider.GetService<ApplicationDbContext>())
    {
        context.Database.SetCommandTimeout((int)TimeSpan.FromMinutes(20).TotalSeconds);
        context.Database.Migrate();
    }
}

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    await RR.Infraestructure.Seeding.AdminSeeder.SeedAdminAsync(sp);
    await RR.Infraestructure.Seeding.ManagerSeeder.SeedManagersAsync(sp);
    await RR.Infraestructure.Seeding.RoomSeeder.SeedRoomsAsync(sp);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfigurationReservaRussas();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}



app.UseCors("AllowCredentials");
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
