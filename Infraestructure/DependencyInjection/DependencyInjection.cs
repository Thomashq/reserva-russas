using Core.Repositories;
using Core.Services;
using Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using RR.Core.Repositories;
using RR.Core.Services;
using RR.Infraestructure.Repositories;
using RR.Service.Service;

namespace RR.Infraestructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Account>, AccountRepository>();
            services.AddScoped<IRepository<Manager>, ManagerRepository>();
            services.AddScoped<IRepository<Student>, StudentRepository>();
            services.AddScoped<IRepository<Servant>, ServantRepository>();
            services.AddScoped<IRepository<Rooms>, RoomsRepository>();
            services.AddScoped<IRepository<Reservation>, ReservationRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IServantService, ServantService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPasswordService, PasswordService>();
            return services;
        }
    }
}
