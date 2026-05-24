using Database_Manager_TBD2.Backend;
using System;
using System.Data;
using System.Text;
using System.Linq;

namespace Database_Manager_TBD2.Backend.TableView
{
    public class DDL
    {
        private readonly Conexion conex;

        public DDL(Conexion connection)
        {
            conex = connection;
        }

        public string GetTableDDL(string schema, string tableName)
        {
            try
            {
                var sb = CreateHeader("TABLE", schema, tableName);

                BuildTable(sb, schema, tableName);

                AppendIfNotEmpty(sb, GetPrimaryKeyDDL(schema, tableName));
                AppendIfNotEmpty(sb, GetUniqueConstraintsDDL(schema, tableName));
                AppendIfNotEmpty(sb, GetForeignKeysDDL(schema, tableName));
                AppendIfNotEmpty(sb, GetCheckConstraintsDDL(schema, tableName));
                AppendIfNotEmpty(sb, GetIndexesDDL(schema, tableName));

                sb.AppendLine("GO");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return FormatError("TABLE", ex);
            }
        }

        public string GetViewDDL(string schema, string name)
            => GetObjectDDL(schema, name, "VIEW");

        public string GetProcedureDDL(string schema, string name)
            => GetObjectDDL(schema, name, "PROCEDURE");

        public string GetFunctionDDL(string schema, string name)
            => GetObjectDDL(schema, name, "FUNCTION");

        public string GetTriggerDDL(string schema, string name)
            => GetObjectDDL(schema, name, "TRIGGER");

        private string GetObjectDDL(string schema, string name, string type)
        {
            try
            {
                var sb = CreateHeader(type, schema, name);

                string sql = $@"
                SELECT OBJECT_DEFINITION(OBJECT_ID(
                    QUOTENAME('{EscapeSql(schema)}') + '.' + QUOTENAME('{EscapeSql(name)}')
                )) AS Definition";

                var row = QuerySingle(sql);

                if (row == null || row["Definition"] == DBNull.Value)
                    sb.AppendLine($"-- Could not retrieve {type} definition");
                else
                    sb.AppendLine(row["Definition"].ToString());

                sb.AppendLine("GO");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return FormatError(type, ex);
            }
        }

        public string GetIndexDDL(string schema, string tableName, string indexName)
        {
            try
            {
                var sb = CreateHeader("INDEX", schema, indexName);

                string metaSql = $@"
                SELECT
                    i.index_id,
                    i.name AS IndexName,
                    i.type_desc AS IndexType,
                    i.is_unique,
                    i.is_primary_key,
                    i.is_unique_constraint
                FROM sys.indexes i
                INNER JOIN sys.tables t 
                    ON i.object_id  = t.object_id
                INNER JOIN sys.schemas s 
                    ON t.schema_id  = s.schema_id
                WHERE s.name = '{EscapeSql(schema)}'
                  AND t.name = '{EscapeSql(tableName)}'
                  AND i.name = '{EscapeSql(indexName)}'
                  AND i.type > 0";

                var meta = QuerySingle(metaSql);

                if (meta == null)
                {
                    sb.AppendLine($"-- Index [{indexName}] not found on [{schema}].[{tableName}]");
                    sb.AppendLine("GO");
                    return sb.ToString();
                }

                int indexId = Convert.ToInt32(meta["index_id"]);

                string colSql = $@"
                SELECT
                    c.name AS ColumnName,
                    ic.is_descending_key,
                    ic.is_included_column
                FROM sys.index_columns ic
                INNER JOIN sys.columns c 
                    ON ic.object_id = c.object_id
                    AND ic.column_id = c.column_id
                INNER JOIN sys.tables t 
                    ON ic.object_id = t.object_id
                INNER JOIN sys.schemas s
                    ON t.schema_id = s.schema_id
                WHERE s.name = '{EscapeSql(schema)}'
                  AND t.name = '{EscapeSql(tableName)}'
                  AND ic.index_id = {indexId}
                ORDER BY ic.is_included_column, ic.key_ordinal";

                var cols = Query(colSql);

                if (cols.Rows.Count == 0)
                {
                    sb.AppendLine("-- No columns found for this index");
                    sb.AppendLine("GO");
                    return sb.ToString();
                }

                var keyCols = cols.AsEnumerable()
                    .Where(r => !Convert.ToBoolean(r["is_included_column"]))
                    .Select(r => $"[{r["ColumnName"]}] {(Convert.ToBoolean(r["is_descending_key"]) ? "DESC" : "ASC")}")
                    .ToList();

                var inclCols = cols.AsEnumerable()
                    .Where(r => Convert.ToBoolean(r["is_included_column"]))
                    .Select(r => $"[{r["ColumnName"]}]")
                    .ToList();

                sb.Append("CREATE ");

                if (Convert.ToBoolean(meta["is_unique"]))
                    sb.Append("UNIQUE ");

                string indexType = meta["IndexType"].ToString().ToUpper();

                sb.Append($"{indexType} INDEX [{indexName}]");
                sb.AppendLine();
                sb.AppendLine($"ON [{schema}].[{tableName}]");
                sb.AppendLine("(");
                sb.AppendLine($"    {string.Join(",\n    ", keyCols)}");
                sb.Append(")");

                if (inclCols.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("INCLUDE");
                    sb.AppendLine("(");
                    sb.AppendLine($"    {string.Join(",\n    ", inclCols)}");
                    sb.Append(")");
                }

                sb.AppendLine(";");
                sb.AppendLine("GO");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return FormatError("INDEX", ex);
            }
        }

