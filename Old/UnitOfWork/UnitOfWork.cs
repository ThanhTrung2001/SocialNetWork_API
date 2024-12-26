using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Old.Repositories.Interface;
using EnVietSocialNetWorkAPI.Old.UnitOfWork.Interface;
using System.Data;
using System.Data.Common;

namespace EnVietSocialNetWorkAPI.Old.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DapperContext _context;
        private IDbTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(DapperContext context) => _context = context;

        public IDbTransaction Transaction => _transaction;

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            //if (!_repositories.ContainsKey(type))
            //{
            //    _repositories[type] = new GenericRepository<T>(_context, _transaction);
            //}
            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task Begin()
        {
            if (_transaction == null)
            {
                _transaction = _context.CreateConnection().BeginTransaction();
                Console.WriteLine("Begin Transaction");
            }
        }

        public async Task Commit()
        {
            try
            {
                Console.WriteLine("Commit Transaction Start");
                _transaction.Commit();
                //// By adding this we can have muliple transactions as part of a single request
                //_transaction.Connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error During Transaction. Rollback");
                _transaction.Rollback();
            }
        }

        public async Task Rollback()
        {
            Console.WriteLine("Rollback Transaction");
            _transaction.Rollback();
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose Transaction");
            if (_transaction != null)
            {
                _transaction.Connection.Close();
                _transaction.Connection.Dispose();
                _transaction.Dispose();

            }
        }
    }
}
