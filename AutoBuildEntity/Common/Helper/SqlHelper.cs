using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace 陈珙.AutoBuildEntity.Common.Helper
{
    public static class SqlHelper
    {
        /// <summary>
        /// mssql查询
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <param name="sqlParameter"></param>
        /// <returns></returns>
        public static DataTable Query(string connStr, string sql, SqlParameter[] sqlParameter = null)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                var cmd = conn.CreateCommand();

                cmd.CommandText = sql;

                if (sqlParameter != null)
                    cmd.Parameters.AddRange(sqlParameter);

                var dr = cmd.ExecuteReader();

                dt.Load(dr);
            }

            return dt;
        }

        /// <summary>
        /// mysql查询
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable MysqlQuery(string connStr, string sql)
        {
            var dt = new DataTable();
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();

                var cmd = conn.CreateCommand();

                cmd.CommandText = sql;

                var dr = cmd.ExecuteReader();

                dt.Load(dr);
            }

            return dt;
        }

        /// <summary>
        /// mysql类型映射
        /// </summary>
        /// <param name="dbtype"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public static string MapCsharpType(string dbtype, bool isNullable)
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
                case "nvarchar": csharpType = "string"; break;
                case "decimal":
                case "money":
                case "numeric":
                case "smallmoney": csharpType = isNullable ? "decimal?" : "decimal"; break;
                case "timestamp":
                case "varbinary":
                case "binary":
                case "image": csharpType = "byte[]"; break;
                case "tinyint": csharpType = "byte"; break;
                case "bigint": csharpType = isNullable ? "long?" : "long"; break;
                case "bit": csharpType = isNullable ? "bool?" : "bool"; break;
                case "datetimeoffset": csharpType = "DateTimeOffset"; break;
                case "float": csharpType = isNullable ? "double?" : "double"; break;
                case "int": csharpType = isNullable ? "int?" : "int"; break;
                case "real": csharpType = "Single"; break;
                case "smallint": csharpType = isNullable ? "short?" : "short"; break;
                case "sql_variant":
                case "sysname": csharpType = "object"; break;
                case "time": csharpType = "TimeSpan"; break;
                case "uniqueidentifier": csharpType = "Guid"; break;
                default: csharpType = "object"; break;
            }
            return csharpType;
        }

        /// <summary>
        /// mssql类型映射
        /// </summary>
        /// <param name="dbtype"></param>
        /// <returns></returns>
        public static Type MapCommonType(string dbtype)
        {
            if (string.IsNullOrEmpty(dbtype)) return Type.Missing.GetType();
            dbtype = dbtype.ToLower();
            Type commonType;
            switch (dbtype)
            {
                case "bigint": commonType = typeof(long); break;
                case "binary": commonType = typeof(byte[]); break;
                case "bit": commonType = typeof(bool); break;
                case "char": commonType = typeof(string); break;
                case "date": commonType = typeof(DateTime); break;
                case "datetime": commonType = typeof(DateTime); break;
                case "datetime2": commonType = typeof(DateTime); break;
                case "datetimeoffset": commonType = typeof(DateTimeOffset); break;
                case "decimal": commonType = typeof(decimal); break;
                case "float": commonType = typeof(double); break;
                case "image": commonType = typeof(byte[]); break;
                case "int": commonType = typeof(int); break;
                case "money": commonType = typeof(decimal); break;
                case "nchar": commonType = typeof(string); break;
                case "ntext": commonType = typeof(string); break;
                case "numeric": commonType = typeof(decimal); break;
                case "nvarchar": commonType = typeof(string); break;
                case "real": commonType = typeof(Single); break;
                case "smalldatetime": commonType = typeof(DateTime); break;
                case "smallint": commonType = typeof(short); break;
                case "smallmoney": commonType = typeof(decimal); break;
                case "sql_variant": commonType = typeof(object); break;
                case "sysname": commonType = typeof(object); break;
                case "text": commonType = typeof(string); break;
                case "time": commonType = typeof(TimeSpan); break;
                case "timestamp": commonType = typeof(byte[]); break;
                case "tinyint": commonType = typeof(byte); break;
                case "uniqueidentifier": commonType = typeof(Guid); break;
                case "varbinary": commonType = typeof(byte[]); break;
                case "varchar": commonType = typeof(string); break;
                case "xml": commonType = typeof(string); break;
                default: commonType = typeof(object); break;
            }
            return commonType;
        }
    }
}
