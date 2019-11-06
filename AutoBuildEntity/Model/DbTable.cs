using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using 陈珙.AutoBuildEntity.Common.Helper;

namespace 陈珙.AutoBuildEntity.Model
{
    /// <summary>
    /// 物理表
    /// </summary>
    public class DbTable
    {
        public string TableName { get; private set; }

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

        /// <summary>
        /// 查询msql物理表和视图
        /// </summary>
        /// <returns></returns>
        public List<string> QueryTablesName()
        {
            var result = SqlHelper.Query(_conn, @"SELECT  name FROM    sysobjects WHERE  xtype IN ( 'u','v' ); ");

            return (from DataRow row in result.Rows select row[0].ToString()).ToList();
        }

        /// <summary>
        /// 查询mysql物理表
        /// </summary>
        /// <returns></returns>
        public List<string> QueryMysqlTablesName()
        {
            var result = SqlHelper.MysqlQuery(_conn, @"show tables; ");

            return (from DataRow row in result.Rows select row[0].ToString()).ToList();
        }


        public List<DbTable> GetTables(List<string> tablesName)
        {
            if (!tablesName.Any())
                return new List<DbTable>();

            var t = new TableColumn(_conn);

            var columns = t.GetMySqlDbColumn(tablesName);
 
            return columns.GroupBy(a => a.TableName).Select(a => new DbTable(a.Key, a.ToList())).ToList();
        }

    }
}
