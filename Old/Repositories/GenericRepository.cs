using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Old.Repositories.Interface;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace EnVietSocialNetWorkAPI.Old.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DapperContext _context;
        //private readonly IDbTransaction _dbTransaction;

        //public GenericRepository(DapperContext context, IDbTransaction transaction){
        //    _context = context;
        //    _dbTransaction = transaction;   
        //}

        public GenericRepository(DapperContext context)
        {
            _context = context;
        }

        private string GetTableName()
        {
            var type = typeof(T);
            var tableAttribute = type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute != null)
                return tableAttribute.Name;

            return type.Name;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var tableName = GetTableName(); // Assuming table name matches entity name
            var query = $"Select * from {tableName}";
            using (var connection = _context.CreateConnection())
            {
                var companies = await connection.QueryAsync<T>(query);
                return companies.ToList();
            }
        }

        public T GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(T entity)
        {
            var tableName = GetTableName();
            var parameters = new DynamicParameters(entity);
            var query = $"INSERT INTO {tableName} VALUES ({string.Join(", ", parameters.ParameterNames)})";
            Console.WriteLine(query);
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(query, parameters);
            }
        }

        public Task AddRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }


        public Task Remove(T entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}
