using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Infrastructure.Data.Repository;

[ExcludeFromCodeCoverage]
public class ServiceRepository(SqlDbContext context) : BaseRepository<Service>(context), IServiceRepository
{
    public async Task<Service?> GetByDoctorIdAndSpecialtyNullableAsync(int doctorId, int specialtyId)
        => await _dbSet.Where(s => s.DoctorId == doctorId && s.SpecialtyId == specialtyId).FirstOrDefaultAsync();

    public async Task<Service> GetByDoctorIdAndSpecialtyAsync(int doctorId, int specialtyId)
        => await _dbSet.Where(s => s.DoctorId == doctorId && s.SpecialtyId == specialtyId).FirstAsync();
}