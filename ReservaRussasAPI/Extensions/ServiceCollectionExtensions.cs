namespace ReservaRussasAPI.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrar todas as configurações de DI
            services.AddRepositories();
            services.AddServices();
            //services.AddValidators();
            //services.AddMappers();

            return services;
        }
    }
}
