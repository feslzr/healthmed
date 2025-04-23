using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Infrastructure.Data.Repository;

[ExcludeFromCodeCoverage]
public class PatientRepository(SqlDbContext context) : BaseRepository<Patient>(context), IPatientRepository
{
    public async Task<Patient?> GetByDocumentNumberAsync(string documentNumber)
        => await _dbSet.Where(p => p.DocumentNumber.Equals(documentNumber))
                       .FirstOrDefaultAsync();

    public async Task<int> GetAccountIdByLoginAsync(string login)
    {
        var entity = await _dbSet.Where(p => p.DocumentNumber.Equals(login) || p.Email.ToLower().Equals(login.ToLower()))
                                 .FirstOrDefaultAsync();
        return entity == null ? 0 : entity.AccountId;
    }

    public async Task<Patient?> GetByDocumentNumberOrEmailAsync(string documentNumber, string email)
        => await _dbSet.Where(p => p.DocumentNumber.Equals(documentNumber) || p.Email.ToLower().Equals(email.ToLower()))
                       .FirstOrDefaultAsync();
}