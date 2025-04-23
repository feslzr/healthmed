using FIAP.PosTech.Hackathon.Domain.Entity;

namespace FIAP.PosTech.Hackathon.Application.Interfaces.Repository;

public interface IPatientRepository : IBaseRepository<Patient>
{
    Task<Patient?> GetByDocumentNumberAsync(string documentNumber);

    Task<int> GetAccountIdByLoginAsync(string login);

    Task<Patient?> GetByDocumentNumberOrEmailAsync(string documentNumber, string email);
}