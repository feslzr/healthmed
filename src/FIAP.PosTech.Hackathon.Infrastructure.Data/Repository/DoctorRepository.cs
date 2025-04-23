using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Entity;
using FIAP.PosTech.Hackathon.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace FIAP.PosTech.Hackathon.Infrastructure.Data.Repository;

[ExcludeFromCodeCoverage]
public class DoctorRepository(SqlDbContext context) : BaseRepository<Doctor>(context), IDoctorRepository
{
    public async Task<Doctor?> GetByDocumentNumberAsync(string documentNumber)
        => await _dbSet.Where(d => d.DocumentNumber.Equals(documentNumber))
                       .FirstOrDefaultAsync();

    public async Task<int> GetAccountIdByDocumentNumberAsync(string documentNumber)
    {
        var entity = await _dbSet.Where(d => d.DocumentNumber.Equals(documentNumber)).FirstOrDefaultAsync();
        return entity == null ? 0 : entity.AccountId;
    }
}