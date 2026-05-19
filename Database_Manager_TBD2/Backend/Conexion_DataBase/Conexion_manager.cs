using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Database_Manager_TBD2.Backend
{
    public static class ConnectionStorage
    {
        private static readonly string FilePath = "connections.json";

        public static void Save(List <Conexion> connections)
        {
            string json = JsonSerializer.Serialize(connections, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static List<Conexion> Load()
        {
            if (!File.Exists(FilePath))
            {
                return new List<Conexion>();
            }

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize< List<Conexion>>(json) ?? new List<Conexion>();
        }
    }
}