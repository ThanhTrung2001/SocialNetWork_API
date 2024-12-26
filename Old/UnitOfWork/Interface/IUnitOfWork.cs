using EnVietSocialNetWorkAPI.Old.Repositories.Interface;
using System.Data;

namespace EnVietSocialNetWorkAPI.Old.UnitOfWork.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        IDbTransaction Transaction { get; }
        //Start the database Transaction
        Task Begin();
        //Commit the database Transaction
        Task Commit();
        //Rollback the database Transaction
        Task Rollback();
    }
}
