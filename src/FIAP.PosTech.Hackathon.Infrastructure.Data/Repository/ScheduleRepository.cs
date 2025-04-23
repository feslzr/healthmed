using FIAP.PosTech.Hackathon.Application.Boundaries.Customer;
using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Domain.Enums;
using FIAP.PosTech.Hackathon.Domain.Extensions;
using FIAP.PosTech.Hackathon.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Infrastructure.Data.Repository;

[ExcludeFromCodeCoverage]
public class ScheduleRepository(SqlDbContext context) : BaseRepository<Schedule>(context), IScheduleRepository
{
    public async Task<Schedule> GetByServiceAsync(int serviceId)
        => await _dbSet.Where(s => s.ServiceId == serviceId).FirstAsync();

    public async Task<List<MedicalServiceOutput>> GetMedicalServicesAsync(
        string? doctorName,
        int? specialtyId,
        DateTime? startDate,
        DateTime? endDate,
        decimal? price,
        int? statusId)
    {
        var result = await _context.Account
            .Join(_context.Doctor,
                ac => ac.Id,
                dr => dr.AccountId,
                (ac, dr) => new { ac, dr })
            .Join(_context.Service,
                temp => temp.dr.Id,
                sv => sv.DoctorId,
                (temp, sv) => new { temp.ac, temp.dr, sv })
            .Join(_context.Schedule,
                temp => temp.sv.Id,
                sc => sc.ServiceId,
                (temp, sc) => new { temp.ac, temp.dr, temp.sv, sc })
            .GroupJoin(_context.Appointment,
                temp => temp.sc.Id,
                ap => ap.ScheduleId,
                (temp, ap) => new { temp.ac, temp.dr, temp.sv, temp.sc, appointments = ap.DefaultIfEmpty() })
            .SelectMany(
                temp => temp.appointments,
                (temp, ap) => new { temp.ac, temp.dr, temp.sv, temp.sc, ap })
            .Where(x =>
                (string.IsNullOrEmpty(doctorName) || x.ac.Name.Contains(doctorName)) &&
                (!specialtyId.HasValue || x.sv.SpecialtyId == specialtyId.Value) &&
                (!price.HasValue || x.sv.Price == price.Value) &&
                (!startDate.HasValue || !endDate.HasValue || (x.sc.Datetime >= startDate.Value && x.sc.Datetime <= endDate.Value)) &&
                (!statusId.HasValue || (x.ap != null && x.ap.StatusId == statusId.Value)))
            .Select(x => new
            {
                x.sc.Id,
                x.ac.Name,
                x.sv.SpecialtyId,
                x.sv.Price,
                x.sc.Datetime,
                StatusId = x.ap != null ? x.ap.StatusId : (int?)null
            })
            .ToListAsync();

        return [.. result.Select(x => new MedicalServiceOutput
        {
            ScheduleId = x.Id,
            DoctorName = x.Name,
            SpecialtyId = x.SpecialtyId,
            SpecialtyDescription = ((MedicalSpecialtyEnum)x.SpecialtyId).GetDescription(),
            AppointmentStatusId = x.StatusId,
            AppointmentStatusDescription = x.StatusId.HasValue
                ? ((AppointmentStatusEnum)x.StatusId.Value).GetDescription()
                : string.Empty,
            Price = x.Price,
            Datetime = x.Datetime,
        })];
    }
}