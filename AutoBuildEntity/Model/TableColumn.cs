using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using 陈珙.AutoBuildEntity.Common.Helper;

namespace 陈珙.AutoBuildEntity.Model
{
    /// <summary>
    /// 物理表的列信息
    /// </summary>
    public class TableColumn
    {
        private readonly string _connStr;
        private readonly string _sqltype;
        public TableColumn()
        {

        }
        public TableColumn(string connStr, string sqltype)
        {
            _connStr = connStr;
            _sqltype = sqltype;
        }

        public string TableName { get; private set; }

        public string TableComment { get; private set; }

        public string Name { get; private set; }

        public string PropertyName => Name.ToCaseCamelName();

        public string Remark { get; private set; }

        public string Type { get; private set; }

        public long Length { get; private set; }

        public bool IsIdentity { get; private set; }

        public bool IsKey { get; private set; }

        public bool IsNullable { get; private set; }

        public string CSharpType => SqlHelper.MapMysqlToCsharpType(Type, IsNullable);

        public List<TableColumn> GetColumn(List<string> tablesName)
        {
            List<TableColumn> list;
            switch (_sqltype)
            {
                case "mysql":
                    list = GetMySqlDbColumn(tablesName); break;
                case "mssql":
                    list = GetMssqlColumn(tablesName); break;
                default: throw new Exception("无法识别");
            }

            return list;
        }

        /// <summary>
        /// 查询列信息
        /// </summary>
        /// <param name="tablesName"></param>
        /// <returns></returns>
        private List<TableColumn> GetMssqlColumn(List<string> tablesName)
        {
            #region 表结构

            var paramKey = string.Join(",", tablesName.Select((a, index) => "@p" + index));
            var paramVal = tablesName.Select((a, index) => new SqlParameter("@p" + index, a)).ToArray();
            var sql = $@"SELECT  obj.name AS tablename ,
        col.name ,
        ISNULL(ep.[value], '') remark ,
        t.name AS type ,
        col.length ,
        COLUMNPROPERTY(col.id, col.name, 'IsIdentity') AS isidentity ,
        CASE WHEN EXISTS ( SELECT   1
                           FROM     dbo.sysindexes si
                                    INNER JOIN dbo.sysindexkeys sik ON si.id = sik.id
                                                              AND si.indid = sik.indid
                                    INNER JOIN dbo.syscolumns sc ON sc.id = sik.id
                                                              AND sc.colid = sik.colid
                                    INNER JOIN dbo.sysobjects so ON so.name = si.name
                                                              AND so.xtype = 'PK'
                           WHERE    sc.id = col.id
                                    AND sc.colid = col.colid ) THEN 1
             ELSE 0
        END AS iskey ,
        col.isnullable
FROM    dbo.syscolumns col
        LEFT  JOIN dbo.systypes t ON col.xtype = t.xusertype
        INNER JOIN dbo.sysobjects obj ON col.id = obj.id
                                         AND obj.xtype IN ( 'U', 'v' )
                                         AND obj.status >= 0
        LEFT  JOIN dbo.syscomments comm ON col.cdefault = comm.id
        LEFT  JOIN sys.extended_properties ep ON col.id = ep.major_id
                                                 AND col.colid = ep.minor_id
                                                 AND ep.name = 'MS_Description'
        LEFT  JOIN sys.extended_properties epTwo ON obj.id = epTwo.major_id
                                                    AND epTwo.minor_id = 0
                                                    AND epTwo.name = 'MS_Description'
WHERE   obj.name IN ({paramKey});";

            #endregion

            var result = SqlHelper.MssqlQuery(_connStr, sql, paramVal);

            return (from DataRow row in result.Rows
                    select new TableColumn
                    {
                        IsIdentity = Convert.ToBoolean(row["isidentity"]),
                        IsKey = Convert.ToBoolean(row["iskey"]),
                        IsNullable = Convert.ToBoolean(row["isnullable"]),
                        Length = Convert.ToInt64(row["length"]),
                        Name = row["name"].ToString(),
                        Remark = row["remark"].ToString(),
                        TableName = row["tablename"].ToString(),
                        Type = row["type"].ToString()
                    }).ToList();
        }

        private List<TableColumn> GetMySqlDbColumn(List<string> tablesName)
        {
            #region 表结构
            var sql = $@"SELECT 
    *,
    (SELECT 
            table_comment
        FROM
            information_schema.tables
        WHERE
            table_name = tablename
        LIMIT 1) tableComment
FROM
    (SELECT 
        TABLE_NAME tablename,
            COLUMN_NAME name,
            COLUMN_comment remark,
            data_type type,
            CASE
                WHEN CHARACTER_MAXIMUM_LENGTH IS NULL THEN 0
                ELSE CHARACTER_MAXIMUM_LENGTH
            END length,
            CASE
                WHEN COLUMN_keY = 'PRI' THEN 1
                ELSE 0
            END iskey,
            CASE
                WHEN IS_NULLABLE = 'NO' THEN 0
                ELSE 1
            END isnullable,
            0 isidentity
    FROM
        INFORMATION_SCHEMA.COLUMNS
    WHERE
        TABLE_NAME IN ('{string.Join("','", tablesName).TrimEnd(',')}') AND Table_schema = '{SqlHelper.DataBase}') t";
            #endregion

            var result = SqlHelper.MysqlQuery(_connStr, sql);

            return (from DataRow row in result.Rows
                    select new TableColumn
                    {
                        IsIdentity = Convert.ToBoolean(row["isidentity"]),
                        IsKey = Convert.ToBoolean(row["iskey"]),
                        IsNullable = Convert.ToBoolean(row["isnullable"]),
                        Length = Convert.ToInt64(row["length"]),
                        Name = row["name"].ToString(),
                        Remark = row["remark"].ToString(),
                        TableName = row["tablename"].ToString(),
                        Type = row["type"].ToString(),
                        TableComment = row["TableComment"].ToString(),
                    }).ToList();
        }
    }
}
