using FIAP.PosTech.Hackathon.Domain.Entity;

namespace FIAP.PosTech.Hackathon.Application.Interfaces.Repository;

public interface IDoctorRepository : IBaseRepository<Doctor>
{
    Task<Doctor?> GetByDocumentNumberAsync(string documentNumber);

    Task<int> GetAccountIdByDocumentNumberAsync(string documentNumber);
}