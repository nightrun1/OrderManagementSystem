using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Interfaces;

namespace OrderManagementSystem.Repositories;

public class BaseRepository<T>(AppDbContext context) : IRepository<T> where T : class
{
    protected readonly AppDbContext Context = context;

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await Context.Set<T>().FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Context.Set<T>().ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        Context.Set<T>().Add(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        Context.Set<T>().Update(entity);
        await Context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await Context.Set<T>().FindAsync(id);
        if (entity is null)
        {
            return;
        }

        Context.Set<T>().Remove(entity);
        await Context.SaveChangesAsync();
    }
}
