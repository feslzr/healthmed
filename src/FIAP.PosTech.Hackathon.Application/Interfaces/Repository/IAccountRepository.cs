using FIAP.PosTech.Hackathon.Domain.Entity;

namespace FIAP.PosTech.Hackathon.Application.Interfaces.Repository;

public interface IAccountRepository : IBaseRepository<Account>
{
    Task<Account> GetAccountByIdAsync(int id);
}