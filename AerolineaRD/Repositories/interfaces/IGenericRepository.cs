using AerolineaRD.Data; // ✅ AGREGADO

public interface IGenericRepository<T> where T : class
{
    AppDbContext Context { get; } // ✅ Exponer contexto para operaciones avanzadas
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(object id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveAsync();
}
