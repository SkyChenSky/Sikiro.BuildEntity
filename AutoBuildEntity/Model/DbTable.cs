using System.Collections.Generic;
using System.Data;
using System.Linq;
using 陈珙.AutoBuildEntity.Common.Helper;

namespace 陈珙.AutoBuildEntity.Model
{
    /// <summary>
    /// 物理表
    /// </summary>
    public class DbTable
    {
        public string TableName { get; }

        public List<TableColumn> Columns { get; set; }

        private readonly string _conn;

        public DbTable(string conn)
        {
            _conn = conn;
        }

        public DbTable(string tableName, List<TableColumn> columns)
        {
            TableName = tableName;
            Columns = columns;
        }

        public List<string> QueryMssqlTablesName()
        {
            var result = SqlHelper.MssqlQuery(_conn, @"SELECT  name FROM    sysobjects WHERE  xtype IN ( 'u','v' ); ");

            return (from DataRow row in result.Rows select row[0].ToString()).ToList();
        }

        public List<string> QueryMysqlTablesName()
        {
            var result = SqlHelper.MysqlQuery(_conn, @"show tables; ");

            return (from DataRow row in result.Rows select row[0].ToString()).ToList();
        }


        public List<DbTable> GetTables(List<string> tablesName, string sqlType)
        {
            if (!tablesName.Any())
                return new List<DbTable>();

            var t = new TableColumn(_conn, sqlType);

            var columns = t.GetColumn(tablesName);

            return columns.GroupBy(a => a.TableName).Select(a => new DbTable(a.Key, a.ToList())).ToList();
        }

    }
}
