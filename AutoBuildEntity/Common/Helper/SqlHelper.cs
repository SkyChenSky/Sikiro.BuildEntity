using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace 陈珙.AutoBuildEntity.Common.Helper
{
    public static class SqlHelper
    {
        public static string DataBase { get; set; }

        public static DataTable MssqlQuery(string connStr, string sql, SqlParameter[] sqlParameter = null)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                DataBase = conn.Database;

                var cmd = conn.CreateCommand();

                cmd.CommandText = sql;

                if (sqlParameter != null)
                    cmd.Parameters.AddRange(sqlParameter);

                var dr = cmd.ExecuteReader();

                dt.Load(dr);
            }

            return dt;
        }

        public static DataTable MysqlQuery(string connStr, string sql)
        {
            var dt = new DataTable();
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();

                DataBase = conn.Database;

                var cmd = conn.CreateCommand();

                cmd.CommandText = sql;

                var dr = cmd.ExecuteReader();

                dt.Load(dr);
            }

            return dt;
        }

        public static string MapMysqlToCsharpType(string dbtype, bool isNullable)
        {
            if (string.IsNullOrEmpty(dbtype)) return dbtype;
            dbtype = dbtype.ToLower();
            string csharpType;
            switch (dbtype)
            {
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime": csharpType = isNullable ? "DateTime?" : "DateTime"; break;
                case "nchar":
                case "ntext":
                case "char":
                case "varchar":
                case "xml":
                case "text":
                case "longtext":
                case "nvarchar": csharpType = "string"; break;
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney": csharpType = isNullable ? "decimal?" : "decimal"; break;
                case "timestamp":
                case "varbinary":
                case "binary":
                case "image": csharpType = "byte[]"; break;
                case "tinyint": csharpType = isNullable ? "byte?" : "byte"; break;
                case "bigint": csharpType = isNullable ? "long?" : "long"; break;
                case "bit": csharpType = isNullable ? "bool?" : "bool"; break;
                case "datetimeoffset": csharpType = isNullable ? "DateTimeOffset?" : "DateTimeOffset"; break;
                case "double":
                case "real":
                case "float": csharpType = isNullable ? "double?" : "double"; break;
                case "int": csharpType = isNullable ? "int?" : "int"; break;
                case "smallint": csharpType = isNullable ? "short?" : "short"; break;
                case "sql_variant":
                case "sysname": csharpType = "object"; break;
                case "time": csharpType = "TimeSpan"; break;
                case "uniqueidentifier": csharpType = "Guid"; break;
                default: csharpType = "object"; break;
            }
            return csharpType;
        }
    }
}
