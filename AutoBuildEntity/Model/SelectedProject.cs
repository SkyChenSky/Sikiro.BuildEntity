using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace 陈珙.AutoBuildEntity.Model
{
    public class SelectedProject
    {
        private readonly string _projectPath;
        private readonly Project _projectDte;
        private readonly List<string> _csFilesName;

        public SelectedProject(string projectPath, Project projectDte, List<string> csFilesName)
        {
            _projectPath = projectPath;
            _projectDte = projectDte;
            _csFilesName = csFilesName;
        }
        public string ProjectPath
        {
            get { return _projectPath; }
        }

        public string ProjectName
        {
            get { return Path.GetFileNameWithoutExtension(ProjectPath); }
        }

        public string ProjectDirectoryName
        {
            get { return Path.GetDirectoryName(ProjectPath); }
        }

        public Project ProjectDte
        {
            get { return _projectDte; }
        }

        public string EntityXmlPath
        {
            get { return Path.Combine(ProjectDirectoryName, Constans.EntityXml); }
        }

        public List<string> CsFilesName
        {
            get { return _csFilesName; }
        }
    }
}