        private void BuildTable(StringBuilder sb, string schema, string table)
        {
            sb.AppendLine($"CREATE TABLE [{schema}].[{table}]");
            sb.AppendLine("(");

            var cols = Query($@"
            SELECT 
                c.column_id,
                c.name AS ColumnName,
                t.name AS DataType,
                c.max_length,
                c.precision,
                c.scale,
                c.is_nullable,
                c.is_identity,
                c.is_computed,
                cc.definition AS ComputedFormula,
                dc.definition AS DefaultValue
            FROM sys.tables tbl
            INNER JOIN sys.schemas s 
                ON tbl.schema_id = s.schema_id
            INNER JOIN sys.columns c 
                ON tbl.object_id = c.object_id
            INNER JOIN sys.types t 
                ON c.user_type_id = t.user_type_id
            LEFT JOIN sys.computed_columns cc 
                ON c.object_id = cc.object_id 
                AND c.column_id = cc.column_id
            LEFT JOIN sys.default_constraints dc 
                ON c.default_object_id = dc.object_id
            WHERE s.name = '{EscapeSql(schema)}'
              AND tbl.name = '{EscapeSql(table)}'
            ORDER BY c.column_id");

            for (int i = 0; i < cols.Rows.Count; i++)
            {
                var c = cols.Rows[i];

                sb.Append("    ");
                sb.Append(FormatColumn(c));

                if (i < cols.Rows.Count - 1)
                    sb.Append(",");

                sb.AppendLine();
            }

            sb.AppendLine(");");
            sb.AppendLine();
        }

        private string FormatColumn(DataRow c)
        {
            string name = c["ColumnName"].ToString();
            string type = c["DataType"].ToString();

            int maxLen = Convert.ToInt32(c["max_length"]);
            byte p = Convert.ToByte(c["precision"]);
            byte s = Convert.ToByte(c["scale"]);

            bool nullable = Convert.ToBoolean(c["is_nullable"]);
            bool identity = Convert.ToBoolean(c["is_identity"]);
            bool computed = Convert.ToBoolean(c["is_computed"]);

            string comp = c["ComputedFormula"]?.ToString();
            string def = c["DefaultValue"]?.ToString();

            var sb = new StringBuilder();

            sb.Append($"[{name}] ");

            if (computed)
                return sb.Append($"AS {comp}").ToString();

            sb.Append(FormatType(type, maxLen, p, s));

            if (identity)
                sb.Append(" IDENTITY(1,1)");

            sb.Append(nullable ? " NULL" : " NOT NULL");

            if (!string.IsNullOrWhiteSpace(def))
                sb.Append($" DEFAULT {def}");

            return sb.ToString();
        }

        private string FormatType(string type, int len, byte p, byte s)
        {
            switch (type.ToLower())
            {
                case "varchar":
                case "char":
                case "varbinary":
                case "binary":
                    return len == -1 ? $"{type}(MAX)" : $"{type}({len})";

                case "nvarchar":
                case "nchar":
                    return len == -1 ? $"{type}(MAX)" : $"{type}({len / 2})";

                case "decimal":
                case "numeric":
                    return $"{type}({p},{s})";

                case "datetime2":
                case "datetimeoffset":
                case "time":
                    return $"{type}({s})";

                case "float":
                    return p == 53 ? type : $"{type}({p})";

                default:
                    return type;
            }
        }
         
        private string GetIndexesDDL(string schema, string table)
        {
            var idxMeta = Query($@"
            SELECT
                i.index_id,
                i.name      AS IndexName,
                i.type_desc AS IndexType,
                i.is_unique
            FROM sys.indexes      i
            INNER JOIN sys.tables t 
                ON i.object_id = t.object_id
            INNER JOIN sys.schemas s 
                ON t.schema_id = s.schema_id
            WHERE s.name = '{EscapeSql(schema)}'
              AND t.name = '{EscapeSql(table)}'
              AND i.type > 0
              AND i.is_primary_key      = 0
              AND i.is_unique_constraint = 0
            ORDER BY i.name");

            if (idxMeta.Rows.Count == 0)
                return "";

            var sb = new StringBuilder();

            foreach (DataRow idx in idxMeta.Rows)
            {
                int indexId = Convert.ToInt32(idx["index_id"]);
                string indexName = idx["IndexName"].ToString();
                string indexType = idx["IndexType"].ToString().ToUpper();
                bool isUnique = Convert.ToBoolean(idx["is_unique"]);

                var cols = Query($@"
                            SELECT
                                c.name AS ColumnName,
                                ic.is_descending_key,
                                ic.is_included_column
                            FROM sys.index_columns ic
                            INNER JOIN sys.columns c  
                                ON ic.object_id = c.object_id
                                AND ic.column_id = c.column_id
                            INNER JOIN sys.tables t  
                                ON ic.object_id = t.object_id
                            INNER JOIN sys.schemas  s  
                                ON t.schema_id = s.schema_id
                            WHERE s.name = '{EscapeSql(schema)}'
                              AND t.name = '{EscapeSql(table)}'
                              AND ic.index_id = {indexId}
                            ORDER BY ic.is_included_column, ic.key_ordinal");

                var keyCols = cols.AsEnumerable()
                    .Where(r => !Convert.ToBoolean(r["is_included_column"]))
                    .Select(r => $"[{r["ColumnName"]}] {(Convert.ToBoolean(r["is_descending_key"]) ? "DESC" : "ASC")}")
                    .ToList();

                var inclCols = cols.AsEnumerable()
                    .Where(r => Convert.ToBoolean(r["is_included_column"]))
                    .Select(r => $"[{r["ColumnName"]}]")
                    .ToList();

                sb.Append("CREATE ");
                if (isUnique) sb.Append("UNIQUE ");
                sb.AppendLine($"{indexType} INDEX [{indexName}]");
                sb.AppendLine($"ON [{schema}].[{table}]");
                sb.AppendLine("(");
                sb.AppendLine($"    {string.Join(",\n    ", keyCols)}");
                sb.Append(")");

                if (inclCols.Count > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine("INCLUDE (");
                    sb.AppendLine($"    {string.Join(",\n    ", inclCols)}");
                    sb.Append(")");
                }

                sb.AppendLine(";");
                sb.AppendLine();
            }

            return sb.ToString();
        }
         
        private string GetPrimaryKeyDDL(string schema, string table) => "";
        private string GetUniqueConstraintsDDL(string schema, string table) => "";
        private string GetForeignKeysDDL(string schema, string table) => "";
        private string GetCheckConstraintsDDL(string schema, string table) => "";

        private DataTable Query(string sql) => conex.ExecuteSelect(sql);

        private DataRow QuerySingle(string sql)
        {
            var dt = Query(sql);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        private void AppendIfNotEmpty(StringBuilder sb, string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                sb.AppendLine(text);
                sb.AppendLine();
            }
        }

        private StringBuilder CreateHeader(string type, string schema, string name)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-- =============================================");
            sb.AppendLine($"-- DDL for {type} [{schema}].[{name}]");
            sb.AppendLine($"-- Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("-- =============================================");
            sb.AppendLine();
            return sb;
        }

        private string FormatError(string type, Exception ex)
            => $"-- ERROR {type}: {ex.Message}";

        private string EscapeSql(string value)
            => value?.Replace("'", "''") ?? "";
    }
}