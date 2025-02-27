using Core.Repositories;
using Core.Services;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using RR.Infraestructure.Repositories;
using RR.Service.Service;

namespace RR.Infraestructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Account>, AccountRepository>();
            services.AddScoped<IAccountService, AccountService>();

            return services;
        }
    }
}
