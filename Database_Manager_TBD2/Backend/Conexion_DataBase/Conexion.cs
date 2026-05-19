using Microsoft.Data.SqlClient;

namespace Database_Manager_TBD2.Backend
{
    public class Conexion
    {
        public string Name { get; set; } = "";

        public string Server { get; set; } = "";

        public string Database { get; set; } = "";

        public bool WithWindowsAuth { get; set; }

        public bool WithSqlAuth { get; set; }

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public override string ToString()
        {
            return $"{Database} - {Name}";
        }

        public string BuildConnectionString()
        {
            if (WithWindowsAuth)
            {
                return
                    $"Server={Server};" +
                    $"Database={Database};" +
                    $"Trusted_Connection=True;" +
                    $"TrustServerCertificate=True;";
            }

            return
                $"Server={Server};" +
                $"Database={Database};" +
                $"User Id={Username};" +
                $"Password={Password};" +
                $"TrustServerCertificate=True;";
        }

        public void TestConnection()
        {
            if (string.IsNullOrWhiteSpace(BuildConnectionString()))
                throw new InvalidOperationException("No hay ConnectionString configurada.");

            using var cn = new SqlConnection(BuildConnectionString());
            cn.Open();
        }
    }
}