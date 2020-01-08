using System.Xml;

namespace 陈珙.AutoBuildEntity.Model
{
    /// <summary>
    /// 配置文件
    /// </summary>
    public class EntityXml
    {
        private readonly string _path;

        public EntityXml(string path)
        {
            _path = path;
        }

        public string ConnString { get; private set; }

        public string EntityTemplate { get; private set; }

        public string Type { get; private set; }

        /// <summary>
        /// 读取_entity.xml
        /// </summary>
        /// <returns></returns>
        public EntityXml Load()
        {
            var xml = new XmlDocument();
            xml.Load(_path);

            var autoEntityNode = xml.SelectSingleNode("AutoEntity");
            if (autoEntityNode == null)
                return this;

            var connStringNode = autoEntityNode.SelectSingleNode("ConnString");
            if (connStringNode != null)
                ConnString = connStringNode.InnerText;

            var templatesNodes = autoEntityNode.SelectSingleNode("Template");
            if (templatesNodes != null)
                EntityTemplate = templatesNodes.InnerText; ;

            var type = autoEntityNode.SelectSingleNode("Type");
            if (type != null)
                Type = type.InnerText.Trim().Replace("\r\n", ""); ;
            Type = Type ?? "mysql";

            return this;
        }
    }
}