using System.IO;
using System.Text;

namespace 陈珙.AutoBuildEntity.Common.Helper
{
    public static class FilesHelper
    {
        public static string Write(string directory, string fileName, string content)
        {
            var path = Path.Combine(directory, fileName + ".cs");
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                byte[] byteFile = Encoding.UTF8.GetBytes(content);
                fs.Write(byteFile, 0, byteFile.Length);
            }
            return path;
        }
    }
}
