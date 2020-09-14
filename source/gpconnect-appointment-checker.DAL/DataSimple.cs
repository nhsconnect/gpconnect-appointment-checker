using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.DAL
{
    public abstract class DataSimple
    {
        protected string connectionString;
        protected IConfiguration _configuration;

        public DataSimple(string _connectionString)
        {
            connectionString = _connectionString;
        }

        public DataSimple(IConfiguration config, string _connStringName = "")
        {
            if (string.IsNullOrWhiteSpace(_connStringName))
            {
                _connStringName = ConnectionStrings.DefaultConnection;
            }
            connectionString = config.GetConnectionString(_connStringName);
            _configuration = config;
        }
    }
}
