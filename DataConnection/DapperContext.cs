using Microsoft.Data.SqlClient;
using System.Data;

namespace EnVietSocialNetWorkAPI.DataConnection
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext (IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection")!;
        }

        public IDbConnection CreateConnection()
        { return new SqlConnection(_connectionString); }

        public IDbConnection CreateMasterConnection()
        { return new SqlConnection(_configuration.GetConnectionString("MasterConnection")); }

    }
}
