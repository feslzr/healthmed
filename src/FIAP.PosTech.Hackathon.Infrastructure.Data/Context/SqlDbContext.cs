using FIAP.PosTech.Hackathon.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Infrastructure.Data.Context;

[ExcludeFromCodeCoverage]
public class SqlDbContext(DbContextOptions<SqlDbContext> options) : DbContext(options)
{
    public DbSet<Account> Account { get; set; }
    public DbSet<Appointment> Appointment { get; set; }
    public DbSet<Doctor> Doctor { get; set; }
    public DbSet<Patient> Patient { get; set; }
    public DbSet<Service> Service { get; set; }
    public DbSet<Schedule> Schedule { get; set; }
}