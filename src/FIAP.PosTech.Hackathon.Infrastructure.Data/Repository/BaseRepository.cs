using FIAP.PosTech.Hackathon.Application.Interfaces.Repository;
using FIAP.PosTech.Hackathon.Domain.Contracts;
using FIAP.PosTech.Hackathon.Domain.Exceptions;
using FIAP.PosTech.Hackathon.Domain.Models;
using FIAP.PosTech.Hackathon.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace FIAP.PosTech.Hackathon.Infrastructure.Data.Repository;

[ExcludeFromCodeCoverage]
public abstract class BaseRepository<TEntity>(SqlDbContext context) : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly SqlDbContext _context = context;

    protected readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public virtual async Task AddAsync(TEntity entity)
    {
        if (entity is IAuditableEntity auditable)
        {
            var now = DateTime.Now;
            auditable.CreatedAt = now;
            auditable.UpdatedAt = now;
        }

        await _dbSet.AddAsync(entity);
    }

    public virtual void Update(TEntity entity)
        => _dbSet.Update(entity);

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        => await _dbSet.AddRangeAsync(entities);

    public virtual IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> expression)
        => _dbSet.Where(expression);

    public virtual async Task<IEnumerable<TEntity>> GetAsllListAsync()
        => await _dbSet.ToListAsync();

    public virtual IQueryable<TEntity> GetAllAsync()
        => _dbSet.AsQueryable();

    public virtual Pagination<TEntity> FindAllPaginate(Expression<Func<TEntity, bool>> expression, int offset, int limit = 100)
    {
        var query = _dbSet.Where(expression).AsQueryable();
        return Paginate(query, offset, limit);
    }

    public virtual Pagination<TEntity> Paginate(IQueryable<TEntity> query, int offset, int limit = 100)
        => new()
        {
            Offset = offset,
            Limit = limit,
            Total = query.Count(),
            Itens = [.. query.Skip(offset).Take(limit)]
        };

    public virtual async Task<TEntity> GetByIdAsync(long id)
        => await _dbSet.FindAsync(id) ?? throw new NullEntityException("Entidade não encontrada");

    public virtual async Task<TEntity> GetByIdAsync(int id)
        => await _dbSet.FindAsync(id) ?? throw new NullEntityException("Entidade não encontrada");

    public virtual void Remove(TEntity entity)
        => _dbSet.Remove(entity);

    public virtual async Task RemoveAsync(long id)
    {
        var entity = await GetByIdAsync(id);
        Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
        => _dbSet.RemoveRange(entities);

    public async Task SaveChangesAsync()
    {
        UpdateTimestamps();
        await _context.SaveChangesAsync();
    }

    void UpdateTimestamps()
    {
        var modifiedEntries = _context.ChangeTracker.Entries()
            .Where(x => x.Entity is IAuditableEntity && x.State == EntityState.Modified);

        foreach (var entry in modifiedEntries)
            ((IAuditableEntity)entry.Entity).UpdatedAt = DateTime.Now;
    }
}