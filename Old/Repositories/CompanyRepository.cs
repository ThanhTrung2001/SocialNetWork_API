using Dapper;
using EnVietSocialNetWorkAPI.DataConnection;
using EnVietSocialNetWorkAPI.Old.Repositories.Interface;
using System.Data;
using System.Linq.Expressions;
using static Dapper.SqlMapper;

namespace EnVietSocialNetWorkAPI.Old.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DapperContext _context;

        public CompanyRepository(DapperContext context)
        {
            _context = context;
        }

        public Task<int> Add(Company entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRange(IEnumerable<Company> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Company> Find(Expression<Func<Company, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Company>> GetAll()
        {
            var query = $"Select * from Companies";
            using (var connection = _context.CreateConnection())
            {
                var companies = await connection.QueryAsync<Company>(query);
                return companies.ToList();
            }
        }

        public Company GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task Remove(Company entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<Company> entities)
        {
            throw new NotImplementedException();
        }

        public async Task AddCompany(CreateCompany company)
        {
            var parameters = new DynamicParameters(company);
            parameters.Add("Name", company.Name, DbType.String);
            parameters.Add("Address", company.Address, DbType.String);
            parameters.Add("Country", company.Country, DbType.String);
            var query = "INSERT INTO Companies (Name, Address, Country) VALUES (@Name, @Address, @Country)";
            Console.WriteLine(query);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
