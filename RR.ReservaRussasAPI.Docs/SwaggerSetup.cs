using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Models;
using Microsoft.OpenApi.Models;

namespace RR.ReservaRussasAPI.Docs
{
    public static class SwaggerSetup
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Reserva de Salas API",
                    Version = "v1",
                    Description = "Documentação da API para o sistema de reservas de salas."
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfigurationReservaRussas(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reserva de Salas API v1");
                c.RoutePrefix = "docs"; // Swagger disponível em /docs
            });

            return app;
        }
    }
}
