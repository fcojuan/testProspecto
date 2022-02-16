using System.Data;
using System.Data.SqlClient;

namespace Prospecto.Data
{
    public class sqlConnectionConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public sqlConnectionConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
