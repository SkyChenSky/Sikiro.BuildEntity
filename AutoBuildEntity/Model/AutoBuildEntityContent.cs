using System.Collections.Generic;
using System.Data.SqlClient;

namespace 陈珙.AutoBuildEntity.Model
{
    /// <summary>
    /// 上下文
    /// </summary>
    public class AutoBuildEntityContent
    {
        public SelectedProject SelectedProject { get; set; }

        public EntityXml EntityXml { get; set; }

        public List<string> TablesName { get; set; }
    }
}
