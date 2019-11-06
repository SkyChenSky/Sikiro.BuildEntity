using System.Collections.Generic;
using System.Linq;
using 陈珙.AutoBuildEntity.Common.Helper;

namespace 陈珙.AutoBuildEntity.Model
{
    /// <summary>
    /// 模版实体
    /// </summary>
    public class TemplateModel
    {
        public TemplateModel(string tableName, List<TableColumn> columns, string projectName)
        {
            TableName = tableName;
            Columns = columns;
            ProjectName = projectName;
        }
        public string TableName { get; private set; }

        public string TableComment => (Columns.FirstOrDefault()?.TableComment) ?? "";

        public string ClassName => TableName.ToCaseCamelName();

        public List<TableColumn> Columns { get; private set; }

        public string ProjectName { get; private set; }
    }
}
