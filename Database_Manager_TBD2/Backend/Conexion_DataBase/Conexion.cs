using Microsoft.Data.SqlClient;
using System.Data;

namespace Database_Manager_TBD2.Backend
{
    public class Conexion
    {
        public string Name { get; set; } = "";

        public string Server { get; set; } = "";

        public string Database { get; set; } = "";

        public bool UseWindowsAuth { get; set; }

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public override string ToString()
        {
            return $"{Database} - {Name}";
        }

        public string BuildConnectionString()
        {
            if (UseWindowsAuth)
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
                $"TrustServerCertificate=True;" +
                $"Encrypt=False;";
        }

        public void TestConnection()
        {
            string connectionString = BuildConnectionString();

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No hay ConnectionString configurada.");

            using var cn = new SqlConnection(connectionString);

            cn.Open();
        }

        public DataTable ExecuteSelect(string sql, Dictionary<string, object>? parameters = null)
        {
            using var cn = new SqlConnection(BuildConnectionString());

            using var cmd = new SqlCommand(sql, cn);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            using var da = new SqlDataAdapter(cmd);

            var dt = new DataTable();

            cn.Open();

            da.Fill(dt);

            return dt;
        }

        public int ExecuteNonQuery(string sql, Dictionary <string, object>? parameters = null)
        {
            using var cn = new SqlConnection(BuildConnectionString());

            using var cmd = new SqlCommand(sql, cn);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
            }

            cn.Open();

            return cmd.ExecuteNonQuery();
        }
    }
}