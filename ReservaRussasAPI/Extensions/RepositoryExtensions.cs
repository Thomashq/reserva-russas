using RR.Core.Repositories;
using RR.Infraestructure.Repositories;
using RR.Infrastructure.Repositories;

namespace ReservaRussasAPI.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Registros manuais específicos (opcional)
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>(); 
            services.AddScoped<IServantRepository, ServantRepository>(); 
            // services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }

    }
} 
