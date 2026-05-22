using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.SqlClient;

namespace Database_Manager_TBD2.Backend.TableView
{
    public class Metadata
    {
        private readonly Conexion conex;

        Metadata (Conexion connectionString)
        {
            conex = connectionString;
        }

        public DataTable GetTables()
        {
            return conex.ExecuteSelect(@"
                SELECT s.name AS tableName, t.name AS ObjectName
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.is_ms_shipped = 0
                  AND s.name <> 'sys'
                  AND t.name NOT LIKE 'sys%'
                  AND t.name NOT LIKE 'MS%'
                ORDER BY s.name, t.name;");
        }

        public DataTable GetViews()
        {
            return conex.ExecuteSelect(@"
                SELECT s.name AS SchemaName, t.name AS ObjectName
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.is_ms_shipped = 0
                  AND s.name <> 'sys'
                  AND t.name NOT LIKE 'sys%'
                  AND t.name NOT LIKE 'MS%'
                ORDER BY s.name, t.name;");
        }

        public DataTable GetProcedures()
        {
            return conex.ExecuteSelect(@"
                SELECT s.name AS SchemaName, t.name AS ObjectName
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.is_ms_shipped = 0
                  AND s.name <> 'sys'
                  AND t.name NOT LIKE 'sys%'
                  AND t.name NOT LIKE 'MS%'
                ORDER BY s.name, t.name;");

        }

        public DataTable GetFunctions()
        {
            return conex.ExecuteSelect(@"
                SELECT s.name AS SchemaName, t.name AS ObjectName
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.is_ms_shipped = 0
                  AND s.name <> 'sys'
                  AND t.name NOT LIKE 'sys%'
                  AND t.name NOT LIKE 'MS%'
                ORDER BY s.name, t.name;");

        }

        public DataTable GetTriggers()
        {
            return conex.ExecuteSelect(@"
                SELECT s.name AS SchemaName, t.name AS ObjectName
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.is_ms_shipped = 0
                  AND s.name <> 'sys'
                  AND t.name NOT LIKE 'sys%'
                  AND t.name NOT LIKE 'MS%'
                ORDER BY s.name, t.name;");

        }

        public DataTable GetIndexes()
        {
            return conex.ExecuteSelect(@"
                SELECT s.name AS SchemaName, t.name AS ObjectName
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.is_ms_shipped = 0
                  AND s.name <> 'sys'
                  AND t.name NOT LIKE 'sys%'
                  AND t.name NOT LIKE 'MS%'
                ORDER BY s.name, t.name;");

        }

        public DataTable GetUsers()
        {
            return conex.ExecuteSelect(@"
                SELECT s.name AS SchemaName, t.name AS ObjectName
                FROM sys.tables t
                INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                WHERE t.is_ms_shipped = 0
                  AND s.name <> 'sys'
                  AND t.name NOT LIKE 'sys%'
                  AND t.name NOT LIKE 'MS%'
                ORDER BY s.name, t.name;");

        }
    }
}
