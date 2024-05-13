namespace UserManagement.Domain.Base
{
    public interface IService<TEntity, TColumn, TFilter> where TEntity : IRow where TColumn : IColumn where TFilter : IFilter
    {
        Task<IResponse> Add(IServiceRequest<TEntity> entities);
        Task<IResponse> Delete(IServiceRequest<int> entities);
        Task<IEnumerable<TColumn>> GetAll(IServiceRequest<TFilter> filter);
        Task<TColumn> GetById(IServiceRequest<int> filter);
    }
}