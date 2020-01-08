using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace 陈珙.AutoBuildEntity.Model
{
    public class SelectedProject
    {
        public SelectedProject(string projectPath, Project projectDte, List<string> csFilesName)
        {
            ProjectPath = projectPath;
            ProjectDte = projectDte;
            CsFilesName = csFilesName;
        }
        public string ProjectPath { get; }

        public string ProjectName => Path.GetFileNameWithoutExtension(ProjectPath);

        public string ProjectDirectoryName => Path.GetDirectoryName(ProjectPath);

        public Project ProjectDte { get; }

        public string EntityXmlPath => Path.Combine(ProjectDirectoryName, Constans.EntityXml);

        public List<string> CsFilesName { get; }
    }
}
