using EV.DataAccess.SettingConfigurations;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace EV.DataAccess.DataConnection
{
    public class DatabaseContext
    {
        private readonly ConnectionStrings _connections;

        public DatabaseContext(IOptions<ConnectionStrings> configuration)
        {
            _connections = configuration.Value;
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connections.SqlConnection);
    }
}
