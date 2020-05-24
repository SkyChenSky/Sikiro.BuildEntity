using System.IO;

namespace 陈珙.AutoBuildEntity.Common.Helper
{
    public static class FilesHelper
    {
        public static string WriteAndSave(string directory, string fileName, string content)
        {
            var path = Path.Combine(directory, fileName + ".cs");

            File.WriteAllText(path, content);

            return path;
        }
    }
}
