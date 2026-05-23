using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace Database_Manager_TBD2.Backend.TableView
{
    public class Metadata
    {
        private readonly Conexion conex;

        public Metadata(Conexion connection)
        {
            conex = connection;
        }

        // ================= TABLES =================
        public DataTable GetTables()
        {
            return conex.ExecuteSelect(@"
                SELECT 
                    s.name AS SchemaName,
                    o.name AS ObjectName
                FROM sys.objects o
                INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                WHERE o.type = 'U'
                  AND o.is_ms_shipped = 0
                ORDER BY s.name, o.name;");
        }

        // ================= VIEWS =================
        public DataTable GetViews()
        {
            return conex.ExecuteSelect(@"
                SELECT 
                    s.name AS SchemaName,
                    o.name AS ObjectName
                FROM sys.objects o
                INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                WHERE o.type = 'V'
                  AND o.is_ms_shipped = 0
                ORDER BY s.name, o.name;");
        }

        // ================= PROCEDURES =================
        public DataTable GetProcedures()
        {
            return conex.ExecuteSelect(@"
                SELECT 
                    s.name AS SchemaName,
                    o.name AS ObjectName
                FROM sys.objects o
                INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                WHERE o.type = 'P'
                  AND o.is_ms_shipped = 0
                ORDER BY s.name, o.name;");
        }

        // ================= FUNCTIONS =================
        public DataTable GetFunctions()
        {
            return conex.ExecuteSelect(@"
                SELECT 
                    s.name AS SchemaName,
                    o.name AS ObjectName
                FROM sys.objects o
                INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                WHERE o.type IN ('FN','TF','IF')
                  AND o.is_ms_shipped = 0
                ORDER BY s.name, o.name;");
        }

        // ================= TRIGGERS =================
        public DataTable GetTriggers()
        {
            return conex.ExecuteSelect(@"
                SELECT 
                    s.name AS SchemaName,
                    o.name AS ObjectName
                FROM sys.triggers t
                INNER JOIN sys.objects o ON t.object_id = o.object_id
                INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                WHERE t.is_ms_shipped = 0
                ORDER BY s.name, o.name;");
        }

        // ================= INDEXES =================
        public DataTable GetIndexes()
        {
            return conex.ExecuteSelect(@"
                SELECT 
                    s.name AS SchemaName,
                    o.name AS TableName,
                    i.name AS IndexName
                FROM sys.indexes i
                INNER JOIN sys.objects o ON i.object_id = o.object_id
                INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
                WHERE o.type = 'U'
                  AND i.name IS NOT NULL
                ORDER BY s.name, o.name, i.name;");
        }

        // ================= USERS =================
        public DataTable GetUsers()
        {
            return conex.ExecuteSelect(@"
                SELECT 
                    name AS UserName,
                    type_desc AS UserType
                FROM sys.database_principals
                WHERE principal_id > 4
                  AND type NOT IN ('A','G','R','X')
                ORDER BY name;");
        }

        // ================= COLUMNS =================
        public DataTable GetColumns(string schema, string table)
        {
            string sql = @"
                SELECT 
                    c.column_id AS ColumnId,
                    c.name AS ColumnName,
                    ty.name AS DataType,
                    c.max_length AS MaxLength,
                    c.precision AS [Precision],
                    c.scale AS Scale,
                    c.is_nullable AS IsNullable,
                    c.is_identity AS IsIdentity,
                    ISNULL(dc.definition, '') AS DefaultValue
                FROM sys.columns c
                INNER JOIN sys.objects o 
                    ON c.object_id = o.object_id
                INNER JOIN sys.schemas s 
                    ON o.schema_id = s.schema_id
                INNER JOIN sys.types ty 
                    ON c.user_type_id = ty.user_type_id
                LEFT JOIN sys.default_constraints dc 
                    ON c.default_object_id = dc.object_id
                WHERE s.name = @Schema
                  AND o.name = @Table
                  AND o.type = 'U'
                ORDER BY c.column_id;";

            return conex.ExecuteSelect(sql, new Dictionary<string, object>
            {
                ["@Schema"] = schema,
                ["@Table"] = table
            });
        }
    }
}