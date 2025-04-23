using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Infrastructure.Data.Repository;

[ExcludeFromCodeCoverage]
public class AppointmentRepository(SqlDbContext context) : BaseRepository<Appointment>(context), IAppointmentRepository
{
    public async Task<Appointment?> GetByPatientIdAndScheduleIdAndStatusNullableAsync(int patientId, int scheduleId, int[] statusId)
        => await _dbSet.Where(a => a.PatientId == patientId
                                && a.ScheduleId == scheduleId
                                && statusId.Contains(a.StatusId))
                       .FirstOrDefaultAsync();

}