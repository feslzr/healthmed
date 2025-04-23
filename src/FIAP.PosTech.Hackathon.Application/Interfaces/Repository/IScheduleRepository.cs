using FIAP.PosTech.Hackathon.Application.Boundaries.Customer;
using FIAP.PosTech.Hackathon.Domain.Entity;

namespace FIAP.PosTech.Hackathon.Application.Interfaces.Repository;

public interface IScheduleRepository : IBaseRepository<Schedule>
{
    Task<Schedule> GetByServiceAsync(int serviceId);

    Task<List<MedicalServiceOutput>> GetMedicalServicesAsync(string? doctorName,
                                                             int? specialtyId,
                                                             DateTime? startDate,
                                                             DateTime? endDate,
                                                             decimal? price,
                                                             int? statusId);
}