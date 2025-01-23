namespace EV.DataAccess.Repositories.Interface
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetByID(object obj);
        T GetBySearch(string search);
        bool Create(object obj);
        bool Update(object obj);
        bool Delete(object obj);
        bool TerminateDelete(object obj);
    }
}
