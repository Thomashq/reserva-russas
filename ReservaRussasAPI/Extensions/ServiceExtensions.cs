using Core.Services;
using RR.Core.Services;
using RR.Core.Services.Base;
using RR.Service;
using RR.Service.Service;

namespace ReservaRussasAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            // Registrar repositórios primeiro
            //services.AddServices();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IServantService, ServantService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IReservationSeriesService, ReservationSeriesService>();
            services.AddScoped<IStudentAdvisorService, StudentAdvisorService>();


            return services;
        }

    }
}
