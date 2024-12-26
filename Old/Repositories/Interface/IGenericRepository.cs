using System.Linq.Expressions;
namespace EnVietSocialNetWorkAPI.Old.Repositories.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        T GetById(int id);
        Task<IEnumerable<T>> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        Task<int> Add(T entity);
        Task AddRange(IEnumerable<T> entities);
        Task Remove(T entity);
        Task RemoveRange(IEnumerable<T> entities);
    }
}
