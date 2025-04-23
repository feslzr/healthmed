using FIAP.PosTech.Hackathon.Domain.Entity;

namespace FIAP.PosTech.Hackathon.Application.Interfaces.Repository;

public interface IServiceRepository : IBaseRepository<Service>
{
    Task<Service?> GetByDoctorIdAndSpecialtyNullableAsync(int doctorId, int specialtyId);

    Task<Service> GetByDoctorIdAndSpecialtyAsync(int doctorId, int specialtyId);
}