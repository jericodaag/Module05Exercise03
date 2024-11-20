using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module07DataAccess.Services
{
    public class DatabaseConnectionService
    {
        private readonly string _connectionString;
        public DatabaseConnectionService()
        {
            _connectionString = "Server=localhost;Database=companydb;User ID=root;Password=;Port=3306;";
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
