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
        public TableColumn()
        {

        }
        public TableColumn(string connStr)
        {
            _connStr = connStr;
        }

        public string TableName { get; private set; }

        public string Name { get; private set; }

        public string Remark { get; private set; }

        public string Type { get; private set; }

        public int Length { get; private set; }

        public bool IsIdentity { get; private set; }

        public bool IsKey { get; private set; }

        public bool IsNullable { get; private set; }

        public string CSharpType
        {
            get
            {
                return SqlHelper.MapCsharpType(Type, IsNullable);
            }
        }

        /// <summary>
        /// 查询列信息
        /// </summary>
        /// <param name="tablesName"></param>
        /// <returns></returns>
        public List<TableColumn> QueryColumn(List<string> tablesName)
        {
            #region 表结构

            var paramKey = string.Join(",", tablesName.Select((a, index) => "@p" + index));
            var paramVal = tablesName.Select((a, index) => new SqlParameter("@p" + index, a)).ToArray();
            var sql = string.Format(@"SELECT  obj.name AS tablename ,
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
WHERE   obj.name IN ({0});", paramKey);

            #endregion

            var result = SqlHelper.Query(_connStr, sql, paramVal);

            return (from DataRow row in result.Rows
                    select new TableColumn
                    {
                        IsIdentity = Convert.ToBoolean(row["isidentity"]),
                        IsKey = Convert.ToBoolean(row["iskey"]),
                        IsNullable = Convert.ToBoolean(row["isnullable"]),
                        Length = Convert.ToInt32(row["length"]),
                        Name = row["name"].ToString(),
                        Remark = row["remark"].ToString(),
                        TableName = row["tablename"].ToString(),
                        Type = row["type"].ToString()
                    }).ToList();
        }
    }
}
