using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Infrastructure.Data.Repository;

[ExcludeFromCodeCoverage]
public class AccountRepository(SqlDbContext context) : BaseRepository<Account>(context), IAccountRepository
{
    public async Task<Account> GetAccountByIdAsync(int id)
        => await _dbSet.Where(c => c.Id == id)
                       .FirstAsync();
}