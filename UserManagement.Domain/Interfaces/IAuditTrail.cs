
namespace UserManagement.Domain.Interfaces
{
    public interface IAuditTrail<T>
    {
        Task InsertAsync(T entity);
        Task UpdateAsync(T oldEntity, T newEntity);
        Task DeleteAsync(T oldEntity);
    }
}
