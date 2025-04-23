using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Infrastructure.Data.Context;
using FIAP.PosTech.Hackathon.Infrastructure.Data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Infrastructure.Data.DependencyInjectionExtension;

[ExcludeFromCodeCoverage]
public static class InfraConfiguration
{
    public static IServiceCollection AddInfraConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();

        services.AddDbContext<SqlDbContext>(options => options.UseSqlServer(configuration["Database:ConnectionString"]), ServiceLifetime.Scoped);

        return services;
    }
}